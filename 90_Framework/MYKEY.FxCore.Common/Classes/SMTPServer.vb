Public Class SMTPServer

    ''' <summary>
    ''' Name des SMTP Servers
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Host As String = ""

    ''' <summary>
    ''' Benutzername zum Anmelden an dem SMTP-Server
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Username As String = ""

    ''' <summary>
    ''' Kennwort für die Anmeldung am SMTP-Server
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Password As String = ""

    ''' <summary>
    ''' Port des SMTP-Servers zum Empfangen von Mail
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port As String = ""

    ''' <summary>
    ''' Ob Verschhlüsselung aktiviert werden soll
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EnableSSL As Boolean = False


End Class
