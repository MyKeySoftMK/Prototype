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

Public Interface IPrintInputControlManagement

#Region "Functions"

    Function CreateNewInputControl() As ServerResult

    Function CreateNewInputControl(InputControlEntity As InputControl) As ServerResult

	Function CopyInputControl(InputControlEntity As InputControl) As ServerResult

    Function ModifyInputControl(InputControlEntity As InputControl) As ServerResult

    Function DeleteInputControl(InputControlGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteInputControls(InputControls As InputControl(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetInputControls(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of InputControl)

	Function GetInputControls(InputControlGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of InputControl)

    Function GetInputControl(InputControlGUID As String, Optional QParam As QueryParameters = Nothing) As InputControl

#End Region

End Interface
