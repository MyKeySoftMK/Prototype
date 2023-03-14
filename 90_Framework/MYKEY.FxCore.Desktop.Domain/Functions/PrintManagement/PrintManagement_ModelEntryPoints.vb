Imports MYKEY.FxCore.DataAccess.FxNTPrints
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class PrintManagement
    Implements IPrintManagement_ModelEntryPoints

    Public Function EntryPoint_BeforeAddNewPrint(PrintEntity As Print) As Object Implements IPrintManagement_ModelEntryPoints.EntryPoint_BeforeAddNewPrint
        Return PrintEntity
    End Function

    Public Function EntryPoint_CheckAddNewPrint(Dbctx As Entities, PrintEntity As Print) As Boolean Implements IPrintManagement_ModelEntryPoints.EntryPoint_CheckAddNewPrint

    End Function

    Public Function EntryPoint_GenerateValidPrintEntity() As Print Implements IPrintManagement_ModelEntryPoints.EntryPoint_GenerateValidPrintEntity

        Return New Print

    End Function

    Public Function EntryPoint_AfterAddNewPrint(PrintEntity As Print) As ServerResult Implements IPrintManagement_ModelEntryPoints.EntryPoint_AfterAddNewPrint

    End Function

    Public Function EntryPoint_AfterDeletePrint(PrintEntity As Print) As ServerResult Implements IPrintManagement_ModelEntryPoints.EntryPoint_AfterDeletePrint

    End Function

    Public Function EntryPoint_AfterModifyPrint(PrintEntity As Print) As ServerResult Implements IPrintManagement_ModelEntryPoints.EntryPoint_AfterModifyPrint

    End Function

    Public Function EntryPoint_BeforeModifyPrint(PrintEntity As Print) As ServerResult Implements IPrintManagement_ModelEntryPoints.EntryPoint_BeforeModifyPrint

    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyPrint(PrintEntity As Print) As Print Implements IPrintManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyPrint
        Return PrintEntity
    End Function

    Public Function EntryPoint_CopyPrint(PrintEntity As Print) As Print Implements IPrintManagement_ModelEntryPoints.EntryPoint_CopyPrint
        Return PrintEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of Print)) As Object Implements IPrintManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

End Class
