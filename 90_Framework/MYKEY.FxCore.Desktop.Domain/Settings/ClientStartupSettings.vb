Imports System.Data
Imports MYKEY.FxCore
Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.Common.Application

Public Class ClientStartupSettings

    Implements IApplicationSettings

#Region "Config"


    Private ConfigDataSet As DataSet = Nothing
    Private ConfigName As String = "CongAdminNTStartupClient"
    Private ConfigPath As String = ApplicationSettings.INTERNALAPPDATA
    Private ConfigFileName As String = ConfigName & ".fxcfg"

#End Region

#Region "Init"

    Public Sub New()

        Initialize(ConfigName, ConfigPath)

    End Sub

    Public Sub DeInitialize() Implements MYKEY.FxCore.Common.Application.IApplicationSettings.DeInitialize
        If ConfigDataSet IsNot Nothing Then
            Settings.DeInitialize(ConfigName, ConfigPath)
        End If
    End Sub

    Public Sub Initialize(ConfigNameInit As String, ConfigPathInit As String) Implements MYKEY.FxCore.Common.Application.IApplicationSettings.Initialize
        If ConfigDataSet Is Nothing Then
            ConfigName = ConfigNameInit
            ConfigPath = ConfigPathInit
            ConfigDataSet = Settings.Initialize(ConfigName, ConfigPath)
        End If
    End Sub
#End Region

#Region "Properties"

    ''' <summary>
    ''' Der letzte benutze Benutzername
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LastUsername As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Client", "LastUsername", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Client", "LastUsername", "")
        End Get
    End Property

    ''' <summary>
    ''' Der zuletzt benutze Server
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LastHostInstance As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Client", "LastHostInstance", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Client", "LastHostInstance", "")
        End Get
    End Property

    ''' <summary>
    ''' Alle Server, die schon mal benutzt wurde
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LastHostInstances() As String
        Set(value As String)
            Settings.SetOption(ConfigDataSet, "Client", "LastHostInstances", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
        Get
            Return Settings.GetOption(ConfigDataSet, "Client", "LastHostInstances", "")
        End Get
    End Property
#End Region

End Class
