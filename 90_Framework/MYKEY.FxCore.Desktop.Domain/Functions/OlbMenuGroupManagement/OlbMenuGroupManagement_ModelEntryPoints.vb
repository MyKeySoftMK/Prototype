Imports MYKEY.FxCore.DataAccess.FxNTOlbMenu
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class OlbMenuGroupManagement
    Implements IOlbMenuGroupManagement_ModelEntryPoints

    Public Function EntryPoint_BeforeAddNewMenuGroup(MenuGroupEntity As MenuGroup) Implements IOlbMenuGroupManagement_ModelEntryPoints.EntryPoint_BeforeAddNewMenuGroup

        With MenuGroupEntity

            NLOGLOGGER.Debug("=> UserName: " & .Name)

            ' Zusätzliche Informationen
            NLOGLOGGER.Debug("=> UserGroupDescription: " & .Description)

        End With

        Return MenuGroupEntity

    End Function

    Public Function EntryPoint_CheckAddNewMenuGroup(Dbctx As FxNTOlbMenu.Entities, UserEntity As MenuGroup) As Boolean Implements IOlbMenuGroupManagement_ModelEntryPoints.EntryPoint_CheckAddNewMenuGroup

        Return True

    End Function

    Public Function EntryPoint_GenerateValidMenuGroupEntity() As MenuGroup Implements IOlbMenuGroupManagement_ModelEntryPoints.EntryPoint_GenerateValidMenuGroupEntity

    End Function

    Public Function EntryPoint_AfterAddNewMenuGroup(MenuGroupEntity As MenuGroup) As ServerResult Implements IOlbMenuGroupManagement_ModelEntryPoints.EntryPoint_AfterAddNewMenuGroup

    End Function

    Public Function EntryPoint_AfterDeleteMenuGroup(MenuGroupEntity As MenuGroup) As ServerResult Implements IOlbMenuGroupManagement_ModelEntryPoints.EntryPoint_AfterDeleteMenuGroup

    End Function

    Public Function EntryPoint_AfterModifyMenuGroup(MenuGroupEntity As MenuGroup) As ServerResult Implements IOlbMenuGroupManagement_ModelEntryPoints.EntryPoint_AfterModifyMenuGroup

    End Function

    Public Function EntryPoint_BeforeModifyMenuGroup(MenuGroupEntity As MenuGroup) As ServerResult Implements IOlbMenuGroupManagement_ModelEntryPoints.EntryPoint_BeforeModifyMenuGroup

    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyMenuGroup(MenuGroupEntity As MenuGroup) As MenuGroup Implements IOlbMenuGroupManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyMenuGroup
        Return MenuGroupEntity
    End Function

    Public Function EntryPoint_CopyMenuGroup(MenuGroupEntity As MenuGroup) As MenuGroup Implements IOlbMenuGroupManagement_ModelEntryPoints.EntryPoint_CopyMenuGroup
        Return MenuGroupEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of MenuGroup)) As Object Implements IOlbMenuGroupManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

        ' Es wird nach der MenuGroupSortId sortiert um die Anzeige der Menüs zu steuern
        Return currentDefaultQuery.OrderBy(Function(e) e.SortID)

    End Function

End Class
