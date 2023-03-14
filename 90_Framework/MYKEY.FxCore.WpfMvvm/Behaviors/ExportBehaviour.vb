Imports System.Windows
Imports System.Windows.Controls

''' <summary>
''' Wird benötigt, wenn man Ersatzwerte definieren will, die angezeigt werden sollen
''' </summary>
''' <remarks>http://blog.magnusmontin.net/2013/09/29/export-data-from-a-datagrid/</remarks>
Public Class ExportBehaviour

    'name of attached property
    'type of attached property
    'type of this owner class
    Public Shared ReadOnly ExportStringProperty As DependencyProperty = DependencyProperty.RegisterAttached("ExportString", GetType(String), GetType(ExportBehaviour), New PropertyMetadata(String.Empty))
    'the default value of the attached property

    Public Shared Function GetExportString(column As DataGridColumn) As String
        Return DirectCast(column.GetValue(ExportStringProperty), String)
    End Function

    Public Shared Sub SetExportString(column As DataGridColumn, value As String)
        column.SetValue(ExportStringProperty, value)
    End Sub



End Class
