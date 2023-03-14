#Region "ItemData mit VB.Net"

Public Class ItemData
    Public ItemText As String
    Public ItemData As String

    'Public Sub New(ByVal _ItemName As String, ByVal _ItemData As Integer)
    '    ItemText = _ItemName
    '    ItemData = _ItemData
    'End Sub

    Public Sub New(ByVal _ItemName As String, ByVal _ItemData As String)
        ItemText = _ItemName
        ItemData = _ItemData
    End Sub

    Public Overrides Function ToString() As String
        Return ItemText
    End Function
End Class

#End Region