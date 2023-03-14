Imports MYKEY.FxCore.DataAccess.FxNTAutomations
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure
Imports MYKEY.FxCore.Desktop.Domain

Partial Public Class AutomationManagement
    Implements IAutomationManagement_ModelEntryPoints

    Public Function EntryPoint_AfterAddNewAutomation(AutomationEntity As Automation) As ServerResult Implements IAutomationManagement_ModelEntryPoints.EntryPoint_AfterAddNewAutomation

    End Function

    Public Function EntryPoint_AfterDeleteAutomation(AutomationEntity As Automation) As ServerResult Implements IAutomationManagement_ModelEntryPoints.EntryPoint_AfterDeleteAutomation

    End Function

    Public Function EntryPoint_AfterModifyAutomation(AutomationEntity As Automation) As ServerResult Implements IAutomationManagement_ModelEntryPoints.EntryPoint_AfterModifyAutomation

    End Function

    Public Function EntryPoint_BeforeAddNewAutomation(AutomationEntity As Automation) As Object Implements IAutomationManagement_ModelEntryPoints.EntryPoint_BeforeAddNewAutomation
        With AutomationEntity

            NLOGLOGGER.Debug("=> Name: " & .Name)

            ' Zusätzliche Informationen
            NLOGLOGGER.Debug("=> Displayname: " & .DisplayName)

        End With

        Return AutomationEntity
    End Function

    Public Function EntryPoint_BeforeModifyAutomation(AutomationEntity As Automation) As ServerResult Implements IAutomationManagement_ModelEntryPoints.EntryPoint_BeforeModifyAutomation

    End Function

    Public Function EntryPoint_CheckAddNewAutomation(Dbctx As Entities, AutomationEntity As Automation) As Boolean Implements IAutomationManagement_ModelEntryPoints.EntryPoint_CheckAddNewAutomation
        Return True
    End Function

    Public Function EntryPoint_CopyAutomation(AutomationEntityCopy As Automation) As Automation Implements IAutomationManagement_ModelEntryPoints.EntryPoint_CopyAutomation
        Return AutomationEntityCopy
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of Automation)) As Object Implements IAutomationManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

    Public Function EntryPoint_GenerateValidAutomationEntity() As Automation Implements IAutomationManagement_ModelEntryPoints.EntryPoint_GenerateValidAutomationEntity
        Dim newAutomation As New Automation

        With newAutomation
            '
        End With

        Return newAutomation
    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyAutomation(AutomationEntity As Automation) As Automation Implements IAutomationManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyAutomation
        Return AutomationEntity
    End Function
End Class
