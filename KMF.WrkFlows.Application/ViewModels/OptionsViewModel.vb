Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.Common.Application.ApplicationSettings
Imports KMF.WrkFlows.Domain

Public Class OptionsViewModel

    Public Shared Property Current As OptionsViewModel
    Public Shared Property ClientMachineSettings As ClientData
    Public Shared Property ClientServerSettings As ServerData
    Public Shared Property UserSettings As UserData

    Public Sub New()

        ' Damit der Logger aktiviert wird
        NLOGLOGGER.Debug("OptionsView is loading")

        ' Init der Anwendungseinstellungen
        InitSettings()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Current = Me
        OptionsViewModel.ClientMachineSettings = New ClientData
        OptionsViewModel.ClientServerSettings = StartViewModel.ServerData
        OptionsViewModel.UserSettings = New UserData


    End Sub

End Class
