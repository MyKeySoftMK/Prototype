Imports System.Net.Mail
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports System.Reflection

''' <summary>
''' Diese Extension erweitert das MailMessage-Objekt um die Möglichkeit sich selbst als EML-Datei zu speichern. Dabei ist es möglich,
''' dass man eine Dateinamen vergeben kann, der verwendet werden soll. Die von .NET gelieferte vergibt nur GUIDs als Namen
''' </summary>
''' <remarks>http://www.codeproject.com/Articles/32434/Adding-Save-functionality-to-System-Net-Mail-MailM</remarks>
Public Module MailMessageExt

    Sub New()
    End Sub

    <System.Runtime.CompilerServices.Extension> _
    Public Sub Save(Message As MailMessage, FileName As String)

        Dim assembly As Assembly = GetType(SmtpClient).Assembly
        Dim _mailWriterType As Type = assembly.[GetType]("System.Net.Mail.MailWriter")

        Using _fileStream As New FileStream(FileName, FileMode.Create)
            ' Get reflection info for MailWriter contructor
            Dim _mailWriterContructor As ConstructorInfo =
                _mailWriterType.GetConstructor(BindingFlags.Instance Or BindingFlags.NonPublic, Nothing, New Type() {GetType(Stream)}, Nothing)

            ' Construct MailWriter object with our FileStream
            Dim _mailWriter As Object =
                _mailWriterContructor.Invoke(New Object() {_fileStream})

            ' Get reflection info for Send() method on MailMessage
            Dim _sendMethod As MethodInfo =
                GetType(MailMessage).GetMethod("Send", BindingFlags.Instance Or BindingFlags.NonPublic)

            ' Call method passing in MailWriter
            ' Bis .Net 4.0 ist folgende Zeile zu verwenden
            '_sendMethod.Invoke(Message, BindingFlags.Instance Or BindingFlags.NonPublic, Nothing, New Object() {_mailWriter, True}, Nothing)
            ' Ab .Net 4.5 ist folgende Zeile zu nutzen, da sich die Signatur der Send-Methode geändert hat
            _sendMethod.Invoke(Message, BindingFlags.Instance Or BindingFlags.NonPublic, Nothing, New Object() {_mailWriter, True, True}, Nothing)

            ' Finally get reflection info for Close() method on our MailWriter
            Dim _closeMethod As MethodInfo =
                _mailWriter.[GetType]().GetMethod("Close", BindingFlags.Instance Or BindingFlags.NonPublic)

            ' Call close method
            _closeMethod.Invoke(_mailWriter, BindingFlags.Instance Or BindingFlags.NonPublic, Nothing, New Object() {}, Nothing)

        End Using
    End Sub

End Module