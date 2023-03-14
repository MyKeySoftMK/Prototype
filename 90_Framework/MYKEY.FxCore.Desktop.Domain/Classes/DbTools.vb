Imports MYKEY.FxCore
Imports MYKEY.FxCore.WpfMvvm
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting
Imports System.Reflection
Imports MYKEY.FxCore.Common.Application
Imports MYKEY.FxCore.EntityFramework
Imports System.Data.Objects
Imports System.Data.Common
Imports System.Data.Entity.Infrastructure


Public Class DbTools

    'Public ReadOnly Property NLogArray As ObservableCollection(Of String)
    '    Get
    '        Return NLogEntries
    '    End Get
    'End Property

    Private Property ServerData As New ServerApplicationSettings
    Private Tables As New List(Of String)
    Dim dbCtx As FxNTSettings.Entities
    Dim DBConn As ConnectionStringSqlClientData

    Public Sub New()

        ' Wir benötigen ein Connection-Objekt um mit der Datenbank zu kommunizieren. Da die Commands
        ' durch einen Direkten SQL-Befehl ausgelöst wird, es es egal, welches EDM wir benutzen
        DBConn = New ConnectionStringSqlClientData
        With DBConn
            .DatabaseModelName = "FxNTSettings.FxNTSettingsEDM"
            .DatabaseModelAssembly = "MYKEY.FxNT.DataAccess"

            .DatabaseName = ServerData.DatabaseName
            .DatabaseUserName = ServerData.DatabaseUserName
            .DatabasePassword = ServerData.DatabasePassword
            .ServerName = ServerData.DatabaseServer
            .DatabaseServerPort = ServerData.DatabasePort
        End With

        dbCtx = New FxNTSettings.Entities(DBConn.EntityConnectionString)

    End Sub


    Public Sub AddDatabaseLogging()

        Dim NlogTargetDatabaseFramework As NLog.Targets.DatabaseTarget
        Dim TargetParam As NLog.Targets.DatabaseParameterInfo
        Dim LogFrameworkToDatabaseRule As NLog.Config.LoggingRule

        Dim NlogTargetDatabaseApplication As NLog.Targets.DatabaseTarget
        Dim LogApplicationToDatabaseRule As NLog.Config.LoggingRule

        ' Zusätzliches Loggen der Programmausführung
        If ServerData.LogFrameworkToDatabase = True Then

            NlogTargetDatabaseFramework = New NLog.Targets.DatabaseTarget
            With NlogTargetDatabaseFramework
                .DBProvider = "mssql"

                .DBHost = ServerData.DatabaseServer & ":" & ServerData.DatabasePort
                .DBDatabase = ServerData.DatabaseName
                .DBUserName = ServerData.DatabaseUserName
                .DBPassword = ServerData.DatabasePassword

                .CommandText = "INSERT INTO [" & ServerData.LogTableFramework & "] (NLogEntry,DateTimeStamp,LogLevel,Logger,LogMessage) VALUES (@EntryId,@DateStamp,@Level,@Logger,@Message);"

                TargetParam = New NLog.Targets.DatabaseParameterInfo
                With TargetParam
                    .Name = "@EntryId"
                    .Layout = Format(Now, "yyyyMMddhhmmssfff").Trim  ' 2013-10-29 17:07:08,299 = 20131029170708299
                End With
                .Parameters.Add(TargetParam)

                TargetParam = New NLog.Targets.DatabaseParameterInfo
                With TargetParam
                    .Name = "@DateStamp"
                    .Layout = "${date}"
                End With
                .Parameters.Add(TargetParam)

                TargetParam = New NLog.Targets.DatabaseParameterInfo
                With TargetParam
                    .Name = "@Level"
                    .Layout = "${level}"
                End With
                NlogTargetDatabaseFramework.Parameters.Add(TargetParam)

                TargetParam = New NLog.Targets.DatabaseParameterInfo
                With TargetParam
                    .Name = "@Logger"
                    .Layout = "${logger}"
                End With
                .Parameters.Add(TargetParam)

                TargetParam = New NLog.Targets.DatabaseParameterInfo
                With TargetParam
                    .Name = "@Message"
                    .Layout = "${message}"
                End With
                .Parameters.Add(TargetParam)
            End With

            ' Hinzufügen der Konfiguration
            NLOGLOGGERConfiguration.AddTarget("databaseFXXT", NlogTargetDatabaseFramework)
            LogFrameworkToDatabaseRule = New NLog.Config.LoggingRule("*", NLog.LogLevel.FromOrdinal(ServerData.LogFrameworkDetails), NlogTargetDatabaseFramework)
            NLOGLOGGERConfiguration.LoggingRules.Add(LogFrameworkToDatabaseRule)

        End If

        ' Logging der Anwendungsinformationen
        NlogTargetDatabaseApplication = New NLog.Targets.DatabaseTarget
        With NlogTargetDatabaseApplication
            .DBProvider = "mssql"

            .DBHost = ServerData.DatabaseServer
            .DBDatabase = ServerData.DatabaseName
            .DBUserName = ServerData.DatabaseUserName
            .DBPassword = ServerData.DatabasePassword

            .CommandText = "INSERT INTO [" & ServerData.LogTableApplication & "] (NLogEntry,DateTimeStamp,LogLevel,Logger,LogMessage) VALUES (@EntryId,@DateStamp,@Level,@Logger,@Message);"

            TargetParam = New NLog.Targets.DatabaseParameterInfo
            With TargetParam
                .Name = "@EntryId"
                .Layout = Format(Now, "yyyyMMddhhmmssfff").Trim  ' 2013-10-29 17:07:08,299 = 20131029170708299
            End With
            .Parameters.Add(TargetParam)

            TargetParam = New NLog.Targets.DatabaseParameterInfo
            With TargetParam
                .Name = "@DateStamp"
                .Layout = "${date}"
            End With
            .Parameters.Add(TargetParam)

            TargetParam = New NLog.Targets.DatabaseParameterInfo
            With TargetParam
                .Name = "@Level"
                .Layout = "${level}"
            End With
            .Parameters.Add(TargetParam)

            TargetParam = New NLog.Targets.DatabaseParameterInfo
            With TargetParam
                .Name = "@Logger"
                .Layout = "${logger}"
            End With
            .Parameters.Add(TargetParam)

            TargetParam = New NLog.Targets.DatabaseParameterInfo
            With TargetParam
                .Name = "@Message"
                .Layout = "${message}"
            End With
            .Parameters.Add(TargetParam)
        End With

        ' Hinzufügen der Konfiguration
        NLOGLOGGERConfiguration.AddTarget("databaseAPPLICATION", NlogTargetDatabaseApplication)
        LogApplicationToDatabaseRule = New NLog.Config.LoggingRule("Application", NLog.LogLevel.Trace, NlogTargetDatabaseApplication)
        NLOGLOGGERConfiguration.LoggingRules.Add(LogApplicationToDatabaseRule)

    End Sub


    ''' <summary>
    ''' Erstellt eine Kopie der Datenbank in einem Verzeichnis
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CreateDBBackupToFile() As ServerResult

        Dim _result As New ServerResult
        Dim _dbResult As ServerResult

        Try
            _dbResult = MsSql.CreateDBBackupToFile(dbCtx, ServerData.DatabaseName, ServerData.DirectoryForDBBackup)
            If _dbResult.HasErrors = True Then
                For Each mesg As String In _dbResult.ErrorMessages
                    _result.ErrorMessages.Add(mesg)
                    Return _result
                    Exit Function
                Next
            End If

            _dbResult = MsSql.CreateLOGBackupToFile(dbCtx, ServerData.DatabaseName, ServerData.DirectoryForDBBackup)
            If _dbResult.HasErrors = True Then
                For Each mesg As String In _dbResult.ErrorMessages
                    _result.ErrorMessages.Add(mesg)
                    Return _result
                    Exit Function
                Next
            End If

            'MsSql.CreateDBBackupToFile(dbCtx, ServerData.ReportServer_ReportingServerDatabaseName, ServerData.Maintenance_DirectoryForDBBackup)
            'MsSql.CreateDBBackupToFile(dbCtx, ServerData.ReportServer_ReportingServerDatabaseName & "TempDB", ServerData.Maintenance_DirectoryForDBBackup)

        Catch ex As Exception

            NLOGLOGGER.Fatal(ex.Message)
            _result.ErrorMessages.Add(ex.Message)

        End Try

        Return _result

    End Function

    Public Function RestoreDBBackupFromFile(BakFileName As String, LogFileName As String) As ServerResult

        Dim _result As New ServerResult
        Dim _dbResult As ServerResult

        Try

            If LogFileName.Length = 0 Then
                ' Nur die BAK zurücklesen
                _dbResult = MsSql.RestoreDBBackupFromFile(dbCtx, ServerData.DatabaseName, BakFileName, True)
                If _dbResult.HasErrors = True Then
                    For Each mesg As String In _dbResult.ErrorMessages
                        _result.ErrorMessages.Add(mesg)
                        Return _result
                        Exit Function
                    Next
                End If

            Else
                ' Erst die BAK zurücklesen und dann die Transaktionlogs einlesen
                _dbResult = MsSql.RestoreDBBackupFromFile(dbCtx, ServerData.DatabaseName, BakFileName, False)
                If _dbResult.HasErrors = True Then
                    For Each mesg As String In _dbResult.ErrorMessages
                        _result.ErrorMessages.Add(mesg)
                        Return _result
                        Exit Function
                    Next
                End If

                _dbResult = MsSql.RestoreLOGBackupToFile(dbCtx, ServerData.DatabaseName, LogFileName)
                If _dbResult.HasErrors = True Then
                    For Each mesg As String In _dbResult.ErrorMessages
                        _result.ErrorMessages.Add(mesg)
                        Return _result
                        Exit Function
                    Next
                End If

            End If

        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
            _result.ErrorMessages.Add(ex.Message)

        End Try

        Return _result

    End Function

    Public Function ClearApplicationTables() As ServerResult

        Tables.Add("CongAdminNTAttendees")
        Tables.Add("CongAdminNTAttendeesSpecial")

        Tables.Add("CongAdminNTAppointments")

        Tables.Add("CongAdminNTAdditionalTexts")

        Tables.Add("CongAdminNTCirculationsToPublishers")
        Tables.Add("CongAdminNTCirculations")

        Tables.Add("CongAdminNTServiceReports")
        Tables.Add("CongAdminNTServiceReportsMissing")

        Tables.Add("CongAdminNTPioniers")

        Tables.Add("CongAdminNTTalks")

        Tables.Add("CongAdminNTVisits")

        Tables.Add("CongAdminNTCongsLangsPubls")
        Tables.Add("CongAdminNTCongsToLangs")
        Tables.Add("CongAdminNTLanguages")

        Tables.Add("CongAdminNTNLog")

        Tables.Add("CongAdminNTPublisherToPrivileges")
        Tables.Add("CongAdminNTPublishersAdditionalInformations")
        Tables.Add("CongAdminNTCongsGroupsToPublishers")
        Tables.Add("CongAdminNTPublishers")

        Tables.Add("CongAdminNTCongsGroups")
        Tables.Add("CongAdminNTCongs")

        Tables.Add("CongAdminNTKingdomHalls")

        Return MsSql.DeleteTabelData(dbCtx, Tables)

    End Function

    Public Function ClearSystemTables() As ServerResult

        Tables.Add("FxNTAdditionalTypes")

        Tables.Add("FxNTNLog")

        Tables.Add("FxNTOLBMenuEntryToUserGroups")
        Tables.Add("FxNTOLBMenuEntries")
        Tables.Add("FxNTOLBMenuGroups")

        Tables.Add("FxNTPrintToPrintGroups")
        Tables.Add("FxNTPrintToInputControls")
        Tables.Add("FxNTPrintInputControls")
        Tables.Add("FxNTPrint")
        Tables.Add("FxNTPrintGroups")

        Tables.Add("FxNTSettings")

        Tables.Add("FxNTUserGroupToUserGroupRole")
        Tables.Add("FxNTUserGroupRoles")
        Tables.Add("FxNTUserToUserGroups")
        Tables.Add("FxNTUserGroups")
        Tables.Add("FxNTUsers")

        Tables.Add("FxNTGroupLists")

        Tables.Add("FxNTAutomationsExecutions")
        Tables.Add("FxNTAutomations")

        Tables.Add("FxNTCheckListsTemplatesSteps")
        Tables.Add("FxNTCheckListsTemplates")
        Tables.Add("FxNTCheckListsSteps")
        Tables.Add("FxNTCheckLists")

        Return MsSql.DeleteTabelData(dbCtx, Tables)

    End Function

    Public Function GetDBVersion(VersionTabel As String) As DbDeployEntry

        Dim _result As New DbDeployEntry
        Dim Command As String = ""
        Dim queryResult As Object

        Try
            Command = "SELECT TOP(1) Folder, ScriptNumber, ScriptStatus FROM [" + VersionTabel + "] ORDER BY ChangeId DESC"
            Console.WriteLine(Command)

            queryResult = dbCtx.Database.SqlQuery(Of DbDeployEntry)(Command).ToList


            If queryResult IsNot Nothing Then

                If queryResult.Count = 1 Then

                    With _result
                        .Folder = queryResult(0).Folder
                        .ScriptNumber = queryResult(0).ScriptNumber
                        .ScriptStatus = queryResult(0).ScriptStatus
                    End With

                End If

            End If

        Catch ex As Exception

            _result.ErrorResult = ex.InnerException.Message

        End Try

        Return _result

    End Function
End Class
