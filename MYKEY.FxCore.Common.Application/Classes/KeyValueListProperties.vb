Imports System.Collections.ObjectModel
Imports System.Collections.Generic


Public Class KeyValueListProperties

    Public Property Header As String = ""

    Public Property KeyValuePairs As ObservableCollection(Of KeyValuePair(Of String, String)) = New ObservableCollection(Of KeyValuePair(Of String, String))

    Public Property CurrentKeyValuePair As KeyValuePair(Of String, String) = New KeyValuePair(Of String, String)()

    ''' <summary>
    ''' Mit der Auswertung der OptionText-Eigenschaft, kann man bestimmte Auswertungen vornehmen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OptionText As String = ""

    ''' <summary>
    ''' Darin werden die Berichtsparamter bei einem automatisieren Ausdruck zwischen gespeichert
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReportParameters As IEnumerable(Of Object)

End Class
