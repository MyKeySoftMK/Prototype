Imports Xceed.Wpf.Toolkit.PropertyGrid
Imports Xceed.Wpf.Toolkit.PropertyGrid.Editors
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data

Public Class FileChooserEditor

    Implements ITypeEditor

    Private dpContainer As New DockPanel()
    Private txtFilePath As New TextBox
    Private btnSelectFile As New Button()


    Public Function ResolveEditor(propertyItem As PropertyItem) As System.Windows.FrameworkElement Implements ITypeEditor.ResolveEditor
        dpContainer.LastChildFill = True

        btnSelectFile.Content = "..."
        AddHandler btnSelectFile.Click, AddressOf btnSelectFile_Click

        DockPanel.SetDock(btnSelectFile, Dock.Right)
        dpContainer.Children.Add(btnSelectFile)
        dpContainer.Children.Add(txtFilePath)

        'create the binding from the bound property item to the editor
        Dim _binding = New Binding("Value")
        'bind to the Value property of the PropertyItem

        _binding.Source = propertyItem
        _binding.ValidatesOnExceptions = True
        _binding.ValidatesOnDataErrors = True

        _binding.Mode = If(propertyItem.IsReadOnly, BindingMode.OneWay, BindingMode.TwoWay)

        BindingOperations.SetBinding(txtFilePath, TextBox.TextProperty, _binding)

        Return dpContainer

    End Function

    Private Sub btnSelectFile_Click(sender As [Object], e As RoutedEventArgs)

        Dim openFile As New Microsoft.Win32.OpenFileDialog()
        openFile.Filter = "Alle Dateien (*.*)|*.*"
        If openFile.ShowDialog() = True Then
            txtFilePath.Text = openFile.FileName

            Dim be As BindingExpression = txtFilePath.GetBindingExpression(TextBox.TextProperty)
            be.UpdateSource()

        End If

    End Sub


End Class

