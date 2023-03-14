Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.DataAccess
Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI
Imports System.Security.Principal

Public Interface IAdditionalTypeManagement

#Region "Functions"

    Function CreateNewAdditionalType(AdditionalTypeEntity As FxNTAdditionalTypes.AdditionalType) As ServerResult

    Function ModifyAdditionalType(AdditionalTypeEntity As FxNTAdditionalTypes.AdditionalType) As ServerResult

    Function DeleteAdditionalType(AdditionalTypeGUID As String, Optional PermanentlyDelete As Boolean = False) As ServerResult

#End Region

#Region "Queries"

    Function GetAdditionalTypes(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTAdditionalTypes.AdditionalType)

    Function GetAdditionalType(AdditionalTypeGUID As String, Optional QParam As QueryParameters = Nothing) As FxNTAdditionalTypes.AdditionalType

#End Region

End Interface
