Imports MYKEY.FxCore.Common.OperationStatusEnum

Public Class OperationStatusData

    Inherits ObservableObject

    ''' <summary>
    ''' Wie das Ergebniss der letzten Operation war, die ausgeführt wurde
    ''' </summary>
    ''' <value></value>
    ''' <returns>Es wird ein allgemeines Ergebnis erzeugt</returns>
    ''' <remarks></remarks>
    Public Property Status As OperationStatus
        Get
            Return _Status
        End Get
        Set(value As OperationStatus)
            If value <> _Status Then
                _Status = value
                Me.OnPropertyChanged("Status")
            End If

        End Set
    End Property
    Private _Status As OperationStatus = OperationStatus.NoOperation

End Class
