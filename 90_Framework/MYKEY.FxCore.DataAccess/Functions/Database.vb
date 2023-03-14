Imports System.Data.Entity

Public Class Database

    ' ''' <summary>
    ' ''' Löscht eine Tabelle in der Datenbank
    ' ''' </summary>
    ' ''' <param name="TableName"></param>
    ' ''' <remarks></remarks>
    Public Shared Sub DeleteTabelData(DbCtxt As DbContext, tables As List(Of String))
        Dim Command As String = ""

        If tables.Count > 0 Then
            For Each TableItem As String In tables
                Command = "DELETE [" + TableItem + "] WHERE CanNotDelete='False'"
                Console.WriteLine(Command)
                DbCtxt.Database.SqlQuery(Of Object)(Command).FirstOrDefault()
            Next
        End If


    End Sub

    
End Class
