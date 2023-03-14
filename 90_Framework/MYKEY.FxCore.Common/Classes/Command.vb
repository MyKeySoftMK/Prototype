Public Class Command

    ''' <summary>
    ''' Pfad und Name der Anwendung, die gestartet werden soll
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Application As String = ""

    ''' <summary>
    ''' Wenn als Application eine Batch-Datei angegeben wurde, dann ist dieser Wert auf TRUE zu setzen,
    ''' damit eine zeilenweise Abarbeitung der Datei erfolgen kann
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Wenn es sich um nicht-lineare Scripte handelt mit Schleifen und Abfragen, dann ist hier FALSE zu setzen</remarks>
    Public Property ApplicationIsBatch As Boolean = False

    ''' <summary>
    ''' Die zusätzlichen Parameter um die Anwendung zu starten
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Arguments As String = ""

    ''' <summary>
    ''' Wie die gestartete Anwendung angezeigt werden soll
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WindowsState As ProcessWindowStyle = ProcessWindowStyle.Normal

    ''' <summary>
    ''' Benutzername um mit erhöhten Rechten auszuführen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AdminUser As String = ""

    ''' <summary>
    ''' Kennwort für den Benutzer mit erhöhten Rechten
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AdminPassword As String = ""

    ''' <summary>
    ''' Domainzugehörigkeit des Benutzers mit erhöhrten Rechten
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AdminDomain As String


End Class
