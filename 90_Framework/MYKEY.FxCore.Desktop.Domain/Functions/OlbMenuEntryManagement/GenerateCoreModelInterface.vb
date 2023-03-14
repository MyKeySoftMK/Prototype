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

Public Interface IOlbMenuEntryManagement

#Region "Functions"

    Function CreateNewMenuEntry() As ServerResult

    Function CreateNewMenuEntry(MenuEntryEntity As MenuEntry) As ServerResult

	Function CopyMenuEntry(MenuEntryEntity As MenuEntry) As ServerResult

    Function ModifyMenuEntry(MenuEntryEntity As MenuEntry) As ServerResult

    Function DeleteMenuEntry(MenuEntryGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteMenuEntries(MenuEntries As MenuEntry(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetMenuEntries(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of MenuEntry)

	Function GetMenuEntries(MenuEntryGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of MenuEntry)

    Function GetMenuEntry(MenuEntryGUID As String, Optional QParam As QueryParameters = Nothing) As MenuEntry

#End Region

End Interface
