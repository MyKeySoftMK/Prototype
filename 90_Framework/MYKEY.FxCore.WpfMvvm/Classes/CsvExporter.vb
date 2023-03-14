Imports System
Imports System.IO
Imports System.Text

''' <summary>
''' 
''' </summary>
''' <remarks>http://blog.magnusmontin.net/2013/09/29/export-data-from-a-datagrid/</remarks>
Public Class CsvExporter
    Implements IExporter

    Private ReadOnly sb As New StringBuilder()
    Private ReadOnly _delimiter As String

    Public Sub New(delimiter As Char)
        _delimiter = delimiter.ToString()
    End Sub

    Public ReadOnly Property Delimiter() As Char
        Get
            Return _delimiter(0)
        End Get
    End Property

    Public Sub AddColumn(value As String) Implements IExporter.AddColumn
        sb.Append(value.Replace(_delimiter, String.Format("""{0}""", _delimiter)))
        sb.Append(_delimiter)
    End Sub

    Public Sub AddLineBreak() Implements IExporter.AddLineBreak
        sb.Remove(sb.Length - 1, 1)
        'remove trailing delimiter
        sb.AppendLine()
    End Sub

    Public Function Export(exportPath As String) As String Implements IExporter.Export
        Dim randomNumber As New Random

        exportPath = exportPath & "\\" & String.Format("{0}.csv", randomNumber.Next())

        If String.IsNullOrEmpty(exportPath) Then
            Dim rnd As New Random()
            exportPath = String.Format("{0}.csv", rnd.[Next]())
        ElseIf Not Path.GetExtension(exportPath).ToLower().Equals(".csv") Then
            Throw New ArgumentException("Invalid file extension.", "exportPath")
        End If

        File.WriteAllText(exportPath, sb.ToString().Trim())
        sb.Clear()

        Return exportPath

    End Function

End Class
