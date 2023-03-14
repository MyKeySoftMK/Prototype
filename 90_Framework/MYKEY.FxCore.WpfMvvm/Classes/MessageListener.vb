Imports System.Diagnostics
Imports System
Imports System.Collections.Generic
Imports System.Windows

Public Class MessageListener

    Inherits DependencyObject
    ''' <summary>
    ''' 
    ''' </summary>
    Private Shared mInstance As MessageListener

    ''' <summary>
    ''' 
    ''' </summary>

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Get MessageListener instance
    ''' </summary>
    Public Shared ReadOnly Property Instance() As MessageListener
        Get
            If mInstance Is Nothing Then
                mInstance = New MessageListener()
            End If
            Return mInstance
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ReceiveMessage"></param>
    Public Sub ReceiveMessage(ReceiveMessage As String)
        Message = ReceiveMessage
        Debug.WriteLine(Message)
        DispatcherHelper.DoEvents()
    End Sub

    ''' <summary>
    ''' Get or set received message
    ''' </summary>
    Public Property Message() As String
        Get
            Return DirectCast(GetValue(MessageProperty), String)
        End Get
        Set(value As String)
            SetValue(MessageProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    Public Shared ReadOnly MessageProperty As DependencyProperty = DependencyProperty.Register("Message", GetType(String), GetType(MessageListener), New UIPropertyMetadata(Nothing))


End Class
