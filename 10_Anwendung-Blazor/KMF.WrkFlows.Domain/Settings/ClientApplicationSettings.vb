Imports System.Data
Imports MYKEY.FxCore.Common.Application

Public Class ClientApplicationSettings
    Implements IApplicationSettings

#Region "Config"


    Private ConfigDataSet As DataSet = Nothing
    Private ConfigName As String = "WrkFlowsClientMachine"
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

#Region "Log2Console"

    Public Property Log2Console_Enable() As Boolean
        Get
            Return Settings.GetOption(ConfigDataSet, "Log2Console", "Enable", False)
        End Get
        Set(ByVal value As Boolean)
            Settings.SetOption(ConfigDataSet, "Log2Console", "Enable", value)
            Settings.Store(ConfigDataSet, ConfigFileName, ConfigPath)
        End Set
    End Property

#End Region

End Class
