Imports MYKEY.FxCore.DataAccess.FxNTUsers
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class UserGroupRoleManagement
    Implements IUserGroupRoleManagement_ModelEntryPoints


    Public Function EntryPoint_BeforeAddNewUserGroupRole(UserRoleEntity As UserGroupRole) Implements IUserGroupRoleManagement_ModelEntryPoints.EntryPoint_BeforeAddNewUserGroupRole

        With UserRoleEntity

            NLOGLOGGER.Debug("=> UserRoleName: " & .Name)

            ' Zusätzliche Informationen
            NLOGLOGGER.Debug("=> UserRoleDescription: " & .Description)

        End With

        Return UserRoleEntity

    End Function

    Public Function EntryPoint_CheckAddNewUserGroupRole(Dbctx As FxNTUsers.Entities, UserEntity As UserGroupRole) As Boolean Implements IUserGroupRoleManagement_ModelEntryPoints.EntryPoint_CheckAddNewUserGroupRole

        Return True

    End Function

    Public Function EntryPoint_GenerateValidUserGroupRoleEntity() As UserGroupRole Implements IUserGroupRoleManagement_ModelEntryPoints.EntryPoint_GenerateValidUserGroupRoleEntity

        Dim _UserGroupRole As New UserGroupRole

        Return _UserGroupRole

    End Function

    Public Function EntryPoint_AfterAddNewUserGroupRole(UserGroupRoleEntity As UserGroupRole) As ServerResult Implements IUserGroupRoleManagement_ModelEntryPoints.EntryPoint_AfterAddNewUserGroupRole

    End Function

    Public Function EntryPoint_AfterDeleteUserGroupRole(UserGroupRoleEntity As UserGroupRole) As ServerResult Implements IUserGroupRoleManagement_ModelEntryPoints.EntryPoint_AfterDeleteUserGroupRole

    End Function

    Public Function EntryPoint_AfterModifyUserGroupRole(UserGroupRoleEntity As UserGroupRole) As ServerResult Implements IUserGroupRoleManagement_ModelEntryPoints.EntryPoint_AfterModifyUserGroupRole

    End Function

    Public Function EntryPoint_BeforeModifyUserGroupRole(UserGroupRoleEntity As UserGroupRole) As ServerResult Implements IUserGroupRoleManagement_ModelEntryPoints.EntryPoint_BeforeModifyUserGroupRole

    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyUserGroupRole(UserGroupRoleEntity As UserGroupRole) As UserGroupRole Implements IUserGroupRoleManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyUserGroupRole
        Return UserGroupRoleEntity
    End Function

    Public Function EntryPoint_CopyUserGroupRole(UserGroupRoleEntity As UserGroupRole) As UserGroupRole Implements IUserGroupRoleManagement_ModelEntryPoints.EntryPoint_CopyUserGroupRole
        Return UserGroupRoleEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of UserGroupRole)) As Object Implements IUserGroupRoleManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

End Class
