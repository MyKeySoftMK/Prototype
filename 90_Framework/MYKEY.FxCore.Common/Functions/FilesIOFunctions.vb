Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports MYKEY.FxCore.Common.ApplicationLogging

Public Class FilesIOFunctions

    Public Function GetFileList(Path As String, Optional SearchLevel As FileIO.SearchOption = FileIO.SearchOption.SearchTopLevelOnly) As List(Of FileInfoEntry)
        Dim _List As New List(Of FileInfoEntry)
        Dim _FileListing As FileInfoEntry
        Dim _IOFileInfo As FileInfo

        NLOGLOGGER.Info("GetFileList for: " & Path)
        For Each File As String In FileSystem.GetFiles(Path, FileIO.SearchOption.SearchTopLevelOnly)

            _FileListing = New FileInfoEntry
            _IOFileInfo = New FileInfo(File)

            With _FileListing
                .FullName = File

                .CRCValue = GetCRC32(File)

                .Name = _IOFileInfo.Name
                .Path = _IOFileInfo.DirectoryName

                .Created = _IOFileInfo.CreationTime

                .Size = _IOFileInfo.Length

                .RelativePath = Mid(.Path, Path.Length + 1)
            End With
            NLOGLOGGER.Debug("=> Name: " & _FileListing.Name)
            NLOGLOGGER.Debug("=> CRC :" & _FileListing.CRCValue)

            _List.Add(_FileListing)
        Next

        NLOGLOGGER.Info("=> Found " & _List.Count & " File(s)")

        Return _List

    End Function

    Public Function GetFileInfo(FilenName As String) As FileInfoEntry
        Dim _IOFileInfo As FileInfo
        Dim _FileData As New FileInfoEntry

        _IOFileInfo = New FileInfo(FilenName)


        With _FileData
            .FullName = FilenName

            .CRCValue = GetCRC32(FilenName)

            .Name = _IOFileInfo.Name
            .Path = _IOFileInfo.DirectoryName

            .Created = _IOFileInfo.CreationTime

            .Size = _IOFileInfo.Length

            '.RelativePath = Mid(.Path, Path.Length + 1)
        End With

        Return _FileData

    End Function

    Public Shared Function GetCRC32(ByVal sFileName As String) As String
        Dim FS As FileStream = New FileStream(sFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 8192)

        Try
            Dim CRC32Result As Integer = &HFFFFFFFF
            Dim Buffer(4096) As Byte
            Dim ReadSize As Integer = 4096
            Dim Count As Integer = FS.Read(Buffer, 0, ReadSize)
            Dim CRC32Table(256) As Integer
            Dim DWPolynomial As Integer = &HEDB88320
            Dim DWCRC As Integer
            Dim i As Integer, j As Integer, n As Integer

            'CRC32 Tabelle erstellen
            For i = 0 To 255
                DWCRC = i
                For j = 8 To 1 Step -1
                    If (DWCRC And 1) Then
                        DWCRC = ((DWCRC And &HFFFFFFFE) \ 2&) And &H7FFFFFFF
                        DWCRC = DWCRC Xor DWPolynomial
                    Else
                        DWCRC = ((DWCRC And &HFFFFFFFE) \ 2&) And &H7FFFFFFF
                    End If
                Next j
                CRC32Table(i) = DWCRC
            Next i

            'CRC32 Hash berechnen
            Do While (Count > 0)
                For i = 0 To Count - 1
                    n = (CRC32Result And &HFF) Xor Buffer(i)
                    CRC32Result = ((CRC32Result And &HFFFFFF00) \ &H100) And &HFFFFFF
                    CRC32Result = CRC32Result Xor CRC32Table(n)
                Next i
                Count = FS.Read(Buffer, 0, ReadSize)
            Loop

            FS.Close()


            Return Hex(Not (CRC32Result))
        Catch ex As Exception
            FS.Close()
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Diese Methode kann verwendet werden, damit immer vor dem Löschen einer Datei auch die Abfrage nach dem 
    ''' Vorhandensein erfolgt und somit kein Fehler ausgelöst wird, falls diese fehlt
    ''' </summary>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DeleteFile(ByVal FileName As String) As Boolean
        Try
            If IO.File.Exists(FileName) = True Then
                IO.File.Delete(FileName)
            End If
            Return True
        Catch
            Return False
        End Try
    End Function

    Public Shared Function ReadFileToString(ByVal TextFile As String) As String
        Dim FileContent As String

        FileContent = FileSystem.ReadAllText(TextFile)

        Return FileContent

    End Function

    Public Shared Function GenerateDirectoryForFile(FileName As String) As String
        Dim _result As String = ""

        Dim path As String = System.IO.Path.GetDirectoryName(FileName)

        If System.IO.Directory.Exists(path) = False Then
            System.IO.Directory.CreateDirectory(path)
            _result = path
        End If

        Return _result

    End Function

    Public Shared Function MoveFile(SourceFile As String, DestinationFile As String) As Boolean
        Dim _result As Boolean = False

        Try
            If File.Exists(SourceFile) = False Then
                ' This statement ensures that the file is created,
                ' but the handle is not kept.
                Dim fs As FileStream = File.Create(SourceFile)
                fs.Close()
            End If

            ' Ensure that the target does not exist.
            If File.Exists(DestinationFile) Then
                File.Delete(DestinationFile)
            End If

            ' Move the file.
            File.Move(SourceFile, DestinationFile)
            Console.WriteLine("{0} moved to {1}", SourceFile, DestinationFile)

            ' See if the original file exists now.
            If File.Exists(SourceFile) Then
                Console.WriteLine("The original file still exists, which is unexpected.")
            Else
                Console.WriteLine("The original file no longer exists, which is expected.")
                _result = True
            End If
        Catch ex As Exception
            Console.WriteLine("The process failed: {0}", ex.ToString())
        End Try

        Return _result
    End Function
End Class
