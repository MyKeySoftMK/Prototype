Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.ComponentModel.Composition
Imports System.Data.Objects

Imports MYKEY.FxCore
Imports MYKEY.FxCore.Common.Application
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTOlbMenu

Imports MYKEY.FxCore.Common
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess.FxNTUsers
Imports System.Data.Entity.Infrastructure

Partial Public Class UserManagement

#Region "Functions"

    ''' <summary>
    ''' Überprüft, ob der Benutzername und das Kennwort mit den in der Datenbank hinterlegten
    ''' Informationen übereinstimmen
    ''' </summary>
    ''' <param name="Username">Benutzername des Benutzers</param>
    ''' <param name="Password">Kennwort des Benutzers</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckCredentials(Username As String, Password As String) As UserManagementEnums.CheckCredentialsResult

        Dim serverUsers As List(Of FxNTUsers.User)
        Dim crypt As New CryptingHASH

        Dim result As UserManagementEnums.CheckCredentialsResult = UserManagementEnums.CheckCredentialsResult.NotChecked

        NLOGLOGGER.Info("Check User Credentials")

        Try

            ' Suchen des Benutzers
            NLOGLOGGER.Debug("=> Try to find user '" & Username & "'")
            serverUsers = (From Users In DbCtx.Users Where Users.Name = Username).ToList
            If serverUsers.Count = 1 Then

                NLOGLOGGER.Debug("=> User '" & Username & "' found")
                ' Prüfen, ob das hinterlegte und übergebene Kennwort passen
                If crypt.ComparePasswords(serverUsers(0).PasswordHASH, Password) = True Then
                    result = UserManagementEnums.CheckCredentialsResult.Success
                Else
                    result = UserManagementEnums.CheckCredentialsResult.PasswordNotCorrect
                End If

            Else
                result = UserManagementEnums.CheckCredentialsResult.UserNameNotFound
            End If


        Catch ex As Exception
            result = UserManagementEnums.CheckCredentialsResult.GeneralFailure
            NLOGLOGGER.Error(ex.Message)
        End Try

        NLOGLOGGER.Info("=> User Credentials for '" & Username & "': " & result.ToString)
        Return result

    End Function

    ''' <summary>
    ''' Diese Methode macht es möglich, dass ein Kennwort geändert werden kann, wenn man das zuvor gespeicherte Kennwort kennt
    ''' </summary>
    ''' <param name="UserGUID">Die UserGUID des Benutzers, dessen Kennwort geänder werden soll</param>
    ''' <param name="oldPassword">Das aktuell gespeicherte Kennwort</param>
    ''' <param name="newPassword">Das neue zu speichernde Kennwort</param>
    ''' <returns></returns>
    ''' <remarks>Um ein Kennwort ohne Verifizierung zu überschreiben (z.B. innerhalb von Admin-Tools), ist als 
    ''' oldPassword eine leere Zeichenkette zu übergeben</remarks>
    Public Function ChangeUserPassword(UserGUID As String, oldPassword As String, newPassword As String) As ServerResult

        Dim serverUsers As List(Of FxNTUsers.User)
        Dim crypt As New CryptingHASH
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult

        ' Für das Hashing/Salting des Kennworts
        Dim result As Boolean = False

        If oldPassword Is Nothing Then
            oldPassword = ""
        End If

        Try
            NLOGLOGGER.Info("Change Users Password")

            ' Suchen des Benutzers
            guid = guidConvert.ConvertFromString(UserGUID)
            NLOGLOGGER.Debug("=> Search User with GUID '" & UserGUID & "'")
            serverUsers = (From Users In DbCtx.Users Where Users.GUID = guid).ToList

            If serverUsers.Count = 1 Then

                NLOGLOGGER.Debug("=> Found User")
                If oldPassword.Length > 0 Then
                    NLOGLOGGER.Debug("=> Check for old Password")
                    ' Prüfen, ob das hinterlegte und übergebene Kennwort passen
                    If crypt.ComparePasswords(serverUsers(0).PasswordHASH, oldPassword) = True Then

                        NLOGLOGGER.Debug("=> Old password PASSED")
                        serverUsers(0).PasswordHASH = crypt.CreatePasswordForDb(newPassword)
                        DbCtx.SaveChanges()

                        NLOGLOGGER.Debug("=> New password is successfull set")

                    Else
                        NLOGLOGGER.Error("=> Old password FAILED")
                        _result.ErrorMessages.Add("Old password was failed")
                    End If

                Else

                    NLOGLOGGER.Debug("=> Check Users-Permissions")

                    '' InventureDeleteInventures wird benötigt
                    'If SessionIdentityPrincipal.IsInRole("00000000-0000-0000-0000-f00000000001") = False Then
                    '    Throw New InvalidOperationException("Userpermission to execute this function is missing")
                    'End If

                    ' Das bestehende Kennwort wird überschrieben
                    serverUsers(0).PasswordHASH = crypt.CreatePasswordForDb(newPassword)
                    DbCtx.SaveChanges()

                    NLOGLOGGER.Debug("=> New password is successfull set'")

                End If

            Else
                NLOGLOGGER.Info("=> User with GUID '" & UserGUID & "' not found")
                NLOGLOGGER.Info("=> No changing of password is possible")

            End If

        Catch ex As Exception

            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)
            _result.ErrorMessages.Add(_exception.Message)

        End Try

        Return _result

    End Function

#End Region


#Region "Queries"


    ''' <summary>
    ''' Gibt die Informationen für einen Benutzer zurück
    ''' </summary>
    ''' <param name="UserName"></param>
    ''' <returns>Wenn kein Benutzer gefunden wird, wird Nothing zurück gegeben</returns>
    ''' <remarks></remarks>
    Public Function GetUserByUserName(UserName As String, Optional QParam As QueryParameters = Nothing) As FxNTUsers.User
        Dim serverUsers As FxNTUsers.User = Nothing

        Try
            NLOGLOGGER.Info("Get User Informations ")

            NLOGLOGGER.Debug("=> Search for User with Username '" & UserName & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverUsers = DefaultQueryUsers.Single(Function(users) users.Name = UserName)
            End With

            NLOGLOGGER.Debug("=> Found User with GUID '" & serverUsers.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverUsers
    End Function

    ''' <summary>
    ''' Sucht einen Benutzer nach seinem Anzeigename
    ''' </summary>
    ''' <param name="UserDisplayName"></param>
    ''' <param name="QParam"></param>
    ''' <returns>Wenn kein Benutzer gefunden wird, wird Nothing zurück gegeben</returns>
    ''' <remarks></remarks>
    Public Function GetUserByDisplayName(UserDisplayName As String, Optional QParam As Common.Application.QueryParameters = Nothing) As FxNTUsers.User

        Dim serverUsers As FxNTUsers.User = Nothing

        Try
            NLOGLOGGER.Info("Get User Informations ")

            NLOGLOGGER.Debug("=> Search for User with DispplayName '" & UserDisplayName & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverUsers = DefaultQueryUsers.Single(Function(users) users.DisplayName = UserDisplayName)
            End With

            NLOGLOGGER.Debug("=> Found User with GUID '" & serverUsers.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverUsers

    End Function

    ''' <summary>
    ''' Diese Methode stellt eine Liste mit aller Benutzerrechte eines bestimmten Benutzers zusammen. Daraus kann man dann entsprechende
    ''' Pricipals-Objekte erstellen
    ''' </summary>
    ''' <param name="UserName"></param>
    ''' <returns></returns>
    ''' <remarks>Noch nicht getestet</remarks>
    Public Function GetUserPrinicpal(UserName As String) As GenericPrincipal

        Dim serverUser As User
        Dim serverRoles As System.Collections.ObjectModel.ObservableCollection(Of UserGroupRole)
        Dim UserIdentity As GenericIdentity
        Dim UserPrincipal As GenericPrincipal
        Dim _counter As Integer = 0
        Dim UsrGrpRoleMagement As New UserGroupRoleManagement(Me.EntityConnectionData)

        Try

            NLOGLOGGER.Info("Create SecurityPricipal-Object ")

            ' Lesen der Berechtigungsrollen des Benutzer
            serverUser = GetUserByUserName(UserName)
            serverRoles = UsrGrpRoleMagement.GetUserRoles(serverUser.GUID.ToString)

            ' Erzeugen eines Benutzers, damit die Sicherheitsfunktionen des .NET-Frameworks genutzt werden können
            NLOGLOGGER.Debug("=> Create .NET UserIdentity")
            UserIdentity = New GenericIdentity(serverUser.GUID.ToString)

            ' Erzeugen und aktivieren des Rechteobjekt (Benutzer und Rollen)
            NLOGLOGGER.Debug("=> Create .NET UserPrincipal")
            Dim RolesArray(serverRoles.Count - 1) As String
            For Each Role As UserGroupRole In serverRoles
                NLOGLOGGER.Debug("=> Add Role: " & Role.Name)
                RolesArray(_counter) = Role.GUID.ToString
                _counter = _counter + 1
            Next
            UserPrincipal = New GenericPrincipal(UserIdentity, RolesArray)

            NLOGLOGGER.Debug("=> Create succesfull SecurityPricipal-Object")
            Return UserPrincipal

        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return Nothing

    End Function

#End Region

End Class
