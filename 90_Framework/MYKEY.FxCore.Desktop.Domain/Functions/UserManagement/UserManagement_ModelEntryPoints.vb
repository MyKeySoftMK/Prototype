Imports MYKEY.FxCore.DataAccess.FxNTUsers
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports System.ComponentModel.Composition
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class UserManagement
    Implements IUserManagement_ModelEntryPoints

    Public Function EntryPoint_BeforeAddNewUser(UserEntity As User) Implements IUserManagement_ModelEntryPoints.EntryPoint_BeforeAddNewUser

        With UserEntity

            NLOGLOGGER.Debug("=> UserName: " & .Name)

            ' Zusätzliche Informationen
            NLOGLOGGER.Debug("=> DisplayName: " & .DisplayName)
            NLOGLOGGER.Debug("=> UserDescription: " & .Description)

            ' Für die Authentifizierung nützliche Informationen
            NLOGLOGGER.Debug("=> IsLocked: " & .IsLocked.ToString)
            NLOGLOGGER.Debug("=> UserIsNotActive: " & .IsNotActive.ToString)

            ' Noch nicht implementierte Informationen
            NLOGLOGGER.Debug("=> ChangePassword: " & .ChangePassword.ToString)
            NLOGLOGGER.Debug("=> DomainUser: " & .DomainUser)

        End With

        Return UserEntity

    End Function

    Public Function EntryPoint_CheckAddNewUser(Dbctx As FxNTUsers.Entities, UserEntity As User) As Boolean Implements IUserManagement_ModelEntryPoints.EntryPoint_CheckAddNewUser

        Return True

    End Function

    Public Function EntryPoint_GenerateValidUserEntity() As User Implements IUserManagement_ModelEntryPoints.EntryPoint_GenerateValidUserEntity

        Dim _User As New User

        Return _User

    End Function

    Public Function EntryPoint_AfterAddNewUser(UserEntity As User) As ServerResult Implements IUserManagement_ModelEntryPoints.EntryPoint_AfterAddNewUser
        Return New ServerResult
    End Function

    Public Function EntryPoint_AfterDeleteUser(UserEntity As User) As ServerResult Implements IUserManagement_ModelEntryPoints.EntryPoint_AfterDeleteUser
        Return New ServerResult
    End Function

    Public Function EntryPoint_AfterModifyUser(UserEntity As User) As ServerResult Implements IUserManagement_ModelEntryPoints.EntryPoint_AfterModifyUser
        Return New ServerResult
    End Function

    Public Function EntryPoint_BeforeModifyUser(UserEntity As User) As ServerResult Implements IUserManagement_ModelEntryPoints.EntryPoint_BeforeModifyUser
        Return New ServerResult
    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifyUser(UserEntity As User) As User Implements IUserManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifyUser
        Return UserEntity
    End Function

    Public Function EntryPoint_CopyUser(UserEntity As User) As User Implements IUserManagement_ModelEntryPoints.EntryPoint_CopyUser
        Return UserEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of User)) As Object Implements IUserManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

End Class
