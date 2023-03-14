Imports System.Windows.Data
Imports System.Drawing
Imports System.IO
Imports System.Windows

<ValueConversion(GetType(Boolean), GetType(Visibility))> _
Public Class BooleanVisibilityConverter

    Implements IValueConverter

    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If TypeOf value Is Boolean AndAlso targetType = GetType(Visibility) Then
            Dim val As Boolean = CBool(value)
            If val Then
                Return Visibility.Visible
            ElseIf parameter IsNot Nothing AndAlso TypeOf parameter Is Visibility Then
                Return parameter
            Else
                Return Visibility.Collapsed
            End If
        End If
        Throw New ArgumentException("Invalid argument/return type. Expected argument: bool and return type: Visibility")

    End Function

    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        If TypeOf value Is Visibility AndAlso targetType = GetType(Boolean) Then
            Dim val As Visibility = DirectCast(value, Visibility)
            If val = Visibility.Visible Then
                Return True
            Else
                Return False
            End If
        End If
        Throw New ArgumentException("Invalid argument/return type. Expected argument: Visibility and return type: bool")

    End Function

End Class
