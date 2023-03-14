' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTCheckListsTemplates
Imports MYKEY.FxCore.Common.Application

Public Interface ICheckListsTemplateManagement

#Region "Functions"

    Function CreateNewCheckListsTemplate() As ServerResult

    Function CreateNewCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As ServerResult

	Function CopyCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As ServerResult

    Function ModifyCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As ServerResult

    Function DeleteCheckListsTemplate(CheckListsTemplateGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteCheckListsTemplates(CheckListsTemplates As CheckListsTemplate(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetCheckListsTemplates(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of CheckListsTemplate)

	Function GetCheckListsTemplates(CheckListsTemplateGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of CheckListsTemplate)

    Function GetCheckListsTemplate(CheckListsTemplateGUID As String, Optional QParam As QueryParameters = Nothing) As CheckListsTemplate

#End Region

End Interface
