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

    Partial Public Class MenuEntry
        Public Property GUID As System.Guid = New Guid("00000000-0000-0000-0000-000000000000")
        Public Property MenuGroupGUID As System.Guid = New Guid("00000000-0000-0000-0000-101000000104")
        Public Property Text As String = ""
        Public Property SortID As Integer = 0
        Public Property Description As String = ""
        Public Property IconName As String = ""
        Public Property [RaiseEvent] As String = ""
        Public Property LoadAssemblyName As String = ""
        Public Property RunUserControlName As String = ""
        Public Property CountInstancesAllowed As Integer = 0
        Public Property RefreshingTime As Integer = 0
        Public Property URLToSupport As String = ""
        Public Property SystemEntry As Boolean = false
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
        Public Property ControllerAction As String = "Function@NotImplementet"
        Public Property BrushName As String = "CODE.Framework-Icon-Home"
    
        Public Overridable Property MenuGroup As MenuGroup
        Public Overridable Property MenuEntryToUserGroups As ICollection(Of MenuEntryToUserGroup) = New HashSet(Of MenuEntryToUserGroup)
    
    End Class

End Namespace
