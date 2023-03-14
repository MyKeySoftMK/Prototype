Imports MYKEY.FxCore.DataAccess.FxNTUsers
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class UserGroupManagement
    Implements IUserGroupManagement_ModelEntryPoints


    Public Function EntryPoint_BeforeAddNewUserGroup(UserGroupEntity As UserGroup) Implements IUserGroupManagement_ModelEntryPoints.EntryPoint_BeforeAddNewUserGroup

        With UserGroupEntity

            NLOGLOGGER.Debug("=> UserName: " & .Name)

            ' Zusätzliche Informationen
            NLOGLOGGER.Debug("=> UserGroupDescription: " & .Description)

        End With

        Return UserGroupEntity

    End Function

    Public Function EntryPoint_CheckAddNewUserGroup(Dbctx As FxNTUsers.Entities, UserEntity As UserGroup) As Boolean Implements IUserGroupManagement_ModelEntryPoints.EntryPoint_CheckAddNewUserGroup

        Return True

    End Function

    Public Function EntryPoint_GenerateValidUserGroupEntity() As UserGroup Implements IUserGroupManagement_ModelEntryPoints.EntryPoint_GenerateValidUserGroupEntity

        Dim _UserGroup As New UserGroup

        Return _UserGroup

    End Function

    Public Function EntryPoint_AfterAddNewUserGroup(UserGroupEntity As UserGroup) As ServerResult Implements IUserGroupManagement_ModelEntryPoints.EntryPoint_AfterAddNewUserGroup

    End Function

    Public Function EntryPoint_AfterDeleteUserGroup(UserGroupEntity As UserGroup) As ServerResult Implements IUserGroupManagement_ModelEntryPoints.EntryPoint_AfterDeleteUserGroup

    End Function

    Public Function EntryPoint_AfterModifyUserGroup(UserGroupEntity As UserGroup) As ServerResult Implements IUserGroupManagement_ModelEntryPoints.EntryPoint_AfterModifyUserGroup

    End Function

    Public Function EntryPoint_BeforeModifyUserGroup(UserGroupEntity As UserGroup) As ServerResult Implements IUserGroupManagement_ModelEntryPoints.EntryPoint_BeforeModifyUserGroup

    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyUserGroup(UserGroupEntity As UserGroup) As UserGroup Implements IUserGroupManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyUserGroup
        Return UserGroupEntity
    End Function

    Public Function EntryPoint_CopyUserGroup(UserGroupEntity As UserGroup) As UserGroup Implements IUserGroupManagement_ModelEntryPoints.EntryPoint_CopyUserGroup
        Return UserGroupEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of UserGroup)) As Object Implements IUserGroupManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

End Class
