' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTUsers
Imports MYKEY.FxCore.Common.Application

Public Interface IUserGroupRoleManagement

#Region "Functions"

    Function CreateNewUserGroupRole() As ServerResult

    Function CreateNewUserGroupRole(UserGroupRoleEntity As UserGroupRole) As ServerResult

	Function CopyUserGroupRole(UserGroupRoleEntity As UserGroupRole) As ServerResult

    Function ModifyUserGroupRole(UserGroupRoleEntity As UserGroupRole) As ServerResult

    Function DeleteUserGroupRole(UserGroupRoleGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteUserGroupRoles(UserGroupRoles As UserGroupRole(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetUserGroupRoles(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of UserGroupRole)

	Function GetUserGroupRoles(UserGroupRoleGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of UserGroupRole)

    Function GetUserGroupRole(UserGroupRoleGUID As String, Optional QParam As QueryParameters = Nothing) As UserGroupRole

#End Region

End Interface
