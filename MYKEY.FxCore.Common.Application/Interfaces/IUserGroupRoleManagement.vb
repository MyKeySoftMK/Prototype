Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.DataAccess
Imports System.Collections.ObjectModel
Imports System.ComponentModel.Composition
Imports System.Security.Principal

Public Interface IUserGroupRoleManagement

#Region "Functions"


    ' UserRoles
    Function CreateNewUserGroupRole(UserGroupRoleEntity As FxNTUsers.UserGroupRole) As ServerResult

    Function ModifyUserGroupRole(UserGroupRoleEntity As FxNTUsers.UserGroupRole) As ServerResult

    Function DeleteUserGroupRole(UserGroupRoleGUID As String, Optional PermanentlyDelete As Boolean = False) As ServerResult


    Function AddUserGroupRoleToUserGroup(UserGroupRoleEntity As FxNTUsers.UserGroupRole, UserGroupGUID As String) As ServerResult

#End Region

#Region "Queries"

    ' UserRoles
    Function GetUserGroupRoles(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTUsers.UserGroupRole)

    Function GetUserGroupRole(UserGroupRoleGUID As String, Optional QParam As QueryParameters = Nothing) As FxNTUsers.UserGroupRole


    Function GetUserGroupRoles(UserGUID As String) As ObservableCollection(Of FxNTUsers.UserGroupRole)

#End Region

End Interface
