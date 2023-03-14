Imports System.Data
Imports MYKEY.FxCore
Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.Common.Application

Public Class ServerApplicationSettings

    Implements IApplicationSettings

    Private ConfigDataSet As DataSet = Nothing
    Private ConfigName As String = "CongAdminNTServer"
    Private ConfigPath As String = ApplicationSettings.INTERNALAPPDATA
    Private ConfigFileName As String = ConfigName & ".fxcfg"

#Region "Initialize"

    Public Sub New()

        Initialize(ConfigName, ConfigPath)

    End Sub

    Public Sub Initialize(ConfigNameInit As String, ConfigPathInit As String) Implements MYKEY.FxCore.Common.Application.IApplicationSettings.Initialize
        If ConfigDataSet Is Nothing Then
            ConfigName = ConfigNameInit
            ConfigPath = ConfigPathInit
            ConfigDataSet = Settings.Initialize(ConfigName, ConfigPath)
        End If
    End Sub

    Public Sub DeInitialize() Implements MYKEY.FxCore.Common.Application.IApplicationSettings.DeInitialize

        If ConfigDataSet IsNot Nothing Then
            Settings.DeInitialize(ConfigName, ConfigPath)
        End If

    End Sub

#End Region

#Region "Properties"

#Region "Server"


    Public Property DirectoryForDBBackup As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Directories", "DirectoryForDBBackup", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Directories", "DirectoryForDBBackup", "C:\Temp")
        End Get
    End Property


#End Region

#Region "Database"

    Public Property DatabaseName As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Database", "DatabaseName", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Database", "DatabaseName", "kmwrkflw")
        End Get
    End Property

    Public Property DatabaseUserName As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Database", "DatabaseUserName", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Database", "DatabaseUserName", "root")
        End Get
    End Property

    Public Property DatabasePassword As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Database", "DatabasePassword", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Database", "DatabasePassword", "pwMySQLAdmin")
        End Get
    End Property

    Public Property DatabaseServer As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Database", "DatabaseServer", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Database", "DatabaseServer", "192.168.192.57")
        End Get
    End Property

    Public Property DatabasePort As Integer
        Set(value As Integer)
            Settings.SetOption(ConfigDataSet, "Database", "DatabasePort", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Database", "DatabasePort", 3306)
        End Get
    End Property

    Public Property SoftDelete As Boolean
        Set(value As Boolean)
            Settings.SetOption(ConfigDataSet, "Database", "SoftDelete", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Database", "SoftDelete", False)
        End Get
    End Property

    Public Property EF6Logging As Boolean
        Set(value As Boolean)
            Settings.SetOption(ConfigDataSet, "Database", "EF6Logging", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Database", "EF6Logging", False)
        End Get
    End Property

#End Region

#Region "SSRS"

    'Public Property ReportingServer As String
    '    Set(value As String)
    '        Settings.SetOption(ConfigDataSet, "SSRS", "ReportingServer", value)
    '        Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
    '    End Set
    '    Get
    '        Return Settings.GetOption(ConfigDataSet, "SSRS", "ReportingServerUrl", "localhost/RSSQLEXPRESS110")
    '    End Get
    'End Property

    'Public Property ReportingServerApplicationRoot As String
    '    Set(value As String)
    '        Settings.SetOption(ConfigDataSet, "SSRS", "ReportingServerApplicationRoot", value)
    '        Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
    '    End Set
    '    Get
    '        Return Settings.GetOption(ConfigDataSet, "SSRS", "ReportingServerApplicationRoot", "/ERP2folioXT")
    '    End Get
    'End Property

    'Public Property ReportingServerDatabaseName As String
    '    Set(value As String)
    '        Settings.SetOption(ConfigDataSet, "SSRS", "ReportingServerDatabaseName", value)
    '        Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
    '    End Set
    '    Get
    '        Return Settings.GetOption(ConfigDataSet, "SSRS", "ReportingServerDatabaseName", "00ReportServer")
    '    End Get
    'End Property
#End Region

#Region "Jasper"

    'Public Property JasperReportServerURL As String
    '    Set(value As String)
    '        Settings.SetOption(ConfigDataSet, "JasperServer", "JasperReportServerURL", value)
    '        Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
    '    End Set
    '    Get
    '        Return Settings.GetOption(ConfigDataSet, "JasperServer", "JasperReportServerURL", "http://192.168.2.250:8080/jasperserver")
    '    End Get
    'End Property

    'Public Property JasperReportServerUsername As String
    '    Set(value As String)
    '        Settings.SetOption(ConfigDataSet, "JasperServer", "JasperReportServerUsername", value)
    '        Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
    '    End Set
    '    Get
    '        Return Settings.GetOption(ConfigDataSet, "JasperServer", "JasperReportServerUsername", "jasperadmin")
    '    End Get
    'End Property

    'Public Property JasperReportServerPassword As String
    '    Set(value As String)
    '        Settings.SetOption(ConfigDataSet, "JasperServer", "JasperReportServerPassword", value)
    '        Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
    '    End Set
    '    Get
    '        Return Settings.GetOption(ConfigDataSet, "JasperServer", "JasperReportServerPassword", "jasperadmin")
    '    End Get
    'End Property

    Public Property JasperReportJrxmlRootDirectory As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "JasperReports", "JasperReportJrxmlRootDirectory", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "JasperReports", "JasperReportJrxmlRootDirectory", "C:\Program Files (x86)\MyKey-Soft\CongAdminNT\CongAdminNT UI")
        End Get
    End Property

#End Region


#Region "Mail"

    Public Property MailTemplateRootDirectory As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Mail", "TemplateRootDirectory", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Mail", "TemplateRootDirectory", "C:\Program Files (x86)\MyKey-Soft\CongAdminNT\CongAdminNT UI")
        End Get
    End Property

#End Region


#Region "NLogTarget - Database"

    Public Property LogFrameworkToDatabase As Boolean
        Set(value As Boolean)
            Settings.SetOption(ConfigDataSet, "NLog", "LogFrameworkToDatabase", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "NLog", "LogFrameworkToDatabase", False)
        End Get
    End Property

    Public Property LogTableFramework As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "NLog", "LogTableFramework", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "NLog", "LogTableFramework", "FxNTNLog")
        End Get
    End Property

    Public Property LogFrameworkDetails As NLogEnums.LogLevels
        Set(value As NLogEnums.LogLevels)
            Settings.SetOption(ConfigDataSet, "NLog", "LogFrameworkDetails", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "NLog", "LogFrameworkDetails", NLogEnums.LogLevels.Info)
        End Get
    End Property

    Public Property LogTableApplication As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "NLog", "LogTableApplication", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "NLog", "LogTableApplication", "CongAdminNTNLog")
        End Get
    End Property

#End Region


#End Region

End Class
