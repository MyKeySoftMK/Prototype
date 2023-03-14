Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Net.Sockets
Imports System.ServiceModel.Channels
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

Public Class TcpClientFuncs
    Implements IDisposable


    ' https://github.com/BrandonPotter/SimpleTCP

    Public Sub New()
        StringEncoder = System.Text.Encoding.UTF8
        ReadLoopIntervalMs = 10
        Delimiter = &H13
    End Sub

    Private _rxThread As Thread = Nothing
    Private _queuedMsg As List(Of Byte) = New List(Of Byte)()
    Public Property Delimiter As Byte
    Public Property StringEncoder As System.Text.Encoding
    Private _client As TcpClient = Nothing
    Public Event DelimiterDataReceived As EventHandler(Of Message)
    Public Event DataReceived As EventHandler(Of Message)
    Friend Property QueueStop As Boolean
    Friend Property ReadLoopIntervalMs As Integer
    Public Property AutoTrimStrings As Boolean

    Public Function Connect(ByVal hostNameOrIpAddress As String, ByVal port As Integer) As TcpClientFuncs
        If String.IsNullOrEmpty(hostNameOrIpAddress) Then
            Throw New ArgumentNullException("hostNameOrIpAddress")
        End If

        _client = New TcpClient()
        _client.Connect(hostNameOrIpAddress, port)
        StartRxThread()
        Return Me
    End Function

    Private Sub StartRxThread()
        If _rxThread IsNot Nothing Then
            Return
        End If

        _rxThread = New Thread(AddressOf ListenerLoop)
        _rxThread.IsBackground = True
        _rxThread.Start()
    End Sub

    Public Function Disconnect() As TcpClientFuncs
        If _client Is Nothing Then
            Return Me
        End If

        _client.Close()
        _client = Nothing
        Return Me
    End Function

    Public ReadOnly Property TcpClient As TcpClient
        Get
            Return _client
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

        _rxThread = Nothing
    End Sub

    Private Sub RunLoopStep()
        If _client Is Nothing Then
            Return
        End If

        If _client.Connected = False Then
            Return
        End If

        Dim delimiter = Me.Delimiter
        Dim c = _client
        Dim bytesAvailable As Integer = c.Available

        If bytesAvailable = 0 Then
            System.Threading.Thread.Sleep(10)
            Return
        End If

        Dim bytesReceived As List(Of Byte) = New List(Of Byte)()

        While c.Available > 0 AndAlso c.Connected
            Dim nextByte As Byte() = New Byte(0) {}
            c.Client.Receive(nextByte, 0, 1, SocketFlags.None)
            bytesReceived.AddRange(nextByte)

            If nextByte(0) = delimiter Then
                Dim msg As Byte() = _queuedMsg.ToArray()
                _queuedMsg.Clear()
                NotifyDelimiterMessageRx(c, msg)
            Else
                _queuedMsg.AddRange(nextByte)
            End If
        End While

        If bytesReceived.Count > 0 Then
            NotifyEndTransmissionRx(c, bytesReceived.ToArray())
        End If
    End Sub

    Private Sub NotifyDelimiterMessageRx(ByVal client As TcpClient, ByVal msg As Byte())
        If DelimiterDataReceivedEvent IsNot Nothing Then
            Dim m As Message = New Message(msg, client, StringEncoder, Delimiter, AutoTrimStrings)
            RaiseEvent DelimiterDataReceived(Me, m)
        End If
    End Sub

    Private Sub NotifyEndTransmissionRx(ByVal client As TcpClient, ByVal msg As Byte())
        If DataReceivedEvent IsNot Nothing Then
            Dim m As Message = New Message(msg, client, StringEncoder, Delimiter, AutoTrimStrings)
            RaiseEvent DataReceived(Me, m)
        End If
    End Sub

    Public Sub Write(ByVal data As Byte())
        If _client Is Nothing Then
            Throw New Exception("Cannot send data to a null TcpClient (check to see if Connect was called)")
        End If

        _client.GetStream().Write(data, 0, data.Length)
    End Sub

    Public Sub Write(ByVal data As String)
        If data Is Nothing Then
            Return
        End If

        Write(StringEncoder.GetBytes(data))
    End Sub

    Public Sub WriteLine(ByVal data As String)
        If String.IsNullOrEmpty(data) Then
            Return
        End If

        If Convert.ToByte(data.LastOrDefault()) <> Delimiter Then
            Write(data & StringEncoder.GetString(New Byte() {Delimiter}))
        Else
            Write(data)
        End If
    End Sub

    Public Function WriteLineAndGetReply(ByVal data As String, ByVal timeout As TimeSpan) As Message
        Dim mReply As Message = Nothing
        AddHandler Me.DataReceived, Sub(s, e)
                                        mReply = e
                                    End Sub

        WriteLine(data)
        Dim sw As Stopwatch = New Stopwatch()
        sw.Start()

        While mReply Is Nothing AndAlso sw.Elapsed < timeout
            System.Threading.Thread.Sleep(10)
        End While

        Return mReply
    End Function

    Private disposedValue As Boolean = False

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not disposedValue Then

            If disposing Then
            End If

            QueueStop = True

            If _client IsNot Nothing Then

                Try
                    _client.Close()
                Catch
                End Try

                _client = Nothing
            End If

            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub

End Class

