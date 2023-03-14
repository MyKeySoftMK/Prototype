
Imports System.Dynamic
Imports System.Runtime.InteropServices
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization

Public Class SerialiseableExpandoObject
    Inherits DynamicObject
    Implements IXmlSerializable, IDictionary(Of String, Object)

    Private ReadOnly root As String = "SerialiseableExpandoObject"
    Private ReadOnly expando As IDictionary(Of String, Object) = Nothing

    Public Sub New()
        expando = New ExpandoObject()
    End Sub

    Public Overrides Function TryGetMember(ByVal binder As GetMemberBinder, <Out> ByRef result As Object) As Boolean
        Dim value As Object

        If expando.TryGetValue(binder.Name, value) Then
            result = value
            Return True
        End If

        Return MyBase.TryGetMember(binder, result)
    End Function

    Public Overrides Function TrySetMember(ByVal binder As SetMemberBinder, ByVal value As Object) As Boolean
        expando(binder.Name) = value
        Return True
    End Function

    Public Function GetSchema() As XmlSchema Implements IXmlSerializable.GetSchema
        Throw New NotImplementedException()
    End Function

    Public Sub ReadXml(ByVal reader As XmlReader) Implements IXmlSerializable.ReadXml
        reader.ReadStartElement(root)

        While Not reader.Name.Equals(root)
            Dim value As Object
            Dim typeContent As String
            Dim underlyingType As Type
            Dim name = reader.Name
            reader.MoveToAttribute("type")
            typeContent = reader.ReadContentAsString()
            underlyingType = Type.[GetType](typeContent)
            reader.MoveToContent()
            expando(name) = reader.ReadElementContentAs(underlyingType, Nothing)
        End While
    End Sub

    Public Sub WriteXml(ByVal writer As XmlWriter) Implements IXmlSerializable.WriteXml
        For Each key In expando.Keys
            Dim value = expando(key)
            writer.WriteStartElement(key)
            'writer.WriteAttributeString("type", value.[GetType]().AssemblyQualifiedName)
            writer.WriteString(value.ToString())
            writer.WriteEndElement()
        Next
    End Sub

    Public Sub Add(ByVal key As String, ByVal value As Object) Implements IDictionary(Of String, Object).Add
        expando.Add(key, value)
    End Sub

    Public Function ContainsKey(ByVal key As String) As Boolean Implements IDictionary(Of String, Object).ContainsKey
        Return expando.ContainsKey(key)
    End Function

    Public ReadOnly Property Keys As ICollection(Of String) Implements IDictionary(Of String, Object).Keys
        Get
            Return expando.Keys
        End Get
    End Property

    Public Function Remove(ByVal key As String) As Boolean Implements IDictionary(Of String, Object).Remove
        Return expando.Remove(key)
    End Function

    Public Function TryGetValue(ByVal key As String, <Out> ByRef value As Object) As Boolean Implements IDictionary(Of String, Object).TryGetValue
        Return expando.TryGetValue(key, value)
    End Function

    Public ReadOnly Property Values As ICollection(Of Object) Implements IDictionary(Of String, Object).Values
        Get
            Return expando.Values
        End Get
    End Property

    Default Public Property Item(ByVal key As String) As Object Implements IDictionary(Of String, Object).Item
        Get
            Return expando(key)
        End Get
        Set(ByVal value As Object)
            expando(key) = value
        End Set
    End Property

    Public Sub Add(ByVal item As KeyValuePair(Of String, Object)) Implements ICollection(Of KeyValuePair(Of String, Object)).Add
        expando.Add(item)
    End Sub

    Public Sub Clear() Implements ICollection(Of KeyValuePair(Of String, Object)).Clear
        expando.Clear()
    End Sub

    Public Function Contains(ByVal item As KeyValuePair(Of String, Object)) As Boolean Implements ICollection(Of KeyValuePair(Of String, Object)).Contains
        Return expando.Contains(item)
    End Function

    Public Sub CopyTo(ByVal array As KeyValuePair(Of String, Object)(), ByVal arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of String, Object)).CopyTo
        expando.CopyTo(array, arrayIndex)
    End Sub

    Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of String, Object)).Count
        Get
            Return expando.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of String, Object)).IsReadOnly
        Get
            Return expando.IsReadOnly
        End Get
    End Property

    Public Function Remove(ByVal item As KeyValuePair(Of String, Object)) As Boolean Implements ICollection(Of KeyValuePair(Of String, Object)).Remove
        Return expando.Remove(item)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, Object)) Implements ICollection(Of KeyValuePair(Of String, Object)).GetEnumerator
        Return expando.GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me.GetEnumerator()
    End Function
End Class


