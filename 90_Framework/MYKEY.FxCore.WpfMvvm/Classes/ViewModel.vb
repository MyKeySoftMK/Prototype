Imports MYKEY.FxCore.Common
Imports System.ComponentModel
Imports System.Threading
Imports System.Windows.Input

Public Class ViewModel

    Inherits ObservableObject

    
    Private WithEvents bWorker As BackgroundWorker

    ''' <summary>
    ''' Hilfsmethode um einen Schaltfläche immer aktiv zu haben
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReturnTrue() As Boolean
        Return True
    End Function

#Region "Constructor"

    Public Sub New()

        ' Backgroundworker für das Ausführen von ASynchronen-Befehlen
        ' Um den BGW zu nutzen, sind noch die Handler für folgende Events zu setzen
        '   - DoWork            (Die Methode, die im Hintergrund ausgeführt werden soll)
        '   - ProgressChanged   (Wenn sich der Fortschritt verändert hat)
        '   - RunWorkerComplete (Wenn der BGW vollständig abgearbeitet wurde)

        bWorker = New BackgroundWorker

        With bWorker
            .WorkerReportsProgress = True
            .WorkerSupportsCancellation = True
        End With

        ExitCommand = New DelegateCommand(AddressOf CloseWindow)

        MainWindowCloseing = False

    End Sub

#End Region

#Region "Properties"

    Public Property MainWindowCloseing As Boolean

    Private _IsBusy As Boolean
    Public Property IsBusy As Boolean
        Set(value As Boolean)
            If value <> _IsBusy Then
                _IsBusy = value
                OnPropertyChanged("IsBusy")
            End If
        End Set
        Get
            Return _IsBusy
        End Get
    End Property

#End Region

#Region "Events"

    ''' <summary>
    ''' Damit man eine View aus einem ViewModel heraus beenden kann
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Event RequestClose As EventHandler
    Public Shared Event RequestHide As EventHandler

    Public Property ExitCommand() As ICommand
        Get
            Return m_ExitCommand
        End Get
        Set(value As ICommand)
            m_ExitCommand = Value
        End Set
    End Property
    Private m_ExitCommand As ICommand

    Public Sub CloseWindow()
        RaiseEvent RequestClose(Me, EventArgs.Empty)
    End Sub

    Public Sub HideWindow()
        RaiseEvent RequestHide(Me, EventArgs.Empty)
    End Sub

#End Region

#Region "BackgroundWorker"
    Dim _doneEvent As New AutoResetEvent(True)

    Public ReadOnly Property WorkerWorkComplete() As AutoResetEvent
        Get
            Return _doneEvent
        End Get
    End Property

    Public Sub WorkerSetWorkComplete()
        _doneEvent.[Set]()
    End Sub

    ''' <summary>
    ''' Das Backgroundworker-Objekt wird benutzt um Hintergrundprozesse zu starten. Das ist auch
    ''' dafür notwendig, wenn wir den BusyIndicator benutzen wollen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Worker As BackgroundWorker
        Set(value As BackgroundWorker)
            bWorker = value
        End Set
        Get
            Return bWorker
        End Get
    End Property

    Private _WorkerProcessPercentage As Integer
    Public Property WorkerProcessPercentage As Integer
        Set(value As Integer)
            If _WorkerProcessPercentage <> value Then
                _WorkerProcessPercentage = value
                OnPropertyChanged("WorkerProcessPercentage")
            End If
        End Set
        Get
            Return _WorkerProcessPercentage
        End Get
    End Property

    Dim _WorkerStartEnabled As Boolean
    Public Property WorkerStartEnabled As Boolean
        Set(value As Boolean)
            If _WorkerStartEnabled <> value Then
                _WorkerStartEnabled = value
                OnPropertyChanged("WorkerStartEnabled")
            End If
        End Set
        Get
            Return _WorkerStartEnabled
        End Get
    End Property

    Dim _WorkerCancelEnabled As Boolean
    Public Property WorkerCancelEnabled As Boolean
        Set(value As Boolean)
            If _WorkerCancelEnabled <> value Then
                _WorkerCancelEnabled = value
                OnPropertyChanged("WorkerCancelEnabled")
            End If
        End Set
        Get
            Return _WorkerCancelEnabled
        End Get
    End Property

    Private _WorkerOutput As String
    Public Property WorkerOutput As String
        Set(value As String)
            If _WorkerOutput <> value Then
                _WorkerOutput = value
                OnPropertyChanged("WorkerOutput")
            End If
        End Set
        Get
            Return _WorkerOutput
        End Get
    End Property



#End Region

#Region "MessageBox"

#Region "Properties"

    ''' <summary>
    ''' Flag to trigger user prompt.
    ''' </summary>
    Public Property ShowMessageBox As Boolean
        Get
            Return _ShowMessageBox
        End Get
        Set(value As Boolean)
            _ShowMessageBox = value
            Me.OnPropertyChanged("ShowMessageBox")
        End Set
    End Property
    Dim _ShowMessageBox As Boolean = False

    ''' <summary>
    ''' Der Text, der in der Messagebox angezeigt werden soll
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MessageBoxDisplayText As String
        Set(value As String)
            If value <> _MessageBoxDisplayText Then
                _MessageBoxDisplayText = value
                Me.OnPropertyChanged("MessageBoxDisplayText")
            End If
        End Set
        Get
            Return _MessageBoxDisplayText
        End Get
    End Property
    Private _MessageBoxDisplayText As String

#End Region

#Region "Commands"

    ''' <summary>
    ''' Delegate for Save and Close action
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property CloseMessageBoxCommand As DelegateCommand(Of Object)
        Get
            If _CloseMessageBoxCommand Is Nothing Then
                _CloseMessageBoxCommand = New DelegateCommand(Of Object)(AddressOf ExecuteCloseMessageBoxCommand, AddressOf ReturnTrue)
            End If
            Return _CloseMessageBoxCommand
        End Get
    End Property
    Private _CloseMessageBoxCommand As DelegateCommand(Of Object)

#End Region

#Region "MVVM"

    Public Sub ExecuteCloseMessageBoxCommand()

        ShowMessageBox = False

    End Sub

#End Region

#End Region

#Region "COM-Port Abfrage"

    Public Event COMPort_DataReceived(COMDevice As String, COMMessage As String)

    Public Property ControlCOMPort As FxCore.Common.SerialPortCommunication

    Public Sub COMPort_Initialize()
        AddHandler ControlCOMPort.CheckedDataReceived, AddressOf COMPort_RaiseDataReceived
    End Sub

    Public Sub COMPort_RaiseDataReceived(COMDevice As String, COMMessage As String)
        RaiseEvent COMPort_DataReceived(COMDevice, COMMessage)
    End Sub

#End Region


End Class
