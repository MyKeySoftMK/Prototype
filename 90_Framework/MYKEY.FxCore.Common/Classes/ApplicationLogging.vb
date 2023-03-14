Imports System.Collections.ObjectModel
Imports NLog
Imports NLog.Common
Imports NLog.Targets.GraylogHttp

Public Class ApplicationLogging

    Public Shared Property NLOGLOGGER As NLog.Logger
        Set(value As NLog.Logger)

        End Set
        Get
            If _NLOGLOGGER Is Nothing Then
                _NLOGLOGGER = NLog.LogManager.GetLogger("System")
                InitLogging()
            End If
            Return _NLOGLOGGER
        End Get
    End Property
    Shared _NLOGLOGGER As NLog.Logger

    Public Shared Property NLOGAPP As NLog.Logger
        Set(value As NLog.Logger)

        End Set
        Get
            If _NLOGAPP Is Nothing Then
                _NLOGAPP = NLog.LogManager.GetLogger("Application")
            End If
            Return _NLOGAPP
        End Get
    End Property
    Shared _NLOGAPP As NLog.Logger

    Public Shared Property NLOGLOGGERConfiguration As NLog.Config.LoggingConfiguration
        Get
            Return LogManager.Configuration
        End Get
        Set(value As NLog.Config.LoggingConfiguration)
            LogManager.Configuration = value
        End Set
    End Property


    Public Shared Sub InitLogging()

        Dim DbParam As NLog.Targets.DatabaseParameterInfo

        ' 1. Create configuration object
        Dim LogConfig As New NLog.Config.LoggingConfiguration



        ' 2. Create targets and add them to the configuration
        Dim LogTargetDebugger As New NLog.Targets.DebuggerTarget
        LogConfig.AddTarget("debugger", LogTargetDebugger)

        ' 2013-12-18 (MKolowicz)
        ' ======================
        ' Da wir die Logeinträge nicht in dem UI anzeigen müssen, brauchen wir auch
        ' kein Event mehr, was die Collection füllt und das UI über die Änderung unterrichtet

        '' Für Sentinel
        'Dim LogTargetViewer As New NLog.Targets.NLogViewerTarget
        'LogConfig.AddTarget("viewer", LogTargetViewer)

        ' Für Chainsaw/Log2Console
        Dim LogTargetViewer As New NLog.Targets.ChainsawTarget
        LogConfig.AddTarget("viewer", LogTargetViewer)

        ' Für Dateilogging
        ' HINWEIS: Um das Dateilogging zu aktivieren ist es einfacher, wenn die ApplicationLogging-Klasse in das Hauptprojekt übernommen wird
        '          Dadurch ist die Steuerung über Variabeln dann besser möglich
        'Dim LogTargetFile As New NLog.Targets.FileTarget
        'If LOG2FILE_LOGPATH.Length > 0 Then
        '    LogConfig.AddTarget("file", LogTargetFile)
        'End If

        '' Für das Logging in eine MS-SQL Datenbank
        'Dim LogTargetMsSql As New NLog.Targets.DatabaseTarget
        'If LOG2DB_HOST.Length > 0 Then
        '    LogConfig.AddTarget("database", LogTargetMsSql)
        'End If

        '' Für das Logging nach Graylog
        'Dim logTargetGraylog As New NLog.Targets.GraylogHttp.GraylogHttpTarget
        'LogConfig.AddTarget("GraylogHttp", logTargetGraylog)

        ' 3. Set target Properties
        With LogTargetDebugger
            .Layout = "${longdate}|[${level}]|${logger}|${message} ${exception:format=tostring}"
        End With

        With LogTargetViewer
            .Address = "udp://127.0.0.1:9966"
        End With

        'If LOG2FILE_LOGPATH.Length > 0 Then
        '    With LogTargetFile
        '        .Encoding = Encoding.UTF8
        '        .Layout = "${longdate}|${machinename}|${level}|${logger}|${identity}|${message} ${exception:format=tostring}"
        '        .FileName = LOG2FILE_LOGPATH & "\UWC.log"
        '        .ArchiveFileName = LOG2FILE_LOGPATH & "\UWC{#}.log"
        '        .ArchiveNumbering = Targets.ArchiveNumberingMode.Rolling
        '        .ArchiveEvery = Targets.FileArchivePeriod.Day
        '        .MaxArchiveFiles = 14
        '        .ArchiveDateFormat = "yyyyMMdd"
        '        .KeepFileOpen = False
        '        .ConcurrentWrites = True
        '    End With
        'End If

        'If LOG2DB_HOST.Length > 0 Then
        '    With LogTargetMsSql
        '        .DBProvider = "mssql"
        '        .DBHost = LOG2DB_HOST
        '        .DBUserName = LOG2DB_USERNAME
        '        .DBPassword = LOG2DB_PASSWORD
        '        .DBDatabase = LOG2DB_DATABASE

        '        .CommandText = "INSERT INTO " & LOG2DB_DATATABLE & "(time_stamp,level,logger,message) VALUES(@time_stamp, @level, @logger, @message);"
        '    End With

        '    DbParam = New NLog.Targets.DatabaseParameterInfo
        '    With DbParam
        '        .Name = "@time_stamp"
        '        .Layout = "${date}"
        '    End With
        '    LogTargetMsSql.Parameters.Add(DbParam)

        '    DbParam = New NLog.Targets.DatabaseParameterInfo
        '    With DbParam
        '        .Name = "@level"
        '        .Layout = "${level}"
        '    End With
        '    LogTargetMsSql.Parameters.Add(DbParam)

        '    DbParam = New NLog.Targets.DatabaseParameterInfo
        '    With DbParam
        '        .Name = "@logger"
        '        .Layout = "${logger}"
        '    End With
        '    LogTargetMsSql.Parameters.Add(DbParam)

        '    DbParam = New NLog.Targets.DatabaseParameterInfo
        '    With DbParam
        '        .Name = "@message"
        '        .Layout = "${message}"
        '    End With
        '    LogTargetMsSql.Parameters.Add(DbParam)

        'End If

        'With logTargetGraylog
        '    .GraylogServer = "http://10.10.10.26"
        '    .GraylogPort = 12201
        '    .Facility = "KMFEdiPartner"
        '    .Layout = "${longdate}|[${level}]|${message}"
        'End With

        ' 4. Define Rules
        ' LogLevel:  Off, Fatal, Error, Warn, Info, Debug, Trace
        Dim LogRuleDebugger As New NLog.Config.LoggingRule("*", NLog.LogLevel.Debug, LogTargetDebugger)
        LogConfig.LoggingRules.Add(LogRuleDebugger)

        Dim LogRuleViewer As New NLog.Config.LoggingRule("*", NLog.LogLevel.Debug, LogTargetViewer)
        LogConfig.LoggingRules.Add(LogRuleViewer)

        'If LOG2FILE_LOGPATH.Length > 0 Then
        '    Dim LogRuleFile As New NLog.Config.LoggingRule("*", NLog.LogLevel.Trace, LogTargetFile)
        '    LogConfig.LoggingRules.Add(LogRuleFile)
        'End If

        'If LOG2DB_HOST.Length > 0 Then
        '    Dim LogRuleMsSql As New NLog.Config.LoggingRule("*", NLog.LogLevel.Debug, LogTargetMsSql)
        '    LogConfig.LoggingRules.Add(LogRuleMsSql)
        'End If

        'Dim LogRuleGraylog As New NLog.Config.LoggingRule("*", NLog.LogLevel.Trace, logTargetGraylog)
        'LogConfig.LoggingRules.Add(LogRuleGraylog)


        ' 5. Activate the Config
        NLog.LogManager.Configuration = LogConfig

        NLOGLOGGER.Info("**********************************************************")
        NLOGLOGGER.Info("* Logging is started                                     *")
        NLOGLOGGER.Info("**********************************************************")

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="LogFileName">"\IValueCalc.log"</param>
    ''' <param name="LogFileNameArchive">"\IValueCalc{#}.log"</param>
    Public Shared Sub NLOGLOGGERChangeFilePathForLog(LogFileName As String, LogFileNameArchive As String)
        Dim _FileTarget As NLog.Targets.FileTarget
        Try
            _FileTarget = LogManager.Configuration.FindTargetByName("file")
            With _FileTarget
                .FileName = LogFileName
                .ArchiveFileName = LogFileNameArchive
            End With

        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' Get the Status of FileLogging
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function NLOGLOGGERFileLoggingIsActive() As Boolean
        Dim _FileTarget As NLog.Targets.FileTarget
        Dim _result As Boolean = False
        Try
            _FileTarget = LogManager.Configuration.FindTargetByName("file")
            If _FileTarget IsNot Nothing Then
                _result = True
            End If

        Catch ex As Exception

        End Try
        Return _result
    End Function

    ''' <summary>
    ''' Sollte das Logging in Dateien durch z.B. die Streamreader.ReadLine verloren gehen, dann kann man hier den
    ''' Eintrag für das File-Logging wieder hinzufügen lassen
    ''' </summary>
    ''' <param name="CurrentLoggerConfig"></param>
    Public Shared Sub NLOGLOGGERAddMissingFileTarget(CurrentLoggerConfig As NLog.Config.LoggingConfiguration)

        'Dim LogTargetFile As New NLog.Targets.FileTarget
        'If LOG2FILE_LOGPATH.Length > 0 Then
        '    CurrentLoggerConfig.AddTarget("file", LogTargetFile)
        'End If

        'If LOG2FILE_LOGPATH.Length > 0 Then
        '    With LogTargetFile
        '        .Encoding = Encoding.UTF8
        '        .Layout = "${longdate}|${machinename}|${level}|${logger}|${identity}|${message} ${exception:format=tostring}"
        '        .FileName = LOG2FILE_LOGPATH & "\EdiPartner.log"
        '        .ArchiveFileName = LOG2FILE_LOGPATH & "\EdiPartner{#}.log"
        '        .ArchiveNumbering = Targets.ArchiveNumberingMode.Rolling
        '        .ArchiveEvery = Targets.FileArchivePeriod.Day
        '        .MaxArchiveFiles = 14
        '        .ArchiveDateFormat = "yyyyMMdd"
        '        .KeepFileOpen = False
        '        .ConcurrentWrites = True
        '    End With
        'End If

        'If LOG2FILE_LOGPATH.Length > 0 Then
        '    Dim LogRuleFile As New NLog.Config.LoggingRule("*", NLog.LogLevel.Trace, LogTargetFile)
        '    CurrentLoggerConfig.LoggingRules.Add(LogRuleFile)
        'End If

        'NLog.LogManager.Configuration = CurrentLoggerConfig

    End Sub
End Class
