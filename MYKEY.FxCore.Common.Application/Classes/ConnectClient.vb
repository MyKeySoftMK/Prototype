Imports MYKEY.FxNT.Common.ApplicationLogging
Imports System.IO

Public Class ConnectClient

    Inherits ObservableObject

    Shared _Connection As Object
   
    Public Shared Property Connection As Object
        Set(value As Object)
            _Connection = value
        End Set
        Get
            Return _Connection
        End Get
    End Property

    Public Shared ReadOnly Property IsConnect As Boolean
        Get
            If _Connection IsNot Nothing Then
                Return _Connection.IsSessionValid
            Else
                Return False
            End If
        End Get
    End Property

    ''' <summary>
    ''' Ob die Anwendungsinstanz instanziert wurde
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared IsApplicationInstanceRunning As Boolean = True


    Public Shared Sub AutoLogin(Optional TransportEncryption As Boolean = True, Optional UserName As String = "Administrator", _
                               Optional Password As String = "geheim", Optional HostInstance As String = "tcpex://localhost:9999/Zyan")

    End Sub

End Class
