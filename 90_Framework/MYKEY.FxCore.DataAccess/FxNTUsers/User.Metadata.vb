Imports System.ComponentModel.DataAnnotations

Namespace FxNTUsers

    <MetadataType(GetType(UserAnnotation))>
    Partial Public Class User

    End Class


    Friend NotInheritable Class UserAnnotation

        '<Required(ErrorMessage:="{0} ist ein Pflichtfeld")>
        'Public Property Name As String = ""

        '<Required(ErrorMessage:="{0} ist ein Pflichtfeld")>
        'Public Property DisplayName As String = ""

    End Class

End Namespace
