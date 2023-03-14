
<Serializable()>
Public Class QueryParameters

    ''' <summary>
    ''' Ab welchem Datensatz die Informationen gelesen werden sollen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Wird für das dynamische Nachladen von Datensätzen beim Anzeigen in einem DataGrid benötigt</remarks>
    Public Property StartIndex As Integer = 0

    ''' <summary>
    ''' Anzahl der zu lesenden Datensätze
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Wird für das dynamische Nachladen von Datensätzen beim Anzeigen in einem DataGrid benötigt</remarks>
    Public Property ItemsCount As Integer = 0

    ''' <summary>
    ''' Die Auflistung all der relevanten Navigation-Properties, die vollständig geladen werden müssen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Die Synxtax der Zeichenketten ist "Users" oder bei untergeordneten Eigenschaften "UsersToUserGroups.Users".</remarks>
    Public Property Includes As New List(Of String)

    ''' <summary>
    ''' Erzwingt das Neuladen von der Datenquelle
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ForceReload As Boolean = False


End Class
