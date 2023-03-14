' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTOlbMenu
Imports MYKEY.FxCore.Common.Application

Public Interface IOlbMenuGroupManagement

#Region "Functions"

    Function CreateNewMenuGroup() As ServerResult

    Function CreateNewMenuGroup(MenuGroupEntity As MenuGroup) As ServerResult

	Function CopyMenuGroup(MenuGroupEntity As MenuGroup) As ServerResult

    Function ModifyMenuGroup(MenuGroupEntity As MenuGroup) As ServerResult

    Function DeleteMenuGroup(MenuGroupGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteMenuGroups(MenuGroups As MenuGroup(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetMenuGroups(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of MenuGroup)

	Function GetMenuGroups(MenuGroupGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of MenuGroup)

    Function GetMenuGroup(MenuGroupGUID As String, Optional QParam As QueryParameters = Nothing) As MenuGroup

#End Region

End Interface
