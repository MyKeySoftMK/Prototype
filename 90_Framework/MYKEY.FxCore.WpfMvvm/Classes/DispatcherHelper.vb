Imports System.Windows.Threading
Imports System.Security.Permissions

Public Class DispatcherHelper

    ''' <summary>
    ''' Simulate Application.DoEvents function of <see cref=" System.Windows.Forms.Application"/> class.
    ''' </summary>
    <SecurityPermissionAttribute(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
    Public Shared Sub DoEvents()
        Dim frame As New DispatcherFrame()
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, New DispatcherOperationCallback(AddressOf ExitFrames), frame)

        Try
            Dispatcher.PushFrame(frame)
        Catch generatedExceptionName As InvalidOperationException
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="frame"></param>
    ''' <returns></returns>
    Private Shared Function ExitFrames(frame As Object) As Object
        DirectCast(frame, DispatcherFrame).[Continue] = False

        Return Nothing
    End Function

End Class
