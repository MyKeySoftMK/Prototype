'------------------------------------------------------------------------------
' <auto-generated>
'     Der Code wurde von einer Vorlage generiert.
'
'     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
'     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Namespace Cars

    Partial Public Class Car
        Public Property GUID As System.Guid
        Public Property CarName As String = ""
        Public Property CarTypeNumber As String = ""
        Public Property CarDecors As String = ""
        Public Property CarSpecial As Boolean = false
        Public Property CarModellYear As String = ""
        Public Property CarColorsText As String = ""
        Public Property Created As Date = New DateTime(599266080000000000, DateTimeKind.Unspecified)
        Public Property CreatorGUID As System.Guid = New Guid("00000000-0000-0000-0000-000000000000")
        Public Property LastModified As Date = New DateTime(599266080000000000, DateTimeKind.Unspecified)
        Public Property ModifierGUID As System.Guid = New Guid("00000000-0000-0000-0000-000000000000")
        Public Property Deleted As Date = New DateTime(599266080000000000, DateTimeKind.Unspecified)
        Public Property DeleterGUID As System.Guid = New Guid("00000000-0000-0000-0000-000000000000")
        Public Property IsNotVisible As Boolean = false
        Public Property CanNotDelete As Boolean = false
        Public Property Rowversion As Date
    
    End Class

End Namespace
