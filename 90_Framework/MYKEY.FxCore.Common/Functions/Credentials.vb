Imports Meziantou.Framework.Win32


Public Class Credentials

    Public Function GetAllCredentials() As List(Of Credential)

        Dim _result As New List(Of Credential)

        For Each credential In CredentialManager.EnumerateCrendentials()
            Console.WriteLine(credential)
            _result.Add(credential)
        Next

        Return _result

    End Function


    Public Sub WriteCredential(AppName As String, UserName As String, Password As String,
                               Optional Comment As String = "", Optional Persist As CredentialPersistence = CredentialPersistence.LocalMachine)

        If Comment.Length = 0 Then
            CredentialManager.WriteCredential(AppName, UserName, Password, Persist)
        Else
            CredentialManager.WriteCredential(AppName, UserName, Password, Comment, Persist)
        End If

    End Sub

    Public Function GetCredential(AppName As String) As Credential

        Dim _result As Credential

        _result = CredentialManager.ReadCredential(AppName)

        Return _result
    End Function

    Public Sub DeleteCredential(AppName As String)

        CredentialManager.DeleteCredential(AppName)

    End Sub

    Public Function GetMso365Credential(Optional Mso365Version As String = "16") As Credential
        Dim _result As Credential


        For Each _credential In CredentialManager.EnumerateCrendentials()


            ' Gesucht wird 'MicrosoftOffice16_Data:SSPI:michael.kolowicz@km-f.de'
            If Mid(_credential.ApplicationName, 1, 28) = "MicrosoftOffice" & Mso365Version & "_Data:SSPI:" Then
                _result = _credential
                Exit For
            End If

        Next


        Return _result

    End Function
End Class
