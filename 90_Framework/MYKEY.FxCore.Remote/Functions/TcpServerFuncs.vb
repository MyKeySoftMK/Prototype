Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

Public Class TcpServerFuncs

    ' https://github.com/BrandonPotter/SimpleTCP

    Public Sub New()
        Delimiter = &H13
        StringEncoder = System.Text.Encoding.UTF8
    End Sub

    Private _listeners As List(Of ServerListener) = New List(Of ServerListener)()
    Public Property Delimiter As Byte
    Public Property StringEncoder As System.Text.Encoding
    Public Property AutoTrimStrings As Boolean
    Public Event ClientConnected As EventHandler(Of TcpClient)
    Public Event ClientDisconnected As EventHandler(Of TcpClient)
    Public Event DelimiterDataReceived As EventHandler(Of Message)
    Public Event DataReceived As EventHandler(Of Message)

    Public ReadOnly Property RegisteredListeners As List(Of ServerListener)
        Get
            Return _listeners
        End Get
    End Property

    Public Function GetIPAddresses() As IEnumerable(Of IPAddress)
        Dim ipAddresses As List(Of IPAddress) = New List(Of IPAddress)()
        Dim enabledNetInterfaces As IEnumerable(Of NetworkInterface) = NetworkInterface.GetAllNetworkInterfaces().Where(Function(nic) nic.OperationalStatus = OperationalStatus.Up)

        For Each netInterface As NetworkInterface In enabledNetInterfaces
            Dim ipProps As IPInterfaceProperties = netInterface.GetIPProperties()

            For Each addr As UnicastIPAddressInformation In ipProps.UnicastAddresses

                If Not ipAddresses.Contains(addr.Address) Then
                    ipAddresses.Add(addr.Address)
                End If
            Next
        Next

        Dim ipSorted = ipAddresses.OrderByDescending(Function(ip) RankIpAddress(ip)).ToList()
        Return ipSorted
    End Function

    Public Function GetListeningIPs() As List(Of IPAddress)
        Dim listenIps As List(Of IPAddress) = New List(Of IPAddress)()

        For Each l In _listeners

            If Not listenIps.Contains(l.IPAddress) Then
                listenIps.Add(l.IPAddress)
            End If
        Next

        Return listenIps.OrderByDescending(Function(ip) RankIpAddress(ip)).ToList()
    End Function

    Public Sub Broadcast(ByVal data As Byte())
        For Each client In _listeners.SelectMany(Function(x) x.ConnectedClients)
            client.GetStream().Write(data, 0, data.Length)
        Next
    End Sub

    Public Sub Broadcast(ByVal data As String)
        If data Is Nothing Then
            Return
        End If

        Broadcast(StringEncoder.GetBytes(data))
    End Sub

    Public Sub BroadcastLine(ByVal data As String)
        If String.IsNullOrEmpty(data) Then
            Return
        End If

        If Convert.ToByte(data.LastOrDefault()) <> Delimiter Then
            Broadcast(data & StringEncoder.GetString(New Byte() {Delimiter}))
        Else
            Broadcast(data)
        End If
    End Sub

    Private Function RankIpAddress(ByVal addr As IPAddress) As Integer
        Dim rankScore As Integer = 1000

        If IPAddress.IsLoopback(addr) Then
            rankScore = 300
        ElseIf addr.AddressFamily = AddressFamily.InterNetwork Then
            rankScore += 100

            If addr.GetAddressBytes().Take(2).SequenceEqual(New Byte() {169, 254}) Then
                rankScore = 0
            End If
        End If

        If rankScore > 500 Then

            For Each nic In TryGetCurrentNetworkInterfaces()
                Dim ipProps = nic.GetIPProperties()

                If ipProps.GatewayAddresses.Any() Then

                    If ipProps.UnicastAddresses.Any(Function(u) u.Address.Equals(addr)) Then
                        rankScore += 1000
                    End If

                    Exit For
                End If
            Next
        End If

        Return rankScore
    End Function

    Private Shared Function TryGetCurrentNetworkInterfaces() As IEnumerable(Of NetworkInterface)
        Try
            Return NetworkInterface.GetAllNetworkInterfaces().Where(Function(ni) ni.OperationalStatus = OperationalStatus.Up)
        Catch __unusedNetworkInformationException1__ As NetworkInformationException
            Return Enumerable.Empty(Of NetworkInterface)()
        End Try
    End Function

    Public Function Start(ByVal port As Integer, ByVal Optional ignoreNicsWithOccupiedPorts As Boolean = True) As TcpServerFuncs
        Dim ipSorted = GetIPAddresses()
        Dim anyNicFailed As Boolean = False

        For Each ipAddr In ipSorted

            Try
                Start(ipAddr, port)
            Catch ex As SocketException
                DebugInfo(ex.ToString())
                anyNicFailed = True
            End Try
        Next

        If Not IsStarted Then Throw New InvalidOperationException("Port was already occupied for all network interfaces")

        If anyNicFailed AndAlso Not ignoreNicsWithOccupiedPorts Then
            [Stop]()
            Throw New InvalidOperationException("Port was already occupied for one or more network interfaces.")
        End If

        Return Me
    End Function

    Public Function Start(ByVal port As Integer, ByVal addressFamilyFilter As AddressFamily) As TcpServerFuncs
        Dim ipSorted = GetIPAddresses().Where(Function(ip) ip.AddressFamily = addressFamilyFilter)

        For Each ipAddr In ipSorted

            Try
                Start(ipAddr, port)
            Catch
            End Try
        Next

        Return Me
    End Function

    Public ReadOnly Property IsStarted As Boolean
        Get
            Return _listeners.Any(Function(l) l.Listener.Active)
        End Get
    End Property

    Public Function Start(ByVal ipAddress As IPAddress, ByVal port As Integer) As TcpServerFuncs
        Dim listener As ServerListener = New ServerListener(Me, ipAddress, port)
        _listeners.Add(listener)
        Return Me
    End Function

    Public Sub [Stop]()
        _listeners.All(Function(l) CSharpImpl.__Assign(l.QueueStop, True))

        While _listeners.Any(Function(l) l.Listener.Active)
            Thread.Sleep(100)
        End While

        _listeners.Clear()
    End Sub

    Public ReadOnly Property ConnectedClientsCount As Integer
        Get
            Return _listeners.Sum(Function(l) l.ConnectedClientsCount)
        End Get
    End Property

    Friend Sub NotifyDelimiterMessageRx(ByVal listener As ServerListener, ByVal client As TcpClient, ByVal msg As Byte())
        If DelimiterDataReceivedEvent IsNot Nothing Then
            Dim m As Message = New Message(msg, client, StringEncoder, Delimiter, AutoTrimStrings)
            RaiseEvent DelimiterDataReceived(Me, m)
        End If
    End Sub

    Friend Sub NotifyEndTransmissionRx(ByVal listener As ServerListener, ByVal client As TcpClient, ByVal msg As Byte())
        If DataReceivedEvent IsNot Nothing Then
            Dim m As Message = New Message(msg, client, StringEncoder, Delimiter, AutoTrimStrings)
            RaiseEvent DataReceived(Me, m)
        End If
    End Sub

    Friend Sub NotifyClientConnected(ByVal listener As ServerListener, ByVal newClient As TcpClient)
        RaiseEvent ClientConnected(Me, newClient)
    End Sub

    Friend Sub NotifyClientDisconnected(ByVal listener As ServerListener, ByVal disconnectedClient As TcpClient)
        RaiseEvent ClientDisconnected(Me, disconnectedClient)
    End Sub

    <System.Diagnostics.Conditional("DEBUG")>
    Private Sub DebugInfo(ByVal format As String, ParamArray args As Object())
        If _debugInfoTime Is Nothing Then
            _debugInfoTime = New System.Diagnostics.Stopwatch()
            _debugInfoTime.Start()
        End If

        System.Diagnostics.Debug.WriteLine(_debugInfoTime.ElapsedMilliseconds & ": " & format, args)
    End Sub

    Private _debugInfoTime As System.Diagnostics.Stopwatch

    Private Class CSharpImpl
        <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
        Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class
End Class

