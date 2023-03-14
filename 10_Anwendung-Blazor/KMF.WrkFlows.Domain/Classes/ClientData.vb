Imports System.ComponentModel
Imports MYKEY.FxCore.Common


<DisplayName("Maschinen-/Clienteinstellungen")>
<Description("Einstellungen die nur auf diesem Client Gültigkeit haben")>
Public Class ClientData
    Inherits ObservableObject

    ' Eine lokale Variabel für das lokale speichern der Einstellungen
    Private ClientMachineSettingsXML As New ClientApplicationSettings

#Region "Log2Console"

    <Category("Log2Console")>
    <DisplayName("Log2Console aktivieren")>
    <Description("Aktiviert beim Starten der Anwendung das LogViewer-Tool Log2Console")>
    Public Property Log2Console_Enable As Boolean
        Set(value As Boolean)
            If value <> ClientMachineSettingsXML.Log2Console_Enable Then
                ClientMachineSettingsXML.Log2Console_Enable = value
                OnPropertyChanged("Log2Console_Enable")
            End If
        End Set
        Get
            Return ClientMachineSettingsXML.Log2Console_Enable
        End Get
    End Property

#End Region


End Class
