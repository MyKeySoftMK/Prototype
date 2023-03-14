Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.ComponentModel.Composition
Imports System.Data.Objects

Imports MYKEY.FxCore
Imports MYKEY.FxCore.Common.Application
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTAdditionalTypes

Imports MYKEY.FxCore.Common
Imports System.Security.Principal
Imports System.Data.Entity.Infrastructure


Partial Class AdditionalTypeManagement

#Region "Functions"


#End Region

#Region "Queries"

    Public Function GetAdditionalTypesByTypeCategory(TypeCategoryGuid As String, Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTAdditionalTypes.AdditionalType)
        Dim serverAdditionalTypes As DbQuery(Of AdditionalType)
        Dim _TypeCategoryGuid As Guid

        NLOGLOGGER.Info("Get List of AdditionalTypes ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            _TypeCategoryGuid = Guid.Parse(TypeCategoryGuid)

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverAdditionalTypes = DefaultQueryAdditionalTypes.Where(Function(e) e.CategoryGUID = _TypeCategoryGuid).OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverAdditionalTypes = DefaultQueryAdditionalTypes.Where(Function(e) e.CategoryGUID = _TypeCategoryGuid)
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverAdditionalTypes.Count & " GroupList/GroupLists in Group '" & TypeCategoryGuid & "' in database")

        Return New ObservableCollection(Of AdditionalType)(serverAdditionalTypes)

    End Function

#End Region

#Region "Proxy-Functions"



#End Region

End Class
