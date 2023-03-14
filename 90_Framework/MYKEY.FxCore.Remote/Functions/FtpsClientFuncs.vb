Imports FluentFTP
Imports FluentFTP.GnuTLS
Imports FluentFTP.GnuTLS.Enums
Imports FluentFTP.Logging

Imports NLog
Imports NLog.Extensions.Logging

Imports ILogger = Microsoft.Extensions.Logging.ILogger



' FluentFTP bietet mit GnuTLS nun den TLS1.3 Support an:
'   https://github.com/robinrodricks/FluentFTP/wiki/FTPS-Connection-using-GnuTLS
' Dieses ist noch zu implementieren, da eine Migration des Projekts von v39 auf v40+ erfolgen muss
'   https://github.com/robinrodricks/FluentFTP/wiki/v40-Migration-Guide

Public Class FtpsClientFuncs

    Private Shared NLOGLOGGER As NLog.Logger



    Public Sub New()
        NLOGLOGGER = NLog.LogManager.GetLogger("System")
        NLOGLOGGER.Debug("Start: FtpsClientFuncs")
    End Sub

    Public Sub New(ByRef NLOGInstance As NLog.Logger)
        NLOGLOGGER = NLOGInstance
        NLOGLOGGER.Debug("Start: FtpsClientFuncs")
    End Sub

    ''' <summary>
    ''' Öffentlicher Zugriff auf die Connection
    ''' </summary>
    ''' <returns></returns>
    Public Property FTPSClnt As FtpClient

    Dim _FTP_LOGDATA As New FtpLoggingData

    Dim _LoggingProvider As NLogLoggerProvider
    Dim _FtpLogger As ILogger

    Public WriteOnly Property FTP_LOGDATA As FtpLoggingData
        Set(value As FtpLoggingData)
            _FTP_LOGDATA = value

            If FTPSClnt Is Nothing Then
                FTPSClnt = New FtpClient
            End If

            If value.FTP_LOGENABLED = 1 Then
                FTPSClnt.Config.LogToConsole = True
                FTPSClnt.Config.LogUserName = (value.FTP_LOGHIDE = 1)
                FTPSClnt.Config.LogPassword = (value.FTP_LOGHIDE = 1)
                FTPSClnt.Config.LogHost = (value.FTP_LOGHIDE = 1)
                FTPSClnt.Config.LogDurations = True

                If FTPSClnt.Logger Is Nothing Then
                    _LoggingProvider = New NLogLoggerProvider
                    _FtpLogger = _LoggingProvider.CreateLogger(GetType(FtpsClientFuncs).FullName)
                    FTPSClnt.Logger = New FtpLogAdapter(_FtpLogger)
                End If

            Else
                FTPSClnt.Logger = Nothing
            End If

        End Set
    End Property


    ''' <summary>
    ''' Herstellen einer Verbindung zu einem FTP-Server
    ''' </summary>
    ''' <param name="FtpServer"></param>
    Public Sub Connect(FtpServer As FtpServer)

        Try

            If FTPSClnt Is Nothing Then
                FTPSClnt = New FtpClient
            End If

            With FTPSClnt

                .Host = FtpServer.ServerAdress
                .Credentials = New Net.NetworkCredential(FtpServer.UserName, FtpServer.Password)
                .Port = FtpServer.Port

                .Config.EncryptionMode = FtpEncryptionMode.Explicit

                ' accept every Server-Certificate
                .Config.ValidateAnyCertificate = True


                ' enable GnuTLS streams for FTP client
                .Config.CustomStream = GetType(GnuTlsStream)
                ' use the default security suite
                ' include all TLS protocols except for TLS 1.0 and TLS 1.1
                ' no profile required
                .Config.CustomStreamConfig = New GnuConfig() With {
                    .LogLevel = _FTP_LOGDATA.FTP_LOGLEVEL,
                    .LogMessages = GnuMessage.All,
                    .SecuritySuite = GnuSuite.Normal,
                    .SecurityOptions = New List(Of GnuOption) From {
                        New GnuOption(GnuOperator.Include, GnuCommand.Protocol_All),
                        New GnuOption(GnuOperator.Exclude, GnuCommand.Protocol_Tls10),
                        New GnuOption(GnuOperator.Exclude, GnuCommand.Protocol_Tls11)
                        },
                    .SecurityProfile = GnuProfile.None,
                    .HandshakeTimeout = 5000
}

                ' connect using Explicit FTPS with TLS 1.2/1.3
                .Config.EncryptionMode = FtpEncryptionMode.Explicit

            End With



            If FTPSClnt.IsConnected = False Then
                'Me.FTPLOG_ENABLE = "1"
                FTPSClnt.Connect()
            End If

            NLOGLOGGER.Info("Connection is successfull")
            NLOGLOGGER.Debug("Connection Type:" & FTPSClnt.ConnectionType)
            NLOGLOGGER.Debug("Encoding:" & FTPSClnt.Encoding.ToString)
            NLOGLOGGER.Debug("Internet Protocol:" & FTPSClnt.InternetProtocol)
            NLOGLOGGER.Debug("Use Encryption:" & FTPSClnt.IsEncrypted)
            NLOGLOGGER.Debug("Server:" & FTPSClnt.ServerOS & "(" & FTPSClnt.ServerType & ")")
            NLOGLOGGER.Debug("Server System:" & FTPSClnt.SystemType)
            NLOGLOGGER.Debug("Endpoints: " & FTPSClnt.SocketLocalEndPoint.ToString & " (local) / " & FTPSClnt.SocketRemoteEndPoint.ToString & " (remote)")

        Catch ex As Exception
            NLOGLOGGER.Fatal("Connection is abnormal ending")
            NLOGLOGGER.Fatal(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Erstellen einer Liste aller Dateien in einem Verzeichnis. Optional kann die Dateiendung angegeben werden
    ''' </summary>
    ''' <param name="DirectoryName"></param>
    ''' <param name="FileNameExtension"></param>
    ''' <returns></returns>
    Public Function GetListOfFtpFiles(DirectoryName As String, Optional FileNameExtension As String = "") As List(Of String)

        Dim result As New List(Of String)
        Dim FtpResult() As FtpListItem

        FtpResult = FTPSClnt.GetListing(DirectoryName, FtpListOption.AllFiles)

        For Each ftpFile As FtpListItem In FtpResult
            Select Case ftpFile.Type
                Case FtpObjectType.File

                    If FileNameExtension.Length > 0 Then
                        If FileNameExtension.ToUpper = Mid(ftpFile.Name, ftpFile.Name.Length - FileNameExtension.Length + 1).ToUpper Then
                            result.Add(ftpFile.Name)
                        End If
                    Else
                        result.Add(ftpFile.Name)
                    End If


            End Select
        Next

        Return result

    End Function

    Public Function GetNewestFtpFile(DirectoryName As String, Optional FileNameExtension As String = "") As List(Of String)

        Dim result As New List(Of String)
        Dim FtpResult() As FtpListItem
        Dim FileListResult() As FtpListItem
        Dim LatestFile As FtpListItem

        FtpResult = FTPSClnt.GetListing(DirectoryName, FtpListOption.AllFiles)
        If FtpResult.Length = 0 Then
            FTPSClnt.SetWorkingDirectory(DirectoryName)
            FtpResult = FTPSClnt.GetListing("", FtpListOption.AllFiles)
        End If

        For Each ftpFile As FtpListItem In FtpResult
            Select Case ftpFile.Type
                Case FtpObjectType.File

                    If FileNameExtension.Length > 0 Then
                        If FileNameExtension.ToUpper = Mid(ftpFile.Name, ftpFile.Name.Length - FileNameExtension.Length + 1).ToUpper Then
                            If FileListResult Is Nothing Then
                                ReDim Preserve FileListResult(0)
                                FileListResult(0) = ftpFile
                            Else
                                ReDim Preserve FileListResult(FileListResult.Count)
                                FileListResult(FileListResult.Count - 1) = ftpFile
                            End If

                        End If
                    Else

                        If FileListResult Is Nothing Then
                            ReDim Preserve FileListResult(0)
                            FileListResult(0) = ftpFile
                        Else
                            ReDim Preserve FileListResult(FileListResult.Count)
                            FileListResult(FileListResult.Count - 1) = ftpFile
                        End If
                    End If


            End Select
        Next

        If FileListResult IsNot Nothing Then
            LatestFile = FileListResult.OrderBy(Function(dirItem) dirItem.Created).Last

            If LatestFile IsNot Nothing Then
                result.Add(LatestFile.Name)
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' Herunterladen einer einzelnen Datei von einem FTP-Server
    ''' </summary>
    ''' <param name="FtpDirectory"></param>
    ''' <param name="FileName"></param>
    ''' <param name="LocalDirectory"></param>
    ''' <returns></returns>
    Public Function DownloadFtpFile(FtpDirectory As String, FileName As String, LocalDirectory As String) As Boolean
        Dim result As Boolean = False

        Try

            If FTPSClnt.DownloadFile(LocalDirectory & "\" & FileName, FtpDirectory & "/" & FileName, FtpLocalExists.Overwrite) = FtpStatus.Success Then
                NLOGLOGGER.Info("Successfull download file: " & FtpDirectory & "/" & FileName)
                result = True
            Else
                NLOGLOGGER.Error("Error download file:" & FtpDirectory & "/" & FileName)
            End If

        Catch ex As Exception
            NLOGLOGGER.Fatal("DownloadFtpFile abnormal ending")
            NLOGLOGGER.Fatal(ex.Message)
            NLOGLOGGER.Fatal(ex.InnerException)
        End Try

        Return result
    End Function

    ''' <summary>
    ''' Herunterladen einer Liste von Dateien von einem FTP-Server
    ''' </summary>
    ''' <param name="FtpDirectory"></param>
    ''' <param name="FileNames"></param>
    ''' <param name="LocalDirectory"></param>
    ''' <returns></returns>
    Public Function DownloadFtpFiles(FtpDirectory As String, FileNames As List(Of String), LocalDirectory As String) As Boolean
        Dim result As Boolean = False
        Dim _ftpList As List(Of String)

        Try

            If FileNames.Count > 0 Then

                ' Erstellen der Liste mit Dateinamen auf dem FTP-Server
                _ftpList = New List(Of String)
                For Each fName As String In FileNames
                    _ftpList.Add(FtpDirectory & "/" & fName)
                Next

                ' Herunterladen in das Lokale Verzeichnis
                If FTPSClnt.DownloadFiles(LocalDirectory & "\", _ftpList, FtpLocalExists.Overwrite).Count = _ftpList.Count Then
                    NLOGLOGGER.Info("Successfull download file: {@0}", _ftpList)
                    result = True
                Else
                    'NLOGLOGGER.Error("Error download file:" & FtpDirectory & "/" & FileName)
                End If

            End If


        Catch ex As Exception
            NLOGLOGGER.Fatal("DownloadFtpFiles abnormal ending")
            NLOGLOGGER.Fatal(ex.Message)
            NLOGLOGGER.Fatal(ex.InnerException)
        End Try

        Return result
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    Public Function GetFtpFileDate(FileName As String) As Date

        Dim result As Date = New Date("1.1.1900")

        Try

            'result = FTPClnt.GetModifiedTime(FileName, FtpDate.Original)
            result = FTPSClnt.GetModifiedTime(FileName)

        Catch ex As Exception

        End Try

        Return result

    End Function

    ''' <summary>
    ''' Hochladen einer einzelnen Datei auf einem FTP-Server
    ''' </summary>
    ''' <param name="FtpDirectory"></param>
    ''' <param name="FileName"></param>
    ''' <param name="LocalDirectory"></param>
    ''' <returns></returns>
    Public Function UploadFtpFile(FtpDirectory As String, FileName As String, LocalDirectory As String) As Boolean
        Dim result As Boolean = False

        Try
            FTPSClnt.UploadFile(LocalDirectory & "\" & FileName, FtpDirectory & "/" & FileName, FtpRemoteExists.Overwrite)
            result = True
        Catch ex As Exception

        End Try

        Return result
    End Function

    ''' <summary>
    ''' Verschieben von Dateien von einem Verzeichnis auf dem FTP-Server in ein anderes Verzeichnis auf dem selben Server
    ''' </summary>
    ''' <param name="FileName"></param>
    ''' <param name="SourceDirectory"></param>
    ''' <param name="DestinationDirectory"></param>
    ''' <param name="NewFileName"></param>
    ''' <returns></returns>
    Public Function MoveFile(FileName As String, SourceDirectory As String, DestinationDirectory As String,
                             Optional NewFileName As String = "") As Boolean

        Dim result As Boolean = False

        Try

            ' Laut Entwickler soll die .MoveFile() anstatt .Rename() genommen werden (https://github.com/robinrodricks/FluentFTP/issues/651)
            If NewFileName.Length > 0 Then
                FTPSClnt.MoveFile(SourceDirectory & "/" & FileName, DestinationDirectory & "/" & NewFileName, FtpRemoteExists.Overwrite)
                'FTPClnt.Rename(SourceDirectory & "/" & FileName, DestinationDirectory & "/" & NewFileName)
            Else
                FTPSClnt.MoveFile(SourceDirectory & "/" & FileName, DestinationDirectory & "/" & FileName, FtpRemoteExists.Overwrite)
                'FTPClnt.Rename(SourceDirectory & "/" & FileName, DestinationDirectory & "/" & FileName)
            End If

            result = True

        Catch ex As Exception
            NLOGLOGGER.Fatal("MoveFile abnormal ending")
            NLOGLOGGER.Fatal(ex.Message)
            NLOGLOGGER.Fatal(ex.InnerException)
        End Try

        Return result
    End Function

    ''' <summary>
    ''' Herunterladen und optionales Verschieben von Dateien aus einem Verzeichnis auf einem FTP-Server 
    ''' </summary>
    ''' <param name="FtpServer"></param>
    ''' <param name="DirectoryName"></param>
    ''' <param name="LocalDirectory"></param>
    ''' <param name="FileNameExtension"></param>
    ''' <param name="MoveDirectory"></param>
    ''' <returns></returns>
    Public Function ConnectWithDownload(FtpServer As FtpServer, DirectoryName As String, LocalDirectory As String,
                                        Optional FileNameExtension As String = "", Optional MoveDirectory As String = "",
                                        Optional DaysCount As Integer = 0, Optional Sorting As Boolean = False) As List(Of String)

        Dim result As New List(Of String)
        Dim _FileListFtp As List(Of String)
        Dim _DownloadResult As Boolean = False
        Dim _DateCompareResult As Integer
        Dim _FilesForDownload As New List(Of String)


        Try
            NLOGLOGGER.Debug("ConnectWithDownload (FTPS) ...")
            NLOGLOGGER.Debug("FTP-Server       : {@0}", FtpServer)
            NLOGLOGGER.Debug("DirectoryName    : " & DirectoryName)
            NLOGLOGGER.Debug("LocalDirectory   : " & LocalDirectory)
            NLOGLOGGER.Debug("FileNameExtension: " & FileNameExtension)
            NLOGLOGGER.Debug("MoveDirectory    : " & MoveDirectory)
            NLOGLOGGER.Debug("DaysCount        : " & DaysCount)

            ' Connect
            Connect(FtpServer)
            If FTPSClnt.IsConnected = False Then
                NLOGLOGGER.Fatal("No connection is etablished")
            Else
                NLOGLOGGER.Info("Connection to FTP is successfull")
            End If


            ' Alle Dateien herunterladen
            '   0 = alle Dateien herunterladen
            '  >1 = Alle Dateien, die eine bestimmte Anzahl von Tagen abgelegt sind
            '  -1 = die aktuelleste Datei herunterladen
            Select Case DaysCount
                Case -1

                    ' Die aktuellste Datei in dem angegebenen Verzeichnis auslesen
                    _FileListFtp = GetNewestFtpFile(DirectoryName, FileNameExtension)
                    NLOGLOGGER.Info("Get the latest File in Directory '" & DirectoryName & "' with Fileextension '" & FileNameExtension & "'")

                Case Else

                    ' Dateien im Verzeichnis auslesen
                    _FileListFtp = GetListOfFtpFiles(DirectoryName, FileNameExtension)
                    NLOGLOGGER.Info("Found " & _FileListFtp.Count & " Files in Directory '" & DirectoryName & "' with Fileextension '" & FileNameExtension & "'")

            End Select


            If _FileListFtp.Count > 0 Then

                For Each FtpFileName As String In _FileListFtp

                    If DaysCount > 0 Then
                        _DateCompareResult = Date.Compare(GetFtpFileDate(DirectoryName & "\" & FtpFileName).Date, Date.Now.AddDays(-DaysCount).Date)
                        If _DateCompareResult > 0 Then

                            NLOGLOGGER.Info("File '" & DirectoryName & "\" & FtpFileName & "' will not be downloaded while to young")
                            NLOGLOGGER.Info("Filedate: " & FormatDateTime(GetFtpFileDate(DirectoryName & "\" & FtpFileName), DateFormat.ShortDate) &
                                             " Needed Value:" & FormatDateTime(Date.Now.AddDays(-DaysCount), DateFormat.ShortDate))
                            Continue For
                        End If
                    End If

                    ' Liste mit der Dateien zum Download erstellen
                    _FilesForDownload.Add(FtpFileName)


                Next

                ' Herunterladen der Dateiliste
                NLOGLOGGER.Trace("Try to download " & _FilesForDownload.Count & " files")
                _DownloadResult = DownloadFtpFiles(DirectoryName, _FilesForDownload, LocalDirectory)

                ' Verschieben der Dateien
                If _DownloadResult = True Then

                    NLOGLOGGER.Info("Successfull download files")

                    ' Verbindung vorbereiten
                    If MoveDirectory.Length > 0 Then
                        If FTPSClnt.IsConnected = False Then
                            Connect(FtpServer)
                        End If
                    End If

                    For Each FtpFileName As String In _FilesForDownload
                        result.Add(FtpFileName)

                        ' Verschieben der heruntergeladenden Dateien
                        If MoveDirectory.Length > 0 Then
                            NLOGLOGGER.Trace("Try to move " & DirectoryName & "/" & FtpFileName & " to " & MoveDirectory)
                            If MoveFile(FtpFileName, DirectoryName, MoveDirectory) = True Then
                                NLOGLOGGER.Info("Successfull move " & DirectoryName & "/" & FtpFileName & " to " & MoveDirectory)
                            Else
                                NLOGLOGGER.Error("Error moving " & DirectoryName & "/" & FtpFileName & " to " & MoveDirectory)
                            End If
                        End If
                    Next

                Else
                    NLOGLOGGER.Error("Error by download files")
                End If
            End If

            If result.Count <> _FileListFtp.Count Then
                ' TODO: Logging des Fehlers
                NLOGLOGGER.Error("Result-Count: " & result.Count & " - Files on FTP-Server Count: " & _FileListFtp.Count)
            End If

            ' Sortieren der geladenen Dateien
            If Sorting = True Then
                NLOGLOGGER.Info("... Sorting Filelist")
                result.Sort()

                ' Ergebniss protokolieren
                For Each _filename As String In result
                    NLOGLOGGER.Debug("==> " & _filename)
                Next
            End If

            FTPSClnt.Disconnect()

        Catch ex As Exception

            NLOGLOGGER.Fatal(ex.Message)
            NLOGLOGGER.Fatal(ex.InnerException)

            FTPSClnt.Disconnect()

        End Try

        Return result

    End Function

    ''' <summary>
    ''' Herunterladen und optionales Verschieben von Dateien aus einem Verzeichnis auf einem FTP-Server 
    ''' </summary>
    ''' <param name="FtpServer"></param>
    ''' <param name="DirectoryName"></param>
    ''' <param name="LocalDirectory"></param>
    ''' <returns></returns>
    Public Function ConnectWithUpload(FtpServer As FtpServer, DirectoryName As String, FileList As List(Of String),
                                      Optional LocalDirectory As String = "", Optional RemoveLocalFile As Boolean = False) As Boolean

        Dim result As Boolean = False


        Try
            ' Connect
            Connect(FtpServer)

            NLOGLOGGER.Info("Uploading Files with FTPS ...")
            For Each FileItem As String In FileList

                If LocalDirectory.Length > 0 Then

                    NLOGLOGGER.Trace("Try to upload: " & FileItem)
                    FTPSClnt.UploadFile(LocalDirectory & "\" & FileItem, DirectoryName & "/" & FileItem, FtpRemoteExists.Overwrite)

                    If FTPSClnt.FileExists(DirectoryName & "/" & FileItem) = True Then
                        NLOGLOGGER.Trace("Successfull upload: " & DirectoryName & "/" & FileItem)
                        result = True
                    Else
                        NLOGLOGGER.Fatal("Upload not successfull: " & DirectoryName & "/" & FileItem)
                        Exit For
                    End If

                Else
                    ' Wenn in der Dateiliste der komplette Pfad ist

                End If

                result = True
            Next



        Catch ex As Exception

        End Try

        Return result

    End Function
End Class
