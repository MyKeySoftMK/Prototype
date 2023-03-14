''' <summary>
''' 
''' </summary>
''' <remarks>http://blog.magnusmontin.net/2013/09/29/export-data-from-a-datagrid/</remarks>
Public Interface IExporter

    Sub AddColumn(value As String)
    Sub AddLineBreak()
    Function Export(exportPath As String) As String

End Interface
