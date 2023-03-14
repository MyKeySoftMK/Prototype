Imports System.Windows
Imports System.Windows.Controls

Public Class PasswordBoxBinding

    Inherits DependencyObject

    Public Shared ReadOnly PasswordProperty As DependencyProperty = _
        DependencyProperty.RegisterAttached("Password", GetType(String), GetType(PasswordBoxBinding), _
                                            New FrameworkPropertyMetadata(String.Empty, New PropertyChangedCallback(AddressOf OnPasswordPropertyChanged)))

    Public Shared ReadOnly AttachProperty As DependencyProperty = _
        DependencyProperty.RegisterAttached("Attach", GetType(Boolean), GetType(PasswordBoxBinding), _
                                            New PropertyMetadata(False, New PropertyChangedCallback(AddressOf Attach)))

    Private Shared ReadOnly IsUpdatingProperty As DependencyProperty = _
        DependencyProperty.RegisterAttached("IsUpdating", GetType(Boolean), GetType(PasswordBoxBinding))

    Public Shared Sub SetAttach(dp As DependencyObject, value As Boolean)
        dp.SetValue(AttachProperty, value)
    End Sub

    Public Shared Function GetAttach(dp As DependencyObject) As Boolean
        Return CBool(dp.GetValue(AttachProperty))
    End Function

    Public Shared Function GetPassword(dp As DependencyObject) As String
        Return DirectCast(dp.GetValue(PasswordProperty), String)
    End Function

    Public Shared Sub SetPassword(dp As DependencyObject, value As String)
        dp.SetValue(PasswordProperty, value)
    End Sub

    Private Shared Function GetIsUpdating(dp As DependencyObject) As Boolean
        Return CBool(dp.GetValue(IsUpdatingProperty))
    End Function

    Private Shared Sub SetIsUpdating(dp As DependencyObject, value As Boolean)
        dp.SetValue(IsUpdatingProperty, value)
    End Sub

    Private Shared Sub OnPasswordPropertyChanged(sender As System.Windows.DependencyObject, e As System.Windows.DependencyPropertyChangedEventArgs)
        Dim passwordBox As PasswordBox = TryCast(sender, PasswordBox)
        RemoveHandler passwordBox.PasswordChanged, AddressOf PasswordChanged
        If Not CBool(GetIsUpdating(passwordBox)) Then
            passwordBox.Password = DirectCast(e.NewValue, String)
        End If
        AddHandler passwordBox.PasswordChanged, AddressOf PasswordChanged
    End Sub

    Private Shared Sub Attach(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim passwordBox As PasswordBox = TryCast(sender, PasswordBox)
        If passwordBox Is Nothing Then
            Return
        End If
        If CBool(e.OldValue) Then
            RemoveHandler passwordBox.PasswordChanged, AddressOf PasswordChanged
        End If
        If CBool(e.NewValue) Then
            AddHandler passwordBox.PasswordChanged, AddressOf PasswordChanged
        End If
    End Sub

    Private Shared Sub PasswordChanged(sender As Object, e As RoutedEventArgs)
        Dim passwordBox As PasswordBox = TryCast(sender, PasswordBox)
        SetIsUpdating(passwordBox, True)
        SetPassword(passwordBox, passwordBox.Password)
        SetIsUpdating(passwordBox, False)
    End Sub

End Class
