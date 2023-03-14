Public Class URL

    ''' <summary>
    ''' Aus einer URL kann der Server ausgelesen werden
    ''' </summary>
    ''' <param name="UrlString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetServerFromURL(UrlString As String) As String

        Dim server As String = ""
        Dim _Start As Integer
        Dim _End As Integer

        If UrlString.Length > 0 Then

            ' Suchen des //
            _Start = InStr(UrlString, "//", CompareMethod.Text) + 2
            _End = InStr(_Start, UrlString, "/", CompareMethod.Text)

            If _Start > 2 And _End > 0 Then
                server = Mid(UrlString, _Start, _End - _Start)

                ' Prüfen, ob eine Portangabe enthalten ist und diese entfernen
                _End = InStr(server, ":", CompareMethod.Text)
                If _End > 0 Then
                    server = Mid(server, 1, _End - 1)
                End If

            End If

        End If

        Return server

    End Function

End Class
