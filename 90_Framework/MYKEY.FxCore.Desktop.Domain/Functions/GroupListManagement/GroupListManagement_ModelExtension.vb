Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.ComponentModel.Composition
Imports System.Data.Objects

Imports MYKEY.FxCore
Imports MYKEY.FxCore.Common.Application
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTGroupLists

Imports MYKEY.FxCore.Common
Imports System.Security.Principal
Imports System.Data.Entity.Infrastructure


Partial Class GroupListManagement

#Region "Functions"


#End Region

#Region "Queries"



#End Region

#Region "Proxy-Functions"

    Public Function GetGroupListsByGroupListName(GroupListName As String, Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTGroupLists.GroupList)
        Dim serverGroupLists As DbQuery(Of GroupList)

        NLOGLOGGER.Info("Get List of GroupLists ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverGroupLists = DefaultQueryGroupLists.Where(Function(e) e.Name = GroupListName).OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverGroupLists = DefaultQueryGroupLists.Where(Function(e) e.Name = GroupListName)
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverGroupLists.Count & " GroupList/GroupLists in Group '" & GroupListName & "' in database")

        Return New ObservableCollection(Of GroupList)(serverGroupLists)

    End Function

#End Region

End Class
