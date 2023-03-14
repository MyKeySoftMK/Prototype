Public Class SMTPOptions

    ''' <summary>
    ''' In welches Verzeichnis eine Kopie der gesendeten Mail abgelegt werden soll
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PathForEMLCopy As String = ""

    ''' <summary>
    ''' Das Versenden der Mails erfolgt auf einen lokalen FakeSMTP-Server
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UseFakeSMTP As Boolean = False

End Class
