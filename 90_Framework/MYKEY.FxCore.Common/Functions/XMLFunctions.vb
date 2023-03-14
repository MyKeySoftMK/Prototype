
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization

Public Class XMLFunctions

    ''' <summary>
    ''' Gibt die Klasse (versehen mit XML-Tags) als XML-Datei zurück
    ''' </summary>
    ''' <param name="ObjectInstance"></param>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    Public Function ExportObjectToXml(ObjectInstance As Object, FileName As String) As Boolean

        Dim _result As Boolean = False

        Try
            Dim XmlSerializer As New Xml.Serialization.XmlSerializer(ObjectInstance.GetType)
            Dim XmlSettings As New Xml.XmlWriterSettings

            With XmlSettings
                .Indent = True
                .NewLineOnAttributes = True
            End With

            Using stream = File.OpenWrite(FileName)

                Using xmlWriter = System.Xml.XmlWriter.Create(stream, XmlSettings)
                    XmlSerializer.Serialize(xmlWriter, ObjectInstance)
                End Using
            End Using

            _result = True
        Catch ex As Exception

        End Try

        Return _result

    End Function

    ''' <summary>
    ''' Gibt die Klasse (versehen mit XML-Tags) als Text-Objekt zurück
    ''' </summary>
    ''' <param name="ObjectInstance"></param>
    ''' <returns></returns>
    Public Function ExportObjectToXml(ObjectInstance As Object, Optional Encoding As XmlEnums.EncodingType = XmlEnums.EncodingType.UTF16) As String

        Dim _result As String = ""

        Try
            Dim _xmlSerializer As New Xml.Serialization.XmlSerializer(ObjectInstance.GetType)

            Select Case Encoding


                Case XmlEnums.EncodingType.UTF16

                    ' Der Standardserializer verwendet UTF-16

                    Dim text As New System.IO.StringWriter()
                    _xmlSerializer.Serialize(text, ObjectInstance)
                    _result = text.ToString
                    text.Close()

                Case XmlEnums.EncodingType.UTF8

                    ' will man aber ein XML-File mit UTF8 muss man eine andere Lösung finden.

                    Dim memStrm As MemoryStream = New MemoryStream()
                    Dim utf8e As UTF8Encoding = New UTF8Encoding()
                    Dim xmlSink As XmlTextWriter = New XmlTextWriter(memStrm, utf8e)
                    _xmlSerializer.Serialize(xmlSink, ObjectInstance)
                    Dim utf8EncodedData As Byte() = memStrm.ToArray()
                    _result = utf8e.GetString(utf8EncodedData)

            End Select



        Catch ex As Exception

        End Try

        Return _result

    End Function

    'Public Function ExportGenericObjectToXml(ObjectInstance As Object, FileName As String) As Boolean

    '    Dim _result As Boolean = False

    '    Try
    '        Dim FileWriter As New JsonFx.Xml.XmlWriter

    '        Dim file As New System.IO.StreamWriter(FileName)

    '        With FileWriter
    '            .Settings.NewLine = vbCrLf
    '            .Settings.PrettyPrint = True
    '            .Settings.Tab = vbTab
    '        End With

    '        FileWriter.Write(ObjectInstance, file)
    '        file.Close()

    '        _result = True
    '    Catch ex As Exception

    '    End Try

    '    Return _result

    'End Function

    ''' <summary>
    ''' Wandelt das Ergebniss eines XML-Strings in das passende Klassen-Objekt
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="xml"></param>
    ''' <returns></returns>
    Public Shared Function Deserialize(Of T)(ByVal xml As String) As T
        Try
            Dim serializer = New Xml.Serialization.XmlSerializer(GetType(T))

            Using reader As System.IO.TextReader = New System.IO.StringReader(xml)
                Dim data = CType(serializer.Deserialize(reader), T)
                Return data
            End Using

        Catch ex As Exception
            Throw
        End Try


    End Function

    ' ungetestet
    Public Shared Function DeserializeFromFile(Of T)(ByVal path As String, ByVal fileName As String) As T
        Try
            Dim serializer = New Xml.Serialization.XmlSerializer(GetType(T))

            Using file = System.IO.File.OpenText(path & fileName)
                Dim data = CType(serializer.Deserialize(file), T)
                Return data
            End Using

        Catch ex As Exception
            Throw
        End Try
    End Function


End Class
