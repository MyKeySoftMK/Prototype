Imports System.Windows.Controls
Imports MYKEY.FxCore.Common
Imports System.Windows.Input
Imports System.Windows
Imports System.IO
Imports System.Text
Imports System.ComponentModel
Imports System.Windows.Data
Imports System.Reflection

Public Class DataGridExportToCsv

    ''' <summary>
    ''' Erstellt eine CSV Datei aus den angezeigten Informationen in einem WPF-DataGrid
    ''' </summary>
    ''' <param name="grid"></param>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    ''' <remarks>http://www.i-programmer.info/projects/38-windows/1564-wpf-data-grid-to-csv-and-html-the-easy-way.html?start=1
    ''' Es werden die darunterliegenden Werte exportiert und nicht die angezeigten. Außerdem ist keine Beeinflussung des CSV möglich,
    ''' wie z.B. welcher Delimiter verwendet werden soll</remarks>
    <Obsolete>
    Public Function ExportToCsv(grid As DataGrid, Path As String) As ServerResult

        Dim _result As New ServerResult
        Dim gridValues As String
        Dim csvFile As StreamWriter
        Dim randomNumber As New Random

        ' copy all of the data displayed in the grid to the Clipboard
        grid.SelectAllCells()

        ' set the copy mode to determine if we need the headings:
        grid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader

        ' copy the data to the clipboard
        ApplicationCommands.Copy.Execute(Nothing, grid)

        ' unselect the cells to restore the grid to its original state
        grid.UnselectAllCells()

        ' retrieve the data on the Clipboard into an Object and cast it to a string
        ' if you want an HTML file change the DataFormates-Value.
        gridValues = DirectCast(Clipboard.GetData(DataFormats.CommaSeparatedValue), String)

        ' clear the Clipboard
        Clipboard.Clear()

        csvFile = New System.IO.StreamWriter(Path & "\\" & String.Format("{0}.csv", randomNumber.Next()))
        csvFile.WriteLine(gridValues)
        csvFile.Close()

        Return _result

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="grid"></param>
    ''' <param name="exporter"></param>
    ''' <param name="exportPath"></param>
    ''' <remarks>http://blog.magnusmontin.net/2013/09/29/export-data-from-a-datagrid/</remarks>
    Public Sub ExportToCsv(grid As DataGrid, exporter As IExporter, exportPath As String, Optional OpenFile As Boolean = False)
        ' Execute the private DoExportUsingRefection method on a background thread by starting a new task 

        Task.Factory.StartNew(Sub() DoExportToCsv(grid, exporter, exportPath, OpenFile))
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="grid"></param>
    ''' <param name="exporter"></param>
    ''' <param name="exportPath"></param>
    ''' <remarks>http://blog.magnusmontin.net/2013/09/29/export-data-from-a-datagrid/</remarks>
    Private Sub DoExportToCsv(grid As DataGrid, exporter As IExporter, exportPath As String, OpenFile As Boolean)
        If grid.ItemsSource Is Nothing OrElse grid.Items.Count.Equals(0) Then
            Throw New InvalidOperationException("You cannot export any data from an empty DataGrid.")
        End If

        Dim checkAccess As Boolean = grid.Dispatcher.CheckAccess()
        Dim collectionView__1 As ICollectionView = Nothing
        Dim columns As IList(Of DataGridColumn) = Nothing
        Dim itemsSource As IEnumerable
        Dim cmbValuePath As String
        Dim headerText As String

        ' Zwischenspeichern der Spalten des übergebenen DataGrid
        If checkAccess Then
            columns = grid.Columns.OrderBy(Function(c) c.DisplayIndex).ToList()
            collectionView__1 = CollectionViewSource.GetDefaultView(grid.ItemsSource)
        Else
            grid.Dispatcher.Invoke(Sub() columns = grid.Columns.OrderBy(Function(c) c.DisplayIndex).ToList())
            grid.Dispatcher.Invoke(Sub() collectionView__1 = CollectionViewSource.GetDefaultView(grid.ItemsSource))
        End If

        ' Ausgeben der Headerzeile
        For Each column As DataGridColumn In columns
            If checkAccess Then
                headerText = column.Header
            Else
                grid.Dispatcher.Invoke(Sub() headerText = column.Header)
            End If
            exporter.AddColumn(headerText)
        Next
        exporter.AddLineBreak()

        ' Ausgeben der Datenzeilen
        For Each o As Object In collectionView__1
            If o.Equals(CollectionView.NewItemPlaceholder) Then
                Continue For
            End If

            For Each column As DataGridColumn In columns
                Dim exportString As String = String.Empty

                ' Wird benötigt, wenn man Ersatzwerte definieren will, die angezeigt werden sollen
                ' z.B.: Rotes Feld wird angezeigt = "Fehler" soll exportiert werden
                If checkAccess Then
                    exportString = ExportBehaviour.GetExportString(column)
                Else
                    grid.Dispatcher.Invoke(Sub() exportString = ExportBehaviour.GetExportString(column))
                End If

                If Not String.IsNullOrEmpty(exportString) Then
                    exporter.AddColumn(exportString)
                ElseIf TypeOf column Is DataGridBoundColumn Then
                    Dim propertyValue As String = String.Empty

                    ' Get the property name from the column's binding 

                    Dim bb As BindingBase = TryCast(column, DataGridBoundColumn).Binding
                    If bb IsNot Nothing Then
                        Dim binding As Binding = TryCast(bb, Binding)
                        If binding IsNot Nothing Then
                            Dim boundProperty As String = binding.Path.Path

                            ' Get the property value using reflection 

                            Dim pi As PropertyInfo = o.[GetType]().GetProperty(boundProperty)
                            If pi IsNot Nothing Then
                                Dim value As Object = pi.GetValue(o)
                                If value IsNot Nothing Then
                                    propertyValue = value.ToString()
                                ElseIf TypeOf column Is DataGridCheckBoxColumn Then
                                    propertyValue = "-"
                                End If
                            End If
                        End If
                    End If

                    exporter.AddColumn(propertyValue)
                ElseIf TypeOf column Is DataGridComboBoxColumn Then
                    Dim cmbColumn As DataGridComboBoxColumn = TryCast(column, DataGridComboBoxColumn)
                    Dim propertyValue As String = String.Empty
                    Dim displayMemberPath As String = String.Empty
                    If checkAccess Then
                        displayMemberPath = cmbColumn.DisplayMemberPath
                    Else
                        grid.Dispatcher.Invoke(Sub() displayMemberPath = cmbColumn.DisplayMemberPath)
                    End If

                    ' Get the property name from the column's binding 

                    Dim bb As BindingBase = cmbColumn.SelectedValueBinding
                    If bb IsNot Nothing Then
                        Dim binding As Binding = TryCast(bb, Binding)
                        If binding IsNot Nothing Then
                            Dim boundProperty As String = binding.Path.Path
                            'returns "Category" (or CategoryId)
                            ' Get the selected property 

                            Dim pi As PropertyInfo = o.[GetType]().GetProperty(boundProperty)
                            If pi IsNot Nothing Then
                                Dim boundProperyValue As Object = pi.GetValue(o)
                                'returns the selected Category object or CategoryId
                                If boundProperyValue IsNot Nothing Then
                                    Dim propertyType As Type = boundProperyValue.[GetType]()
                                    If propertyType.IsPrimitive OrElse propertyType.Equals(GetType(Guid)) OrElse propertyType.Equals(GetType(String)) Then

                                        If checkAccess Then
                                            itemsSource = cmbColumn.ItemsSource
                                            cmbValuePath = cmbColumn.SelectedValuePath
                                        Else
                                            grid.Dispatcher.Invoke(Sub() itemsSource = cmbColumn.ItemsSource)
                                            grid.Dispatcher.Invoke(Sub() cmbValuePath = cmbColumn.SelectedValuePath)
                                        End If

                                        If itemsSource IsNot Nothing Then
                                            ' Find the Category object in the ItemsSource of the ComboBox with
                                            '                                                 * an Id (SelectedValuePath) equal to the selected CategoryId 

                                            Dim comboBoxSource As IEnumerable(Of Object) = itemsSource.Cast(Of Object)()
                                            Dim obj As Object = (From oo In comboBoxSource
                                                                 Let prop = oo.[GetType]().GetProperty(cmbValuePath)
                                                                 Where prop IsNot Nothing AndAlso prop.GetValue(oo).Equals(boundProperyValue)
                                                                 Select oo).FirstOrDefault()
                                            If obj IsNot Nothing Then
                                                ' Get the Name (DisplayMemberPath) of the Category object 

                                                If String.IsNullOrEmpty(displayMemberPath) Then
                                                    propertyValue = obj.[GetType]().ToString()
                                                Else
                                                    Dim displayNameProperty As PropertyInfo = obj.[GetType]().GetProperty(displayMemberPath)
                                                    If displayNameProperty IsNot Nothing Then
                                                        Dim displayName As Object = displayNameProperty.GetValue(obj)
                                                        If displayName IsNot Nothing Then
                                                            propertyValue = displayName.ToString()
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        Else
                                            ' Export the scalar property value of the selected object
                                            '                                                 * specified by the SelectedValuePath property of the DataGridComboBoxColumn 

                                            propertyValue = boundProperyValue.ToString()
                                        End If
                                    ElseIf Not String.IsNullOrEmpty(displayMemberPath) Then
                                        ' Get the Name (DisplayMemberPath) property of the selected Category object 

                                        Dim pi2 As PropertyInfo = boundProperyValue.[GetType]().GetProperty(displayMemberPath)

                                        If pi2 IsNot Nothing Then
                                            Dim displayName As Object = pi2.GetValue(boundProperyValue)
                                            If displayName IsNot Nothing Then
                                                propertyValue = displayName.ToString()
                                            End If
                                        End If
                                    Else
                                        propertyValue = o.[GetType]().ToString()
                                    End If
                                End If
                            End If
                        End If
                    End If

                    exporter.AddColumn(propertyValue)
                End If
            Next
            exporter.AddLineBreak()
        Next

        If OpenFile = True Then
            ' Create and open export file 
            Process.Start(exporter.Export(exportPath))
        Else
            ' Only create export file
            exporter.Export(exportPath)
        End If


    End Sub


End Class
