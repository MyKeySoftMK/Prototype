Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading.Tasks


Public Class TcpListenerEx
        Inherits TcpListener

        Public Sub New(ByVal localEP As IPEndPoint)
            MyBase.New(localEP)
        End Sub

        Public Sub New(ByVal localaddr As IPAddress, ByVal port As Integer)
            MyBase.New(localaddr, port)
        End Sub

        Public Overloads ReadOnly Property Active As Boolean
            Get
                Return MyBase.Active
            End Get
        End Property
    End Class
