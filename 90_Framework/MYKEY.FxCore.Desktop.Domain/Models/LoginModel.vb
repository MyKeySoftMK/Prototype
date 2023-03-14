Imports MYKEY.FxCore.WpfMvvm
Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.Common.Application
Imports MYKEY.FxCore.Common.OperationStatusEnum
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports System.ComponentModel
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.CongAdminNT.DbSetup

''' <summary>
''' Diese Klasse beinhaltet alle Methoden die für das Verbinden mit einerm Server
''' benötigt werden.
''' </summary>
''' <remarks></remarks>
Public Class LoginModel

    Inherits Model
    Implements ILogin

    Private _LoginData As LoginData
    Private _OperationStatusData As OperationStatusData
    Private Credentials As Hashtable

#Region "Contructor"

    Public Sub New()
        _LoginData = New LoginData
        AddHandler _LoginData.PropertyChanged, AddressOf ModelPropertyChanged

        _OperationStatusData = New OperationStatusData
    End Sub

    Public Sub New(login As LoginData)
        _LoginData = login
        _OperationStatusData = New OperationStatusData
    End Sub

    Protected Overrides Sub Finalize()
        Disconnect()
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' Diese Methode wird benötigt, wenn eine Eigenschaft die vom Model geändert wird, die View
    ''' benachrichtigen muss
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ModelPropertyChanged(sender As Object, e As PropertyChangedEventArgs)

        Select Case e.PropertyName
            Case "OperationStatus"
                Me.OnPropertyChanged("IsBusy")
            Case Else
                Me.OnPropertyChanged(e.PropertyName)
        End Select

    End Sub
#End Region

#Region "Properties"

    ''' <summary>
    ''' Die Anmeldeinformationen des Servers
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LoginData As LoginData
        Set(value As LoginData)
            If value IsNot _LoginData Then
                _LoginData = value
            End If
        End Set
        Get
            Return _LoginData
        End Get
    End Property

    ''' <summary>
    ''' Ob der Client mit dem Server verbunden ist
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsConnect As Boolean Implements ILogin.IsConnect
        Get
            If ConnectClient.Connection IsNot Nothing Then
                Return ConnectClient.IsConnect
            Else
                Return False
            End If
        End Get
    End Property

    ''' <summary>
    ''' Wie das Ergebniss der letzten Operation war, die ausgeführt wurde
    ''' </summary>
    ''' <value></value>
    ''' <returns>Es wird ein allgemeines Ergebnis erzeugt</returns>
    ''' <remarks></remarks>
    Public Property OperationStatus As OperationStatus Implements ILogin.OperationStatus
        Get
            Return _OperationStatusData.Status
        End Get
        Set(value As OperationStatus)
            If value <> _OperationStatusData.Status Then
                _OperationStatusData.Status = value
                Me.OnPropertyChanged("OperationStatus")
            End If
        End Set
    End Property

    ''' <summary>
    ''' Ob der Server per Ping erreichbar ist
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsAvailable As Boolean Implements ILogin.IsAvailable
        Get
            Dim _result As Boolean = False
            Dim _AppServer As String
            If _LoginData IsNot Nothing Then

                If _LoginData.HostInstance.Length > 0 Then

                    _AppServer = URL.GetServerFromURL(_LoginData.HostInstance)
                    If _AppServer.Length > 0 Then
                        _result = (Network.Ping(_AppServer).Status = Net.NetworkInformation.IPStatus.Success)
                    End If

                End If
            End If

            Return _result

        End Get
    End Property


    ''' <summary>
    ''' Um den Benutzer darüber zu informieren, das der Benutzername/Kennwort falsch war
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete>
    Public IsUserPasswordWrong As Boolean = False

    ''' <summary>
    ''' Benutzer darüber informaieren, wie der Passwort-Check ausgewertet wurde
    ''' </summary>
    ''' <remarks></remarks>
    Public CheckCredentialsResult As UserManagementEnums.CheckCredentialsResult = UserManagementEnums.CheckCredentialsResult.NotChecked

    Public ReadOnly Property IsApplicationInstanceRunning As Boolean
        Get
            Return Application.ConnectClient.IsApplicationInstanceRunning
        End Get
    End Property
#End Region

#Region "Functions"

    ''' <summary>
    ''' Stellt eine Verbindung zu einem Server her, mit den Informationen, die in LoginData hinterlegt sind
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Connect() Implements ILogin.Connect


        Try

            Dim _UserMgm As New UserManagement
            Me.CheckCredentialsResult = _UserMgm.CheckCredentials(_LoginData.Username, _LoginData.Password)

            If Me.CheckCredentialsResult <> UserManagementEnums.CheckCredentialsResult.Success Then
                Exit Sub
            End If
            CurrentUserSettings.UserName = _LoginData.Username

        Catch ex As Exception
            NLOGLOGGER.Error(ex.Message)

        End Try


    End Sub

    ''' <summary>
    ''' Alternative Aufrufmöglichkeit um beim Verbindenn die Verbindungsdaten mitzugeben
    ''' </summary>
    ''' <param name="login"></param>
    ''' <remarks></remarks>
    Public Sub Connect(login As LoginData)
        _LoginData = login
        Connect()
    End Sub

    ''' <summary>
    ''' Trennt die Verbindung zum Server
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Disconnect() Implements ILogin.Disconnect

    End Sub

#End Region


    Public Function CheckDataBaseSchemaVersion() As ServerResult Implements ILogin.CheckDataBaseShemaVersion
        Dim _result As New ServerResult

        ' Prüfen, ob die passenden Datenbank-Version verwendet wird
        Dim _DbTools As New DbTools
        Dim _DbSchemaInfo As DbDeployEntry
        Dim _firstError As Boolean = True
        _DbSchemaInfo = _DbTools.GetDBVersion("CongAdminNTVersion")

        NLOGLOGGER.Debug("DBSchemaInfo for this application:")
        'NLOGLOGGER.Debug("=> Major-Release: " & CongAdminNTSchema.DbSchemaInfo.Folder)
        'NLOGLOGGER.Debug("=> Minor-Release: " & CongAdminNTSchema.DbSchemaInfo.ScriptNumber)
        'NLOGLOGGER.Debug("=> Script-Status: " & CongAdminNTSchema.DbSchemaInfo.ScriptStatus)

        NLOGLOGGER.Debug("DBSchemaInfo from connected Database:")
        NLOGLOGGER.Debug("=> Major-Release: " & _DbSchemaInfo.Folder)
        NLOGLOGGER.Debug("=> Minor-Release: " & _DbSchemaInfo.ScriptNumber)
        NLOGLOGGER.Debug("=> Script-Status: " & _DbSchemaInfo.ScriptStatus)

        ' Genereller Fehler bei der Auswertung
        If _DbSchemaInfo.ErrorResult.Length > 0 Then

            _result.ErrorMessages.Add("Beim Zugriff auf die Datenbank ist ein allgemeiner Fehler aufgetreten")
            _result.ErrorMessages.Add(_DbSchemaInfo.ErrorResult)

        Else
            '' Fehler bei der Auswertung der Schemainformationen
            'If _DbSchemaInfo.Folder <> CongAdminNTSchema.DbSchemaInfo.Folder Then
            '    _result.ErrorMessages.Add("Datenbankschema und Anwendungsversion passen nicht. Bitte aktualisieren")
            '    _result.ErrorMessages.Add("Die Major-Release Nummer der Anwendung (=" & CongAdminNTSchema.DbSchemaInfo.Folder & ") und der Datenbank (=" & _DbSchemaInfo.Folder & ") stimmen nicht überein")
            '    _firstError = False
            'End If

            'If _DbSchemaInfo.ScriptNumber <> CongAdminNTSchema.DbSchemaInfo.ScriptNumber Then
            '    If _firstError = True Then
            '        _result.ErrorMessages.Add("Datenbankschema und Anwendungsversion passen nicht. Bitte aktualisieren")
            '        _firstError = False
            '    End If
            '    _result.ErrorMessages.Add("Die Minor-Release Nummer der Anwendung (=" & CongAdminNTSchema.DbSchemaInfo.ScriptNumber & ") und der Datenbank (=" & _DbSchemaInfo.ScriptNumber & ") stimmem nicht überein")
            'End If

            'If _DbSchemaInfo.ScriptStatus <> 1 Then
            '    If _firstError = True Then
            '        _result.ErrorMessages.Add("Datenbankschema und Anwendungsversion passen nicht. Bitte aktualisieren")
            '        _firstError = False
            '    End If
            '    _result.ErrorMessages.Add("Der Update-Script auf Version (" & _DbSchemaInfo.Folder & "." & _DbSchemaInfo.ScriptNumber & ") war nicht erfolgreich")
            'End If
        End If

        If _result.HasErrors = False Then
            NLOGLOGGER.Info("Schemacheck successfull")
        Else
            NLOGLOGGER.Fatal("Application schema and database schema are not equal")
        End If

        Return _result

    End Function

End Class
