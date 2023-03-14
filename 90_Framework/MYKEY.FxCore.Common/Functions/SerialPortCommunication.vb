Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO.Ports
Imports System.ComponentModel
Imports System.Threading

Public Class SerialPortCommunication

#Region "Properties"

    ''' <summary>
    ''' Der instanzierte Serial-Port
    ''' </summary>
    ''' <remarks></remarks>
    Dim _ConnectedPort As System.IO.Ports.SerialPort

    Public Property ReceivedData As String = ""

    ''' <summary>
    ''' Der verbundene Serial-Port
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ConnectedPort As System.IO.Ports.SerialPort
        Get
            If _ConnectedPort.IsOpen = True Then
                Return _ConnectedPort
            Else
                Return Nothing
            End If
        End Get
    End Property

    ''' <summary>
    ''' Name des zu verwendenden Ports
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PortName As String

    ''' <summary>
    ''' Übertragungsrate
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Baud As Integer

    ''' <summary>
    ''' Paritäts Einstellung
    ''' </summary>
    ''' <remarks></remarks>
    Public ParityMode As System.IO.Ports.Parity

    ''' <summary>
    ''' Bit-Einstellung
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BitLength As Integer

    ''' <summary>
    ''' StopBit-Einstellung
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StopBitMode As System.IO.Ports.StopBits

    ''' <summary>
    ''' Flussteuerung-Einstellung
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HandshakeMode As System.IO.Ports.Handshake

#End Region

#Region "Events"

    ''' <summary>
    ''' Wenn bei der Initialisierung ein Fehler auftritt 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Event PortInitError(sender As String, message As String)

    ''' <summary>
    ''' Reagiert auf das eingehen von Messages
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SerialDataReceived(sender As Object, e As SerialDataReceivedEventArgs)
        ReceivedData = ""

        ReceivedData = _ConnectedPort.ReadLine()

        ' Wenn es häufig dazu kommt, dass der Puffer nicht komplett gelesen wird, dann kann man nach dem 
        ' lesen noch versuchen, den Thread ein wenig zuverzögern. Durch das Verwenden von ReadLine sollte 
        ' sowas aber nicht mehr nötig sein. Setzt man die Verzögerung aber ein, dann reduziert man den
        ' Zeitabstand zwischen zwei Scans, was bei "schnellen Mitarbeitern" schnell mal zu Exceptions führen
        ' kann
        'Thread.Sleep(500)

        ' Leitet den empfangenen Text weiter an das Steuerelement
        RaiseEvent DataReceived(PortName, ReceivedData)

    End Sub

    ''' <summary>
    ''' Weiterleiten der empfangenen Daten
    ''' </summary>
    ''' <param name="CommPortName"></param>
    ''' <param name="Data"></param>
    ''' <remarks></remarks>
    Public Event DataReceived(CommPortName As String, Data As String)

    Public Event CheckedDataReceived(CommPortName As String, Data As String)

#End Region

#Region "Initialize"

    <Obsolete()>
    Public Sub New(PortName As String, _Baud As Integer, _ParityMode As System.IO.Ports.Parity, _BitLength As Integer, _StopBitMode As System.IO.Ports.StopBits, _HandShakeMode As System.IO.Ports.Handshake)

        Me.PortName = PortName
        Me.Baud = _Baud
        Me.ParityMode = _ParityMode
        Me.BitLength = _BitLength
        Me.StopBitMode = _StopBitMode
        Me.HandshakeMode = _HandShakeMode

        CreatePort()

    End Sub

    Public Sub New(Serial As SerialPortType)

        Me.PortName = Serial.PortName
        Me.Baud = Serial.Baud
        Me.ParityMode = Serial.ParityMode
        Me.BitLength = Serial.BitLength
        Me.StopBitMode = Serial.StopBitMode
        Me.HandshakeMode = Serial.HandshakeMode

        CreatePort()
    End Sub

    Public Sub CreatePort()
        _ConnectedPort = New System.IO.Ports.SerialPort(Me.PortName, Me.Baud, Me.ParityMode, Me.BitLength, Me.StopBitMode)

        With _ConnectedPort
            .Handshake = Me.HandshakeMode
            .ReadTimeout = 500
            .WriteTimeout = 500

            AddHandler .DataReceived, AddressOf SerialDataReceived

        End With

        Try

            With _ConnectedPort
                .Open()
                .Write("SI\r\n")
            End With

        Catch ex As Exception
            RaiseEvent PortInitError(PortName, ex.Message)
        End Try

    End Sub

    Protected Overrides Sub Finalize()

        ' Schließen und Freigeben des Ports
        If _ConnectedPort.IsOpen = True Then
            _ConnectedPort.Close()
        End If
        _ConnectedPort.Dispose()

        MyBase.Finalize()
    End Sub

#End Region

#Region "Functions"

    ''' <summary>
    ''' Leitet die Daten erneut weiter
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ReceivedDataChecked()
        RaiseEvent CheckedDataReceived(PortName, ReceivedData)
    End Sub

#End Region

End Class

Public Class SerialPortType

    ''' <summary>
    ''' Name des zu verwendenden Ports
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PortName As String
        Set(value As String)
            _PortName = value
        End Set
        Get
            Return _PortName.ToUpper
        End Get
    End Property
    Dim _PortName As String = "Com1"

    ''' <summary>
    ''' Übertragungsrate
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Baud As Integer = 16800

    ''' <summary>
    ''' Paritäts Einstellung
    ''' </summary>
    ''' <remarks></remarks>
    Public ParityMode As System.IO.Ports.Parity = Parity.None

    ''' <summary>
    ''' Bit-Einstellung
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BitLength As Integer = 8

    ''' <summary>
    ''' StopBit-Einstellung
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StopBitMode As System.IO.Ports.StopBits = StopBits.None

    ''' <summary>
    ''' Flussteuerung-Einstellung
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HandshakeMode As System.IO.Ports.Handshake = Handshake.None


End Class