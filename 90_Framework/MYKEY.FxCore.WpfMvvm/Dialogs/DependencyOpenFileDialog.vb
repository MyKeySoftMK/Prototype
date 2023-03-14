Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Microsoft.Win32
Imports System.Windows

Public Class DependencyOpenFileDialog
    Inherits DependencyFileDialog
    Private m_dialog As OpenFileDialog

    ''' <summary>
    ''' Overridden from DependencyFileDialog. Provides access to an instance of the Microsoft.Win32.OpenFileDialog class.
    ''' </summary>
    Protected Overrides ReadOnly Property Dialog() As FileDialog
        Get
            If m_dialog Is Nothing Then
                m_dialog = New OpenFileDialog()
            End If
            Return m_dialog
        End Get
    End Property

    ''' <summary>
    ''' Overridden from DependencyFileDialog. Provides support for the MultiSelect property. Smells fragile.
    ''' </summary>
    Protected Overrides Sub ExecuteFileOkCommand()
        If MultiSelect Then
            FileOkCommand.Execute(Dialog.FileNames)
        Else
            MyBase.ExecuteFileOkCommand()
        End If
    End Sub

    ''' <summary>
    ''' Gets or sets an option indicating whether OpenFileDialog allows users to select multiple files.
    ''' </summary>
    Public Property MultiSelect() As Boolean
        Get
            Return CBool(GetValue(MultiSelectProperty))
        End Get
        Set(value As Boolean)
            SetValue(MultiSelectProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the MultiSelect property
    ''' </summary>
    Public Shared ReadOnly MultiSelectProperty As DependencyProperty = DependencyProperty.Register("MultiSelect", GetType(Boolean), GetType(DependencyOpenFileDialog), New UIPropertyMetadata(False, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets a value indicating whether the read-only check box displayed by DependencyOpenFileDialog is selected. 
    ''' </summary>
    Public Property ReadOnlyChecked() As Boolean
        Get
            Return CBool(GetValue(ReadOnlyCheckedProperty))
        End Get
        Set(value As Boolean)
            SetValue(ReadOnlyCheckedProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the ReadOnlyChecked property
    ''' </summary>
    Public Shared ReadOnly ReadOnlyCheckedProperty As DependencyProperty = DependencyProperty.Register("ReadOnlyChecked", GetType(Boolean), GetType(DependencyOpenFileDialog), New UIPropertyMetadata(False, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets a value indicating whether DependencyOpenFileDialog contains a read-only check box.
    ''' </summary>
    Public Property ShowReadOnly() As Boolean
        Get
            Return CBool(GetValue(ShowReadOnlyProperty))
        End Get
        Set(value As Boolean)
            SetValue(ShowReadOnlyProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the ShowReadOnly property
    ''' </summary>
    Public Shared ReadOnly ShowReadOnlyProperty As DependencyProperty = DependencyProperty.Register("ShowReadOnly", GetType(Boolean), GetType(DependencyOpenFileDialog), New UIPropertyMetadata(False, AddressOf DialogPropertyChangedCallback))
End Class


