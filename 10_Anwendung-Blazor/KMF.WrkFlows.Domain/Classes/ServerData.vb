Imports System.ComponentModel
Imports MYKEY.FxCore.Common
Imports KMF.WrkFlows.Domain
Imports MYKEY.FxCore.Desktop.Domain
Imports MYKEY.FxCore.Common.Application.DatabaseSettings
Imports MYKEY.FxCore.Common.ApplicationLogging

<DisplayName("Datenbankservereinstellungen")>
<Description("Einstellungen um sich mit der Datenbank zu verbinden")>
Public Class ServerData
    Inherits ObservableObject

    Public Sub New()
        NLOGLOGGER.Debug("Init a new ServerData-Instance")
    End Sub

    ' Eine lokale Variabel für das lokale speichern der Einstellungen
    Private ClientServerSettingsXML As New ServerApplicationSettings

#Region "Wartung"

    <Category("Wartung")>
    <DisplayName("Backup-Verzeichnis")>
    <Description("Verzeichnis, in das die Datensicherungen gespeichert werden sollen")>
    Public Property Maintenance_DirectoryForDBBackup As String
        Set(value As String)
            If value <> ClientServerSettingsXML.DirectoryForDBBackup Then
                ClientServerSettingsXML.DirectoryForDBBackup = value
                OnPropertyChanged("Maintenance_DirectoryForDBBackup")
            End If
        End Set
        Get
            Return ClientServerSettingsXML.DirectoryForDBBackup
        End Get
    End Property

    <Category("Wartung")>
    <DisplayName("EF6-DataAccess Logging aktivieren")>
    <Description("Jeder Befehl, der von der Datenbankschicht erzeugt und an die Datenbank gesendet wird, wird protokollieret")>
    Public Property Maintenance_EF6DBLogging As Boolean
        Set(value As Boolean)
            If value <> ClientServerSettingsXML.EF6Logging Then
                ClientServerSettingsXML.EF6Logging = value
                OnPropertyChanged("Maintenance_EF6DBLogging")
            End If
        End Set
        Get
            Return ClientServerSettingsXML.EF6Logging
        End Get
    End Property

#End Region

#Region "Server"

    <Category("Server")>
    <DisplayName("Datenbank")>
    <Description("Name der Datenbank. Normalerweise 'kmwrkflw'")>
    Public Property Server_DatabaseName As String
        Set(value As String)
            If value <> ClientServerSettingsXML.DatabaseName Then
                ClientServerSettingsXML.DatabaseName = value
                DATABASE_NAME = value
                OnPropertyChanged("Server_DatabaseName")
            End If
        End Set
        Get
            DATABASE_NAME = ClientServerSettingsXML.DatabaseName
            Return DATABASE_NAME
        End Get
    End Property

    <Category("Server")>
    <DisplayName("Datenbankbenutzer")>
    <Description("Name des zugriffsberechtigten Datenbankbenutzers. Normalerweise 'root'")>
    Public Property Server_DatabaseUserName As String
        Set(value As String)
            If value <> ClientServerSettingsXML.DatabaseUserName Then
                ClientServerSettingsXML.DatabaseUserName = value
                DATABASE_USERNAME = value
                OnPropertyChanged("Server_DatabaseUserName")
            End If
        End Set
        Get
            DATABASE_USERNAME = ClientServerSettingsXML.DatabaseUserName
            Return DATABASE_USERNAME
        End Get
    End Property

    <Category("Server")>
    <DisplayName("Kennwort")>
    <Description("Kennwort für den Berechtigten Datenbankbenutzer. Normalerweise 'pwMySQLAdmin'")>
    Public Property Server_DatabasePassword As String
        Set(value As String)
            If value <> ClientServerSettingsXML.DatabasePassword Then
                ClientServerSettingsXML.DatabasePassword = value
                DATABASE_PASSWORD = value
                OnPropertyChanged("Server_DatabasePassword")
            End If
        End Set
        Get
            DATABASE_PASSWORD = ClientServerSettingsXML.DatabasePassword
            Return DATABASE_PASSWORD
        End Get
    End Property

    <Category("Server")>
    <DisplayName("Datenbankserver")>
    <Description("Host- und Instanzname der Datenbank. Normalerweise '192.168.192.56'")>
    Public Property Server_DatabaseServer As String
        Set(value As String)
            If value <> ClientServerSettingsXML.DatabaseServer Then
                ClientServerSettingsXML.DatabaseServer = value
                DATABASE_SERVER = value
                OnPropertyChanged("Server_DatabaseServer")
            End If
        End Set
        Get
            DATABASE_SERVER = ClientServerSettingsXML.DatabaseServer
            Return DATABASE_SERVER
        End Get
    End Property

    <Category("Server")>
    <DisplayName("Datenbankserverport")>
    <Description("Port des Datenbankserver. Bei Infrastruktur-Setup ist es normalerweise '3306'")>
    Public Property Server_DatabasePort As Integer
        Set(value As Integer)
            If value <> ClientServerSettingsXML.DatabasePort Then
                ClientServerSettingsXML.DatabasePort = value
                DATABASE_PORT = value
                OnPropertyChanged("Server_DatabasePort")
            End If
        End Set
        Get
            DATABASE_PORT = ClientServerSettingsXML.DatabasePort
            Return DATABASE_PORT
        End Get
    End Property

    <Category("Server")>
    <DisplayName("Softdelete-Mechanismus")>
    <Description("Bei 'Softdelete' werden die Einträge nicht aus der Datenbank entfernt, sondern nur als gelöscht markiert")>
    Public Property Server_DatabaseSoftDelete As Boolean
        Set(value As Boolean)
            If value <> ClientServerSettingsXML.SoftDelete Then
                ClientServerSettingsXML.SoftDelete = value
                OnPropertyChanged("Server_DatabaseSoftDelete")
            End If
        End Set
        Get
            Return ClientServerSettingsXML.SoftDelete
        End Get
    End Property

#End Region

#Region "NLogTarget - Database"

    <Category("NLogTarget - Database")>
    <DisplayName("LogFrameworkToDatabase")>
    <Description("")>
    Public Property NLogDB_LogFrameworkToDatabase As Boolean
        Set(value As Boolean)
            If value <> ClientServerSettingsXML.LogFrameworkToDatabase Then
                ClientServerSettingsXML.LogFrameworkToDatabase = value
                OnPropertyChanged("NLogDB_LogFrameworkToDatabase")
            End If
        End Set
        Get
            Return ClientServerSettingsXML.LogFrameworkToDatabase
        End Get
    End Property

    <Category("NLogTarget - Database")>
    <DisplayName("LogTableFramework")>
    <Description("'FxNTNLog'")>
    Public Property NLogDB_LogTableFramework As String
        Set(value As String)
            If value <> ClientServerSettingsXML.LogTableFramework Then
                ClientServerSettingsXML.LogTableFramework = value
                OnPropertyChanged("NLogDB_LogTableFramework")
            End If
        End Set
        Get
            Return ClientServerSettingsXML.LogTableFramework
        End Get
    End Property

    <Category("NLogTarget - Database")>
    <DisplayName("LogFrameworkDetails")>
    <Description("'Info'")>
    Public Property NLogDB_LogFrameworkDetails As NLogEnums.LogLevels
        Set(value As NLogEnums.LogLevels)
            If value <> ClientServerSettingsXML.LogFrameworkDetails Then
                ClientServerSettingsXML.LogFrameworkDetails = value
                OnPropertyChanged("NLogDB_LogFrameworkDetails")
            End If
        End Set
        Get
            Return ClientServerSettingsXML.LogFrameworkDetails
        End Get
    End Property

    <Category("NLogTarget - Database")>
    <DisplayName("LogTableApplication")>
    <Description("'CongAdminNTNLog'")>
    Public Property NLogDB_LogTableApplication As String
        Set(value As String)
            If value <> ClientServerSettingsXML.LogTableApplication Then
                ClientServerSettingsXML.LogTableApplication = value
                OnPropertyChanged("NLogDB_LogTableApplication")
            End If
        End Set
        Get
            Return ClientServerSettingsXML.LogTableApplication
        End Get
    End Property

#End Region

#Region "Mail"

    <Category("Mail")>
    <DisplayName("MailTemplateRootDirectory")>
    <Description("Das Verzeichnis, in der Mail-Vorlagen abgelegt werden (z.B.: 'C:\Program Files (x86)\MyKey-Soft\CongAdminNT\CongAdminNT UI'")>
    Public Property Mail_TemplateRootDirectory As String
        Set(value As String)
            If value <> ClientServerSettingsXML.MailTemplateRootDirectory Then
                ClientServerSettingsXML.MailTemplateRootDirectory = value
                OnPropertyChanged("Mail_TemplateRootDirectory")
            End If
        End Set
        Get
            Return ClientServerSettingsXML.MailTemplateRootDirectory
        End Get
    End Property

#End Region

End Class
