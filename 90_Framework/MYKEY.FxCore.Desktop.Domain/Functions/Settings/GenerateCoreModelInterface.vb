' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTSettings
Imports MYKEY.FxCore.Common.Application

Public Interface ISettingManagement

#Region "Functions"

    Function CreateNewSetting() As ServerResult

    Function CreateNewSetting(SettingEntity As Setting) As ServerResult

	Function CopySetting(SettingEntity As Setting) As ServerResult

    Function ModifySetting(SettingEntity As Setting) As ServerResult

    Function DeleteSetting(SettingGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteSettings(Settings As Setting(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetSettings(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Setting)

	Function GetSettings(SettingGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Setting)

    Function GetSetting(SettingGUID As String, Optional QParam As QueryParameters = Nothing) As Setting

#End Region

End Interface
