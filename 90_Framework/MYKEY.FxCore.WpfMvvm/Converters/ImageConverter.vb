Imports System.Windows.Data
Imports System.Drawing
Imports System.IO
Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.Common.Application
Imports MYKEY.FxCore.Common.Application.ApplicationSettings

''' <summary>
''' One-way converter from String to System.Windows.Media.ImageSource
''' </summary>
<ValueConversion(GetType(String), GetType(System.Windows.Media.ImageSource))> _
Public Class ImageConverter

    Implements IValueConverter

    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        ' empty images are empty...
        If value Is Nothing Then
            Return Nothing
        End If

        Dim _image As New ImagesResources("MYKEY.FxCore.ImagesResources", EXE_FOLDER_NAME)
        Dim bitmap = New System.Windows.Media.Imaging.BitmapImage()

        Try

            bitmap.BeginInit()
            bitmap.StreamSource = _image.GetImageFromPath(value)
            bitmap.EndInit()

        Catch ex As Exception
            bitmap = Nothing
        End Try

        Return bitmap
    End Function

    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Return Nothing
    End Function

End Class
