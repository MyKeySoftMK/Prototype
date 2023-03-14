Imports System.Windows.Data
Imports System.Drawing
Imports System.IO
Imports System.ComponentModel

<ValueConversion(GetType(System.Guid), GetType(String))> _
Public Class GuidStringConverter

    Implements IValueConverter

    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert

        If value Is Nothing Then
            Return Nothing
        End If

        Return value.ToString()

    End Function

    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack

        If value Is Nothing Then
            Return Nothing
        End If

        Dim guidConvert As New GuidConverter

        Return guidConvert.ConvertFromString(value)

    End Function

End Class
