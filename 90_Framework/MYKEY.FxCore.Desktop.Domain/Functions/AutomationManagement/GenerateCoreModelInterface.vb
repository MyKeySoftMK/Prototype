' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTAutomations
Imports MYKEY.FxCore.Common.Application

Public Interface IAutomationManagement

#Region "Functions"

    Function CreateNewAutomation() As ServerResult

    Function CreateNewAutomation(AutomationEntity As Automation) As ServerResult

	Function CopyAutomation(AutomationEntity As Automation) As ServerResult

    Function ModifyAutomation(AutomationEntity As Automation) As ServerResult

    Function DeleteAutomation(AutomationGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteAutomations(Automations As Automation(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetAutomations(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Automation)

	Function GetAutomations(AutomationGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Automation)

    Function GetAutomation(AutomationGUID As String, Optional QParam As QueryParameters = Nothing) As Automation

#End Region

End Interface
