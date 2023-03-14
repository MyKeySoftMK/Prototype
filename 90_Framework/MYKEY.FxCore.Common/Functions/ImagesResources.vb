Imports System.Reflection
Imports System.Drawing
Imports System.Resources
Imports System.Globalization

Public Class ImagesResources

    Private _Assembly As Assembly
    Public ReadOnly Property Assembly As Assembly
        Get
            Return _Assembly
        End Get
    End Property

    Public Sub New(NamespaceName As String, EXE_FOLDER As String)

        _Assembly = Reflections.GetAssembly(NamespaceName, EXE_FOLDER)

    End Sub

    ''' <summary>
    ''' Auslesen eines Images aus der Resourcen-Datei
    ''' </summary>
    ''' <param name="Path">Es wird die Struktur abgebildet (z.B. Icons/CrystalProject/24x24/apps/blockdevice.png)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetImageFromPath(Path As String) As Object
        Dim objResManager As ResourceManager = New ResourceManager(_Assembly.GetName.Name + ".g", _Assembly)
        Dim objResourceSet As ResourceSet = objResManager.GetResourceSet(CultureInfo.CurrentCulture, True, True)

        For Each objDictionaryEntry As DictionaryEntry In objResourceSet

            If objDictionaryEntry.Key.ToString = Path.ToLower Then
                Return objDictionaryEntry.Value
            End If
        Next

        Return Nothing

    End Function

    ''' <summary>
    ''' Es wird eine Liste aller in der Resourcen-Datei enthaltenen Images zurückgegeben als String-Kollektion
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetImageNamesList(Optional Page As Integer = 0, Optional PageSize As Integer = 0) As List(Of String)
        Dim _List As New List(Of String)

        Dim objResManager As ResourceManager = New ResourceManager(_Assembly.GetName.Name + ".g", _Assembly)
        Dim objResourceSet As ResourceSet = objResManager.GetResourceSet(CultureInfo.CurrentCulture, True, True)

        If Page > 0 And PageSize > 0 Then
            Try

                For i As Integer = (Page - 1) * PageSize To (Page) * PageSize
                    _List.Add(objResourceSet(i).Key.ToString)
                Next

            Catch ex As Exception

            End Try

        Else
            ' Gibt alle Objekte zurück
            For Each objDictionaryEntry As DictionaryEntry In objResourceSet
                _List.Add(objDictionaryEntry.Key.ToString)
            Next
        End If

        Return _List
    End Function

    ''' <summary>
    ''' Es wird eine Liste aller in der Resourcen-Datei enthaltenen Images zurückgegeben als Object-Kollektion
    ''' </summary>
    ''' <param name="Page">Index der Page, die man ermitteln möchte (Startindex=1)</param>
    ''' <param name="PageSize">Größe der zurück erwarteten Page</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetImageList(Optional Page As Integer = 0, Optional PageSize As Integer = 0) As List(Of Object)
        Dim _List As New List(Of Object)

        Dim objResManager As ResourceManager = New ResourceManager(_Assembly.GetName.Name + ".g", _Assembly)
        Dim objResourceSet As ResourceSet = objResManager.GetResourceSet(CultureInfo.CurrentCulture, True, True)

        If Page > 0 And PageSize > 0 Then
            Try

                For i As Integer = (Page - 1) * PageSize To (Page) * PageSize
                    _List.Add(objResourceSet(i))
                Next

            Catch ex As Exception

            End Try

        Else
            ' Gibt alle Objekte zurück
            For Each objDictionaryEntry As DictionaryEntry In objResourceSet
                _List.Add(objDictionaryEntry.Value)
            Next
        End If

        Return _List
    End Function

End Class
