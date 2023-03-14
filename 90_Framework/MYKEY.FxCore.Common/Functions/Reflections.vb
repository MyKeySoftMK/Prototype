Imports System.Dynamic
Imports System.Reflection

Public Class Reflections

    ''' <summary>
    ''' Sucht die entsprechende Assembly und gibt es als geladenes Objekt zurück
    ''' </summary>
    ''' <param name="NamespaceName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetAssembly(NamespaceName As String, EXE_FOLDER As String) As Assembly

        Dim _FileName As String = ""
        Dim _AssemblyName As String = EXE_FOLDER & NamespaceName

        ' Prüfen ob der Assemblyname eingetragen wurde
        If IO.File.Exists(_AssemblyName) Then
            _FileName = _AssemblyName
        End If

        ' Prüfen ob es eine DLL ist
        If IO.File.Exists(_AssemblyName & ".dll") Then
            _FileName = _AssemblyName & ".dll"
        End If

        'Prüfen ob es eine EXE ist
        If IO.File.Exists(_AssemblyName & ".exe") Then
            _FileName = _AssemblyName & ".exe"
        End If

        ' Laden der Assembly
        Return Assembly.LoadFile(_FileName)
    End Function

    ''' <summary>
    ''' This function will get any property of any object by name
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="PropName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetPropertyValue(ByVal obj As Object, ByVal PropName As String) As Object
        Dim objType As Type = obj.GetType()
        Dim pInfo As System.Reflection.PropertyInfo = objType.GetProperty(PropName)
        Dim PropValue As Object = pInfo.GetValue(obj, Reflection.BindingFlags.GetProperty, Nothing, Nothing, Nothing)

        Return PropValue
    End Function

    Public Shared Function GetFullAssemblyName(NamespaceName As String, EXE_FOLDER As String)
        Return GetAssembly(NamespaceName, EXE_FOLDER).FullName
    End Function

    ''' <summary>
    ''' Mit dieser Methode kann man dynamisch Eigenschaften zu ExpandoObject hinzufügen
    ''' </summary>
    ''' <param name="expando">ExpandoObject-Objekt</param>
    ''' <param name="propertyName">Name der Eigenschaft, die hinzugefügt werden soll</param>
    ''' <param name="propertyValue">Der dazugehörige Wert</param>
    Public Shared Sub AddProperty(ByVal expando As ExpandoObject, ByVal propertyName As String, ByVal propertyValue As Object)
        Dim exDict = TryCast(expando, IDictionary(Of String, Object))

        If exDict.ContainsKey(propertyName) Then
            exDict(propertyName) = propertyValue
        Else
            exDict.Add(propertyName, propertyValue)
        End If
    End Sub

    Public Shared Sub AddProperty(ByVal expando As SerialiseableExpandoObject, ByVal propertyName As String, ByVal propertyValue As Object)
        Dim exDict = TryCast(expando, IDictionary(Of String, Object))

        If exDict.ContainsKey(propertyName) Then
            exDict(propertyName) = propertyValue
        Else
            exDict.Add(propertyName, propertyValue)
        End If
    End Sub
End Class
