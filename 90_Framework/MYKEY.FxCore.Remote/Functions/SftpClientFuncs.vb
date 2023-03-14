Imports Renci.SshNet
Imports Renci.SshNet.Sftp
Imports System.IO

Public Class SftpClientFuncs

    Private Shared NLOGLOGGER As NLog.Logger

    Public Sub New()
        NLOGLOGGER = NLog.LogManager.GetLogger("System")
        NLOGLOGGER.Debug("Start: SftpClientFuncs")
    End Sub

    Public Sub New(ByRef NLOGInstance As NLog.Logger)
        NLOGLOGGER = NLOGInstance
        NLOGLOGGER.Debug("Start: SftpClientFuncs")
    End Sub

    ''' <summary>
    ''' Öffentlicher Zugriff auf die Connection
    ''' </summary>
    ''' <returns></returns>
    Public Property SFTPClnt As SftpClient

    ''' <summary>
    ''' Herstellen einer Verbindung zu einem FTP-Server
    ''' </summary>
    ''' <param name="FtpServer"></param>
    Public Sub Connect(FtpServer As FtpServer)

        Try

            If SFTPClnt Is Nothing Then


                SFTPClnt = New SftpClient(FtpServer.ServerAdress, FtpServer.Port, FtpServer.UserName, FtpServer.Password)

            End If

            If SFTPClnt.IsConnected = False Then
                SFTPClnt.Connect()
            End If

            NLOGLOGGER.Info("Connection is successfull")
            NLOGLOGGER.Debug("ClientVersion: " & SFTPClnt.ConnectionInfo.ClientVersion)
            NLOGLOGGER.Debug("ServerVersion: " & SFTPClnt.ConnectionInfo.ServerVersion)
            NLOGLOGGER.Debug("CurrentClientCompressionAlgorithm: " & SFTPClnt.ConnectionInfo.CurrentClientCompressionAlgorithm)
            NLOGLOGGER.Debug("CurrentClientEncryption: " & SFTPClnt.ConnectionInfo.CurrentClientEncryption)
            NLOGLOGGER.Debug("CurrentClientHmacAlgorithm: " & SFTPClnt.ConnectionInfo.CurrentClientHmacAlgorithm)
            NLOGLOGGER.Debug("CurrentHostKeyAlgorithm: " & SFTPClnt.ConnectionInfo.CurrentHostKeyAlgorithm)
            NLOGLOGGER.Debug("CurrentKeyExchangeAlgorithm: " & SFTPClnt.ConnectionInfo.CurrentKeyExchangeAlgorithm)
            NLOGLOGGER.Debug("CurrentServerEncryption: " & SFTPClnt.ConnectionInfo.CurrentServerEncryption)
            NLOGLOGGER.Debug("CurrentServerEncryption: " & SFTPClnt.ConnectionInfo.CurrentServerEncryption)
            NLOGLOGGER.Debug("CurrentServerHmacAlgorithm: " & SFTPClnt.ConnectionInfo.CurrentServerHmacAlgorithm)


        Catch ex As Exception
            NLOGLOGGER.Fatal("Connection is abnormal ending")
            NLOGLOGGER.Fatal(ex.Message)
            NLOGLOGGER.Fatal(ex.InnerException)
        End Try

    End Sub

    ''' <summary>
    ''' Hochladen einer einzelnen Datei auf einem FTP-Server
    ''' </summary>
    ''' <param name="FtpDirectory"></param>
    ''' <param name="FileName"></param>
    ''' <param name="LocalDirectory"></param>
    ''' <returns></returns>
    Public Function UploadFtpFile(FtpDirectory As String, FileName As String, LocalDirectory As String) As Boolean
        Dim result As Boolean = False
        Dim fs As FileStream

        Try

            fs = New FileStream(LocalDirectory & "\" & FileName, FileMode.Open)
            If fs IsNot Nothing Then
                SFTPClnt.UploadFile(fs, FtpDirectory & "/" & FileName)
                result = True
            End If
            fs.Close()

        Catch ex As Exception
            NLOGLOGGER.Fatal("Upload is abnormal ending")
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
    ''' <returns></returns>
    Public Function ConnectWithUpload(FtpServer As FtpServer, DirectoryName As String, FileList As List(Of String),
                                      Optional LocalDirectory As String = "", Optional RemoveLocalFile As Boolean = False) As Boolean

        Dim result As Boolean = False
        Dim fs As FileStream

        Try

            ' Connect
            Connect(FtpServer)

            NLOGLOGGER.Info("Uploading Files with SFTP ...")
            For Each FileItem As String In FileList

                If LocalDirectory.Length > 0 Then

                    fs = New FileStream(LocalDirectory & "\" & FileItem, FileMode.Open)
                    If fs IsNot Nothing Then
                        NLOGLOGGER.Trace("Try to upload: " & FileItem)
                        SFTPClnt.UploadFile(fs, DirectoryName & "/" & FileItem)

                        If SFTPClnt.Exists(DirectoryName & "/" & FileItem) = True Then
                            NLOGLOGGER.Trace("Successfull upload: " & DirectoryName & "/" & FileItem)
                            result = True
                            fs.Close()
                        Else
                            NLOGLOGGER.Fatal("Upload not successfull: " & DirectoryName & "/" & FileItem)
                            fs.Close()
                            Exit For
                        End If

                    Else

                        result = True
                    End If

                Else
                    ' Wenn in der Dateiliste der komplette Pfad ist

                End If

                result = True
            Next

        Catch ex As Exception
            NLOGLOGGER.Fatal("Connect and Upload is abnormal ending")
            NLOGLOGGER.Fatal(ex.Message)
            NLOGLOGGER.Fatal(ex.InnerException)
        End Try

        Return result

    End Function

    ''' <summary>
    ''' Erstellen einer Liste aller Dateien in einem Verzeichnis. Optional kann die Dateiendung angegeben werden
    ''' </summary>
    ''' <param name="DirectoryName"></param>
    ''' <param name="FileNameExtension"></param>
    ''' <returns></returns>
    Public Function GetListOfFtpFiles(DirectoryName As String, Optional FileNameExtension As String = "") As List(Of String)

        Dim result As New List(Of String)
        Dim FtpResult() As SftpFile

        FtpResult = SFTPClnt.ListDirectory(DirectoryName)

        For Each ftpFile As SftpFile In FtpResult
            If ftpFile.IsDirectory = False Then
                If FileNameExtension.Length > 0 Then
                    If FileNameExtension.ToUpper = Mid(ftpFile.Name, ftpFile.Name.Length - FileNameExtension.Length + 1).ToUpper Then
                        result.Add(ftpFile.Name)
                    End If
                Else
                    result.Add(ftpFile.Name)
                End If
            End If
        Next

        Return result

    End Function

    Public Function GetNewestFtpFile(DirectoryName As String, Optional FileNameExtension As String = "") As List(Of String)

        Dim result As New List(Of String)
        Dim FtpResult As IEnumerable(Of SftpFile)
        Dim FileListResult() As SftpFile
        Dim LatestFile As SftpFile

        FtpResult = SFTPClnt.ListDirectory(DirectoryName)

        For Each ftpFile As SftpFile In FtpResult

            If ftpFile.IsDirectory = False Then
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
            End If
        Next

        LatestFile = FileListResult.OrderBy(Function(dirItem) dirItem.LastWriteTime).Last

        If LatestFile IsNot Nothing Then
            result.Add(LatestFile.Name)
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
        Dim _DestFile As FileStream

        Try
            _DestFile = File.Open(LocalDirectory & "\" & FileName, FileMode.Create)

            SFTPClnt.DownloadFile(FtpDirectory & "/" & FileName, _DestFile)
            _DestFile.Close()

            NLOGLOGGER.Info("Successfull download file: " & FtpDirectory & "/" & FileName)
            result = True

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

        Try

            If FileNames.Count > 0 Then

                ' Erstellen der Liste mit Dateinamen auf dem FTP-Server
                For Each fName As String In FileNames
                    DownloadFtpFile(FtpDirectory, fName, LocalDirectory)
                Next

            End If

            result = True

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
            result = SFTPClnt.GetLastWriteTime(FileName)

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
        Dim _sourceFile As SftpFile

        Try

            _sourceFile = SFTPClnt.Get(SourceDirectory & "\" & FileName)
            If NewFileName.Length > 0 Then
                _sourceFile.MoveTo(DestinationDirectory & "\" & NewFileName)
            Else
                _sourceFile.MoveTo(DestinationDirectory & "\" & FileName)
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
            NLOGLOGGER.Debug("ConnectWithDownload (SFTP) ...")
            NLOGLOGGER.Debug("FTP-Server       : {@0}", FtpServer)
            NLOGLOGGER.Debug("DirectoryName    : " & DirectoryName)
            NLOGLOGGER.Debug("LocalDirectory   : " & LocalDirectory)
            NLOGLOGGER.Debug("FileNameExtension: " & FileNameExtension)
            NLOGLOGGER.Debug("MoveDirectory    : " & MoveDirectory)
            NLOGLOGGER.Debug("DaysCount        : " & DaysCount)

            ' Connect
            Connect(FtpServer)
            If SFTPClnt.IsConnected = False Then
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
                        If SFTPClnt.IsConnected = False Then
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

            SFTPClnt.Disconnect()

        Catch ex As Exception

            NLOGLOGGER.Fatal(ex.Message)
            NLOGLOGGER.Fatal(ex.InnerException)
            SFTPClnt.Disconnect()

        End Try

        Return result

    End Function

End Class
