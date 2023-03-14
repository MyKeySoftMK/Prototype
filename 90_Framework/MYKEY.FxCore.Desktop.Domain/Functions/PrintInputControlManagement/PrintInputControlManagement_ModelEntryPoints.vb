Imports MYKEY.FxCore.DataAccess.FxNTPrints
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class PrintInputControlManagement
    Implements IPrintInputControlManagement_ModelEntryPoints

    Public Function EntryPoint_BeforeAddNewInputControl(InputControlEntity As InputControl) As Object Implements IPrintInputControlManagement_ModelEntryPoints.EntryPoint_BeforeAddNewInputControl
        Return InputControlEntity
    End Function

    Public Function EntryPoint_CheckAddNewInputControl(Dbctx As Entities, InputControlEntity As InputControl) As Boolean Implements IPrintInputControlManagement_ModelEntryPoints.EntryPoint_CheckAddNewInputControl
        Return True
    End Function

    Public Function EntryPoint_GenerateValidInputControlEntity() As InputControl Implements IPrintInputControlManagement_ModelEntryPoints.EntryPoint_GenerateValidInputControlEntity
        Return New InputControl
    End Function

    Public Function EntryPoint_AfterAddNewInputControl(InputControlEntity As InputControl) As ServerResult Implements IPrintInputControlManagement_ModelEntryPoints.EntryPoint_AfterAddNewInputControl

    End Function

    Public Function EntryPoint_AfterDeleteInputControl(InputControlEntity As InputControl) As ServerResult Implements IPrintInputControlManagement_ModelEntryPoints.EntryPoint_AfterDeleteInputControl

    End Function

    Public Function EntryPoint_AfterModifyInputControl(InputControlEntity As InputControl) As ServerResult Implements IPrintInputControlManagement_ModelEntryPoints.EntryPoint_AfterModifyInputControl

    End Function

    Public Function EntryPoint_BeforeModifyInputControl(InputControlEntity As InputControl) As ServerResult Implements IPrintInputControlManagement_ModelEntryPoints.EntryPoint_BeforeModifyInputControl

    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyInputControl(InputControlEntity As InputControl) As InputControl Implements IPrintInputControlManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyInputControl
        Return InputControlEntity
    End Function

    Public Function EntryPoint_CopyInputControl(InputControlEntity As InputControl) As InputControl Implements IPrintInputControlManagement_ModelEntryPoints.EntryPoint_CopyInputControl
        Return InputControlEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of InputControl)) As Object Implements IPrintInputControlManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

End Class
