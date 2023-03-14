Imports System.Data.Objects
Imports System.Data.Entity
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.Common
Imports System.IO
Imports System.Data.Entity.Core.Objects

Public Class MsSql

#Region "EF6 - DbContext"


    Public Shared Function DeleteTabelData(DbContext As DbContext, tables As List(Of String)) As ServerResult
        Dim _result As New ServerResult
        Dim Command As String = ""

        Try

            If tables.Count > 0 Then
                For Each TableItem As String In tables
                    Command = "DELETE [" + TableItem + "] WHERE CanNotDelete='False'"
                    NLOGLOGGER.Debug(Command)
                    DbContext.Database.SqlQuery(Of Object)(Command).ToList()
                Next
            End If
        Catch ex As Exception
            NLOGLOGGER.Fatal(Command)
            _result.ErrorMessages.Add(Command)
        End Try

        Return _result
    End Function

    Public Shared Sub SetIndexGuid(DbContext As DbContext, TableName As String, IndexColumn As String, SearchColumn As String, SearchValue As String, Optional IndexValue As String = "")
        Dim Command As String = ""

        If IndexValue.Length = 0 Then
            IndexValue = System.Guid.Empty.ToString
        End If

        Command = "UPDATE [" + TableName + "] SET [" + IndexColumn + "] = '" + IndexValue + "' WHERE [" + SearchColumn + "] = '" + SearchValue + "'"
        NLOGLOGGER.Debug(Command)
        Try
            DbContext.Database.SqlQuery(Of Object)(Command).ToList()
        Catch ex As Exception

        End Try

    End Sub

    <Obsolete>
    Public Shared Function GetMaxFieldSize(DbContext As DbContext, TableName As String, ColumnName As String) As Integer
        Dim Command As String = ""
        Dim result As Integer

        Command = "SELECT character_maximum_length FROM information_schema.columns WHERE table_name = '" & TableName & "' AND column_name = '" & ColumnName & "'"
        NLOGLOGGER.Debug(Command)

        Try
            result = DbContext.Database.SqlQuery(Of Integer)(Command).First()
        Catch ex As Exception

        End Try

        Return result

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="DbContext"></param>
    ''' <param name="Rootkey">HKEY_LOCAL_MACHINE</param>
    ''' <param name="key"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ReadRegistry(DbContext As DbContext, Rootkey As String, key As String, value As String) As String
        Dim Command As String = ""
        Dim _dbResult As List(Of RegResult)
        Dim _result As String = ""

        Command = "DECLARE @RegLoc VARCHAR(200)" & vbCrLf
        Command = Command & "SELECT @RegLoc='" & key & "'" & vbCrLf

        Command = Command & "EXEC [master].[dbo].[xp_regread]    @rootkey='" & Rootkey & "'," & vbCrLf
        Command = Command & "                                    @key=@RegLoc," & vbCrLf
        Command = Command & "                                    @value_name='" & value & "'" & vbCrLf


        NLOGLOGGER.Debug(Command)

        Try
            _dbResult = DbContext.Database.SqlQuery(Of RegResult)(Command).ToList()
            _result = _dbResult(0).Data
        Catch ex As Exception

        End Try

        Return _result


    End Function


    Public Shared Function CreateDBBackupToFile(DbContext As DbContext, DBName As String, PathToStoreBackup As String) As ServerResult

        Dim Command As String = ""
        Dim _result As New ServerResult
        Dim _FileName As String
        Dim _TempPath As String

        ' Das erstellen der Sicherung in einen lokalen Ordner ist notwendig, das der Prozess des MSSQL-Servers keine Rechte hat auf Netzwerkfreigaben zu schreiben
        _TempPath = ReadRegistry(DbContext, "HKEY_LOCAL_MACHINE", "SOFTWARE\Microsoft\Windows NT\CurrentVersion", "SystemRoot") & "\Temp\"

        _FileName = Now.Year & "-" & Format(Now.Month, "00") & "-" & Format(Now.Day, "00") & "_" & Format(Now.Hour, "00") & "-" & Format(Now.Minute, "00") & " " & DBName & ".bak"

        Command = "BACKUP DATABASE [" + DBName + "]" & vbCrLf
        Command = Command & "TO  DISK = N'" & _TempPath & _FileName & "'" & vbCrLf
        Command = Command & "WITH NOFORMAT, NOINIT,  NAME = N'" & DBName & "-Vollständig Datenbank Sichern'," & vbCrLf
        Command = Command & "SKIP, NOREWIND, NOUNLOAD, STATS = 10" & vbCrLf

        NLOGLOGGER.Debug(Command)

        Try

            ' Sicherung erstellen
            DbContext.Database.SqlQuery(Of Object)(Command).ToList()
            NLOGLOGGER.Info("Database was successfull backuped")

            ' Kopieren an den vorgesehenen Ort
            If FilesIOFunctions.MoveFile(_TempPath & _FileName, PathToStoreBackup & "\" & _FileName) = False Then
                _result.ErrorMessages.Add("Database is backuped. But moving from '" & _TempPath & "\" & _FileName & "' to '" & PathToStoreBackup & "\" & _FileName & "' is failed")
            End If

        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
            _result.ErrorMessages.Add(ex.Message)

        End Try

        Return _result

    End Function

    Public Shared Function CreateLOGBackupToFile(DbContext As DbContext, DBName As String, PathToStoreBackup As String) As ServerResult

        Dim Command As String = ""
        Dim _result As New ServerResult
        Dim _FileName As String
        Dim _TempPath As String

        ' Das erstellen der Sicherung in einen lokalen Ordner ist notwendig, das der Prozess des MSSQL-Servers keine Rechte hat auf Netzwerkfreigaben zu schreiben
        _TempPath = ReadRegistry(DbContext, "HKEY_LOCAL_MACHINE", "SOFTWARE\Microsoft\Windows NT\CurrentVersion", "SystemRoot") & "\Temp\"

        _FileName = Now.Year & "-" & Format(Now.Month, "00") & "-" & Format(Now.Day, "00") & "_" & Format(Now.Hour, "00") & "-" & Format(Now.Minute, "00") & " " & DBName & ".trn"

        Command = "BACKUP LOG [" + DBName + "]" & vbCrLf
        Command = Command & "TO  DISK = N'" & _TempPath & _FileName & "'" & vbCrLf
        Command = Command & "WITH NOFORMAT, NOINIT,  NAME = N'" & DBName & "-Transaktionsprotokoll  Sichern'," & vbCrLf
        Command = Command & "SKIP, NOREWIND, NOUNLOAD, STATS = 10" & vbCrLf

        NLOGLOGGER.Debug(Command)

        Try
            DbContext.Database.SqlQuery(Of Object)(Command).ToList()
            NLOGLOGGER.Info("Log was successfull backuped")

            ' Kopieren an den vorgesehenen Ort
            If FilesIOFunctions.MoveFile(_TempPath & _FileName, PathToStoreBackup & "\" & _FileName) = False Then
                _result.ErrorMessages.Add("Log is backuped. But moving from '" & _TempPath & "\" & _FileName & "' to '" & PathToStoreBackup & "\" & _FileName & "' is failed")
            End If

        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
            _result.ErrorMessages.Add(ex.Message)

        End Try
        Return _result

    End Function

    Public Shared Function RestoreDBBackupFromFile(DbContext As DbContext, DBName As String, FileToRestore As String, Optional OnlyBAK As Boolean = False) As ServerResult
        Dim Command As String = ""
        Dim _result As New ServerResult

        ' http://www.sqlteam.com/forums/topic.asp?TOPIC_ID=170474

        ' ALTER DATABASE CongAdminNT SET OFFLINE WITH ROLLBACK IMMEDIATE

        ' RESTORE DATABASE CongAdminNT
        ' FROM DISK = N'C:\Temp\2015-06-04_17-06 CongAdminNT.bak'
        ' WITH REPLACE, RECOVERY

        Command = "ALTER DATABASE [" + DBName + "] SET OFFLINE WITH ROLLBACK IMMEDIATE" & vbCrLf

        Command = Command & "RESTORE DATABASE [" + DBName + "]"
        Command = Command & "FROM  DISK = N'" & FileToRestore & "'" & vbCrLf
        Command = Command & "WITH REPLACE,"
        If OnlyBAK = True Then
            Command = Command & "RECOVERY"
        Else
            Command = Command & "NORECOVERY"
        End If

        NLOGLOGGER.Debug(Command)

        Try

            DbContext.Database.SqlQuery(Of Object)(Command).ToList()
            NLOGLOGGER.Info("Database was successfull restored")

        Catch ex As Exception

            NLOGLOGGER.Fatal(ex.Message)
            _result.ErrorMessages.Add(ex.Message)

        End Try
        Return _result

    End Function

    Public Shared Function RestoreLOGBackupToFile(DbContext As DbContext, DBName As String, FileToRestore As String) As ServerResult

        Dim Command As String = ""
        Dim _result As New ServerResult

        ' http://www.sqlteam.com/forums/topic.asp?TOPIC_ID=170474

        Command = "RESTORE LOG [" + DBName + "]" & vbCrLf
        Command = Command & "FROM  DISK = N'" & FileToRestore & "'" & vbCrLf
        Command = Command & "WITH RECOVERY"

        NLOGLOGGER.Debug(Command)

        Try
            DbContext.Database.SqlQuery(Of Object)(Command).ToList()
            NLOGLOGGER.Info("Log was successfull restored")

        Catch ex As Exception

            NLOGLOGGER.Fatal(ex.Message)
            _result.ErrorMessages.Add(ex.Message)

        End Try
        Return _result

    End Function

#End Region

#Region "EF4 - ObjContext"


    <Obsolete>
    Public Shared Sub DeleteTabelData(ObjContext As ObjectContext, tables As List(Of String))
        Dim Command As String = ""

        If tables.Count > 0 Then
            For Each TableItem As String In tables
                Command = "DELETE [" + TableItem + "] WHERE CanNotDelete='False'"
                NLOGLOGGER.Debug(Command)
                ObjContext.ExecuteStoreCommand(Command)
            Next
        End If

    End Sub

    <Obsolete>
    Public Shared Sub SetIndexGuid(ObjContext As ObjectContext, TableName As String, IndexColumn As String, SearchColumn As String, SearchValue As String, Optional IndexValue As String = "")
        Dim Command As String = ""

        If IndexValue.Length = 0 Then
            IndexValue = System.Guid.Empty.ToString
        End If

        Command = "UPDATE [" + TableName + "] SET [" + IndexColumn + "] = '" + IndexValue + "' WHERE [" + SearchColumn + "] = '" + SearchValue + "'"
        NLOGLOGGER.Debug(Command)
        ObjContext.ExecuteStoreCommand(Command)

    End Sub

    <Obsolete>
    Public Shared Function GetMaxFieldSize(ObjContext As ObjectContext, TableName As String, ColumnName As String) As Integer
        Dim Command As String = ""
        Dim result As Integer

        Command = "SELECT character_maximum_length FROM information_schema.columns WHERE table_name = '" & TableName & "' AND column_name = '" & ColumnName & "'"
        NLOGLOGGER.Debug(Command)
        result = ObjContext.ExecuteStoreQuery(Of Integer)(Command).First()

        Return result

    End Function

    <Obsolete>
    Public Shared Sub CreateDBBackupToFile(ObjContext As ObjectContext, DBName As String, PathToStoreBackup As String)
        Dim Command As String = ""

        Command = "BACKUP DATABASE [" + DBName + "]" & vbCrLf
        Command = Command & "TO  DISK = N'" & PathToStoreBackup & "\" & Now.Year & "-" & Format(Now.Month, "00") & "-" & Format(Now.Day, "00") & "_" & Format(Now.Hour, "00") & "-" & Format(Now.Minute, "00") & " " & DBName & ".bak'" & vbCrLf
        Command = Command & "WITH NOFORMAT, NOINIT,  NAME = N'" & DBName & "-Vollständig Datenbank Sichern'," & vbCrLf
        Command = Command & "SKIP, NOREWIND, NOUNLOAD, STATS = 10" & vbCrLf

        NLOGLOGGER.Debug(Command)

        ObjContext.ExecuteStoreCommand(Command)

    End Sub

    <Obsolete>
    Public Shared Sub CreateLOGBackupToFile(ObjContext As ObjectContext, DBName As String, PathToStoreBackup As String)
        Dim Command As String = ""

        Command = "BACKUP LOG [" + DBName + "]" & vbCrLf
        Command = Command & "TO  DISK = N'" & PathToStoreBackup & "\" & Now.Year & "-" & Format(Now.Month, "00") & "-" & Format(Now.Day, "00") & "_" & Format(Now.Hour, "00") & "-" & Format(Now.Minute, "00") & " " & DBName & ".trn'" & vbCrLf
        Command = Command & "WITH NOFORMAT, NOINIT,  NAME = N'" & DBName & "-Transaktionsprotokoll  Sichern'," & vbCrLf
        Command = Command & "SKIP, NOREWIND, NOUNLOAD, STATS = 10" & vbCrLf

        NLOGLOGGER.Debug(Command)

        ObjContext.ExecuteStoreCommand(Command)

    End Sub

#End Region

End Class

Public Class RegResult

    Public Property Value As String

    Public Property Data As String

End Class