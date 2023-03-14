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

Public Interface IPrintManagement

#Region "Functions"

    Function CreateNewPrint() As ServerResult

    Function CreateNewPrint(PrintEntity As Print) As ServerResult

	Function CopyPrint(PrintEntity As Print) As ServerResult

    Function ModifyPrint(PrintEntity As Print) As ServerResult

    Function DeletePrint(PrintGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeletePrints(Prints As Print(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetPrints(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Print)

	Function GetPrints(PrintGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Print)

    Function GetPrint(PrintGUID As String, Optional QParam As QueryParameters = Nothing) As Print

#End Region

End Interface
