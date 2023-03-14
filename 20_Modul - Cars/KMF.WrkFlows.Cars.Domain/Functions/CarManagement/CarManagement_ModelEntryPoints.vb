Imports System.Data.Entity.Infrastructure
Imports KMF.WrkFlows.Cars.DataAccess.Cars
Imports MYKEY.FxCore.Common

Partial Public Class CarManagement

    Implements ICarManagement_ModelEntryPoints

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of Car)) As Object Implements ICarManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

    Public Function EntryPoint_BeforeAddNewCar(CarEntity As Car) As Object Implements ICarManagement_ModelEntryPoints.EntryPoint_BeforeAddNewCar
        Return CarEntity
    End Function

    Public Function EntryPoint_CheckAddNewCar(Dbctx As Entities, CarEntity As Car) As Boolean Implements ICarManagement_ModelEntryPoints.EntryPoint_CheckAddNewCar
        Return True
    End Function

    Public Function EntryPoint_CopyCar(CarEntityCopy As Car) As Car Implements ICarManagement_ModelEntryPoints.EntryPoint_CopyCar
        Return CarEntityCopy
    End Function

    Public Function EntryPoint_GenerateValidCarEntity() As Car Implements ICarManagement_ModelEntryPoints.EntryPoint_GenerateValidCarEntity
        Dim _Car As New Car

        With _Car
            .CarName = ""
            .CarTypeNumber = ""
            .CarColorsText = ""
            .CarDecors = ""
            .CarModellYear = ""
            .CarSpecial = False
        End With

        Return _Car
    End Function

    Public Function EntryPoint_AfterAddNewCar(CarEntity As Car) As ServerResult Implements ICarManagement_ModelEntryPoints.EntryPoint_AfterAddNewCar

    End Function

    Public Function EntryPoint_BeforeModifyCar(CarEntity As Car) As ServerResult Implements ICarManagement_ModelEntryPoints.EntryPoint_BeforeModifyCar

    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyCar(CarEntity As Car) As Car Implements ICarManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyCar
        Return CarEntity
    End Function

    Public Function EntryPoint_AfterModifyCar(CarEntity As Car) As ServerResult Implements ICarManagement_ModelEntryPoints.EntryPoint_AfterModifyCar

    End Function

    Public Function EntryPoint_AfterDeleteCar(CarEntity As Car) As ServerResult Implements ICarManagement_ModelEntryPoints.EntryPoint_AfterDeleteCar

    End Function
End Class
