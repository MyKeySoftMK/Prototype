Imports WinSCP

<Obsolete>
Public Class WinScpClientFunctions

    ' 2023-02-24 MK
    ' =============

    ' Diese Klasse und die dazugehörigen Dateien sind durch die Verwendung von Fluent 40.0+ obsolet.
    ' Dennoch belassen wir diese Klasse in der DLL aus a) Kompatiblitätsgründen und b) um eine Alternative
    ' bei Problemen mit der FTPS-Klasse zu haben. 

    ' Deshalb auch zukünftig die Auswahlmöglichkeit "WSCP" in der INI zulassen und konfigurieren

    Private Shared NLOGLOGGER As NLog.Logger


    Public Sub New()
        NLOGLOGGER = NLog.LogManager.GetLogger("System")
        NLOGLOGGER.Debug("Start: WinScpClientFuncs")
    End Sub

    Public Sub New(ByRef NLOGInstance As NLog.Logger)
        NLOGLOGGER = NLOGInstance
        NLOGLOGGER.Debug("Start: WinScpClientFuncs")
    End Sub

    ''' <summary>
    ''' Öffentlicher Zugriff auf die Connection
    ''' </summary>
    ''' <returns></returns>
    Public Property FTPSClnt As Session

    ''' <summary>
    ''' Herstellen einer Verbindung zu einem FTP-Server
    ''' </summary>
    ''' <param name="FtpServer"></param>
    Public Sub Connect(FtpServer As FtpServer)

        Dim FTPCon As New SessionOptions


        Try

            If FTPSClnt Is Nothing Then

                FTPSClnt = New Session

            End If

            With FTPCon
                .Protocol = Protocol.Ftp
                .HostName = FtpServer.ServerAdress
                .UserName = FtpServer.UserName
                .Password = FtpServer.Password
                .PortNumber = FtpServer.Port
                .FtpSecure = FtpSecure.Explicit
                .GiveUpSecurityAndAcceptAnyTlsHostCertificate = True
            End With

            If FTPSClnt.Opened = False Then
                FTPSClnt.Open(FTPCon)
            End If

            NLOGLOGGER.Info("Connection is successfull")


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
        Dim FtpResult As RemoteDirectoryInfo

        FtpResult = FTPSClnt.ListDirectory(DirectoryName)

        For Each ftpFile As RemoteFileInfo In FtpResult.Files
            Select Case ftpFile.IsDirectory
                Case False

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
        Dim FtpResult As RemoteDirectoryInfo
        Dim FileListResult() As RemoteFileInfo
        Dim LatestFile As RemoteFileInfo

        FtpResult = FTPSClnt.ListDirectory(DirectoryName)

        For Each ftpFile As RemoteFileInfo In FtpResult.Files
            Select Case ftpFile.IsDirectory
                Case False

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
            LatestFile = FileListResult.OrderBy(Function(dirItem) dirItem.LastWriteTime).Last

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
        Dim _TransOptions As New TransferOptions
        Dim _TransResult As TransferOperationResult

        Try

            With _TransOptions
                .TransferMode = TransferMode.Binary
            End With


            _TransResult = FTPSClnt.GetFiles(FtpDirectory & "/" & FileName, LocalDirectory & "\" & FileName, False, _TransOptions)
            _TransResult.Check()
            If _TransResult.Transfers.Count > 0 Then
                For Each transfer As TransferEventArgs In _TransResult.Transfers
                    NLOGLOGGER.Info("Successfull download file: " & transfer.FileName)
                Next
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
        Dim _TransOptions As New TransferOptions
        Dim _TransResult As TransferOperationResult

        Try

            If FileNames.Count > 0 Then


                With _TransOptions
                    .TransferMode = TransferMode.Binary
                End With

                For Each fName As String In FileNames

                    _TransResult = FTPSClnt.GetFiles(FtpDirectory & "/" & fName, LocalDirectory & "\" & fName, False, _TransOptions)
                    _TransResult.Check()
                    If _TransResult.Transfers.Count > 0 Then
                        For Each transfer As TransferEventArgs In _TransResult.Transfers
                            NLOGLOGGER.Info("Successfull download file: " & transfer.FileName)
                        Next
                        result = True
                    Else
                        NLOGLOGGER.Error("Error download file:" & FtpDirectory & "/" & fName)
                    End If

                Next

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
        Dim _FileInfo As RemoteFileInfo

        Try

            'result = FTPClnt.GetModifiedTime(FileName, FtpDate.Original)
            _FileInfo = FTPSClnt.GetFileInfo(FileName)
            result = _FileInfo.LastWriteTime

        Catch ex As Exception
            NLOGLOGGER.Fatal("GetFtpFileDate abnormal ending")
            NLOGLOGGER.Fatal(ex.Message)
            NLOGLOGGER.Fatal(ex.InnerException)
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
        Dim _TransOptions As New TransferOptions
        Dim _TransResult As TransferOperationResult

        Try

            With _TransOptions
                .TransferMode = TransferMode.Binary
                .OverwriteMode = OverwriteMode.Overwrite
            End With


            _TransResult = FTPSClnt.PutFiles(LocalDirectory & "\" & FileName, FtpDirectory & "/" & FileName, False, _TransOptions)
            _TransResult.Check()
            If _TransResult.Transfers.Count > 0 Then
                For Each transfer As TransferEventArgs In _TransResult.Transfers
                    NLOGLOGGER.Info("Successfull upload file: " & transfer.FileName)
                Next
                result = True
            Else
                NLOGLOGGER.Error("Error upload file:" & FtpDirectory & "/" & FileName)
            End If

        Catch ex As Exception
            NLOGLOGGER.Fatal("UploadFtpFile abnormal ending")
            NLOGLOGGER.Fatal(ex.Message)
            NLOGLOGGER.Fatal(ex.InnerException)
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

            If NewFileName.Length > 0 Then
                FTPSClnt.MoveFile(SourceDirectory & "/" & FileName, DestinationDirectory & "/" & NewFileName)
            Else
                FTPSClnt.MoveFile(SourceDirectory & "/" & FileName, DestinationDirectory & "/" & FileName)
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
            NLOGLOGGER.Debug("ConnectWithDownload (WinSCP/FTPS) ...")
            NLOGLOGGER.Debug("FTP-Server       : {@0}", FtpServer)
            NLOGLOGGER.Debug("DirectoryName    : " & DirectoryName)
            NLOGLOGGER.Debug("LocalDirectory   : " & LocalDirectory)
            NLOGLOGGER.Debug("FileNameExtension: " & FileNameExtension)
            NLOGLOGGER.Debug("MoveDirectory    : " & MoveDirectory)
            NLOGLOGGER.Debug("DaysCount        : " & DaysCount)

            ' Connect
            Connect(FtpServer)
            If FTPSClnt.Opened = False Then
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
                        If FTPSClnt.Opened = False Then
                            NLOGLOGGER.Trace("Open Connection for moving files")
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

            FTPSClnt.Close()

        Catch ex As Exception

            NLOGLOGGER.Fatal(ex.Message)
            NLOGLOGGER.Fatal(ex.InnerException)
            FTPSClnt.Close()

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

            NLOGLOGGER.Info("Uploading Files with WinSCP/FTPS ...")
            For Each FileItem As String In FileList

                If LocalDirectory.Length > 0 Then

                    Me.UploadFtpFile(DirectoryName, FileItem, LocalDirectory)

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
