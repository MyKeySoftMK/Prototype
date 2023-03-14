Imports System.Data.Entity.Infrastructure
Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.DataAccess.FxNTCheckLists
Imports MYKEY.FxCore.Desktop.Domain

Partial Public Class CheckListManagement
    Implements ICheckListManagement_ModelEntryPoints

    Public Function EntryPoint_AfterAddNewCheckList(CheckListEntity As CheckList) As ServerResult Implements ICheckListManagement_ModelEntryPoints.EntryPoint_AfterAddNewCheckList
    End Function

    Public Function EntryPoint_AfterDeleteCheckList(CheckListEntity As CheckList) As ServerResult Implements ICheckListManagement_ModelEntryPoints.EntryPoint_AfterDeleteCheckList
    End Function

    Public Function EntryPoint_AfterModifyCheckList(CheckListEntity As CheckList) As ServerResult Implements ICheckListManagement_ModelEntryPoints.EntryPoint_AfterModifyCheckList
    End Function

    Public Function EntryPoint_BeforeAddNewCheckList(CheckListEntity As CheckList) As Object Implements ICheckListManagement_ModelEntryPoints.EntryPoint_BeforeAddNewCheckList
    End Function

    Public Function EntryPoint_BeforeModifyCheckList(CheckListEntity As CheckList) As ServerResult Implements ICheckListManagement_ModelEntryPoints.EntryPoint_BeforeModifyCheckList
    End Function

    Public Function EntryPoint_CheckAddNewCheckList(Dbctx As Entities, CheckListEntity As CheckList) As Boolean Implements ICheckListManagement_ModelEntryPoints.EntryPoint_CheckAddNewCheckList
    End Function

    Public Function EntryPoint_CopyCheckList(CheckListEntityCopy As CheckList) As CheckList Implements ICheckListManagement_ModelEntryPoints.EntryPoint_CopyCheckList
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of CheckList)) As Object Implements ICheckListManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery
    End Function

    Public Function EntryPoint_GenerateValidCheckListEntity() As CheckList Implements ICheckListManagement_ModelEntryPoints.EntryPoint_GenerateValidCheckListEntity
    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyCheckList(CheckListEntity As CheckList) As CheckList Implements ICheckListManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyCheckList
    End Function
End Class
