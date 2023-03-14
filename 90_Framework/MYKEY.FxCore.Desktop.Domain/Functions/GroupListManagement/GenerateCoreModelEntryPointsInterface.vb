' ###################################################################
' #  T4-Name	: GenerateCoreModelEntryPointsInterface.tt          #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTGroupLists
Imports MYKEY.FxCore.Common.Application
Imports System.Data.Entity.Infrastructure

Public Interface IGroupListManagement_ModelEntryPoints

#Region "Queries"

#Region "Queries"

    ''' <summary>
    ''' Zum definieren, wie die Standardabfrage aussehen soll
    ''' </summary>
    ''' <param name="defaultQueryResult"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of GroupList))

#End Region


#End Region 

#Region "Functions"

	''' <summary>
    ''' Bevor ein Eintrag hinzugefügt wird, können mit dieser Funktion noch Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="GroupListEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_BeforeAddNewGroupList(GroupListEntity As GroupList)

    ''' <summary>
    ''' In dieser Funktion können Prüfungen erfolgen, die mit den möglichen Entity-Validations nicht möglich sind
    ''' </summary>
    ''' <param name="Dbctx"></param>
    ''' <param name="GroupListEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_CheckAddNewGroupList(Dbctx As Entities, GroupListEntity As GroupList) As Boolean

    ''' <summary>
    ''' Erstellt einen neuen Eintrag der mit den Standardwerten gefüllt wird
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_CopyGroupList(GroupListEntityCopy as GroupList) As GroupList

    ''' <summary>
    ''' Der Kopierte Eintrag kann vor dem speichern nochmal geändert werden
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_GenerateValidGroupListEntity() As GroupList

	''' <summary>
    ''' Nachdem ein Eintrag hinzugefügt wurde, kann mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="GroupListEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterAddNewGroupList(GroupListEntity As GroupList) As ServerResult

	''' <summary>
    ''' Bevor ein Eintrag verändert wird, kann mit dieser Funktion mit den unveränderten Daten noch gearbeitet werden
    ''' </summary>
    ''' <param name="GroupListEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_BeforeModifyGroupList(GroupListEntity As GroupList) As ServerResult

	''' <summary>
    ''' Bevor ein Eintrag in die Datenbank geschrieben wird, kann mit dieser Funktion die Entität noch angepasst werden
    ''' </summary>
    ''' <param name="GroupListEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_ModifyEntityBeforeModifyGroupList(GroupListEntity As GroupList) As GroupList

	''' <summary>
    ''' Nachdem ein Eintrag verändert wurde, kann mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="GroupListEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterModifyGroupList(GroupListEntity As GroupList) As ServerResult

	''' <summary>
    ''' Nachdem ein Eintrag gelöscht wurde, können mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="GroupListEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterDeleteGroupList(GroupListEntity As GroupList) As ServerResult


	
#End Region


End Interface
