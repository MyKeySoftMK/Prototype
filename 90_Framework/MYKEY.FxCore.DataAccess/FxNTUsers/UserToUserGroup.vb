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

Namespace FxNTUsers

    Partial Public Class UserToUserGroup
        Public Property GUID As System.Guid
        Public Property UserGUID As System.Guid
        Public Property UserGroupGUID As System.Guid
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
    
        Public Overridable Property UserGroup As UserGroup
        Public Overridable Property User As User
    
    End Class

End Namespace
