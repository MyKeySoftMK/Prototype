Imports System.Windows
Imports System.Windows.Controls
Imports System.Globalization

#Region "Documentation Tags"
''' <summary>
'''     WPF Maskable TextBox class. Just specify the TextBoxMaskBehavior.Mask attached property to a TextBox. 
'''     It protect your TextBox from unwanted non numeric symbols and make it easy to modify your numbers.
''' </summary>
''' <remarks>
''' <para>
'''     Class Information:
'''	    <list type="bullet">
'''         <item name="authors">Authors: Ruben Hakopian</item>
'''         <item name="date">February 2009</item>
'''         <item name="originalURL">http://www.rubenhak.com/?p=8</item>
'''     </list>
''' </para>
''' </remarks>
#End Region
Public Class TextBoxMaskBehavior

    ' Quelle: http://www.codeproject.com/Articles/34228/WPF-Maskable-TextBox-for-Numeric-Values

    ' Benötigt wird diese Klasse, damit Textboxen, die an Dezimalwerte gebunden wurden, in der Lage sind,
    ' ein entsprechendes Dezimaltrennzeichen aufzunehmen und in korrekte Werte zu schreiben
    ' http://stackoverflow.com/questions/16914224/wpf-textbox-to-enter-decimal-values



#Region "MinimumValue Property"

    Public Shared Function GetMinimumValue(obj As DependencyObject) As Double
        Return CDbl(obj.GetValue(MinimumValueProperty))
    End Function

    Public Shared Sub SetMinimumValue(obj As DependencyObject, value As Double)
        obj.SetValue(MinimumValueProperty, value)
    End Sub

    Public Shared ReadOnly MinimumValueProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("MinimumValue",
                                            GetType(Double),
                                            GetType(TextBoxMaskBehavior),
                                            New FrameworkPropertyMetadata(Double.NaN, AddressOf MinimumValueChangedCallback))

    Private Shared Sub MinimumValueChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim _this As TextBox = TryCast(d, TextBox)
        ValidateTextBox(_this)
    End Sub
#End Region

#Region "MaximumValue Property"

    Public Shared Function GetMaximumValue(obj As DependencyObject) As Double
        Return CDbl(obj.GetValue(MaximumValueProperty))
    End Function

    Public Shared Sub SetMaximumValue(obj As DependencyObject, value As Double)
        obj.SetValue(MaximumValueProperty, value)
    End Sub

    Public Shared ReadOnly MaximumValueProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("MaximumValue",
                                            GetType(Double),
                                            GetType(TextBoxMaskBehavior),
                                            New FrameworkPropertyMetadata(Double.NaN, AddressOf MaximumValueChangedCallback))

    Private Shared Sub MaximumValueChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim _this As TextBox = TryCast(d, TextBox)
        ValidateTextBox(_this)
    End Sub
#End Region

#Region "Mask Property"

    Public Shared Function GetMask(obj As DependencyObject) As MaskType
        Return DirectCast(obj.GetValue(MaskProperty), MaskType)
    End Function

    Public Shared Sub SetMask(obj As DependencyObject, value As MaskType)
        obj.SetValue(MaskProperty, value)
    End Sub

    Public Shared ReadOnly MaskProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("Mask",
                                            GetType(MaskType),
                                            GetType(TextBoxMaskBehavior),
                                            New FrameworkPropertyMetadata(AddressOf MaskChangedCallback))

    Private Shared Sub MaskChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If TypeOf e.OldValue Is TextBox Then
            RemoveHandler TryCast(e.OldValue, TextBox).PreviewTextInput, AddressOf TextBox_PreviewTextInput
            DataObject.RemovePastingHandler(TryCast(e.OldValue, TextBox), DirectCast(AddressOf TextBoxPastingEventHandler, DataObjectPastingEventHandler))
        End If

        Dim _this As TextBox = TryCast(d, TextBox)
        If _this Is Nothing Then
            Return
        End If

        If DirectCast(e.NewValue, MaskType) <> MaskType.Any Then
            AddHandler _this.PreviewTextInput, AddressOf TextBox_PreviewTextInput
            DataObject.AddPastingHandler(_this, DirectCast(AddressOf TextBoxPastingEventHandler, DataObjectPastingEventHandler))
        End If

        ValidateTextBox(_this)
    End Sub

#End Region

#Region "Private Static Methods"

    Private Shared Sub ValidateTextBox(_this As TextBox)
        If GetMask(_this) <> MaskType.Any Then
            _this.Text = ValidateValue(GetMask(_this), _this.Text, GetMinimumValue(_this), GetMaximumValue(_this))
        End If
    End Sub

    Private Shared Sub TextBoxPastingEventHandler(sender As Object, e As DataObjectPastingEventArgs)
        Dim _this As TextBox = TryCast(sender, TextBox)
        Dim clipboard As String = TryCast(e.DataObject.GetData(GetType(String)), String)
        clipboard = ValidateValue(GetMask(_this), clipboard, GetMinimumValue(_this), GetMaximumValue(_this))
        If Not String.IsNullOrEmpty(clipboard) Then
            _this.Text = clipboard
        End If
        e.CancelCommand()
        e.Handled = True
    End Sub

    Private Shared Sub TextBox_PreviewTextInput(sender As Object, e As System.Windows.Input.TextCompositionEventArgs)
        Dim _this As TextBox = TryCast(sender, TextBox)
        Dim isValid As Boolean = IsSymbolValid(GetMask(_this), e.Text)
        e.Handled = Not isValid
        If isValid Then
            Dim caret As Integer = _this.CaretIndex
            Dim text As String = _this.Text
            Dim textInserted As Boolean = False
            Dim selectionLength As Integer = 0

            If _this.SelectionLength > 0 Then
                text = text.Substring(0, _this.SelectionStart) + text.Substring(_this.SelectionStart + _this.SelectionLength)
                caret = _this.SelectionStart
            End If

            If e.Text = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator Then
                While True
                    Dim ind As Integer = text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    If ind = -1 Then
                        Exit While
                    End If

                    text = text.Substring(0, ind) + text.Substring(ind + 1)
                    If caret > ind Then
                        caret -= 1
                    End If
                End While

                If caret = 0 Then
                    text = Convert.ToString("0") & text
                    caret += 1
                Else
                    If caret = 1 AndAlso String.Empty + text(0) = NumberFormatInfo.CurrentInfo.NegativeSign Then
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + "0" + text.Substring(1)
                        caret += 1
                    End If
                End If

                If caret = text.Length Then
                    selectionLength = 1
                    textInserted = True
                    text = text + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator & Convert.ToString("0")
                    caret += 1
                End If
            ElseIf e.Text = NumberFormatInfo.CurrentInfo.NegativeSign Then
                textInserted = True
                If _this.Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign) Then
                    text = text.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, String.Empty)
                    If caret <> 0 Then
                        caret -= 1
                    End If
                Else
                    text = NumberFormatInfo.CurrentInfo.NegativeSign + _this.Text
                    caret += 1
                End If
            End If

            If Not textInserted Then
                text = text.Substring(0, caret) + e.Text + (If((caret < _this.Text.Length), text.Substring(caret), String.Empty))

                caret += 1
            End If

            Try
                Dim val As Double = Convert.ToDouble(text)
                Dim newVal As Double = ValidateLimits(GetMinimumValue(_this), GetMaximumValue(_this), val)
                If val <> newVal Then
                    text = newVal.ToString()
                ElseIf val = 0 Then
                    If Not text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) Then
                        text = "0"
                    End If
                End If
            Catch
                text = "0"
            End Try

            While text.Length > 1 AndAlso text(0) = "0"c AndAlso String.Empty + text(1) <> NumberFormatInfo.CurrentInfo.NumberDecimalSeparator
                text = text.Substring(1)
                If caret > 0 Then
                    caret -= 1
                End If
            End While

            While text.Length > 2 AndAlso String.Empty + text(0) = NumberFormatInfo.CurrentInfo.NegativeSign _
                AndAlso text(1) = "0"c AndAlso String.Empty + text(2) <> NumberFormatInfo.CurrentInfo.NumberDecimalSeparator
                text = NumberFormatInfo.CurrentInfo.NegativeSign + text.Substring(2)
                If caret > 1 Then
                    caret -= 1
                End If
            End While

            If caret > text.Length Then
                caret = text.Length
            End If

            _this.Text = text
            _this.CaretIndex = caret
            _this.SelectionStart = caret
            _this.SelectionLength = selectionLength
            e.Handled = True
        End If
    End Sub

    Private Shared Function ValidateValue(mask As MaskType, value As String, min As Double, max As Double) As String
        If String.IsNullOrEmpty(value) Then
            Return String.Empty
        End If

        value = value.Trim()
        Select Case mask
            Case MaskType.[Integer]
                Try
                    Convert.ToInt64(value)
                    Return value
                Catch
                End Try
                Return String.Empty

            Case MaskType.[Decimal]
                Try
                    Convert.ToDouble(value)

                    Return value
                Catch
                End Try
                Return String.Empty
        End Select

        Return value
    End Function

    Private Shared Function ValidateLimits(min As Double, max As Double, value As Double) As Double
        If Not min.Equals(Double.NaN) Then
            If value < min Then
                Return min
            End If
        End If

        If Not max.Equals(Double.NaN) Then
            If value > max Then
                Return max
            End If
        End If

        Return value
    End Function

    Private Shared Function IsSymbolValid(mask As MaskType, str As String) As Boolean
        Select Case mask
            Case MaskType.Any
                Return True

            Case MaskType.[Integer]
                If str = NumberFormatInfo.CurrentInfo.NegativeSign Then
                    Return True
                End If
                Exit Select

            Case MaskType.[Decimal]
                If str = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator OrElse str = NumberFormatInfo.CurrentInfo.NegativeSign Then
                    Return True
                End If
                Exit Select
        End Select

        If mask.Equals(MaskType.[Integer]) OrElse mask.Equals(MaskType.[Decimal]) Then
            For Each ch As Char In str
                If Not [Char].IsDigit(ch) Then
                    Return False
                End If
            Next

            Return True
        End If

        Return False
    End Function

#End Region


End Class
