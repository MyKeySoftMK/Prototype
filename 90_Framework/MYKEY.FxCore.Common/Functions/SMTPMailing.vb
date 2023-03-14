Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime.MediaTypeNames.Application
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports System.IO
Imports System.Text.RegularExpressions

Public Class SMTPMailing

    ' Bei der Verwendnung von T-Online sind einige Besonderheiten zu beachten.
    ' Will man SSL verwenden, dann muss man das Certifikat herunterladen und einbinden

    ' Dim Cert_TOnline As New System.Security.Cryptography.X509Certificates.X509Certificate(Application.StartupPath & "\deutsche-telekom-root-ca-2.crt")
    '
    '        .ClientCertificates.Add(Cert_TOnline)


    ' Bei T-Online ist nach der Erstellung eines FreeMail-Accounts noch das Email-Passwort zu setzen
    ' (Menü => Einstellungen => Konto-Einstellungen => Passwörter E-Mail Password)

    ' Folgende Einstellungen dienen als Beispiel
    ' SMTPHost = "securesmtp.t-online.de"
    ' SMTPPort_TLS = "587"
    ' SMTPUser = "appdevtestmail@t-online.de"
    ' SMTPPasswort = "Hallo24Mail!"  <= Ist ein anderes Kennwort, als das, was man zum Einloggen über die Webansicht von FreeMail eingibt
    ' EnableSSL = True


    ''' <summary>
    ''' Versenden einer Email über einen SMTP-Server
    ''' </summary>
    ''' <param name="Mail"></param>
    ''' <param name="Server"></param>
    ''' <param name="Options"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendEmail(Mail As SMTPEmail, Server As SMTPServer, Options As SMTPOptions) As ServerResult

        Dim _SmtpServer As New SmtpClient
        Dim _Message As New MailMessage
        Dim _result As New ServerResult
        Dim _FileInfo As FileSystemInfo
        ' Dim _SmtpServerEML As New SmtpClient
        Dim _AlternateTextView As AlternateView


        Try

            If Options.UseFakeSMTP = False Then

                ' Erzeugen des Servers
                With _SmtpServer
                    .Host = Server.Host
                    .Port = Server.Port
                    .EnableSsl = Server.EnableSSL

                    ' Es ist wichtig, dass die Credentials NACH dem setzen der Eigenschaft .UseDefaultCredentials geschieht
                    .UseDefaultCredentials = False
                    .Credentials = New NetworkCredential(Server.Username, Server.Password)

                    _SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network

                End With
            Else

                With _SmtpServer
                    .Host = "127.0.0.1"
                    .Port = 25
                End With

            End If

            ' Erzeugen des Mailtextes
            With _Message
                .Sender = New MailAddress(Mail.From)
                .From = New MailAddress(Mail.From)

                For Each addr As MailAddress In Mail.To
                    .To.Add(addr)
                Next
                For Each addr As MailAddress In Mail.Cc
                    .CC.Add(addr)
                Next
                For Each addr As MailAddress In Mail.Bcc
                    .Bcc.Add(addr)
                Next

                .Subject = Mail.Subject

                ' Erzeugen des Body der Message
                .Body = Mail.Message
                .IsBodyHtml = False

                ' Eine alternative Ansicht hinzufügen, wenn möglich
                If Mail.MessageHtml.Length > 0 Then
                    _AlternateTextView = AlternateView.CreateAlternateViewFromString(Mail.MessageHtml, Nothing, Mime.MediaTypeNames.Text.Html)
                    .AlternateViews.Add(_AlternateTextView)
                End If

                For Each att As String In Mail.Attachments
                    _FileInfo = New FileInfo(att)
                    Select Case _FileInfo.Extension.ToUpper
                        Case ".RTF"
                            .Attachments.Add(New Attachment(att, Rtf))
                        Case ".PDF"
                            .Attachments.Add(New Attachment(att, Pdf))
                        Case ".ZIP"
                            .Attachments.Add(New Attachment(att, Zip))
                        Case Else
                            .Attachments.Add(New Attachment(att, Octet))
                    End Select

                Next

            End With

            _SmtpServer.Send(_Message)

            NLOGLOGGER.Debug("Mail send successfull")

            ' Erzeugen des SMTPServers für das Erstellen von EML-Dateien
            If Options.PathForEMLCopy.Length > 0 Then

                ' "Extension"-Methode
                _Message.Save(Options.PathForEMLCopy & "\" & Format(Now(), "yyyyMMddHHmmss ") &
                              Regex.Replace(Mail.Subject, "[\\/:*?""<>|\r\n]", "", RegexOptions.Singleline) &
                              ".eml")

                ' "Normale" Methode - damit werden EML-Dateien erstell, deren Namen aber nicht vorgegeben werden kann
                'With _SmtpServerEML
                '    .DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory
                '    .PickupDirectoryLocation = Options.PathForEMLCopy
                'End With

                '_SmtpServerEML.Send(_Message)

            End If


        Catch ex As Exception
            NLOGLOGGER.Fatal("Mail cannot be send: " & ex.Message)
            _result.ErrorMessages.Add(ex.Message)
        End Try

        ' Sendet das "Beendet"-Signal an den Server
        _SmtpServer.Dispose()

        Return _result

    End Function

End Class
