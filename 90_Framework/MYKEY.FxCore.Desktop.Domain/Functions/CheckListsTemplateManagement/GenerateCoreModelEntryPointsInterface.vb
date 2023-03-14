' ###################################################################
' #  T4-Name	: GenerateCoreModelEntryPointsInterface.tt          #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTCheckListsTemplates
Imports MYKEY.FxCore.Common.Application
Imports System.Data.Entity.Infrastructure

Public Interface ICheckListsTemplateManagement_ModelEntryPoints

#Region "Queries"

#Region "Queries"

    ''' <summary>
    ''' Zum definieren, wie die Standardabfrage aussehen soll
    ''' </summary>
    ''' <param name="defaultQueryResult"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of CheckListsTemplate))

#End Region


#End Region 

#Region "Functions"

	''' <summary>
    ''' Bevor ein Eintrag hinzugefügt wird, können mit dieser Funktion noch Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="CheckListsTemplateEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_BeforeAddNewCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate)

    ''' <summary>
    ''' In dieser Funktion können Prüfungen erfolgen, die mit den möglichen Entity-Validations nicht möglich sind
    ''' </summary>
    ''' <param name="Dbctx"></param>
    ''' <param name="CheckListsTemplateEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_CheckAddNewCheckListsTemplate(Dbctx As Entities, CheckListsTemplateEntity As CheckListsTemplate) As Boolean

    ''' <summary>
    ''' Erstellt einen neuen Eintrag der mit den Standardwerten gefüllt wird
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_CopyCheckListsTemplate(CheckListsTemplateEntityCopy as CheckListsTemplate) As CheckListsTemplate

    ''' <summary>
    ''' Der Kopierte Eintrag kann vor dem speichern nochmal geändert werden
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_GenerateValidCheckListsTemplateEntity() As CheckListsTemplate

	''' <summary>
    ''' Nachdem ein Eintrag hinzugefügt wurde, kann mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="CheckListsTemplateEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterAddNewCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As ServerResult

	''' <summary>
    ''' Bevor ein Eintrag verändert wird, kann mit dieser Funktion mit den unveränderten Daten noch gearbeitet werden
    ''' </summary>
    ''' <param name="CheckListsTemplateEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_BeforeModifyCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As ServerResult

	''' <summary>
    ''' Bevor ein Eintrag in die Datenbank geschrieben wird, kann mit dieser Funktion die Entität noch angepasst werden
    ''' </summary>
    ''' <param name="CheckListsTemplateEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_ModifyEntityBeforeModifyCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As CheckListsTemplate

	''' <summary>
    ''' Nachdem ein Eintrag verändert wurde, kann mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="CheckListsTemplateEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterModifyCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As ServerResult

	''' <summary>
    ''' Nachdem ein Eintrag gelöscht wurde, können mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="CheckListsTemplateEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterDeleteCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As ServerResult


	
#End Region


End Interface
