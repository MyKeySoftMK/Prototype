Imports MYKEY.FxCore.DataAccess.FxNTPrints
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class PrintGroupManagement
    Implements IPrintGroupManagement_ModelEntryPoints

    Public Function EntryPoint_BeforeAddNewPrintGroup(PrintGroupEntity As PrintGroup) As Object Implements IPrintGroupManagement_ModelEntryPoints.EntryPoint_BeforeAddNewPrintGroup

    End Function

    Public Function EntryPoint_CheckAddNewPrintGroup(Dbctx As Entities, PrintGroupEntity As PrintGroup) As Boolean Implements IPrintGroupManagement_ModelEntryPoints.EntryPoint_CheckAddNewPrintGroup

    End Function

    Public Function EntryPoint_GenerateValidPrintGroupEntity() As PrintGroup Implements IPrintGroupManagement_ModelEntryPoints.EntryPoint_GenerateValidPrintGroupEntity
        Return New PrintGroup
    End Function
    Public Function EntryPoint_AfterAddNewPrintGroup(PrintGroupEntity As PrintGroup) As ServerResult Implements IPrintGroupManagement_ModelEntryPoints.EntryPoint_AfterAddNewPrintGroup

    End Function

    Public Function EntryPoint_AfterDeletePrintGroup(PrintGroupEntity As PrintGroup) As ServerResult Implements IPrintGroupManagement_ModelEntryPoints.EntryPoint_AfterDeletePrintGroup

    End Function

    Public Function EntryPoint_AfterModifyPrintGroup(PrintGroupEntity As PrintGroup) As ServerResult Implements IPrintGroupManagement_ModelEntryPoints.EntryPoint_AfterModifyPrintGroup

    End Function

    Public Function EntryPoint_BeforeModifyPrintGroup(PrintGroupEntity As PrintGroup) As ServerResult Implements IPrintGroupManagement_ModelEntryPoints.EntryPoint_BeforeModifyPrintGroup

    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyPrintGroup(PrintGroupEntity As PrintGroup) As PrintGroup Implements IPrintGroupManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyPrintGroup
        Return PrintGroupEntity
    End Function

    Public Function EntryPoint_CopyPrintGroup(PrintGroupEntity As PrintGroup) As PrintGroup Implements IPrintGroupManagement_ModelEntryPoints.EntryPoint_CopyPrintGroup
        Return PrintGroupEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of PrintGroup)) As Object Implements IPrintGroupManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

End Class
