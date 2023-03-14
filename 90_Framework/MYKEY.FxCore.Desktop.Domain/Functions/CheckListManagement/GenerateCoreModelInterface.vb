' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTCheckLists
Imports MYKEY.FxCore.Common.Application

Public Interface ICheckListManagement

#Region "Functions"

    Function CreateNewCheckList() As ServerResult

    Function CreateNewCheckList(CheckListEntity As CheckList) As ServerResult

	Function CopyCheckList(CheckListEntity As CheckList) As ServerResult

    Function ModifyCheckList(CheckListEntity As CheckList) As ServerResult

    Function DeleteCheckList(CheckListGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteCheckLists(CheckLists As CheckList(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetCheckLists(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of CheckList)

	Function GetCheckLists(CheckListGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of CheckList)

    Function GetCheckList(CheckListGUID As String, Optional QParam As QueryParameters = Nothing) As CheckList

#End Region

End Interface
