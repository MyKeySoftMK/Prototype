Public Class ProcessEvents

    ' Um die Icons entsprechend anzuzeigen
    Public Shared Event SERVICE_STATE(SrvStat As ServiceStates.States, MessageToShow As String)

    ' Damit das Event von Außen aufgerufen werden kann
    Public Shared Sub RAISE_SERVICE_STATE(SrvStat As ServiceStates.States, MessageToShow As String)
        RaiseEvent SERVICE_STATE(SrvStat, MessageToShow)
    End Sub

End Class
