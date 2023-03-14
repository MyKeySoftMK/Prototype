Imports System.IO
Imports System.Collections.ObjectModel

''' <summary>
''' http://stackoverflow.com/questions/6415037/populate-treeview-from-list-of-file-paths-in-wpf
''' </summary>
''' <remarks></remarks>
Public Class TreeViewHelper

    Public Function GetItems(path As String) As ObservableCollection(Of TreeViewItem)
        Dim items = New ObservableCollection(Of TreeViewItem)()

        Dim dirInfo = New DirectoryInfo(path)

        For Each directory As DirectoryInfo In dirInfo.GetDirectories()
            Dim item = New TreeViewDirectoryItem() With { _
                 .Name = directory.Name, _
                 .Path = directory.FullName, _
                 .Items = GetItems(directory.FullName)}

            items.Add(item)
        Next

        For Each file As FileInfo In dirInfo.GetFiles()

            ' Nur Dateien mit der Endunng JRXML werden berücksichtigt
            If file.Extension.ToUpper = ".JRXML" Then

                ' Wenn an der Benamung erkannt wird, dass es sich um einen Subreport handelt, wird dieser
                ' nicht in der Liste angezeigt
                If file.Name.Contains("_sub") = False Then

                    Dim item = New TreeViewFileItem() With { _
                         .Name = file.Name, _
                         .Path = file.FullName _
                    }

                    items.Add(item)
                End If

            End If


        Next

        Return items
    End Function


End Class
