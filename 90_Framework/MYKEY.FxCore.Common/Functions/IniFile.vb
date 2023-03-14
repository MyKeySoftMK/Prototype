Option Compare Text

Imports Microsoft.VisualBasic.FileIO

''' <summary>
''' Library for handling reading and manipulation of Ini files.
''' </summary>
''' <remarks>http://inifile.codeplex.com/</remarks>
Public Class IniFile

#Region " Instance "
    ''' <summary>
    ''' Internal list of <see cref="Section"/>s in the file.
    ''' </summary>
    ''' <remarks></remarks>
    Private _sections As List(Of Section)

    ''' <summary>
    ''' Specifies whether the key name portion of the key is whitespace sensitive.
    ''' </summary>
    ''' <remarks></remarks>
    Private _nameWhitespaceLiteral As Boolean
    ''' <summary>
    ''' Specifies whether the value portion of the key is whitespace sensitive.
    ''' </summary>
    ''' <remarks></remarks>
    Private _valueWhitespaceLiteral As Boolean

    ''' <summary>
    ''' Token at the beginning of a line which denotes a comment. (';' character.)
    ''' </summary>
    ''' <remarks></remarks>
    Private Const COMMENT As String = ";"
    ''' <summary>
    ''' Token at the beginning of a line which denotes a <see cref="Section"/> name start. ('[' character.)
    ''' </summary>
    ''' <remarks></remarks>
    Private Const SECTION_BEGIN As Char = "["
    ''' <summary>
    ''' Token at the end of a line which denotes a <see cref="Section"/> name end. (']' character.)
    ''' </summary>
    ''' <remarks></remarks>
    Private Const SECTION_END As Char = "]"
    ''' <summary>
    ''' Token within a <see cref="Key"/> which defines the separation of the <see cref="Key.Name"/> and <see cref="Key.Value"/>. ('=' character.).
    ''' Only the first token encountered in a line is processed (so the <see cref="Key.Value"/> can contain this token in its text).
    ''' </summary>
    ''' <remarks></remarks>
    Private Const KEY_VALUE_SEPARATOR As Char = "="


    ''' <summary>
    ''' Describes an Ini file section.
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure Section
        ''' <summary>
        ''' Section name defined between the <see cref="SECTION_BEGIN"/> and <see cref="SECTION_END"/> tokens.
        ''' </summary>
        ''' <remarks></remarks>
        Dim Name As String

        ''' <summary>
        ''' Text after the <see cref="COMMENT"/> token appearing immediately above the section name.
        ''' </summary>
        ''' <remarks></remarks>
        Dim Comments As String

        ''' <summary>
        ''' Sequential order in the file.
        ''' </summary>
        ''' <remarks></remarks>
        Dim Order As Integer

        ''' <summary>
        ''' <see cref="Key"/> data.
        ''' </summary>
        ''' <remarks></remarks>
        Dim Keys As List(Of Key)
    End Structure

    ''' <summary>
    ''' Describes an Ini file key.
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure Key
        ''' <summary>
        ''' Key name.
        ''' </summary>
        ''' <remarks></remarks>
        Dim Name As String

        ''' <summary>
        ''' Text appearing after the <see cref="KEY_VALUE_SEPARATOR"/>.
        ''' </summary>
        ''' <remarks></remarks>
        Dim Value As String

        ''' <summary>
        ''' Text after the <see cref="COMMENT"/> token appearing immediately above the key.
        ''' </summary>
        ''' <remarks></remarks>
        Dim Comments As String

        ''' <summary>
        ''' Sequential order in the containing <see cref="Section"/>.
        ''' </summary>
        ''' <remarks></remarks>
        Dim Order As Integer
    End Structure

    ''' <summary>
    ''' Creates a new object with the read/write path set by <paramref name="fileName"/>.
    ''' </summary>
    ''' <param name="fileName">Target file name which will be read on instantiation and written to on save.
    ''' This file does not need to exist at instantiation time.</param>
    ''' <param name="keyNameWhitespaceLiteral">Sets the <see cref="KeyNameWhitespaceLiteral"/> property.</param>
    ''' <param name="keyValueWhitespaceLiteral">Sets the <see cref="KeyValueWhitespaceLiteral"/> property.</param>
    ''' <remarks></remarks>
    Public Sub New(fileName As String,
                   Optional ByVal keyNameWhitespaceLiteral As Boolean = False,
                   Optional ByVal keyValueWhitespaceLiteral As Boolean = False)

        Me.FileName = fileName

        _sections = New List(Of Section)
        _nameWhitespaceLiteral = keyNameWhitespaceLiteral
        _valueWhitespaceLiteral = keyValueWhitespaceLiteral

        If FileSystem.FileExists(fileName) Then
            ReadFile()
        End If
    End Sub

#End Region


#Region " I/O "

    ''' <summary>
    ''' Returns the current object content as a single string.
    ''' </summary>
    ''' <param name="preserveOrders">Determines if the section and key order settings should be honored.
    ''' On large ini files, this may incur a performance hit.</param>
    ''' <returns>String containing the entire Ini contents.</returns>
    ''' <remarks>The output of this function is the same as what is written in <see cref="SaveFile"/>.</remarks>
    Public Function GetOutputString(Optional preserveOrders As Boolean = False) As String

        Return GetOutputString(_sections, preserveOrders, preserveOrders)

    End Function

    ''' <summary>
    ''' Helper method which generates the output content.
    ''' </summary>
    ''' <param name="sections">Sections to process for writing.</param>
    ''' <param name="preserveSectionOrders">Determines if the section order settings should be honored.</param>
    ''' <param name="preserveKeyOrders">Determines if the key order settings should be honored.</param>
    ''' <remarks></remarks>
    Private Function GetOutputString(sections As List(Of Section),
                                     preserveSectionOrders As Boolean,
                                     preserveKeyOrders As Boolean) As String

        If Not preserveSectionOrders Then
            ' Order is not a consideration for sections, just dump the data.

            Dim output As New System.Text.StringBuilder

            For Each nameSection As Section In sections
                output.Append(GetSectionOutput(nameSection,
                                               preserveOrders:=preserveKeyOrders))
            Next

            Return output.ToString

        Else
            ' Order matters, sort the sections before outputing.
            Dim sortedSections As New List(Of Section)

            For Each nameSection As Section In _sections
                Dim order As Integer = nameSection.Order

                ' When there is nothing yet added or the order is the max integer value, append to the end.
                If sortedSections.Count = 0 OrElse
                    order = Integer.MaxValue Then

                    sortedSections.Add(nameSection)

                Else
                    ' Explicit order is provided.
                    ' Locate where it should go with respect to other sections.
                    Dim insertBefore As Section = sortedSections.Find(
                        Function(sorted) sorted.Order > order)

                    ' Check that the section found contains data.
                    If Not SectionHasData(insertBefore) Then
                        ' No lower order found, add to the end.
                        sortedSections.Add(nameSection)

                    Else
                        ' The current section will be inserted at the index where section with the
                        ' higher order value was found.
                        Dim insertAtIndex As Integer = sortedSections.IndexOf(insertBefore)

                        sortedSections.Insert(insertAtIndex, nameSection)
                    End If

                End If
            Next

            ' Recursive call to produce the output.
            Return GetOutputString(sortedSections, False, preserveKeyOrders)

        End If

    End Function


    ''' <summary>
    ''' Reads the current contents of <see cref="FileName"/> into the object.
    ''' </summary>
    ''' <remarks>
    ''' <para>Regarding comments and association with <see cref="Section"/>s or <see cref="Key"/>s:</para>
    ''' <para>All comments appearing immediately above a section (regardless of whitespace) will be associated with the respective section.
    ''' The same rule applies for <see cref="Key"/>s as well.</para>
    ''' <para>
    ''' The one exception is when comments appear at the end of a file where there is no section or key below it to assign to.
    ''' When this happens, a blank <see cref="Key"/> is generated with an empty <see cref="Key.Name"/> and <see cref="Key.Value"/> property, but will have the <see cref="Key.Comments"/> property containing the comments.</para>
    ''' <para>To avoid pulling this "placeholder" key, use the <see cref="GetAllKeysWithAName"/> method.</para>
    ''' </remarks>
    Public Sub ReadFile()

        ' Clear all existing data.
        _sections = New List(Of Section)

        Dim comments As New List(Of String)

        ' Create the default section.
        Dim currentSection As Section = IniFile.MakeSection("", order:=0)

        Using iniFileText As IO.TextReader = FileSystem.OpenTextFileReader(FileName)
            With iniFileText

                ' Check for the end of the file.
                While .Peek <> -1

                    ' Grab the next line.
                    Dim lineRaw As String = .ReadLine
                    Dim lineTrim As String = lineRaw.Trim

                    If lineTrim.StartsWith(COMMENT) Then
                        ' Remove the comment token and add the remaining text as a comment.
                        comments.Add(lineTrim.Substring(COMMENT.Length).Trim)


                        ' Section check.
                    ElseIf lineTrim.StartsWith(SECTION_BEGIN) AndAlso
                        lineTrim.EndsWith(SECTION_END) Then

                        ' If a section was previously being built then add it to the object collection.
                        ' Detect condition by presense of either a section name or existance of keys.
                        If currentSection.Name <> "" OrElse currentSection.Keys.Count > 0 Then
                            _sections.Add(currentSection)
                        End If


                        ' Create a new section.
                        ' Take everything between the first and last char as the name.
                        currentSection = IniFile.MakeSection(
                            lineTrim.Substring(1, lineTrim.Length - 2),
                            comments:=String.Join(ControlChars.NewLine, comments.ToArray),
                            order:=_sections.Count)

                        ' Reset comments as new comments will apply to the next component.
                        comments = New List(Of String)


                        ' Key check.
                    ElseIf lineTrim.Contains(KEY_VALUE_SEPARATOR) Then

                        ' Grab the components into an array.
                        Dim components As String() = lineRaw.Split({KEY_VALUE_SEPARATOR}, 2)

                        Dim keyName As String = components(0)
                        Dim keyValue As String = components(1)

                        Dim key As Key = IniFile.MakeKey(
                            If(KeyNameWhitespaceLiteral, keyName, keyName.Trim),
                            If(KeyValueWhitespaceLiteral, keyValue, keyValue.Trim),
                            comments:=String.Join(ControlChars.NewLine, comments.ToArray),
                            order:=currentSection.Keys.Count)

                        ' Reset comments as new comments will apply to the next component.
                        comments = New List(Of String)

                        ' Add the new key to the current section being built.
                        currentSection.Keys.Add(key)


                    Else
                        ' Unknown line. Ignore it.
                    End If

                End While

                ' Check if there are comments which are not assigned.
                If comments.Count > 0 Then
                    ' "Orphaned" comments. Create a blank key to hold them.
                    currentSection.Keys.Add(
                        MakeKey("", "",
                                comments:=String.Join(ControlChars.NewLine, comments.ToArray),
                                order:=currentSection.Keys.Count))
                End If

                ' Add the section which was being built at the time the file ended.
                _sections.Add(currentSection)

            End With
        End Using

    End Sub

    ''' <summary>
    ''' Writes the current object content to <see cref="FileName"/>.
    ''' </summary>
    ''' <param name="preserveOrders">Determines if the section and key order settings should be honored.
    ''' On large ini files, this may incur a performance hit.</param>
    ''' <param name="reloadAfterSave">Reloads the ini file after the save completes.
    ''' This action essentially rebuilds the section and key ordering according to any changes.</param>
    ''' <exception cref="IO.FileNotFoundException">Thrown when the <see cref="FileName"/> property is empty.</exception>
    Public Sub SaveFile(Optional preserveOrders As Boolean = False,
                        Optional reloadAfterSave As Boolean = True)

        If FileName.Trim = "" Then
            Throw New IO.FileNotFoundException("A file name is not assigned.")
        End If

        ' Write the output to the current file.
        FileSystem.WriteAllText(
            FileName, GetOutputString(_sections, preserveOrders, preserveOrders), False)

        If reloadAfterSave Then
            ReadFile()
        End If

    End Sub

#End Region


#Region " Properties "

    ''' <summary>
    ''' The file path where the object will be read from and written to.
    ''' </summary>
    ''' <value>New full file path location.</value>
    ''' <returns>Full file path and name of the current object.</returns>
    ''' <remarks></remarks>
    Public Property FileName As String

    ''' <summary>
    ''' Raw section data stored by the object.
    ''' </summary>
    ''' <returns>Section data as stored internally.</returns>
    Public ReadOnly Property Sections As List(Of Section)
        Get
            Return _sections
        End Get
    End Property

    ''' <summary>
    ''' Specifies if the parser includes leading and trailing whitespaces from the source in the <see cref="Key.Name"/> property.
    ''' </summary>
    ''' <returns>Boolean indicating leading and trailing whitespace sensitivity.</returns>
    ''' <remarks>This property only applies to the reading of the ini file.
    ''' It does not modify the output format.</remarks>
    Public ReadOnly Property KeyNameWhitespaceLiteral As Boolean
        Get
            Return _nameWhitespaceLiteral
        End Get
    End Property

    ''' <summary>
    ''' Specifies if the parser includes leading and trailing whitespaces from the source in the <see cref="Key.Value"/> property.
    ''' </summary>
    ''' <returns>Boolean indicating leading and trailing whitespace sensitivity.</returns>
    ''' <remarks>This property only applies to the reading of the ini file.
    ''' It does not modify the output format.</remarks>
    Public ReadOnly Property KeyValueWhitespaceLiteral As Boolean
        Get
            Return _valueWhitespaceLiteral
        End Get
    End Property

#End Region


#Region " Accessors "

    ''' <summary>
    ''' Returns the all the keys within <paramref name="sectionName"/> which have a non-empty <see cref="Key.Name"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <returns>Keys within the provided section which have a non-empty <see cref="Key.Name"/> property.</returns>
    Public Function GetAllKeysWithAName(sectionName As String) As Key()

        Dim keys As New List(Of Key)
        keys.AddRange(GetSectionKeys(sectionName))

        Return keys.FindAll(Function(x) x.Name <> "").ToArray

    End Function

    ''' <summary>
    ''' Returns the all the keys matching <paramref name="keyName"/> in the specified <paramref name="sectionName"/> which have a non-empty <see cref="Key.Value"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <param name="keyName">If provided, only the keys matching the specified value are returned.
    ''' If not provided, all keys in the section are returned.</param>
    ''' <returns>Keys within the provided section which have a non-empty <see cref="Key.Value"/> property.</returns>
    Public Function GetAllKeysWithAValue(sectionName As String,
                                         Optional keyName As String = "") As Key()

        Dim keys As New List(Of Key)
        keys.AddRange(GetAllKeys(sectionName, keyName:=keyName))

        Return keys.FindAll(Function(x) x.Value <> "").ToArray

    End Function

    ''' <summary>
    ''' Returns the all the keys matching <paramref name="keyName"/> in the specified <paramref name="sectionName"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <param name="keyName">If provided, only the keys matching the specified value are returned.
    ''' If not provided, all keys in the section are returned.</param>
    ''' <returns>Key data within the provided section.</returns>
    Public Function GetAllKeys(sectionName As String,
                               Optional keyName As String = "") As Key()

        If Not SectionNameExists(sectionName) Then
            Return New Key() {}

        Else
            ' Section exists, extract the keys.
            Dim section As Section = GetSection(sectionName)

            If keyName = "" Then
                ' No name specified, return all.
                Return section.Keys.ToArray

            Else
                ' Find all keys matching the provided name.
                Return section.Keys.FindAll(
                    Function(checkKey) checkKey.Name = keyName).ToArray
            End If
        End If

    End Function

    ''' <summary>
    ''' Returns the all the sections with the specified <paramref name="name"/> in the object.
    ''' </summary>
    ''' <param name="name">If provided, only the sections matching the specified value are returned.
    ''' If not provided, all sections are returned.</param>
    ''' <returns>Section data.</returns>
    Public Function GetAllSections(Optional name As String = "") As Section()

        If name = "" Then
            ' No name specified, return all.
            Return _sections.ToArray

        Else
            ' Find all sections matching the provided name.
            Return _sections.FindAll(
                Function(checkSection) checkSection.Name = name).ToArray
        End If

    End Function


    ''' <summary>
    ''' Returns the full key within the specified <paramref name="sectionName"/>. If the key does not exist, an empty <see cref="IniFile.Key"/> is returned.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <param name="keyName">Name of the target key.</param>
    ''' <returns>Entire key of the first occurance of <paramref name="keyName"/> within <paramref name="sectionName"/>.</returns>
    ''' <remarks>In the event of multiple <paramref name="keyName"/>s within <paramref name="sectionName"/>, only the first occurrance is returned.</remarks>
    Public Function GetKey(sectionName As String, keyName As String) As Key

        If Not KeyNameExists(sectionName, keyName) Then
            Return New Key

        Else
            ' Return the first key with the matching name.
            Return GetSection(sectionName).Keys.Find(
                Function(searchKey) searchKey.Name = keyName)
        End If

    End Function

    ''' <summary>
    ''' Returns the comments of <paramref name="keyName"/> within the specified <paramref name="sectionName"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <param name="keyName">Name of the target key.</param>
    ''' <returns>Comments of the first occurance of the <paramref name="keyName"/>s within <paramref name="sectionName"/>.</returns>
    ''' <remarks>In the event of multiple <paramref name="keyName"/>s within <paramref name="sectionName"/>, only the comments of the first occurrance is returned.</remarks>
    Public Function GetKeyComments(sectionName As String, keyName As String) As String

        Dim key As Key = GetKey(sectionName, keyName)

        Return If(KeyHasData(key), key.Comments, "")

    End Function

    ''' <summary>
    ''' Gets the order of <paramref name="keyName"/> in the specified <paramref name="sectionName"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <param name="keyName">Name of the target key.</param>
    ''' <returns>Order of the first occurance of the <paramref name="keyName"/>s within <paramref name="sectionName"/>.</returns>
    ''' <remarks>In the event of multiple <paramref name="keyName"/>s within <paramref name="sectionName"/>, only the order of the first occurrance is returned.</remarks>
    Public Function GetKeyOrder(sectionName As String, keyName As String) As Integer

        Dim key As Key = GetKey(sectionName, keyName)

        Return If(KeyHasData(key), key.Order, -1)

    End Function

    ''' <summary>
    ''' Gets the value of <paramref name="keyName"/> in the specified <paramref name="sectionName"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <param name="keyName">Name of the target key.</param>
    ''' <param name="defaultValue">The value returned in the event the key does not exist.</param>
    ''' <returns>Value of the first occurance of the <paramref name="keyName"/>s within <paramref name="sectionName"/>.</returns>
    ''' <remarks>In the event of multiple <paramref name="keyName"/>s within <paramref name="sectionName"/>, only the value of the first occurrance is returned.</remarks>
    Public Function GetKeyValue(sectionName As String, keyName As String,
                                Optional defaultValue As String = "") As String

        Dim key As Key = GetKey(sectionName, keyName)

        Return If(KeyHasData(key), key.Value, defaultValue)

    End Function


    ''' <summary>
    ''' Returns the section with the specified <paramref name="name"/>. If the section does not exist, an empty <see cref="IniFile.Section"/> is returned.
    ''' </summary>
    ''' <param name="name">Section name to get.</param>
    ''' <returns>Entire section of the first occurance of the section with the specified <paramref name="name"/>.</returns>
    ''' <remarks>In the event of multiple sections with the specified <paramref name="name"/>, only the first occurrance is returned.</remarks>
    Public Function GetSection(name As String) As Section

        If Not SectionNameExists(name) Then
            Return New Section

        Else
            ' Return the first section with the matching name.
            Return _sections.Find(
                Function(searchSection) searchSection.Name = name)
        End If

    End Function

    ''' <summary>
    ''' Returns the comments of the section with the specified <paramref name="name"/>.
    ''' </summary>
    ''' <param name="name">Name of the target section.</param>
    ''' <returns>Comments of the first occurance of the section with the specified <paramref name="name"/>.</returns>
    ''' <remarks>In the event of multiple sections with the specified <paramref name="name"/>, only the comments of the first occurrance is returned.</remarks>
    Public Function GetSectionComments(name As String) As String

        Dim section As Section = GetSection(name)

        Return If(SectionHasData(section), section.Comments, "")

    End Function

    ''' <summary>
    ''' Returns the order of the section with the specified <paramref name="name"/>.
    ''' </summary>
    ''' <param name="name">Name of the target section.</param>
    ''' <returns>Order of the first occurance of the section with the specified <paramref name="name"/>.</returns>
    ''' <remarks>In the event of multiple sections with the specified <paramref name="name"/>, only the order of the first occurrance is returned.</remarks>
    Public Function GetSectionOrder(name As String) As Integer

        Dim section As Section = GetSection(name)

        Return If(SectionHasData(section), section.Order, -1)

    End Function

    ''' <summary>
    ''' Returns the all the keys in the section with the specified <paramref name="name"/>.
    ''' </summary>
    ''' <param name="name">Name of the target section.</param>
    ''' <returns>Keys of the first occurance of the section with the specified <paramref name="name"/>.</returns>
    ''' <remarks>Alternate way to call <see cref="GetAllKeys"/>.</remarks>
    Public Function GetSectionKeys(name As String) As Key()

        Return GetAllKeys(name)

    End Function


    ''' <summary>
    ''' Returns the count of keys with the name <paramref name="keyName"/> appearing in <paramref name="sectionName"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <param name="keyName">Target key name to count.</param>
    ''' <returns>Number of times <paramref name="keyName"/> appears in the first occurrance of section <paramref name="sectionName"/>.</returns>
    ''' <remarks>In the event of multiple sections named <paramref name="sectionName"/>, only the first occurance is searched.</remarks>
    Public Function KeyNameCount(sectionName As String, keyName As String) As Integer

        If Not KeyNameExists(sectionName, keyName) Then
            Return 0

        Else
            ' The key exists at least once.
            ' Pull all matches of the key name and return the count.
            Return GetSection(sectionName).Keys.FindAll(
                Function(searchKey) searchKey.Name = keyName).Count
        End If

    End Function

    ''' <summary>
    ''' Returns whether or not a key with the name <paramref name="keyName"/> exists in <paramref name="sectionName"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <param name="keyName">Target key name to find.</param>
    ''' <returns>Whether a key with the specified <paramref name="keyName"/> exists within <paramref name="sectionName"/>.</returns>
    Public Function KeyNameExists(sectionName As String, keyName As String) As Boolean

        Dim section As Section = GetSection(sectionName)

        If Not SectionHasData(section) Then
            Return False

        Else
            ' Pull the first key with the matching name.
            Dim key As Key = section.Keys.Find(
                Function(searchKey) searchKey.Name = keyName)

            Return KeyHasData(key)
        End If

    End Function

    ''' <summary>
    ''' Returns the count of sections with the specified <paramref name="name"/>.
    ''' </summary>
    ''' <param name="name">Target section name to count.</param>
    ''' <returns>Number of sections with the specified <paramref name="name"/>.</returns>
    Public Function SectionNameCount(name As String) As Integer

        If Not SectionNameExists(name) Then
            Return 0

        Else
            ' The section exists at least once.
            ' Pull all matches of the section name and return the count.
            Return _sections.FindAll(
                Function(searchSection) searchSection.Name = name).Count
        End If
    End Function

    ''' <summary>
    ''' Returns whether or not a section with the specified <paramref name="name"/> exists.
    ''' </summary>
    ''' <param name="name">Target section name to find.</param>
    ''' <returns>Whether a section with the specified <paramref name="name"/> exists.</returns>
    Public Function SectionNameExists(name As String) As Boolean

        ' Pull the first section with the matching name.
        Dim section As Section = _sections.Find(
            Function(searchSection) searchSection.Name = name)

        Return SectionHasData(section)

    End Function

#End Region


#Region " Modifiers "

    ''' <summary>
    ''' Creates a new <see cref="IniFile.Key"/> with the passed in values and adds it to the specified <paramref name="sectionName"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section.</param>
    ''' <param name="keyName">Name of the new key.</param>
    ''' <param name="value">Value of the new key.</param>
    ''' <param name="comments">Comments for the new key.</param>
    ''' <param name="order">Order of the new key respective to the existing keys in the specified <paramref name="sectionName"/>.</param>
    ''' <returns>Whether or not the resulting <see cref="IniFile.Key"/> was successfully added.</returns>
    Public Function AddKey(sectionName As String, keyName As String, value As String,
                           Optional comments As String = "",
                           Optional order As Integer = Integer.MaxValue) As Boolean

        Return AddKey(sectionName,
                      MakeKey(keyName, value, comments:=comments, order:=order))

    End Function

    ''' <summary>
    ''' Adds the <paramref name="key"/> to the specified <paramref name="sectionName"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section.</param>
    ''' <param name="key">Full <see cref="IniFile.Key"/> to be added.</param>
    ''' <returns>Whether or not the <paramref name="key"/> was successfully added.</returns>
    Public Function AddKey(sectionName As String, key As Key) As Boolean

        If Not SectionNameExists(sectionName) Then
            Return False
        Else
            GetSection(sectionName).Keys.Add(key)
            Return True
        End If

    End Function

    ''' <summary>
    ''' Creates a new <see cref="IniFile.Section"/> with the passed in values.
    ''' </summary>
    ''' <param name="name">Name of the new section.</param>
    ''' <param name="comments">Comments for the new section.</param>
    ''' <param name="order">Order of the new section respective to the existing sections.</param>
    ''' <returns>Whether or not the resulting <see cref="IniFile.Section"/> was successfully added.</returns>
    Public Function AddSection(name As String,
                               Optional comments As String = "",
                               Optional order As Integer = Integer.MaxValue) As Boolean

        Return AddSection(MakeSection(name, comments:=comments, order:=order))

    End Function

    ''' <summary>
    ''' Adds the <paramref name="section"/> to the object.
    ''' </summary>
    ''' <param name="section">Full <see cref="IniFile.Section"/> to be added.</param>
    ''' <returns>Whether or not the <paramref name="section"/> was successfully added.</returns>
    Public Function AddSection(section As Section) As Boolean

        _sections.Add(section)
        Return True

    End Function


    ''' <summary>
    ''' Deletes the key with the name <paramref name="keyName"/> from the specified <paramref name="sectionName"/>.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <param name="keyName">Name of the target key to delete.</param>
    ''' <returns>Whether or not the <paramref name="keyName"/> was deleted.</returns>
    ''' <remarks>In the event of multiple <paramref name="keyName"/>s within <paramref name="sectionName"/>, only the first occurrance is deleted.</remarks>
    Public Function DeleteKey(sectionName As String, keyName As String) As Boolean

        If Not KeyNameExists(sectionName, keyName) Then
            Return False
        Else
            GetSection(sectionName).Keys.Remove(GetKey(sectionName, keyName))
            Return True
        End If

    End Function

    ''' <summary>
    ''' Deletes the section with the specified <paramref name="name"/>.
    ''' </summary>
    ''' <param name="name">Name of the target section to delete.</param>
    ''' <returns>Whether or not the section was deleted.</returns>
    ''' <remarks>In the event of multiple sections matching <paramref name="name"/>, only the first occurrance is deleted.</remarks>
    Public Function DeleteSection(name As String) As Boolean

        If Not SectionNameExists(name) Then
            Return False
        Else
            _sections.Remove(GetSection(name))
            Return True
        End If

    End Function

    ''' <summary>
    ''' Deletes all keys from the section with the specified <paramref name="name"/>.
    ''' </summary>
    ''' <param name="name">Name of the target section to delete the keys from.</param>
    ''' <returns>Whether or not the keys were deleted.</returns>
    ''' <remarks>In the event of multiple sections matching <paramref name="name"/>, only the keys from the first occurrance are deleted.</remarks>
    Public Function DeleteSectionKeys(name As String) As Boolean

        If SectionNameExists(name) Then
            Return False
        Else
            GetSection(name).Keys.Clear()
            Return True
        End If

    End Function


    ''' <summary>
    ''' Sets the data of the key with <paramref name="keyName"/> within the specified <see cref="Section"/>.
    ''' Optionally, this method will create the section and/or key as needed.
    ''' </summary>
    ''' <param name="sectionName">Parent section to search.</param>
    ''' <param name="keyName">Name of the target key.</param>
    ''' <param name="value">Value to set.</param>
    ''' <param name="order">Order to set.</param>
    ''' <param name="comments">Comments to set.</param>
    ''' <param name="createIfNotExist">When True, if the target path of <paramref name="sectionName"/> and/or <paramref name="keyName"/> does not exist, it is created as needed.
    ''' When False, if the path does not exist, no action is taken.</param>
    ''' <returns>Whether the specified key was updated or created.
    ''' False should only be returned in calls where <paramref name="createIfNotExist"/> is set to False.</returns>
    ''' <remarks>In the event of multiple <paramref name="keyName"/>s within <paramref name="sectionName"/>, only the first occurrance is updated.</remarks>
    Public Function SetKeyData(sectionName As String, keyName As String,
                               Optional value As String = Nothing,
                               Optional order As Integer = Integer.MaxValue,
                               Optional comments As String = Nothing,
                               Optional createIfNotExist As Boolean = True) As Boolean

        Dim section As Section

        If Not KeyNameExists(sectionName, keyName) Then

            ' Check if the key should be created.
            If Not createIfNotExist Then
                Return False

            Else
                section = GetSection(sectionName)

                ' Check if the section pulled is valid.
                If Not SectionHasData(section) Then
                    ' Section does not exist, create it.
                    section = MakeSection(sectionName)

                    _sections.Add(section)
                End If

                ' Create the key.
                section.Keys.Add(MakeKey(keyName,
                                         If(value IsNot Nothing, value, ""),
                                         comments:=If(comments IsNot Nothing, comments, ""),
                                         order:=order))

                Return True
            End If

        Else
            section = GetSection(sectionName)
            Dim currentKey As Key = GetKey(sectionName, keyName)

            Dim newKey As Key = MakeKey(
                keyName,
                If(value IsNot Nothing, value, currentKey.Value),
                comments:=If(comments IsNot Nothing, comments, currentKey.Comments),
                order:=If(order = Integer.MaxValue, currentKey.Order, order))


            Dim currentKeyIndex As Integer = section.Keys.IndexOf(currentKey)

            ' Add the new key in the index of the existing key.
            section.Keys.Insert(currentKeyIndex, newKey)

            ' The old key is now one position higher.
            section.Keys.RemoveAt(currentKeyIndex + 1)

            Return True
        End If

    End Function

    ''' <summary>
    ''' Sets the comments of the section with the specified <paramref name="name"/>.
    ''' </summary>
    ''' <param name="name">Name of the target section.</param>
    ''' <param name="comments">Comments to set.</param>
    ''' <param name="order">Order to set.</param>
    ''' <param name="keys">Collection of full keys to set.</param>
    ''' <param name="createIfNotExist">When True, if the target section with the specified <paramref name="name"/> does not exist, it is created.
    ''' When False, if the section does not exist, no action is taken.</param>
    ''' <returns>Whether the specified section was updated or created.
    ''' False should only be returned in calls where <paramref name="createIfNotExist"/> is set to False.</returns>
    ''' <remarks>In the event of multiple sections with the same <paramref name="name"/>, only the first occurrance is updated.</remarks>
    Public Function SetSectionData(name As String,
                                   Optional order As Integer = Integer.MaxValue,
                                   Optional comments As String = Nothing,
                                   Optional keys As List(Of Key) = Nothing,
                                   Optional createIfNotExist As Boolean = True) As Boolean

        If Not SectionNameExists(name) Then

            ' Check if the section should be created.
            If Not createIfNotExist Then
                Return False

            Else
                ' Create the section.
                _sections.Add(MakeSection(name,
                                          comments:=If(comments IsNot Nothing, comments, ""),
                                          order:=order,
                                          keys:=keys))

                Return True
            End If

        Else
            Dim currentSection As Section = GetSection(name)

            Dim newSection As Section = MakeSection(
                name,
                comments:=If(comments IsNot Nothing, comments, currentSection.Comments),
                order:=If(order = Integer.MaxValue, currentSection.Order, order),
                keys:=If(keys IsNot Nothing, keys, currentSection.Keys))


            Dim currentSectionIndex As Integer = _sections.IndexOf(currentSection)

            ' Add the new section in the index of the existing key.
            _sections.Insert(currentSectionIndex, newSection)

            ' The old section is now one position higher.
            _sections.RemoveAt(currentSectionIndex + 1)

            Return True
        End If

    End Function

#End Region


#Region " Shared "

    ''' <summary>
    ''' Creates a new object from the provided text <paramref name="contents"/>.
    ''' </summary>
    ''' <param name="contents">Ini text.</param>
    ''' <param name="fileName">Destination file for the new object. If this file exists, it will be overwritten.
    ''' If a value is not provided the returned object will not contain a value in <see cref="FileName"/> property (it can be set if needed, however).</param>
    ''' <returns>New object with the <paramref name="contents"/> assigned.</returns>
    ''' <remarks></remarks>
    Public Shared Function CreateFromString(contents As String, Optional fileName As String = "") As IniFile

        Dim isTempFile As Boolean = False

        If fileName.Trim = "" Then
            ' Use a file in the temp directory.
            fileName = FileSystem.GetTempFileName
            isTempFile = True
        End If

        ' Write the contents to the file.
        FileSystem.WriteAllText(fileName, contents, False)

        ' Return the new object.
        Dim ini As New IniFile(fileName)

        ' Remove temp files and reset the file name of the current object.
        If isTempFile AndAlso FileSystem.FileExists(fileName) Then
            FileSystem.DeleteFile(fileName)
            ini.FileName = ""
        End If

        Return ini

    End Function

    ''' <summary>
    ''' Gets the value of <paramref name="keyName"/> in the specified <paramref name="section"/>.
    ''' </summary>
    ''' <param name="section">Section to search.</param>
    ''' <param name="keyName">Name of the target key.</param>
    ''' <param name="defaultValue">The value returned in the event the key does not exist.</param>
    ''' <returns>Value of the first occurance of the <paramref name="keyName"/>s within <paramref name="section"/>.</returns>
    ''' <remarks>In the event of multiple <paramref name="keyName"/>s within <paramref name="section"/>, only the value of the first occurrance is returned.</remarks>
    Public Shared Function GetKeyValue(section As Section, keyName As String,
                                       Optional defaultValue As String = "") As String

        Dim key As Key = section.Keys.Find(
            Function(searchKey) searchKey.Name = keyName)

        Return If(KeyHasData(key), key.Value, defaultValue)

    End Function


    ''' <summary>
    ''' Gets the comment output for writing to a file.
    ''' </summary>
    ''' <param name="comment">The comment for any component.</param>
    ''' <returns></returns>
    Public Shared Function GetCommentOutput(comment As String) As String

        Dim commentPrefix As String = IniFile.COMMENT & " "
        comment = comment.Trim

        ' If there is a comment value supplied, append the token to be beginning of each line and return.
        ' For empty/whitespace only comments, return an empty string.
        Return If(comment = "", "",
                  commentPrefix & comment.Replace(ControlChars.NewLine, ControlChars.NewLine & commentPrefix))

    End Function

    ''' <summary>
    ''' Gets the <see cref="Key"/> output which is written to the ini file.
    ''' </summary>
    ''' <param name="key">The key to get output for.</param>
    ''' <returns>Format:
    ''' {Comment}
    ''' {Name}={Value}
    ''' </returns>
    Public Shared Function GetKeyOutput(key As Key) As String

        ' For readability, add line breaks before any keys with comments.
        Dim commentsSpacer As String = If(key.Comments.Trim = "", "",
                                          Space(1).Replace(" ", ControlChars.NewLine))

        If KeyHasData(key) Then
            Return commentsSpacer &
                String.Format("{3}{0}{1}={2}",
                              ControlChars.NewLine,
                              key.Name, key.Value, GetCommentOutput(key.Comments)).Trim

            ' "Orphaned" key condition.
        ElseIf key.Comments.Trim <> "" Then
            Return commentsSpacer & GetCommentOutput(key.Comments).Trim

            ' Empty key.
        Else
            Return ""
        End If

    End Function


    ''' <summary>
    ''' Gets the <see cref="Section"/> output which is written to the ini file.
    ''' </summary>
    ''' <param name="section">The section to get output for.</param>
    ''' <param name="preserveOrders">Whether or not the ordering of the keys should be honored.
    ''' For large sections, this may incur a peformance hit.</param>
    ''' <returns>Format:
    ''' {Comment}
    ''' [{Section}]
    ''' {Key1}
    ''' {...}
    ''' {KeyN}
    ''' {carriage return}
    ''' {carriage return}
    ''' </returns>
    ''' <remarks>Using LINQ would make the key sorting easier.</remarks>
    Public Shared Function GetSectionOutput(section As Section,
                                            Optional preserveOrders As Boolean = False) As String

        If Not preserveOrders Then
            ' Order is not a consideration, just dump the data.

            ' Section header.
            Dim output As New System.Text.StringBuilder(
                String.Format("{2}{0}[{1}]",
                              ControlChars.NewLine,
                              section.Name, GetCommentOutput(section.Comments)).Trim)
            output.AppendLine("")

            For Each nameKey As Key In section.Keys
                ' Each key on its own line.
                output.AppendLine(GetKeyOutput(nameKey))
            Next

            ' Add blank lines to the end.
            output.Append(String.Format("{0}{0}", ControlChars.NewLine))

            Return output.ToString


        Else
            ' Order matters, sort the keys before outputing.
            Dim sortedKeys As New List(Of Key)

            For Each nameKey As Key In section.Keys
                Dim order As Integer = nameKey.Order

                ' When there is nothing yet added or the order is the max integer value, append to the end.
                If sortedKeys.Count = 0 OrElse
                    order = Integer.MaxValue Then

                    sortedKeys.Add(nameKey)

                Else
                    ' Explicit order is provided.
                    ' Locate where it should go with respect to other keys.
                    Dim insertBefore As Key = sortedKeys.Find(
                        Function(sorted) sorted.Order > order)

                    ' Check that the key found contains data.
                    If Not KeyHasData(insertBefore) Then
                        ' No lower order found, add to the end.
                        sortedKeys.Add(nameKey)

                    Else
                        ' The current key will be inserted at the index where key with the
                        ' higher order value was found.
                        Dim insertAtIndex As Integer = sortedKeys.IndexOf(insertBefore)

                        sortedKeys.Insert(insertAtIndex, nameKey)
                    End If

                End If
            Next


            ' Wrap the keys in a new section so a recursive call can produce the output.
            Dim sectionOutput As Section = MakeSection(section.Name, section.Comments,
                                                       keys:=sortedKeys)

            Return GetSectionOutput(sectionOutput)

        End If

    End Function

    ''' <summary>
    ''' Creates a new unattached <see cref="IniFile.Section"/>.
    ''' </summary>
    ''' <param name="name">Name of the section.</param>
    ''' <param name="comments">Comments for the section.</param>
    ''' <param name="order">Sequential placement with respect to existing sections.</param>
    ''' <param name="keys">Keys for the section.</param>
    ''' <returns>New section with the specified properties assigned.</returns>
    ''' <remarks></remarks>
    Public Shared Function MakeSection(name As String,
                                       Optional comments As String = "",
                                       Optional order As Integer = Integer.MaxValue,
                                       Optional keys As List(Of Key) = Nothing) As Section

        If keys Is Nothing Then
            keys = New List(Of Key)
        End If

        Return New Section With {.Name = name,
                                 .Comments = comments,
                                 .Order = order,
                                 .Keys = keys}
    End Function

    ''' <summary>
    ''' Creates a new unattached <see cref="IniFile.Key"/>.
    ''' </summary>
    ''' <param name="name">Name of the key.</param>
    ''' <param name="value">Value assigned to the key.</param>
    ''' <param name="comments">Comments for the key.</param>
    ''' <param name="order">Sequential placement with respect to existing keys.</param>
    ''' <returns>New key with the specified properties assigned.</returns>
    ''' <remarks></remarks>
    Public Shared Function MakeKey(name As String, value As String,
                                   Optional comments As String = "",
                                   Optional order As Integer = Integer.MaxValue) As Key

        Return New Key With {.Name = name,
                             .Value = value,
                             .Comments = comments,
                             .Order = order}
    End Function


    ''' <summary>
    ''' Returns whether or not the supplied <paramref name="key"/> contains data.
    ''' </summary>
    ''' <param name="key">Key to evaluate.</param>
    ''' <returns>True when the <paramref name="key"/> has a value for the <see cref="IniFile.Key.Name"/> property, otherwise False.</returns>
    Public Shared Function KeyHasData(key As Key) As Boolean

        Return Not String.IsNullOrEmpty(key.Name)

    End Function

    ''' <summary>
    ''' Returns whether or not the supplied <paramref name="section"/> contains data.
    ''' </summary>
    ''' <param name="section">Key to evaluate.</param>
    ''' <returns>True when the <paramref name="section"/> has a value for the <see cref="IniFile.Section.Name"/> property, otherwise False.</returns>
    Public Shared Function SectionHasData(section As Section) As Boolean

        Return Not String.IsNullOrEmpty(section.Name)

    End Function

#End Region


End Class