Imports MYKEY.FxCore.DataAccess.FxNTGroupLists
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class GroupListManagement

    Implements IGroupListManagement_ModelEntryPoints

    Public Function EntryPoint_BeforeAddNewGroupList(GroupListEntity As GroupList) Implements IGroupListManagement_ModelEntryPoints.EntryPoint_BeforeAddNewGroupList

        With GroupListEntity

            NLOGLOGGER.Debug("=> Value: " & .Value)

            ' Zusätzliche Informationen
            NLOGLOGGER.Debug("=> ListName: " & .Name)
            NLOGLOGGER.Debug("=> ListName-Description: " & .Description)
            NLOGLOGGER.Debug("=> Value-Description: " & .ValueDescription)

        End With

        Return GroupListEntity

    End Function

    Public Function EntryPoint_CheckAddNewGroupList(Dbctx As FxNTGroupLists.Entities, GroupListEntity As GroupList) As Boolean Implements IGroupListManagement_ModelEntryPoints.EntryPoint_CheckAddNewGroupList

        Return True

    End Function

    Public Function EntryPoint_GenerateValidGroupListEntity() As GroupList Implements IGroupListManagement_ModelEntryPoints.EntryPoint_GenerateValidGroupListEntity

    End Function

    Public Function EntryPoint_AfterAddNewGroupList(GroupListEntity As GroupList) As ServerResult Implements IGroupListManagement_ModelEntryPoints.EntryPoint_AfterAddNewGroupList

    End Function

    Public Function EntryPoint_AfterDeleteGroupList(GroupListEntity As GroupList) As ServerResult Implements IGroupListManagement_ModelEntryPoints.EntryPoint_AfterDeleteGroupList

    End Function

    Public Function EntryPoint_AfterModifyGroupList(GroupListEntity As GroupList) As ServerResult Implements IGroupListManagement_ModelEntryPoints.EntryPoint_AfterModifyGroupList

    End Function

    Public Function EntryPoint_BeforeModifyGroupList(GroupListEntity As GroupList) As ServerResult Implements IGroupListManagement_ModelEntryPoints.EntryPoint_BeforeModifyGroupList

    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyGroupList(GroupListEntity As GroupList) As GroupList Implements IGroupListManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyGroupList
        Return GroupListEntity
    End Function

    Public Function EntryPoint_CopyGroupList(GroupListEntity As GroupList) As GroupList Implements IGroupListManagement_ModelEntryPoints.EntryPoint_CopyGroupList
        Return GroupListEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of GroupList)) As Object Implements IGroupListManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

End Class
