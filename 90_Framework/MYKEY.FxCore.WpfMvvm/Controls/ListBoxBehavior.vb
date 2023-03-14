Imports System.Windows
Imports System.Windows.Controls
Imports System.Collections.Specialized

Public Class ListBoxBehavior

    Shared Associations As New Dictionary(Of ListBox, Capture)()

    Public Shared Function GetScrollOnNewItem(obj As DependencyObject) As Boolean
        Return CBool(obj.GetValue(ScrollOnNewItemProperty))
    End Function

    Public Shared Sub SetScrollOnNewItem(obj As DependencyObject, value As Boolean)
        obj.SetValue(ScrollOnNewItemProperty, value)
    End Sub

    Public Shared ReadOnly ScrollOnNewItemProperty As DependencyProperty = _
        DependencyProperty.RegisterAttached("ScrollOnNewItem", GetType(Boolean), GetType(ListBoxBehavior), _
                                            New UIPropertyMetadata(False, AddressOf OnScrollOnNewItemChanged))

    Public Shared Sub OnScrollOnNewItemChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim listBox = TryCast(d, ListBox)
        If listBox Is Nothing Then
            Return
        End If
        Dim oldValue As Boolean = CBool(e.OldValue), newValue As Boolean = CBool(e.NewValue)
        If newValue = oldValue Then
            Return
        End If
        If newValue Then

            AddHandler listBox.Loaded, AddressOf ListBox_Loaded
            AddHandler listBox.Unloaded, AddressOf ListBox_Unloaded

        Else
            RemoveHandler listBox.Loaded, AddressOf ListBox_Loaded
            RemoveHandler listBox.Unloaded, AddressOf ListBox_Unloaded

            If Associations.ContainsKey(listBox) Then
                Associations(listBox).Dispose()
            End If
        End If
    End Sub

    Private Shared Sub ListBox_Unloaded(sender As Object, e As RoutedEventArgs)
        Dim listBox = DirectCast(sender, ListBox)
        If Associations.ContainsKey(listBox) Then
            Associations(listBox).Dispose()
        End If
        RemoveHandler listBox.Unloaded, AddressOf ListBox_Unloaded
    End Sub

    Private Shared Sub ListBox_Loaded(sender As Object, e As RoutedEventArgs)
        Dim listBox = DirectCast(sender, ListBox)
        Dim incc = TryCast(listBox.Items, INotifyCollectionChanged)
        If incc Is Nothing Then
            Return
        End If
        RemoveHandler listBox.Unloaded, AddressOf ListBox_Unloaded
        Associations(listBox) = New Capture(listBox)
    End Sub

    Class Capture
        Implements IDisposable

        Public Property listBox() As ListBox
            Get
                Return m_listBox
            End Get
            Set(value As ListBox)
                m_listBox = value
            End Set
        End Property
        Private m_listBox As ListBox

        Public Property incc() As INotifyCollectionChanged
            Get
                Return m_incc
            End Get
            Set(value As INotifyCollectionChanged)
                m_incc = value
            End Set
        End Property
        Private m_incc As INotifyCollectionChanged

        Public Sub New(listBox As ListBox)
            Me.listBox = listBox
            incc = TryCast(listBox.ItemsSource, INotifyCollectionChanged)
            If incc IsNot Nothing Then
                AddHandler incc.CollectionChanged, AddressOf incc_CollectionChanged
            End If
        End Sub

        Private Sub incc_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs)
            If e.Action = NotifyCollectionChangedAction.Add Then
                listBox.ScrollIntoView(e.NewItems(0))
                listBox.SelectedItem = e.NewItems(0)
            End If
        End Sub

        Public Sub Dispose() Implements System.IDisposable.Dispose
            If incc IsNot Nothing Then
                RemoveHandler incc.CollectionChanged, AddressOf incc_CollectionChanged
            End If
        End Sub

    End Class

End Class
