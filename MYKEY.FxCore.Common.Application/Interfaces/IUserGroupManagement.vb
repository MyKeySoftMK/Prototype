Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.DataAccess
Imports System.Collections.ObjectModel
Imports System.ComponentModel.Composition
Imports System.Security.Principal

Public Interface IUserGroupManagement

#Region "Functions"


    ' UserGroups
    Function CreateNewUserGroup(UserGroupEntity As FxNTUsers.UserGroup) As ServerResult

    Function ModifyUserGroup(UserGroupEntity As FxNTUsers.UserGroup) As ServerResult

    Function DeleteUserGroup(UserGroupGUID As String, Optional PermanentlyDelete As Boolean = False) As ServerResult


    Function AddUserToUserGroup(UserGroupEntity As FxNTUsers.UserGroup, UserGUID As String) As ServerResult

    Function AddUserRoleToUserGroup(UserGroupEntity As FxNTUsers.UserGroup, UserRoleGUID As String) As ServerResult

    Function AddUserRoleToUserGroup(UserRoleGUID As String, UserGroupGUID As String) As ServerResult

    Function RemoveUserRoleFromUserGroup(UserRoleGUID As String, UserGroupGUID As String) As ServerResult

#End Region

#Region "Queries"

    ' UserGroups
    Function GetUserGroups(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTUsers.UserGroup)

    Function GetUserGroup(UserGroupGUID As String, Optional QParam As QueryParameters = Nothing) As FxNTUsers.UserGroup

#End Region

End Interface
