Imports Xceed.Wpf.Toolkit.PropertyGrid.Editors
Imports Xceed.Wpf.Toolkit.PropertyGrid
Imports System.Windows.Controls
Imports System.Windows

''' <summary>
''' Diese PasswordEditor-Klasse wird für das maskierte Anzeigen von Passwort-Feldern im Xceed-PropertyGrid verwendet
''' </summary>
''' <remarks></remarks>
Public Class PasswordEditor
    Implements ITypeEditor

    Private m_PropertyItem As PropertyItem
    Private m_PasswordBox As PasswordBox

    Public Function ResolveEditor(propertyItem As Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem) As Windows.FrameworkElement Implements ITypeEditor.ResolveEditor

        m_PropertyItem = propertyItem
        m_PasswordBox = New PasswordBox()
        m_PasswordBox.Password = DirectCast(propertyItem.Value, [String])
        AddHandler m_PasswordBox.LostFocus, AddressOf OnLostFocus
        Return m_PasswordBox

    End Function

    Private Sub OnLostFocus(sender As Object, e As RoutedEventArgs)
        If 0 <> m_PasswordBox.Password.CompareTo(DirectCast(m_PropertyItem.Value, String)) Then
            m_PropertyItem.Value = m_PasswordBox.Password
        End If
    End Sub

End Class
