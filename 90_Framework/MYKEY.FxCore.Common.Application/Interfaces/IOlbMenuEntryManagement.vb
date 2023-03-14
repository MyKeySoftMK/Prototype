Imports MYKEY.FxNT.Common
Imports MYKEY.FxNT.DataAccess
Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI
Imports System.Security.Principal

Public Interface IOlbMenuEntryManagement


#Region "Functions"

    Function CreateNewMenuEntry(MenuEntryEntity As DataAccess.FxNTOlbMenu.MenuEntry) As ServerResult

    Function DeleteMenuEntry(MenuEntryGUID As String, Optional PermanentlyDelete As Boolean = False) As ServerResult

    Function ModifyMenuEntry(MenuEntryEntity As DataAccess.FxNTOlbMenu.MenuEntry) As ServerResult

#End Region

#Region "Queries"

    Function GetMenuEntries(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of DataAccess.FxNTOlbMenu.MenuEntry)

    Function GetMenuEntry(MenuEntryGUID As String, Optional QParam As QueryParameters = Nothing) As DataAccess.FxNTOlbMenu.MenuEntry

#End Region

End Interface
