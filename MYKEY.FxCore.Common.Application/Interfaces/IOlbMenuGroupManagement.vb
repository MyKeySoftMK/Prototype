Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.DataAccess
Imports System.Collections.ObjectModel
Imports System.Security.Principal

Public Interface IOlbMenuGroupManagement


#Region "Functions"

    Function CreateNewMenuGroup(MenuGroupEntity As DataAccess.FxNTOlbMenu.MenuGroup) As ServerResult

    Function DeleteMenuGroup(MenuGroupGUID As String, Optional PermanentlyDelete As Boolean = False) As ServerResult

    Function ModifyMenuGroup(MenuGroupEntity As DataAccess.FxNTOlbMenu.MenuGroup) As ServerResult

#End Region

#Region "Queries"

    Function GetMenuGroups(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of DataAccess.FxNTOlbMenu.MenuGroup)

    Function GetMenuGroup(MenuGroupGUID As String, Optional QParam As QueryParameters = Nothing) As DataAccess.FxNTOlbMenu.MenuGroup


    ' Proxy-Functions
    Function GetUserPrincipal(UserName As String) As GenericPrincipal

    Function GetUserByUserName(UserName As String) As MYKEY.FxCore.DataAccess.FxNTUsers.User

#End Region

End Interface
