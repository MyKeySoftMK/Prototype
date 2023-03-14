Imports System.Collections.ObjectModel
Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.Common.Application
Imports KMF.WrkFlows.Domain
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.Common.Application.ApplicationSettings
Imports MYKEY.FxCore.Desktop.Domain
Imports MYKEY.FxCore.DataAccess

Public Class StartViewModel

    Public Shared Property Current As StartViewModel
    Public Shared Property ClientMachineSettings As New ClientData
    Public RuntimeChecks As New RuntimeChecks
    Public Shared Property ServerData As New ServerData


    Public Sub New()

        Dim _ServerModel As ServerModel

        ' Starten des Log2Console-Viewers
        If ClientMachineSettings.Log2Console_Enable = True Then
            _ServerModel = New ServerModel
            _ServerModel.OpenLog2Console()

            ' Starten einer normalen Text-Console
            ConsoleManager.Show()

        End If

        ' Damit der Logger aktiviert wird
        NLOGLOGGER.Debug("StartupView is loading")

        ' Zwischenspeichern des StartViewModel um auf generelle Informationen zu greifen zu können
        Current = Me

        ' Init der Anwendungseinstellungen
        InitSettings()


        ' Damit die Daten in ServerData initialisiert werden, einen einfachen Logeintrag schreiben
        NLOGLOGGER.Debug("Initalize ServerData for Database (Name)    : " & StartViewModel.ServerData.Server_DatabaseName)
        NLOGLOGGER.Debug("Initalize ServerData for Database (Server)  : " & StartViewModel.ServerData.Server_DatabaseServer)
        NLOGLOGGER.Debug("Initalize ServerData for Database (User)    : " & StartViewModel.ServerData.Server_DatabaseUserName)
        NLOGLOGGER.Debug("Initalize ServerData for Database (Password): " & StartViewModel.ServerData.Server_DatabasePassword)
        NLOGLOGGER.Debug("Initalize ServerData for Database (Port)    : " & StartViewModel.ServerData.Server_DatabasePort)

    End Sub


    ''' <summary>
    ''' Überprüfung der anwendungsspezifischen Runtime-Umgebung
    ''' </summary>
    Public Sub ExecuteRuntimeCheck()

        Dim _UserData As UserData
        Dim _errorText As String = ""

        ' Nachdem Lesen der Daten den Funktions-Check ausführen
        With RuntimeChecks
            .FolderChecksAdd(StartViewModel.ServerData.Mail_TemplateRootDirectory, True)
            .FolderChecksAdd(StartViewModel.ServerData.Maintenance_DirectoryForDBBackup)

            _UserData = New UserData

            .FolderChecksAdd(_UserData.Common_PDFPath, True, True, True)
            .FolderChecksAdd(_UserData.Export_RootDirectory)
            .FolderChecksAdd(_UserData.Mail_EMLPath)

            ' Audsführen der Überprüfungen
            .Check()


            If .RuntimeCheckResults.Count > 0 Then
                ' Anzeigen der MSG-BOX mit allen relevanten Fehlern
                For Each txt In .RuntimeCheckResults
                    _errorText = _errorText & txt & vbCrLf
                Next

                'Controller.Message(_errorText, "Systemcheck", MessageBoxButtons.OK, MessageBoxImages.Stop)

            End If

        End With
    End Sub


End Class
