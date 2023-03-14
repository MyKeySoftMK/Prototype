Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.DataAccess
Imports System.Collections.ObjectModel
Imports System.ComponentModel.Composition
Imports System.Security.Principal

Public Interface IUserManagement

#Region "Functions"

    ' Users

    ''' <summary>
    ''' Erstellt einen neuen Benutzer
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function CreateNewUser(UserEntity As FxNTUsers.User) As ServerResult

    Function ModifyUser(UserEntity As FxNTUsers.User) As ServerResult

    Function DeleteUser(UserGUID As String, Optional PermanentlyDelete As Boolean = False) As ServerResult


    Function ChangeUserPassword(UserGUID As String, oldPassword As String, newPassword As String) As ServerResult

    Function CheckCredentials(UserGUID As String, Password As String) As UserManagementEnums.CheckCredentialsResult

    Function AddUserToUserGroup(UserEntity As FxNTUsers.User, UserGroupGUID As String) As ServerResult

    Function AddUserToUserGroup(UserGUID As String, UserGroupGUID As String) As ServerResult

    Function RemoveUserFromUserGroup(UserGUID As String, UserGroupGUID As String) As ServerResult


#End Region

#Region "Queries"

    ' Users
    Function GetUsers(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTUsers.User)

    Function GetUser(UserGUID As String, Optional QParam As QueryParameters = Nothing) As FxNTUsers.User


    Function GetUserByUserName(UserName As String, Optional QParam As QueryParameters = Nothing) As FxNTUsers.User

    Function GetUserByDisplayName(UserDisplayName As String, Optional QParam As QueryParameters = Nothing) As FxNTUsers.User

    Function GetUserPrincipal(UserName As String) As GenericPrincipal

#End Region

End Interface
