Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.DataAccess
Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI
Imports System.Security.Principal

Public Interface IGroupListManagement

#Region "Functions"

    Function CreateNewGroupList(GroupListEntity As FxNTGroupLists.GroupList) As ServerResult

    Function ModifyGroupList(GroupListEntity As FxNTGroupLists.GroupList) As ServerResult

    Function DeleteGroupList(GroupListGUID As String, Optional PermanentlyDelete As Boolean = False) As ServerResult

#End Region

#Region "Queries"

    Function GetGroupLists(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTGroupLists.GroupList)

    Function GetGroupList(GroupListGUID As String, Optional QParam As QueryParameters = Nothing) As FxNTGroupLists.GroupList


    Function GetGroupListsByGroupListName(GroupListName As String, Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTGroupLists.GroupList)

#End Region

End Interface
