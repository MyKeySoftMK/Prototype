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

Namespace FxNTOlbMenu

    Partial Public Class MenuGroup
        Public Property GUID As System.Guid = New Guid("00000000-0000-0000-0000-000000000000")
        Public Property Name As String = ""
        Public Property SortID As Integer = 0
        Public Property Description As String = ""
        Public Property IconSelected As String = ""
        Public Property IconUnselected As String = ""
        Public Property SystemGroup As Boolean = false
        Public Property CreatorGUID As System.Guid
        Public Property Created As Date
        Public Property LastModified As Date
        Public Property ModifierGUID As System.Guid
        Public Property IsDeleted As Boolean
        Public Property Deleted As Date
        Public Property DeleterGUID As System.Guid
        Public Property IsNotVisible As Boolean
        Public Property CanNotDelete As Boolean
        Public Property Rowversion As Byte()
    
        Public Overridable Property MenuEntries As ICollection(Of MenuEntry) = New HashSet(Of MenuEntry)
    
    End Class

End Namespace
