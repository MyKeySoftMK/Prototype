Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Microsoft.Win32
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel
Imports System.Windows.Input
Imports System.Reflection

''' <summary>
''' A wrapper around a <see cref="Microsoft.Win32.FileDialog"/> FileDialog class
''' </summary>
''' <remarks></remarks>
<DesignTimeVisible(False)>
Public MustInherit Class DependencyFileDialog

    Inherits Control

    ''' <summary>
    ''' Overridden in derviced classes to provide access to the appropriate FileDialog subclass
    ''' </summary>
    Protected MustOverride ReadOnly Property Dialog() As FileDialog

    ''' <summary>
    ''' Constructor for the DependencyFileDialog. Hooks up the FileOk event to the <seealso cref="FileOkCommand"/> FileOkAction
    ''' </summary>
    Public Sub New()
        Visibility = System.Windows.Visibility.Collapsed
        AddHandler Dialog.FileOk, AddressOf Dialog_FileOk
    End Sub

    ''' <summary>
    ''' Handle the FileDialog's "Save" or "Open" click event by firing the FileOkAction command
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Dialog_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs)
        ExecuteFileOkCommand()
    End Sub

    ''' <summary>
    ''' Executes the FileOkCommand using the current FileName property. Marked as virtual so it can be overridden in
    ''' DependencyOpenFileDialog. Not happy with this situation.
    ''' </summary>
    Protected Overridable Sub ExecuteFileOkCommand()
        If FileOkCommand IsNot Nothing Then
            FileOkCommand.Execute(Dialog.FileName)
        End If
    End Sub

    ''' <summary>
    ''' Gets or sets the filter string that determines what types of files are displayed from either 
    ''' the DependencyOpenFileDialog or DependencySaveFileDialog.
    ''' </summary>
    Public Property Filter() As String
        Get
            Return DirectCast(GetValue(FilterProperty), String)
        End Get
        Set(value As String)
            SetValue(FilterProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the Filter property
    ''' </summary>
    Public Shared ReadOnly FilterProperty As DependencyProperty = DependencyProperty.Register("Filter", GetType(String), GetType(DependencyFileDialog), New UIPropertyMetadata("", AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets a string containing the full path of the file selected in a file dialog.
    ''' </summary>
    Public Property FileName() As String
        Get
            Return DirectCast(GetValue(FileNameProperty), String)
        End Get
        Set(value As String)
            SetValue(FileNameProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the FileName property
    ''' </summary>
    Public Shared ReadOnly FileNameProperty As DependencyProperty = DependencyProperty.Register("FileName", GetType(String), GetType(DependencyFileDialog), New UIPropertyMetadata(String.Empty, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets the text that appears in the title bar of a file dialog.
    ''' </summary>
    Public Property Title() As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the Title property
    ''' </summary>
    Public Shared ReadOnly TitleProperty As DependencyProperty = DependencyProperty.Register("Title", GetType(String), GetType(DependencyFileDialog), New UIPropertyMetadata("Select a file", AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets a value indicating whether a file dialog automatically adds an extension to a file name if the user omits an extension.
    ''' </summary>
    Public Property AddExtension() As Boolean
        Get
            Return CBool(GetValue(AddExtensionProperty))
        End Get
        Set(value As Boolean)
            SetValue(AddExtensionProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the AddExtension property
    ''' </summary>
    Public Shared ReadOnly AddExtensionProperty As DependencyProperty = DependencyProperty.Register("AddExtension", GetType(Boolean), GetType(DependencyFileDialog), New UIPropertyMetadata(True, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets a value indicating whether a file dialog displays a warning if the user specifies a file name that does not exist.
    ''' </summary>
    Public Property CheckFileExists() As Boolean
        Get
            Return CBool(GetValue(CheckFileExistsProperty))
        End Get
        Set(value As Boolean)
            SetValue(CheckFileExistsProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the CheckFileExists property
    ''' </summary>
    Public Shared ReadOnly CheckFileExistsProperty As DependencyProperty = DependencyProperty.Register("CheckFileExists", GetType(Boolean), GetType(DependencyFileDialog), New UIPropertyMetadata(False, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets a value that specifies whether warnings are displayed if the user types invalid paths and file names.
    ''' </summary>
    Public Property CheckPathExists() As Boolean
        Get
            Return CBool(GetValue(CheckPathExistsProperty))
        End Get
        Set(value As Boolean)
            SetValue(CheckPathExistsProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the CheckPathExists property
    ''' </summary>
    Public Shared ReadOnly CheckPathExistsProperty As DependencyProperty = DependencyProperty.Register("CheckPathExists", GetType(Boolean), GetType(DependencyFileDialog), New UIPropertyMetadata(True, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets the list of custom places for file dialog boxes. 
    ''' </summary>
    Public Property CustomPlaces() As IList(Of FileDialogCustomPlace)
        Get
            Return DirectCast(GetValue(CustomPlacesProperty), IList(Of FileDialogCustomPlace))
        End Get
        Set(value As IList(Of FileDialogCustomPlace))
            SetValue(CustomPlacesProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the CustomPlaces property
    ''' </summary>
    Public Shared ReadOnly CustomPlacesProperty As DependencyProperty = DependencyProperty.Register("CustomPlaces", GetType(IList(Of FileDialogCustomPlace)), GetType(DependencyFileDialog), New UIPropertyMetadata(Nothing, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets a value that specifies the default extension string to use to filter the list of files that are displayed.
    ''' </summary>
    Public Property DefaultExt() As String
        Get
            Return DirectCast(GetValue(DefaultExtProperty), String)
        End Get
        Set(value As String)
            SetValue(DefaultExtProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the DefaultExt property
    ''' </summary>
    Public Shared ReadOnly DefaultExtProperty As DependencyProperty = DependencyProperty.Register("DefaultExt", GetType(String), GetType(DependencyFileDialog), New UIPropertyMetadata(String.Empty, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets a value indicating whether a file dialog returns either the location of the file referenced by a shortcut 
    ''' or the location of the shortcut file (.lnk).
    ''' </summary>
    Public Property DereferenceLinks() As Boolean
        Get
            Return CBool(GetValue(DereferenceLinksProperty))
        End Get
        Set(value As Boolean)
            SetValue(DereferenceLinksProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the DereferenceLinks property
    ''' </summary>
    Public Shared ReadOnly DereferenceLinksProperty As DependencyProperty = DependencyProperty.Register("DereferenceLinks", GetType(Boolean), GetType(DependencyFileDialog), New UIPropertyMetadata(False, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets an array that contains one file name for each selected file.
    ''' </summary>
    Public ReadOnly Property FileNames() As String()
        Get
            Return Dialog.FileNames
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the index of the filter currently selected in a file dialog.
    ''' </summary>
    Public Property FilterIndex() As Integer
        Get
            Return CInt(GetValue(FilterIndexProperty))
        End Get
        Set(value As Integer)
            SetValue(FilterIndexProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the FilterIndex property
    ''' </summary>
    Public Shared ReadOnly FilterIndexProperty As DependencyProperty = DependencyProperty.Register("FilterIndex", GetType(Integer), GetType(DependencyFileDialog), New UIPropertyMetadata(1, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets or sets the initial directory that is displayed by a file dialog.
    ''' </summary>
    Public Property InitialDirectory() As String
        Get
            Return DirectCast(GetValue(InitialDirectoryProperty), String)
        End Get
        Set(value As String)
            SetValue(InitialDirectoryProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the InitialDirectory property
    ''' </summary>
    Public Shared ReadOnly InitialDirectoryProperty As DependencyProperty = DependencyProperty.Register("InitialDirectory", GetType(String), GetType(DependencyFileDialog), New UIPropertyMetadata(String.Empty, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Gets a string that only contains the file name for the selected file.
    ''' </summary>
    Public ReadOnly Property SafeFileName() As String
        Get
            Return Dialog.SafeFileName
        End Get
    End Property

    ''' <summary>
    ''' Gets an array that contains one safe file name for each selected file.
    ''' </summary>
    Public ReadOnly Property SafeFileNames() As String()
        Get
            Return Dialog.SafeFileNames
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether the dialog accepts only valid Win32 file names.
    ''' </summary>
    Public Property ValidateNames() As Boolean
        Get
            Return CBool(GetValue(ValidateNamesProperty))
        End Get
        Set(value As Boolean)
            SetValue(ValidateNamesProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the ValidateNames property
    ''' </summary>
    Public Shared ReadOnly ValidateNamesProperty As DependencyProperty = DependencyProperty.Register("ValidateNames", GetType(Boolean), GetType(DependencyFileDialog), New UIPropertyMetadata(False, AddressOf DialogPropertyChangedCallback))

    ''' <summary>
    ''' Uses reflection to set the property in the contained FileDialog
    ''' </summary>
    ''' <param name="obj">The FileDialog whose property has been changed</param>
    ''' <param name="args">Information about the property and values that has been changed.</param>
    Protected Shared Sub DialogPropertyChangedCallback(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim dfd As DependencyFileDialog = TryCast(obj, DependencyFileDialog)
        Dim changedProp As PropertyInfo = dfd.Dialog.[GetType]().GetProperty(args.[Property].Name)
        If changedProp.CanWrite Then
            changedProp.SetValue(dfd.Dialog, args.NewValue, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' This property should be bound to the Command in your ViewModel that is to be performed when the user clicks
    ''' on either "Open" or "Save"
    ''' </summary>
    Public Property FileOkCommand() As ICommand
        Get
            Return DirectCast(GetValue(FileOkCommandProperty), ICommand)
        End Get
        Set(value As ICommand)
            SetValue(FileOkCommandProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' The Dependency Property for the FileOkCommand command
    ''' </summary>
    Public Shared ReadOnly FileOkCommandProperty As DependencyProperty = DependencyProperty.Register("FileOkCommand", GetType(ICommand), GetType(DependencyFileDialog))

    Private _showDialogCommand As ICommand

    ''' <summary>
    ''' When fired, this command causes the Dialog to be displayed
    ''' </summary>
    Public ReadOnly Property ShowDialogCommand() As ICommand
        Get
            If _showDialogCommand Is Nothing Then
                _showDialogCommand = New DelegateCommand(Function(p) Dialog.ShowDialog())
            End If
            Return _showDialogCommand
        End Get
    End Property


End Class
