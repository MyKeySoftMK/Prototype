Imports MYKEY.FxCore.DataAccess.FxNTAdditionalTypes
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class AdditionalTypeManagement
    Implements IAdditionalTypeManagement_ModelEntryPoints

    Public Function EntryPoint_BeforeAddNewAdditionalType(AdditionalTypeEntity As AdditionalType) Implements IAdditionalTypeManagement_ModelEntryPoints.EntryPoint_BeforeAddNewAdditionalType

        With AdditionalTypeEntity

            NLOGLOGGER.Debug("=> Displaytext: " & .DisplayText)

            ' Zusätzliche Informationen
            NLOGLOGGER.Debug("=> TypeCat-GUID: " & .CategoryGUID.ToString)

        End With

        Return AdditionalTypeEntity

    End Function

    Public Function EntryPoint_CheckAddNewAdditionalType(Dbctx As FxNTAdditionalTypes.Entities, AdditionalTypeEntity As AdditionalType) As Boolean Implements IAdditionalTypeManagement_ModelEntryPoints.EntryPoint_CheckAddNewAdditionalType

        Return True

    End Function

    Public Function EntryPoint_GenerateValidAdditionalTypeEntity() As AdditionalType Implements IAdditionalTypeManagement_ModelEntryPoints.EntryPoint_GenerateValidAdditionalTypeEntity

    End Function

    Public Function EntryPoint_AfterAddNewAdditionalType(AdditionalTypeEntity As AdditionalType) As ServerResult Implements IAdditionalTypeManagement_ModelEntryPoints.EntryPoint_AfterAddNewAdditionalType

    End Function

    Public Function EntryPoint_AfterDeleteAdditionalType(AdditionalTypeEntity As AdditionalType) As ServerResult Implements IAdditionalTypeManagement_ModelEntryPoints.EntryPoint_AfterDeleteAdditionalType

    End Function

    Public Function EntryPoint_AfterModifyAdditionalType(AdditionalTypeEntity As AdditionalType) As ServerResult Implements IAdditionalTypeManagement_ModelEntryPoints.EntryPoint_AfterModifyAdditionalType

    End Function

    Public Function EntryPoint_BeforeModifyAdditionalType(AdditionalTypeEntity As AdditionalType) As ServerResult Implements IAdditionalTypeManagement_ModelEntryPoints.EntryPoint_BeforeModifyAdditionalType

    End Function


    Public Function EntryPoint_ModifyEntityBeforeModifyAdditionalType(AdditionalTypeEntity As AdditionalType) As AdditionalType Implements IAdditionalTypeManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyAdditionalType
        Return AdditionalTypeEntity
    End Function

    Public Function EntryPoint_CopyAdditionalType(AdditionalTypeEntity As AdditionalType) As AdditionalType Implements IAdditionalTypeManagement_ModelEntryPoints.EntryPoint_CopyAdditionalType
        Return AdditionalTypeEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of AdditionalType)) As Object Implements IAdditionalTypeManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function
End Class
