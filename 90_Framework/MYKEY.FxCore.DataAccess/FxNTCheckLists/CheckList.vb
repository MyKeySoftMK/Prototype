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

Namespace FxNTCheckLists

    Partial Public Class CheckList
        Public Property GUID As System.Guid
        Public Property Name As String = ""
        Public Property Description As String = ""
        Public Property UserGuid As System.Guid = New Guid("00000000-0000-0000-0000-000000000002")
        Public Property DueDate As Date = New DateTime(599266080000000000, DateTimeKind.Unspecified)
        Public Property CreatorGUID As System.Guid
        Public Property Created As Date
        Public Property LastModified As Date
        Public Property ModifierGUID As System.Guid
        Public Property Deleted As Date
        Public Property DeleterGUID As System.Guid
        Public Property IsNotVisible As Boolean
        Public Property CanNotDelete As Boolean
        Public Property Rowversion As Byte()
    
        Public Overridable Property Steps As ICollection(Of CheckListsStep) = New HashSet(Of CheckListsStep)
    
    End Class

End Namespace
