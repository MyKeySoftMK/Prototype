Imports Xceed.Wpf.Toolkit.PropertyGrid.Attributes
Imports System.Collections.ObjectModel
Imports System.Drawing.Printing

Public Class PrinterItemsSource
    Implements IItemsSource

    ''' <summary>
    ''' Gibt alle lokal installierten Drucker zurück
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetValues() As ItemCollection Implements IItemsSource.GetValues

        Dim _PrinterItems As New ItemCollection
        Dim printers As PrinterSettings.StringCollection = PrinterSettings.InstalledPrinters

        If (Not (printers) Is Nothing) Then
            For Each printer As String In printers
                _PrinterItems.Add(printer, printer)
            Next
        End If

        Return _PrinterItems

    End Function
End Class
