Imports System.Collections.ObjectModel

''' <summary>
''' http://stackoverflow.com/questions/6415037/populate-treeview-from-list-of-file-paths-in-wpf
''' </summary>
''' <remarks></remarks>
Public Class TreeViewDirectoryItem

    Inherits TreeViewItem

    Public Property Items As ObservableCollection(Of TreeViewItem)

    Sub New()
        Items = New ObservableCollection(Of TreeViewItem)
    End Sub

End Class
