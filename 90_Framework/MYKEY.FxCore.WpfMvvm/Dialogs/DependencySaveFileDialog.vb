Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Microsoft.Win32
Imports System.Windows

Public Class DependencySaveFileDialog
    Inherits DependencyFileDialog
    Private m_dialog As SaveFileDialog

    ''' <summary>
    ''' Overridden from DependencyFileDialog. Provides access to an instance of the Microsoft.Win32.SaveFileDialog class.
    ''' </summary>
    Protected Overrides ReadOnly Property Dialog() As FileDialog
        Get
            If m_dialog Is Nothing Then

                m_dialog = New SaveFileDialog()
            End If
            Return m_dialog
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether DependencySaveFileDialog prompts the user for permission 
    ''' to create a file if the user specifies a file that does not exist. 
    ''' </summary>
    Public Property CreatePrompt() As Boolean
        Get
            Return CBool(GetValue(CreatePromptProperty))
        End Get
        Set(value As Boolean)
            SetValue(CreatePromptProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the CreatePrompt property
    ''' </summary>
    Public Shared ReadOnly CreatePromptProperty As DependencyProperty = DependencyProperty.Register("CreatePrompt", GetType(Boolean), GetType(DependencySaveFileDialog), New UIPropertyMetadata(False, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets a value indicating whether SaveFileDialog displays a warning if the user specifies the name of a file that already exists. 
    ''' </summary>
    Public Property OverwritePrompt() As Boolean
        Get
            Return CBool(GetValue(OverwritePromptProperty))
        End Get
        Set(value As Boolean)
            SetValue(OverwritePromptProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the OverwritePrompt property
    ''' </summary>
    Public Shared ReadOnly OverwritePromptProperty As DependencyProperty = DependencyProperty.Register("OverwritePrompt", GetType(Boolean), GetType(DependencySaveFileDialog), New UIPropertyMetadata(True, AddressOf DialogPropertyChangedCallback))
End Class



