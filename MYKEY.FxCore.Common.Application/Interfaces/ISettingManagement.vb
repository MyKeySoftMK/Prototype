Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.DataAccess
Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI
Imports System.Security.Principal

Public Interface ISettingManagement

#Region "Functions"

    Function CreateNewSetting(AdditionalTypeEntity As FxNTSettings.Setting) As ServerResult

    Function ModifySetting(AdditionalTypeEntity As FxNTSettings.Setting) As ServerResult

    Function DeleteSetting(AdditionalTypeGUID As String, Optional PermanentlyDelete As Boolean = False) As ServerResult

#End Region

#Region "Queries"

    Function GetSettings(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTSettings.Setting)

    Function GetSetting(SettingGUID As String, Optional QParam As QueryParameters = Nothing) As FxNTSettings.Setting

#End Region

End Interface
