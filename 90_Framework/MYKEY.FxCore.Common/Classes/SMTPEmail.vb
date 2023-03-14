Imports System.Net.Mail

Public Class SMTPEmail

    ''' <summary>
    ''' Name des Absenders
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property From As String = ""

    ''' <summary>
    ''' Auflistung der Empfängeradressen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property [To] As List(Of MailAddress) = New List(Of MailAddress)

    ''' <summary>
    ''' Auflistung der Empfänger die eine Kopie erhalten sollen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Cc As List(Of MailAddress) = New List(Of MailAddress)

    ''' <summary>
    ''' Auflistung der Empfänger, die eine Blindcopy erhalten sollen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Bcc As List(Of MailAddress) = New List(Of MailAddress)

    ''' <summary>
    ''' Inhalt der Betreffzeile
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Subject As String = ""

    ''' <summary>
    ''' Inhalt des Bodytextes
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Message As String = ""

    ''' <summary>
    ''' Inhalt des Bodytextes
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MessageHtml As String = ""


    ''' <summary>
    ''' Ob es sich bei dem Bodytext um HTML-Code handelt
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete>
    Public Property IsBodyHtml As Boolean = False

    ''' <summary>
    ''' Auflistung der Anhänge die an die Email eingefügt werden sollen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Attachments As List(Of String) = New List(Of String)



End Class
