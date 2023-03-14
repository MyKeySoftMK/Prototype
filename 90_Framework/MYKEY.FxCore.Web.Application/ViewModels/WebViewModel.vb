Imports MYKEY.FxCore.Common

Public Class WebViewModel

    Inherits ObservableObject

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

End Class
