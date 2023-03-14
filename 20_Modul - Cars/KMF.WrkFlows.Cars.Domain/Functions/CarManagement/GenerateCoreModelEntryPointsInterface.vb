' ###################################################################
' #  T4-Name	: GenerateCoreModelEntryPointsInterface.tt          #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports KMF.WrkFlows.Cars.DataAccess
Imports KMF.WrkFlows.Cars.DataAccess.Cars
Imports MYKEY.FxCore.Common.Application
Imports System.Data.Entity.Infrastructure

Public Interface ICarManagement_ModelEntryPoints

#Region "Queries"

#Region "Queries"

    ''' <summary>
    ''' Zum definieren, wie die Standardabfrage aussehen soll
    ''' </summary>
    ''' <param name="defaultQueryResult"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of Car))

#End Region


#End Region 

#Region "Functions"

	''' <summary>
    ''' Bevor ein Eintrag hinzugefügt wird, können mit dieser Funktion noch Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="CarEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_BeforeAddNewCar(CarEntity As Car)

    ''' <summary>
    ''' In dieser Funktion können Prüfungen erfolgen, die mit den möglichen Entity-Validations nicht möglich sind
    ''' </summary>
    ''' <param name="Dbctx"></param>
    ''' <param name="CarEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_CheckAddNewCar(Dbctx As Entities, CarEntity As Car) As Boolean

    ''' <summary>
    ''' Erstellt einen neuen Eintrag der mit den Standardwerten gefüllt wird
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_CopyCar(CarEntityCopy as Car) As Car

    ''' <summary>
    ''' Der Kopierte Eintrag kann vor dem speichern nochmal geändert werden
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_GenerateValidCarEntity() As Car

	''' <summary>
    ''' Nachdem ein Eintrag hinzugefügt wurde, kann mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="CarEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterAddNewCar(CarEntity As Car) As ServerResult

	''' <summary>
    ''' Bevor ein Eintrag verändert wird, kann mit dieser Funktion mit den unveränderten Daten noch gearbeitet werden
    ''' </summary>
    ''' <param name="CarEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_BeforeModifyCar(CarEntity As Car) As ServerResult

	''' <summary>
    ''' Bevor ein Eintrag in die Datenbank geschrieben wird, kann mit dieser Funktion die Entität noch angepasst werden
    ''' </summary>
    ''' <param name="CarEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_ModifyEntityBeforeModifyCar(CarEntity As Car) As Car

	''' <summary>
    ''' Nachdem ein Eintrag verändert wurde, kann mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="CarEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterModifyCar(CarEntity As Car) As ServerResult

	''' <summary>
    ''' Nachdem ein Eintrag gelöscht wurde, können mit dieser Funktion noch weitere Veränderungen vorgenommen werden
    ''' </summary>
    ''' <param name="CarEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Function EntryPoint_AfterDeleteCar(CarEntity As Car) As ServerResult


	
#End Region


End Interface
