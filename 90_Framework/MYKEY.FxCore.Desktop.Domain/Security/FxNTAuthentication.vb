Imports MYKEY.FxCore.Common.Application

Public Class FxNTAuthentication

    Implements IAuthenticationProvider

    Private NLOGLOGGER As NLog.Logger
    Public Sub New(LOGGER As NLog.Logger)
        NLOGLOGGER = LOGGER
        NLOGLOGGER.Debug("Server Auth.       : FxXTAuthentication")
    End Sub

    Public Function Authenticate(authRequest As AuthRequestMessage) As AuthResponseMessage Implements IAuthenticationProvider.Authenticate

        Dim _Response As New AuthResponseMessage
        Dim _Username As String
        Dim _Password As String

        Dim _Host As String = Net.Dns.GetHostName

        _Response.Success = False

        'NLOGLOGGER.Info("Try to Authenticate with FxXTAuthentication")
        'Dim _UserManagement As UserManagement = Substitute.For(Of UserManagement)()
        'With _UserManagement
        '    .SessionClientAddress.Returns(System.Net.Dns.GetHostEntry(_Host).AddressList(1).ToString())
        '    .SessionIdentityName.Returns("Authenticate")
        'End With

        '' Prüfen die Angaben vollständig sind
        'If authRequest.Credentials.ContainsKey("username") = False Then
        '    Throw New System.Security.SecurityException("NoUsername")
        'End If

        'If authRequest.Credentials.ContainsKey("password") = False Then
        '    Throw New System.Security.SecurityException("NoPassword")
        'End If

        '' Auslesen der Informationen zur einfacheren Verarbeitung
        '_Username = authRequest.Credentials("username")
        'NLOGLOGGER.Debug("=> Username: " & _Username)
        '_Password = authRequest.Credentials("password")
        'NLOGLOGGER.Debug("=> Password: " & _Password)

        '' Überprüfung
        'If _UserManagement.CheckCredentials(_Username, _Password) = True Then

        '    ' Erfogreicher Login
        '    With _Response
        '        .AuthenticatedIdentity = New System.Security.Principal.GenericIdentity(_Username)
        '        .Success = True
        '        NLOGLOGGER.Debug("=> Result: Success")
        '    End With

        'Else

        '    ' Abgewiesener Login
        '    With _Response
        '        .AuthenticatedIdentity = New System.Security.Principal.GenericIdentity(_Username)
        '        .Success = False
        '        .ErrorMessage = "PasswordUserIsWrong"
        '        NLOGLOGGER.Debug("=> Result: Failed")
        '    End With

        'End If

        ' Rückgabe des Ergebnisses
        Return _Response


    End Function

End Class
