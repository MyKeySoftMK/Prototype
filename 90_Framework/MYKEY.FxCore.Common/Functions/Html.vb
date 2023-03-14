Imports System.Web
Imports System.Text.RegularExpressions

Public Class Html

    Public Shared Function StripHTML(HTMLText As String, Optional decode As Boolean = True) As String
        Dim reg As New Regex("<[^>]+>", RegexOptions.IgnoreCase)
        Dim stripped = reg.Replace(HTMLText, "")
        Return If(decode, HttpUtility.HtmlDecode(stripped), stripped)
    End Function

End Class
