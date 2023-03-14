' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTGroupLists
Imports MYKEY.FxCore.Common.Application

Public Interface IGroupListManagement

#Region "Functions"

    Function CreateNewGroupList() As ServerResult

    Function CreateNewGroupList(GroupListEntity As GroupList) As ServerResult

	Function CopyGroupList(GroupListEntity As GroupList) As ServerResult

    Function ModifyGroupList(GroupListEntity As GroupList) As ServerResult

    Function DeleteGroupList(GroupListGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteGroupLists(GroupLists As GroupList(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetGroupLists(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of GroupList)

	Function GetGroupLists(GroupListGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of GroupList)

    Function GetGroupList(GroupListGUID As String, Optional QParam As QueryParameters = Nothing) As GroupList

#End Region

End Interface
