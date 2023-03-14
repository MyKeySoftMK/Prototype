Imports MYKEY.FxCore.DataAccess.FxNTOlbMenu
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class OlbMenuEntryManagement

    Implements IOlbMenuEntryManagement_ModelEntryPoints


    Public Function EntryPoint_BeforeAddNewMenuEntry(MenuEntryEntity As MenuEntry) Implements IOlbMenuEntryManagement_ModelEntryPoints.EntryPoint_BeforeAddNewMenuEntry

        With MenuEntryEntity

            NLOGLOGGER.Debug("=> Text: " & .Text)

            ' Zusätzliche Informationen
            NLOGLOGGER.Debug("=> Description: " & .Description)
            NLOGLOGGER.Debug("=> Systementry: " & .SystemEntry)

        End With

        Return MenuEntryEntity

    End Function

    Public Function EntryPoint_CheckAddNewMenuEntry(Dbctx As FxNTOlbMenu.Entities, UserEntity As MenuEntry) As Boolean Implements IOlbMenuEntryManagement_ModelEntryPoints.EntryPoint_CheckAddNewMenuEntry

        Return True

    End Function

    Public Function EntryPoint_GenerateValidMenuEntryEntity() As MenuEntry Implements IOlbMenuEntryManagement_ModelEntryPoints.EntryPoint_GenerateValidMenuEntryEntity

    End Function

    Public Function EntryPoint_AfterAddNewMenuEntry(MenuEntryEntity As MenuEntry) As ServerResult Implements IOlbMenuEntryManagement_ModelEntryPoints.EntryPoint_AfterAddNewMenuEntry

    End Function

    Public Function EntryPoint_AfterDeleteMenuEntry(MenuEntryEntity As MenuEntry) As ServerResult Implements IOlbMenuEntryManagement_ModelEntryPoints.EntryPoint_AfterDeleteMenuEntry

    End Function

    Public Function EntryPoint_AfterModifyMenuEntry(MenuEntryEntity As MenuEntry) As ServerResult Implements IOlbMenuEntryManagement_ModelEntryPoints.EntryPoint_AfterModifyMenuEntry

    End Function

    Public Function EntryPoint_BeforeModifyMenuEntry(MenuEntryEntity As MenuEntry) As ServerResult Implements IOlbMenuEntryManagement_ModelEntryPoints.EntryPoint_BeforeModifyMenuEntry

    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyMenuEntry(MenuEntryEntity As MenuEntry) As MenuEntry Implements IOlbMenuEntryManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyMenuEntry
        Return MenuEntryEntity
    End Function

    Public Function EntryPoint_CopyMenuEntry(MenuEntryEntity As MenuEntry) As MenuEntry Implements IOlbMenuEntryManagement_ModelEntryPoints.EntryPoint_CopyMenuEntry
        Return MenuEntryEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of MenuEntry)) As Object Implements IOlbMenuEntryManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

End Class
