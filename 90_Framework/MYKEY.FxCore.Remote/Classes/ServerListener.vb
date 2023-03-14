Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks


Public Class ServerListener
    Private _listener As TcpListenerEx = Nothing
    Private _connectedClients As List(Of TcpClient) = New List(Of TcpClient)()
    Private _disconnectedClients As List(Of TcpClient) = New List(Of TcpClient)()
    Private _parent As TcpServerFuncs = Nothing
    Private _queuedMsg As List(Of Byte) = New List(Of Byte)()
    Private _delimiter As Byte = &H13
    Private _rxThread As Thread = Nothing

    Public ReadOnly Property ConnectedClientsCount As Integer
        Get
            Return _connectedClients.Count
        End Get
    End Property

    Public ReadOnly Property ConnectedClients As IEnumerable(Of TcpClient)
        Get
            Return _connectedClients
        End Get
    End Property

    Friend Sub New(ByVal parentServer As TcpServerFuncs, ByVal ip As IPAddress, ByVal portnumber As Integer)
        QueueStop = False
        _parent = parentServer
        IPAddress = ip
        Port = portnumber
        ReadLoopIntervalMs = 10
        _listener = New TcpListenerEx(IPAddress, Port)
        _listener.Start()
        System.Threading.ThreadPool.QueueUserWorkItem(AddressOf ListenerLoop)
    End Sub

    Private Sub StartThread()
        If _rxThread IsNot Nothing Then
            Return
        End If

        _rxThread = New Thread(AddressOf ListenerLoop)
        _rxThread.IsBackground = True
        _rxThread.Start()
    End Sub

    Friend Property QueueStop As Boolean
    Public Property IPAddress As IPAddress
    Friend Property Port As Integer
    Friend Property ReadLoopIntervalMs As Integer

    Friend ReadOnly Property Listener As TcpListenerEx
        Get
            Return _listener
        End Get
    End Property

    Private Sub ListenerLoop(ByVal state As Object)
        While Not QueueStop

            Try
                RunLoopStep()
            Catch
            End Try

            System.Threading.Thread.Sleep(ReadLoopIntervalMs)
        End While

        _listener.[Stop]()
    End Sub

    Private Function IsSocketConnected(ByVal s As Socket) As Boolean
        Dim part1 As Boolean = s.Poll(1000, SelectMode.SelectRead)
        Dim part2 As Boolean = (s.Available = 0)

        If (part1 AndAlso part2) OrElse Not s.Connected Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub RunLoopStep()
        If _disconnectedClients.Count > 0 Then
            Dim disconnectedClients = _disconnectedClients.ToArray()
            _disconnectedClients.Clear()

            For Each disC In disconnectedClients
                _connectedClients.Remove(disC)
                _parent.NotifyClientDisconnected(Me, disC)
            Next
        End If

        If _listener.Pending() Then
            Dim newClient = _listener.AcceptTcpClient()
            _connectedClients.Add(newClient)
            _parent.NotifyClientConnected(Me, newClient)
        End If

        _delimiter = _parent.Delimiter

        For Each c In _connectedClients

            If IsSocketConnected(c.Client) = False Then
                _disconnectedClients.Add(c)
            End If

            Dim bytesAvailable As Integer = c.Available

            If bytesAvailable = 0 Then
                Continue For
            End If

            Dim bytesReceived As List(Of Byte) = New List(Of Byte)()

            While c.Available > 0 AndAlso c.Connected
                Dim nextByte As Byte() = New Byte(0) {}
                c.Client.Receive(nextByte, 0, 1, SocketFlags.None)
                bytesReceived.AddRange(nextByte)

                If nextByte(0) = _delimiter Then
                    Dim msg As Byte() = _queuedMsg.ToArray()
                    _queuedMsg.Clear()
                    _parent.NotifyDelimiterMessageRx(Me, c, msg)
                Else
                    _queuedMsg.AddRange(nextByte)
                End If
            End While

            If bytesReceived.Count > 0 Then
                _parent.NotifyEndTransmissionRx(Me, c, bytesReceived.ToArray())
            End If
        Next
    End Sub
End Class

