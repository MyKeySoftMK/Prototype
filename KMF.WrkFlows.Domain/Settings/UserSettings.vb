Imports System.Data
Imports MYKEY.FxCore
Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.Common.Application

Public Class UserSettings
    Implements IApplicationSettings

#Region "Config"


    Private ConfigDataSet As DataSet = Nothing
    Private ConfigName As String = "WrkFlowsUser"
    Private ConfigPath As String = ApplicationSettings.INTERNALAPPDATA
    Private ConfigFileName As String = ConfigName & ".fxcfg"

#End Region

#Region "Init"

    Public Sub New()

        Initialize(ConfigName, ConfigPath)

    End Sub

    Public Sub Initialize(ConfigNameInit As String, ConfigPathInit As String) Implements IApplicationSettings.Initialize
        If ConfigDataSet Is Nothing Then
            ConfigName = ConfigNameInit
            ConfigPath = ConfigPathInit
            ConfigDataSet = Settings.Initialize(ConfigName, ConfigPath)
        End If
    End Sub

    Public Sub DeInitialize() Implements IApplicationSettings.DeInitialize
        If ConfigDataSet IsNot Nothing Then
            Settings.DeInitialize(ConfigName, ConfigPath)
        End If
    End Sub

#End Region

#Region "Allgemein"

    Public Property Common_Printer As String
        Get
            Return Settings.GetOption(ConfigDataSet, "Common", "Printer", "")
        End Get
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Common", "Printer", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
    End Property


    Public Property Common_PDFPath As String
        Get
            Return Settings.GetOption(ConfigDataSet, "Common", "PDFPath", "")
        End Get
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Common", "PDFPath", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
    End Property


#End Region

#Region "Email"

    Public Property Mail_EnableMail As Boolean
        Get
            Return Settings.GetOption(ConfigDataSet, "Mail", "EnableMail", False)
        End Get
        Set(value As Boolean)
            Settings.SetOption(ConfigDataSet, "Mail", "EnableMail", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
    End Property

    Public Property Mail_SMTPEnableSSL As Boolean
        Get
            Return Settings.GetOption(ConfigDataSet, "Mail", "SMTPEnableSSL", True)
        End Get
        Set(value As Boolean)
            Settings.SetOption(ConfigDataSet, "Mail", "SMTPEnableSSL", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
    End Property

    Public Property Mail_SMTPHost As String
        Get
            Return Settings.GetOption(ConfigDataSet, "Mail", "SMTPHost", "securesmtp.t-online.de")
        End Get
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Mail", "SMTPHost", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
    End Property

    Public Property Mail_SMTPPort As Integer
        Get
            Return Settings.GetOption(ConfigDataSet, "Mail", "SMTPPort", 587)
        End Get
        Set(value As Integer)
            Settings.SetOption(ConfigDataSet, "Mail", "SMTPPort", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
    End Property

    Public Property Mail_SMTPUser As String
        Get
            Return Settings.GetOption(ConfigDataSet, "Mail", "SMTPUser", "youremail@t-online.de")
        End Get
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Mail", "SMTPUser", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
    End Property

    Public Property Mail_SMTPPassword As String
        Get
            Return Settings.GetOption(ConfigDataSet, "Mail", "SMTPPassword", "")
        End Get
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Mail", "SMTPPassword", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
    End Property

    Public Property Mail_EMLPath As String
        Get
            Return Settings.GetOption(ConfigDataSet, "Mail", "EMLPath", "")
        End Get
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Mail", "EMLPath", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
    End Property

#End Region

#Region "Export"

    Public Property ExportRootDirectory As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Export", "RootDirectory", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Export", "RootDirectory", "C:\Temp")
        End Get
    End Property


#End Region


End Class
