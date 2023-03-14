
Imports iText.Forms
Imports iText.Forms.Fields
Imports iText.Kernel.Pdf
Imports iText.Kernel.Pdf.Canvas.Parser
Imports iText.Kernel.Pdf.Canvas.Parser.Listener
Imports System.Collections
Imports System.IO

''' <summary>
''' Methoden zum Ausfüllen von PDF-Formularen
''' </summary>
''' <remarks>Quelle: DotNetPro 01/2016 S. 126</remarks>
Public Class PdfForms

    ''' <summary>
    ''' Gibt eine Collection der Feldnamen zurück, die in dem PDF-Dokument enthalten sind und die beschrieben
    ''' werden können
    ''' </summary>
    ''' <param name="FileNamePdf"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFieldNames(ByVal FileNamePdf As String) As List(Of String)

        Dim _result As New List(Of String)
        Dim pdfReader As PdfReader
        Dim pdfDoc As PdfDocument
        Dim pdfAcroFrm As PdfAcroForm

        If File.Exists(FileNamePdf) = True Then

            pdfReader = New PdfReader(FileNamePdf)
            pdfDoc = New PdfDocument(pdfReader)
            pdfAcroFrm = PdfAcroForm.GetAcroForm(pdfDoc, True)
            For Each field As Object In pdfAcroFrm.GetFormFields
                _result.Add(field.Key.ToString())
            Next

        End If

        Return _result

    End Function

    ''' <summary>
    ''' Schreibt die übergebenen Werte in das Dokument und erzeugt ein neues PDF mit dem Inhalt
    ''' </summary>
    ''' <param name="SourcePdfFileName"></param>
    ''' <param name="DestinationPdfFileName"></param>
    ''' <param name="Values"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetFields(SourcePdfFileName As String, DestinationPdfFileName As String, Values As List(Of KeyValuePair(Of String, String))) As ServerResult
        Dim _result As New ServerResult
        Dim pdfReader As PdfReader
        Dim pdfDoc As PdfDocument
        Dim pdfAcroFrm As PdfAcroForm
        Dim pdfFields As IDictionary(Of String, PdfFormField)
        Dim pdfNewValue As PdfFormField

        Try

            If File.Exists(SourcePdfFileName) = True Then
                pdfReader = New PdfReader(SourcePdfFileName)
                pdfDoc = New PdfDocument(pdfReader)
                pdfAcroFrm = PdfAcroForm.GetAcroForm(pdfDoc, True)
                pdfFields = pdfAcroFrm.GetFormFields


                For Each value As KeyValuePair(Of String, String) In Values
                    pdfFields.TryGetValue(value.Key, pdfNewValue)
                    If pdfNewValue IsNot Nothing Then
                        pdfNewValue.SetValue(value.Value)
                    End If
                Next

                With pdfAcroFrm
                    .FlattenFields()
                End With

                With pdfDoc
                    .Close()
                End With


            End If

        Catch ex As Exception
            _result.ErrorMessages.Add(ex.InnerException.ToString)
        End Try

        Return _result

    End Function

    Public Function GetText(ByVal FileNamePdf As String) As String

        Dim _result As String = ""
        Dim pdfReader As PdfReader
        Dim pdfDoc As PdfDocument
        Dim strategy As ITextExtractionStrategy

        If File.Exists(FileNamePdf) = True Then

            pdfReader = New PdfReader(FileNamePdf)
            pdfDoc = New PdfDocument(pdfReader)


            For page As Integer = 1 To pdfDoc.GetNumberOfPages
                strategy = New SimpleTextExtractionStrategy()

                _result = _result + PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy)
            Next

            pdfDoc.Close()
            pdfReader.Close()

        End If

        Return _result

    End Function
End Class
