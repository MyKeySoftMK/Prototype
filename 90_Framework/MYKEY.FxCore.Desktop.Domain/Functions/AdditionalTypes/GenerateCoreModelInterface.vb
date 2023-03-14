' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTAdditionalTypes
Imports MYKEY.FxCore.Common.Application

Public Interface IAdditionalTypeManagement

#Region "Functions"

    Function CreateNewAdditionalType() As ServerResult

    Function CreateNewAdditionalType(AdditionalTypeEntity As AdditionalType) As ServerResult

	Function CopyAdditionalType(AdditionalTypeEntity As AdditionalType) As ServerResult

    Function ModifyAdditionalType(AdditionalTypeEntity As AdditionalType) As ServerResult

    Function DeleteAdditionalType(AdditionalTypeGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteAdditionalTypes(AdditionalTypes As AdditionalType(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetAdditionalTypes(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of AdditionalType)

	Function GetAdditionalTypes(AdditionalTypeGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of AdditionalType)

    Function GetAdditionalType(AdditionalTypeGUID As String, Optional QParam As QueryParameters = Nothing) As AdditionalType

#End Region

End Interface
