Imports System
Imports System.Runtime.InteropServices
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports MYKEY.FxCore.Common.ApplicationLogging

Public Class Network

    Public Enum ResourceScope
        RESOURCE_CONNECTED = 1
        RESOURCE_GLOBALNET
        RESOURCE_REMEMBERED
        RESOURCE_RECENT
        RESOURCE_CONTEXT
    End Enum

    Public Enum ResourceType
        RESOURCETYPE_ANY
        RESOURCETYPE_DISK
        RESOURCETYPE_PRINT
        RESOURCETYPE_RESERVED
    End Enum

    Public Enum ResourceUsage
        RESOURCEUSAGE_CONNECTABLE = 1
        RESOURCEUSAGE_CONTAINER = 2
        RESOURCEUSAGE_NOLOCALDEVICE = 4
        RESOURCEUSAGE_SIBLING = 8
        RESOURCEUSAGE_ATTACHED = 16
    End Enum

    Public Enum ResourceDisplayType
        RESOURCEDISPLAYTYPE_GENERIC
        RESOURCEDISPLAYTYPE_DOMAIN
        RESOURCEDISPLAYTYPE_SERVER
        RESOURCEDISPLAYTYPE_SHARE
        RESOURCEDISPLAYTYPE_FILE
        RESOURCEDISPLAYTYPE_GROUP
        RESOURCEDISPLAYTYPE_NETWORK
        RESOURCEDISPLAYTYPE_ROOT
        RESOURCEDISPLAYTYPE_SHAREADMIN
        RESOURCEDISPLAYTYPE_DIRECTORY
        RESOURCEDISPLAYTYPE_TREE
        RESOURCEDISPLAYTYPE_NDSCONTAINER
    End Enum

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure NETRESOURCE
        Public dwScope As ResourceScope
        Public dwType As ResourceType
        Public dwDisplayType As ResourceDisplayType
        Public dwUsage As ResourceUsage
        Public lpLocalName As String
        Public lpRemoteName As String
        Public lpComment As String
        Public lpProvider As String
    End Structure

    <DllImport("mpr.dll")> _
    Public Shared Function WNetAddConnection2(ByRef netResource As NETRESOURCE, ByVal password As String, ByVal username As String, ByVal flags As Integer) As Integer
    End Function

    Public Shared Function ConnectNetShare(ByVal DriveLetter As String, ByVal ShareName As String, Optional ByVal UserName As String = Nothing, Optional ByVal Password As String = Nothing) As Integer
        Dim NetDrive As New NETRESOURCE()

        With NetDrive
            .dwType = ResourceType.RESOURCETYPE_DISK
            .lpLocalName = DriveLetter
            .lpRemoteName = ShareName
        End With

        Return WNetAddConnection2(NetDrive, Password, UserName, 0)

    End Function

    ''' <summary>
    ''' Gibt den Namen des PC zurück auf dem das Programm ausgeführt wird
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetHostName() As String

        Return System.Net.Dns.GetHostName.ToUpper

    End Function

    ''' <summary>
    ''' Gibt die eigene IP-Adresse zurück
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetHostIP() As String
        Dim HostEntry() As System.Net.IPAddress
        Dim CurrentIP4Address As String = "0.0.0.0"


        HostEntry = System.Net.Dns.GetHostAddresses(GetHostName)

        For Each ip As IPAddress In HostEntry
            If ip.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                CurrentIP4Address = ip.ToString
                Exit For
            End If
        Next

        Return CurrentIP4Address

    End Function

    ''' <summary>
    ''' Versucht einen Rechner anzupingen
    ''' </summary>
    ''' <param name="HOST">Es kann eine IP-Adresse oder der Host-Name verwendet werden</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Ping(HOST As String) As PingReply
        Dim _Result As PingReply
        Dim _Ping As New Ping

        Try
            With _Ping
                _Result = .Send(HOST)
            End With

        Catch ex As Exception
            Return _Result
        End Try

        Return _Result
    End Function

    ''' <summary>
    ''' Diese Methode prüft per Ping auf eine Webseite, ob Internetverbindung besteht. Per Default wird
    ''' die Google-Seite benutzt.
    ''' </summary>
    ''' <remarks>Die GetIsNetworkAvailable aus dem Namensraum System.Net.NetworkInformation ist dafür zu ungenau,
    ''' da es nur auf die Verfügbarkeit von Netzwerk allgemein überprüft</remarks>
    ''' <param name="HostName">Auf welchen Server/Webseite der Ping auflaufen soll</param>
    ''' <returns></returns>
    Public Shared Function IsConnectedToInternet(Optional HostName As String = "www.google.de") As Boolean
        Dim _result As Boolean = False

        If Ping(HostName).Status = IPStatus.Success Then
            _result = True
        End If

        Return _result

    End Function

    Public Enum SQLServerConnectionResult
        ServerOnlineSQLUp = 0
        ServerOnlineSQLDown = 1
        ServerOffline = 2
    End Enum

    ''' <summary>
    ''' http://www.xtremedotnettalk.com/database-xml-reporting/85191-seeing-sql-server-exists.html
    ''' </summary>
    ''' <param name="SQLServerName"></param>
    ''' <returns></returns>
    ''' <remarks>Port 1433 ist die Standardinstallation bei SQLServer. 1435 ist der Default bei der Infrastruktur-Installation</remarks>
    Public Shared Function TestSQLServerConnection(ByVal SQLServerName As String, Optional Port As Integer = 1433) As SQLServerConnectionResult
        ' Test a connection to a SQL Server by IP address or name
        Dim _result As SQLServerConnectionResult = SQLServerConnectionResult.ServerOffline
        Dim _PingResult As PingReply
        Dim _Count As Integer

        NLOGLOGGER.Debug("TestSQLServerConnection-Sequenz")

        Try
            ' Trim name from Instancename
            _Count = SQLServerName.IndexOf("\")
            If _Count > 0 Then
                SQLServerName = SQLServerName.Substring(0, _Count)
            End If
            NLOGLOGGER.Debug("==> SQLServerName: " & SQLServerName)

            ' Attempt to resolve the server name
            Dim _IPHost As New System.Net.IPHostEntry()
            'MyIPHost = System.Net.Dns.Resolve(SQLServerName)
            _IPHost = System.Net.Dns.GetHostEntry(SQLServerName)
            NLOGLOGGER.Debug("==> SQLServerDNS: " & _IPHost.HostName)
            NLOGLOGGER.Debug("==> SQLServerIPsCount: " & _IPHost.AddressList.Count)

            Dim SqlIpAdress As System.Net.IPAddress
            If _IPHost.AddressList.Count = 0 Then
                ' Wenn GetHostEntry.AdressList keine Einträge enthält
                Dim IpAdresses() As Net.IPAddress = System.Net.Dns.GetHostAddresses(SQLServerName)
                For Each ip As IPAddress In IpAdresses
                    If ip.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                        SqlIpAdress = ip
                        Exit For
                    End If
                Next
            Else
                ' Get the address object
                For Each ip As IPAddress In _IPHost.AddressList
                    If ip.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                        SqlIpAdress = ip
                        Exit For
                    End If
                Next
            End If
            NLOGLOGGER.Debug("==> SQLServerIP: " & SqlIpAdress.ToString)

            ' Check, if the server is up
            _PingResult = Ping(SqlIpAdress.ToString)
            If _PingResult.Status = IPStatus.Success Then
                _result = SQLServerConnectionResult.ServerOnlineSQLDown
            End If
            NLOGLOGGER.Debug("==> SQLServerPingResult: " & _result)

            ' See if we can connect to port 1433. 1433 is the default
            ' port for SQL Server. If the server is configured to
            ' use a different port, change it here.
            Dim _TCPClient As System.Net.Sockets.TcpClient = New System.Net.Sockets.TcpClient()
            NLOGLOGGER.Debug("==> SQLServerConncect IP:Port: " & SqlIpAdress.ToString & ":" & Port)
            _TCPClient.Connect(SqlIpAdress, Port)

            ' Return success
            _result = SQLServerConnectionResult.ServerOnlineSQLUp
            NLOGLOGGER.Debug("==> SQLServerConnectResult: " & _result)

            ' Everything ok so far, close the TCP connection
            _TCPClient.Close()
            _TCPClient = Nothing

            ' Housekeeping
            SqlIpAdress = Nothing
            _IPHost = Nothing

        Catch excServerConnection As Exception
            ' If an exception is raised, let the caller know 
            ' the SQL Server could not be reached
            NLOGLOGGER.Fatal(excServerConnection.Message)
        End Try

        Return _result

    End Function


    ''' <summary>
    ''' Send a WakeOnLan-PowerOn Packet
    ''' Example: 
    '''     WakeOnLan("34-17-EB-AF-73-0D")
    ''' </summary>
    ''' <param name="MacAddr"></param>
    Public Shared Sub WakeOnLan(MacAddr As String)

        Dim client As UdpClient = New UdpClient()
        client.Connect(IPAddress.Broadcast, 40000)
        Dim _MacBytes() As String

        Dim packet As Byte() = New Byte(101) {}

        For i As Integer = 0 To 6 - 1
            packet(i) = &HFF
        Next

        ' Unterschiedliche Schreibweisen berücksichtigen
        If InStr(MacAddr, "-", CompareMethod.Text) > 0 Then
            _MacBytes = Split(MacAddr, "-")
        End If
        If InStr(MacAddr, ":", CompareMethod.Text) > 0 Then
            _MacBytes = Split(MacAddr, ":")
        End If


        For i As Integer = 1 To 16

            For j As Integer = 0 To 6 - 1
                packet(i * 6 + j) = CByte("&H" & _MacBytes(j))
            Next
        Next

        client.Send(packet, packet.Length)
    End Sub


End Class
