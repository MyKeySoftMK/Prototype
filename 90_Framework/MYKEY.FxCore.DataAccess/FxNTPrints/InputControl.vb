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

Namespace FxNTPrints

    Partial Public Class InputControl
        Public Property GUID As System.Guid
        Public Property ParameterName As String = ""
        Public Property Text As String = ""
        Public Property Description As String = ""
        Public Property TypeGuid As System.Guid = New Guid("00000000-0000-0000-0000-00a000000000")
        Public Property DataValues As String = ""
        Public Property DataQueryStatement As String = ""
        Public Property DataMinValue As String = ""
        Public Property DataMaxValue As String = ""
        Public Property CreatorGUID As System.Guid
        Public Property Created As Date
        Public Property LastModified As Date
        Public Property ModifierGUID As System.Guid
        Public Property IsNotVisible As Boolean
        Public Property Deleted As Date
        Public Property DeleterGUID As System.Guid
        Public Property CanNotDelete As Boolean
        Public Property Rowversion As Byte()
    
        Public Overridable Property Prints As ICollection(Of PrintToInputControl) = New HashSet(Of PrintToInputControl)
    
    End Class

End Namespace
