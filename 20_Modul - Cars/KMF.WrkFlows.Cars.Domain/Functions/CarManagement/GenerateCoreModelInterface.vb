' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports KMF.WrkFlows.Cars.DataAccess
Imports KMF.WrkFlows.Cars.DataAccess.Cars
Imports MYKEY.FxCore.Common.Application

Public Interface ICarManagement

#Region "Functions"

    Function CreateNewCar() As ServerResult

    Function CreateNewCar(CarEntity As Car) As ServerResult

	Function CopyCar(CarEntity As Car) As ServerResult

    Function ModifyCar(CarEntity As Car) As ServerResult

    Function DeleteCar(CarGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteCars(Cars As Car(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetCars(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Car)

	Function GetCars(CarGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Car)

    Function GetCar(CarGUID As String, Optional QParam As QueryParameters = Nothing) As Car

#End Region

End Interface
