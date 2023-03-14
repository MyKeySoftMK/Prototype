Public Class RuntimeChecks

    Public Property FolderChecks As New List(Of ChecksFolder)
    Public Property RuntimeCheckResults As List(Of String)

    Public Sub FolderChecksAdd(FolderName As String,
                               Optional ExistsSeverity As Boolean = False,
                               Optional WriteableCheck As Boolean = False, Optional WriteableSeverity As Boolean = False)

        Dim _FolderCheck As New ChecksFolder

        With _FolderCheck
            .FolderName = FolderName
            .Folder_Exists_Severity = ExistsSeverity
            .Folder_Writeable_Check = WriteableCheck
            .Folder_Writeable_Severity = WriteableSeverity
        End With

        FolderChecksAdd(_FolderCheck)
    End Sub

    Public Sub FolderChecksAdd(FolderCheck As ChecksFolder)

        FolderChecks.Add(FolderCheck)

    End Sub

    Public Sub Check()

        ' Durchlauf der Checks löschen 
        RuntimeCheckResults = New List(Of String)

        ' Ausführen der einzelnen Tests
        FolderChecksRun()

    End Sub

    Private Sub FolderChecksRun()

        For Each check As ChecksFolder In FolderChecks
            With check

                ' Prüfen auf Vorhandensein des Verzeichnisses
                If IO.Directory.Exists(.FolderName) = False Then

                    ' Ergebniss wegschreiben
                    .Folder_Exists_Result = False

                    ' Zusätzlichen Fehlertext erzeugen
                    If .Folder_Exists_Severity = True Then
                        RuntimeCheckResults.Add("Folder not exists: " & .FolderName)
                    End If

                Else

                    .Folder_Exists_Result = True

                End If

                ' Prüfen auf Schreibbarkeit in dem Verzeichnis
                If .Folder_Exists_Result = True Then

                    Try

                        IO.File.Create(.FolderName & "_test.tmp")
                        .Folder_Writeable_Check = True
                        IO.File.Delete(.FolderName & "_test.tmp")

                    Catch ex As Exception

                        .Folder_Writeable_Check = False

                        ' Zusätzlichen Fehlertext erzeugen
                        If .Folder_Writeable_Severity = True Then
                            RuntimeCheckResults.Add("Folder not writeable: " & .FolderName)
                        End If

                    End Try

                Else

                    .Folder_Writeable_Check = False

                    ' Zusätzlichen Fehlertext erzeugen
                    If .Folder_Writeable_Severity = True Then
                        RuntimeCheckResults.Add("Folder not writeable (while not found): " & .FolderName)
                    End If
                End If


            End With
        Next

    End Sub

End Class
