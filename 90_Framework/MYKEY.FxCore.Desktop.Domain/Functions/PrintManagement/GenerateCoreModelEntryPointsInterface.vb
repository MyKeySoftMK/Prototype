' ###################################################################
' #  T4-Name	: GenerateCoreModelEntryPointsInterface.tt          #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTPrints
Imports MYKEY.FxCore.Common.Application
Imports System.Data.Entity.Infrastructure

Public Interface IPrintManagement_ModelEntryPoints

#Region "Queries"

#Region "Queries"

    ''' <summary>
    ''' Zum definieren, wie die Standardabfrage aussehen soll
    ''' </summary>
    ''' <param name="defaultQueryResult"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of Print))

#End Region


#End Region 

#Region "Functions"

	''' <summary>
    ''' Bevor ein Eintrag hinzugefügt wird, können mit dieser Funktion noch Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="PrintEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_BeforeAddNewPrint(PrintEntity As Print)

    ''' <summary>
    ''' In dieser Funktion können Prüfungen erfolgen, die mit den möglichen Entity-Validations nicht möglich sind
    ''' </summary>
    ''' <param name="Dbctx"></param>
    ''' <param name="PrintEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_CheckAddNewPrint(Dbctx As Entities, PrintEntity As Print) As Boolean

    ''' <summary>
    ''' Erstellt einen neuen Eintrag der mit den Standardwerten gefüllt wird
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_CopyPrint(PrintEntityCopy as Print) As Print

    ''' <summary>
    ''' Der Kopierte Eintrag kann vor dem speichern nochmal geändert werden
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_GenerateValidPrintEntity() As Print

	''' <summary>
    ''' Nachdem ein Eintrag hinzugefügt wurde, kann mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="PrintEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterAddNewPrint(PrintEntity As Print) As ServerResult

	''' <summary>
    ''' Bevor ein Eintrag verändert wird, kann mit dieser Funktion mit den unveränderten Daten noch gearbeitet werden
    ''' </summary>
    ''' <param name="PrintEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_BeforeModifyPrint(PrintEntity As Print) As ServerResult

	''' <summary>
    ''' Bevor ein Eintrag in die Datenbank geschrieben wird, kann mit dieser Funktion die Entität noch angepasst werden
    ''' </summary>
    ''' <param name="PrintEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_ModifyEntityBeforeModifyPrint(PrintEntity As Print) As Print

	''' <summary>
    ''' Nachdem ein Eintrag verändert wurde, kann mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="PrintEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterModifyPrint(PrintEntity As Print) As ServerResult

	''' <summary>
    ''' Nachdem ein Eintrag gelöscht wurde, können mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="PrintEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterDeletePrint(PrintEntity As Print) As ServerResult


	
#End Region


End Interface
