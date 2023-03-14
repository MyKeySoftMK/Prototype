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

Partial Class UserGroupRoleManagement

#Region "Functions"

    <Obsolete>
    Public Function AddUserRoleToUserGroup(UserRoleEntity As FxNTUsers.UserGroupRole, UserGroupGUID As String) As ServerResult
        Dim newUserRoleToGroup As FxNTUsers.UserGroupToUserGroupRole
        Dim _result As New ServerResult
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid
        Dim serverUsersRole As FxNTUsers.UserGroupRole

        Try
            NLOGLOGGER.Info("UserRole will assign to a Usergroup ")

            newUserRoleToGroup = New FxNTUsers.UserGroupToUserGroupRole
            With newUserRoleToGroup
                _guid = guidConvert.ConvertFromString(UserGroupGUID)
                .UserGroupGUID = _guid
                NLOGLOGGER.Debug("=> UserGroupGUID: " & UserGroupGUID)
            End With

            NLOGLOGGER.Debug("=> Get UserRole with GUID '" & UserRoleEntity.GUID.ToString & "'")
            serverUsersRole = DbCtx.UserGroupRoles.Single(Function(usersroles) usersroles.GUID = UserRoleEntity.GUID)

            NLOGLOGGER.Debug("=> Add usergroup information")
            serverUsersRole.UserGroups.Add(newUserRoleToGroup)

            NLOGLOGGER.Debug("=> Apply changes")
            'EntityContext.UserGroupRoles.ApplyCurrentValues(serverUsersRole)
            DbCtx.SaveChanges()


            NLOGLOGGER.Info("User is successfull assign to UserGroup")
            _result.ReturnValue = serverUsersRole.UserGroups(serverUsersRole.UserGroups.Count - 1).GUID.ToString
            NLOGLOGGER.Info("=> EntryGUID: " & _result.ReturnValue)

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
    ''' Diese Methode stellt eine Liste mit aller Benutzerrechte eines bestimmten Benutzers zusammen. Daraus kann man dann entsprechende
    ''' Pricipals-Objekte erstellen
    ''' </summary>
    ''' <param name="UserGUID"></param>
    ''' <returns></returns>
    ''' <remarks>Noch nicht getestet</remarks>
    <Obsolete>
    Public Function GetUserRoles(UserGUID As String) As System.Collections.ObjectModel.ObservableCollection(Of FxNTUsers.UserGroupRole)

        Dim CurrentUser As FxNTUsers.User
        Dim CurrentRoles As New ObservableCollection(Of FxNTUsers.UserGroupRole)
        Dim UserMgm As New UserManagement(Me.EntityConnectionData)


        ' Lesen der Benutzerinformation
        Dim QParam As QueryParameters = New QueryParameters
        With QParam
            .Includes.Add("UserGroupsList")
            .Includes.Add("UserGroupsList.UserGroup")
            .Includes.Add("UserGroupsList.UserGroup.UserGroupRolesList")
            .Includes.Add("UserGroupsList.UserGroup.UserGroupRolesList.UserGroupRole")
        End With
        CurrentUser = UserMgm.GetUser(UserGUID, QParam)

        'Erzeugen einer Liste aller Benutzerrechte
        For Each grp As FxNTUsers.UserToUserGroup In CurrentUser.UserGroups

            For Each rol As FxNTUsers.UserGroupToUserGroupRole In grp.UserGroup.UserGroupRoles

                ' TODO: Nur die Rollen hinzufügen, die noch nicht eingefügt sind
                CurrentRoles.Add(rol.UserGroupRole)

            Next

        Next

        ' Rückgabe der gesammelten Informationen
        Return CurrentRoles

    End Function

#End Region

End Class
