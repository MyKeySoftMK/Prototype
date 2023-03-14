Imports System.Drawing.Printing
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.IO
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports PdfiumPrinter


Public Class Pdfium

    ' Die Open Source-Bibliothek PDFium ermöglicht es Entwicklern, PDF-Dateien in eigenen Anwendungen zu verwenden 
    ' und dort zu bearbeiten. Wichtige Bestandteile stammen von Foxit, darunter die Rendering Engine. Sie kann für 
    ' eigene Anwendungen genutzt werden. PDFium wurde unter BSD-Lizenz veröffentlicht. Sie gestattet es, den Code 
    ' anzupassen und zu erweitern, sofern die Urheber genannt werden. Der Code selbst kann unter 
    ' code.google.com bezogen werden. Hier gibt es auch die passende Dokumentation, die bei der Verwendung von 
    ' PDFium unterstützt. 

    ' Additionally, you'll have to install ONE of the following NuGet Packages(based on your needs): 
    '   - PdfiumViewer.Native.x86_64.v8-xfa *
    '   - PdfiumViewer.Native.x86.v8-xfa *
    '   - PdfiumViewer.Native.x86.no_v8-no_xfa
    '   - PdfiumViewer.Native.x86_64.no_v8-no_xfa 

#Region "dll Wrappers"

    <DllImport("winspool.drv", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Shared Function GetDefaultPrinter(pszBuffer As StringBuilder, ByRef size As Integer) As Boolean
    End Function

#End Region

    Public Shared Function SendPdfToPrinter(pdfFileName As String, Optional printerName As String = "") As Boolean

        '#Region "Get Connected Printer Name"
        Dim dp As New StringBuilder(256)
        Dim size As Integer = dp.Capacity
        Dim success As Boolean = False

        Dim CurrentPrinter As String = ""
        Dim PdfPrn As PdfPrinter

        Try

            Dim printers As PrinterSettings.StringCollection = PrinterSettings.InstalledPrinters
            If (Not (printers) Is Nothing) Then
                For Each printer As String In printers
                    NLOGLOGGER.Debug("Printers: " & printer)
                Next

                ' Festelegen, welcher Drucker verwendet werden soll
                Try
                    If printerName.Length = 0 Then

                        ' Wenn kein Drucker angegeben wurde, dann den Standdarddrucker auswählen
                        If GetDefaultPrinter(dp, size) Then
                            CurrentPrinter = dp.ToString().Trim()
                        End If
                    Else
                        CurrentPrinter = printerName.Trim()
                    End If
                Catch ex As Exception

                End Try


                PdfPrn = New PdfPrinter(CurrentPrinter)
                NLOGLOGGER.Debug("Printer selected: " & CurrentPrinter)

                With PdfPrn
                    .Print(pdfFileName)
                    NLOGLOGGER.Debug("Successfull printed: " & pdfFileName)
                End With

                success = True
            Else

                NLOGLOGGER.Fatal("No Printer detected")
                Return success
            End If

        Catch ex As Exception

            NLOGLOGGER.Fatal(ex.Message)

        End Try

        Return success

    End Function
End Class
