Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.ComponentModel.Composition
Imports System.Data.Objects

Imports MYKEY.FxCore
Imports MYKEY.FxCore.Common.Application
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTOlbMenu
Imports MYKEY.FxCore.DataAccess.FxNTPrints

Imports MYKEY.FxCore.Common
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess.FxNTUsers
Imports System.Data.Entity.Infrastructure
Imports System.Data.Common
Imports System.IO
Imports System.Data

Partial Class PrintManagement

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sqlCommand"></param>
    ''' <returns></returns>
    ''' <remarks>Weil das EF6 immer mit typisierten Objekten arbeitet, kann man in einem solchen Fall kein Mapping machen, weil
    ''' nicht im Vorraus bekannt ist, welche Spaltennamen verwendet werden. Ein Zugriff erfolgt aus diesem Grund dann direkt
    ''' per DbDataReader und dem Aufbereiten der Daten 
    ''' (Siehe: http://stackoverflow.com/questions/27017811/how-to-select-varying-amount-of-columns-with-executestorequery)</remarks>
    Public Function ReturnDataQueryStatementResult(sqlCommand As String) As List(Of KeyValuePair(Of String, String))

        Dim _result As New List(Of KeyValuePair(Of String, String))

        Using md = New FxNTPrints.Entities(EntityConnectionData.EntityConnectionString)
            Dim conn = md.Database.Connection

            conn.Open()
            Using cmd As IDbCommand = conn.CreateCommand()
                cmd.CommandText = sqlCommand

                Using reader = DirectCast(cmd.ExecuteReader(), DbDataReader)
                    While reader.Read()

                        ' Wenn nur eine Spalte zurückgegeben wird, dann ist ID und Anzeige identisch
                        If reader.FieldCount = 1 Then
                            _result.Add(New KeyValuePair(Of String, String)(reader(0).ToString, reader(0).ToString))
                        Else
                            _result.Add(New KeyValuePair(Of String, String)(reader(0).ToString, reader(1).ToString))
                        End If

                    End While
                End Using
            End Using
        End Using

        Return _result

    End Function

    ''' <summary>
    ''' Gibt eine nach dem Pfadnamen sortierte Liste der benutzerdefinierten Ausdrucke aus, wie sie in der Datenbank hinterlegt wurden
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPrintsUserDef(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Print)
        Dim serverPrints As DbQuery(Of Print)

        NLOGLOGGER.Info("Get List of UserPrints ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverPrints = DefaultQueryPrints.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverPrints = DefaultQueryPrints.Where(Function(Print) Print.IsSystemReport = False).OrderBy(Function(Print) Print.JSPrintPath)
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverPrints.Count & " UserPrint/UserPrints in database")

        Return New ObservableCollection(Of Print)(serverPrints)

    End Function



End Class
