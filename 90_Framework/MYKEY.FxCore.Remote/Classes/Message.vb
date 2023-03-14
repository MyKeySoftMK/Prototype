Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading.Tasks

Public Class Message
    Private _tcpClient As TcpClient
    Private _encoder As System.Text.Encoding = Nothing
    Private _writeLineDelimiter As Byte
    Private _autoTrim As Boolean = False

    Friend Sub New(ByVal data As Byte(), ByVal tcpClient As TcpClient, ByVal stringEncoder As System.Text.Encoding, ByVal lineDelimiter As Byte)
        data = data
        _tcpClient = tcpClient
        _encoder = stringEncoder
        _writeLineDelimiter = lineDelimiter
    End Sub

    Friend Sub New(ByVal databytes As Byte(), ByVal tcpClient As TcpClient, ByVal stringEncoder As System.Text.Encoding, ByVal lineDelimiter As Byte, ByVal autoTrim As Boolean)
        Data = databytes
        _tcpClient = tcpClient
        _encoder = stringEncoder
        _writeLineDelimiter = lineDelimiter
        _autoTrim = autoTrim
    End Sub

    Public Property Data As Byte()

    Public ReadOnly Property MessageString As String
        Get

            If _autoTrim Then
                Return _encoder.GetString(Data).Trim()
            End If

            Return _encoder.GetString(Data)
        End Get
    End Property

    Public Sub Reply(ByVal data As Byte())
        _tcpClient.GetStream().Write(data, 0, data.Length)
    End Sub

    Public Sub Reply(ByVal data As String)
        If String.IsNullOrEmpty(data) Then
            Return
        End If

        Reply(_encoder.GetBytes(data))
    End Sub

    Public Sub ReplyLine(ByVal data As String)
        If String.IsNullOrEmpty(data) Then
            Return
        End If

        If Convert.ToByte(data.LastOrDefault()) <> _writeLineDelimiter Then
            Reply(data & _encoder.GetString(New Byte() {_writeLineDelimiter}))
        Else
            Reply(data)
        End If
    End Sub

    Public ReadOnly Property TcpClient As TcpClient
        Get
            Return _tcpClient
        End Get
    End Property
End Class

