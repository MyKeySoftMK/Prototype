' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTPrints
Imports MYKEY.FxCore.Common.Application

Public Interface IPrintGroupManagement

#Region "Functions"

    Function CreateNewPrintGroup() As ServerResult

    Function CreateNewPrintGroup(PrintGroupEntity As PrintGroup) As ServerResult

	Function CopyPrintGroup(PrintGroupEntity As PrintGroup) As ServerResult

    Function ModifyPrintGroup(PrintGroupEntity As PrintGroup) As ServerResult

    Function DeletePrintGroup(PrintGroupGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeletePrintGroups(PrintGroups As PrintGroup(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetPrintGroups(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of PrintGroup)

	Function GetPrintGroups(PrintGroupGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of PrintGroup)

    Function GetPrintGroup(PrintGroupGUID As String, Optional QParam As QueryParameters = Nothing) As PrintGroup

#End Region

End Interface
