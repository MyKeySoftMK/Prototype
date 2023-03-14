Imports MYKEY.FxCore.DataAccess.FxNTSettings
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common
Imports System.Data.Entity.Infrastructure

Partial Public Class SettingManagement
    Implements ISettingManagement_ModelEntryPoints

    Public Function EntryPoint_BeforeAddNewSetting(SettingEntity As Setting) Implements ISettingManagement_ModelEntryPoints.EntryPoint_BeforeAddNewSetting

        With SettingEntity

            NLOGLOGGER.Debug("=> Name: " & .Name)

            ' Zusätzliche Informationen
            NLOGLOGGER.Debug("=> Description: " & .Description)
            NLOGLOGGER.Debug("=> Value: " & .Value)
            NLOGLOGGER.Debug("=> Type: " & .SettingType)

        End With

        Return SettingEntity

    End Function

    Public Function EntryPoint_CheckAddNewSetting(Dbctx As FxNTSettings.Entities, AdditionalTypeEntity As Setting) As Boolean Implements ISettingManagement_ModelEntryPoints.EntryPoint_CheckAddNewSetting

        Return True

    End Function

    Public Function EntryPoint_GenerateValidSettingEntity() As Setting Implements ISettingManagement_ModelEntryPoints.EntryPoint_GenerateValidSettingEntity

    End Function

    Public Function EntryPoint_AfterAddNewSetting(SettingEntity As Setting) As ServerResult Implements ISettingManagement_ModelEntryPoints.EntryPoint_AfterAddNewSetting

    End Function

    Public Function EntryPoint_AfterDeleteSetting(SettingEntity As Setting) As ServerResult Implements ISettingManagement_ModelEntryPoints.EntryPoint_AfterDeleteSetting

    End Function

    Public Function EntryPoint_AfterModifySetting(SettingEntity As Setting) As ServerResult Implements ISettingManagement_ModelEntryPoints.EntryPoint_AfterModifySetting

    End Function

    Public Function EntryPoint_BeforeModifySetting(SettingEntity As Setting) As ServerResult Implements ISettingManagement_ModelEntryPoints.EntryPoint_BeforeModifySetting

    End Function

    Public Function EntryPoint_ModifyEntityBeforeModifySetting(SettingEntity As Setting) As Setting Implements ISettingManagement_ModelEntryPoints.EntryPoint_ModifyEntityBeforeModifySetting
        Return SettingEntity
    End Function

    Public Function EntryPoint_CopySetting(SettingEntity As Setting) As Setting Implements ISettingManagement_ModelEntryPoints.EntryPoint_CopySetting
        Return SettingEntity
    End Function

    Public Function EntryPoint_DefineDefaultQuery(currentDefaultQuery As DbQuery(Of Setting)) As Object Implements ISettingManagement_ModelEntryPoints.EntryPoint_DefineDefaultQuery

    End Function

End Class
