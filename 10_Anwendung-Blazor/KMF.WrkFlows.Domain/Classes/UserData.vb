Imports System.ComponentModel
Imports MYKEY.FxCore.Common
Imports KMF.WrkFlows.Domain

<DisplayName("Benutzereinstellungen")>
<Description("Einstellungen die nur für den angemeldeten Benutzer Gültigkeit haben")>
Public Class UserData
    Inherits ObservableObject

    ' Eine lokale Variabel für das lokale speichern der Einstellungen
    Private UserSettingsXML As New UserSettings

#Region "Allgemein"

    <Category("Allgemein")>
    <DisplayName("Drucker")>
    <Description("Der Standarddrucker für die meisten Listen und Dokumente")>
    Public Property Common_Printer As String
        Set(value As String)
            If value <> UserSettingsXML.Common_Printer Then
                UserSettingsXML.Common_Printer = value
                OnPropertyChanged("Common_Printer")
            End If
        End Set
        Get
            Return UserSettingsXML.Common_Printer
        End Get
    End Property

    <Category("Allgemein")>
    <DisplayName("PDF-Verzeichnis")>
    <Description("In welches Verzeichnis die automatisierten PDF-Dateien gespeichert werden")>
    Public Property Common_PDFPath As String
        Set(value As String)
            If value <> UserSettingsXML.Common_PDFPath Then
                UserSettingsXML.Common_PDFPath = value
                OnPropertyChanged("Common_PDFPath")
            End If
        End Set
        Get
            Return UserSettingsXML.Common_PDFPath
        End Get
    End Property


#End Region

#Region "Email"

    <Category("EMail")>
    <DisplayName("Email aktivieren")>
    <Description("Ob das Versenden von Emails über einen SMTP-Server aktiviert werden soll")>
    Public Property Mail_EnableMail As Boolean
        Set(value As Boolean)
            If value <> UserSettingsXML.Mail_EnableMail Then
                UserSettingsXML.Mail_EnableMail = value
                OnPropertyChanged("Mail_EnableMail")
            End If
        End Set
        Get
            Return UserSettingsXML.Mail_EnableMail
        End Get
    End Property

    <Category("EMail")>
    <DisplayName("SMTP: SSL aktivieren")>
    <Description("Ob das Versenden von Emails über eine verschlüsselte Verbindung erfolgt")>
    Public Property Mail_SMTPEnableSSL As Boolean
        Set(value As Boolean)
            If value <> UserSettingsXML.Mail_SMTPEnableSSL Then
                UserSettingsXML.Mail_SMTPEnableSSL = value
                OnPropertyChanged("Mail_SMTPEnableSSL")
            End If
        End Set
        Get
            Return UserSettingsXML.Mail_SMTPEnableSSL
        End Get
    End Property

    <Category("EMail")>
    <DisplayName("SMTP: Host/IP")>
    <Description("Hostname oder IP-Adresse des verwendeten SMTP-Server")>
    Public Property Mail_SMTPHost As String
        Set(value As String)
            If value <> UserSettingsXML.Mail_SMTPHost Then
                UserSettingsXML.Mail_SMTPHost = value
                OnPropertyChanged("Mail_SMTPHost")
            End If
        End Set
        Get
            Return UserSettingsXML.Mail_SMTPHost
        End Get
    End Property

    <Category("EMail")>
    <DisplayName("SMTP: Port")>
    <Description("Kommunikationsport des verwendeten SMTP-Server")>
    Public Property Mail_SMTPPort As Integer
        Set(value As Integer)
            If value <> UserSettingsXML.Mail_SMTPPort Then
                UserSettingsXML.Mail_SMTPPort = value
                OnPropertyChanged("Mail_SMTPPort")
            End If
        End Set
        Get
            Return UserSettingsXML.Mail_SMTPPort
        End Get
    End Property

    <Category("EMail")>
    <DisplayName("SMTP: Anmeldename")>
    <Description("Anmeldenam für den verwendeten SMTP-Server")>
    Public Property Mail_SMTPUser As String
        Set(value As String)
            If value <> UserSettingsXML.Mail_SMTPUser Then
                UserSettingsXML.Mail_SMTPUser = value
                OnPropertyChanged("Mail_SMTPUser")
            End If
        End Set
        Get
            Return UserSettingsXML.Mail_SMTPUser
        End Get
    End Property

    <Category("EMail")>
    <DisplayName("SMTP: Kennwort")>
    <Description("Kennwort für das Anmelden am verwendeten SMTP-Server")>
    Public Property Mail_SMTPPassword As String
        Set(value As String)
            If value <> UserSettingsXML.Mail_SMTPPassword Then
                UserSettingsXML.Mail_SMTPPassword = value
                OnPropertyChanged("Mail_SMTPPassword")
            End If
        End Set
        Get
            Return UserSettingsXML.Mail_SMTPPassword
        End Get
    End Property

    <Category("EMail")>
    <DisplayName("Lokales EML-Verzeichnis")>
    <Description("Verzeichnis in das die Kopien der versendeten Emails gespeichert werden sollen")>
    Public Property Mail_EMLPath As String
        Set(value As String)
            If value <> UserSettingsXML.Mail_EMLPath Then
                UserSettingsXML.Mail_EMLPath = value
                OnPropertyChanged("Mail_EMLPath")
            End If
        End Set
        Get
            Return UserSettingsXML.Mail_EMLPath
        End Get
    End Property
#End Region

#Region "Export"

    <Category("Export")>
    <DisplayName("ExportRootDirectory")>
    <Description("Das Verzeichnis, in der Exports abgelegt werden (z.B.: 'C:\Temp'")>
    Public Property Export_RootDirectory As String
        Set(value As String)
            If value <> UserSettingsXML.ExportRootDirectory Then
                UserSettingsXML.ExportRootDirectory = value
                OnPropertyChanged("Export_RootDirectory")
            End If
        End Set
        Get
            Return UserSettingsXML.ExportRootDirectory
        End Get
    End Property

#End Region

End Class
