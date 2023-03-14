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

Public Interface IUserGroupManagement

#Region "Functions"

    Function CreateNewUserGroup() As ServerResult

    Function CreateNewUserGroup(UserGroupEntity As UserGroup) As ServerResult

	Function CopyUserGroup(UserGroupEntity As UserGroup) As ServerResult

    Function ModifyUserGroup(UserGroupEntity As UserGroup) As ServerResult

    Function DeleteUserGroup(UserGroupGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteUserGroups(UserGroups As UserGroup(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetUserGroups(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of UserGroup)

	Function GetUserGroups(UserGroupGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of UserGroup)

    Function GetUserGroup(UserGroupGUID As String, Optional QParam As QueryParameters = Nothing) As UserGroup

#End Region

End Interface
