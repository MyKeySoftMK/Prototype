Public Class ChecksFolder

    ''' <summary>
    ''' Name des Verzeichnisses
    ''' </summary>
    ''' <returns></returns>
    Public Property FolderName As String

    ''' <summary>
    ''' Ergebis der Vorhandensein-Überprüfung
    ''' </summary>
    ''' <returns></returns>
    Public Property Folder_Exists_Result As Boolean

    ''' <summary>
    ''' Wichtigkeit/Dringlichkeit des vorhandenseins
    ''' </summary>
    ''' <returns></returns>
    Public Property Folder_Exists_Severity As Boolean

    ''' <summary>
    ''' Ob eine Prüfung auf Schreibbar erfolgen soll, wenn das Verzeichnis besteht
    ''' </summary>
    ''' <returns></returns>
    Public Property Folder_Writeable_Check As Boolean

    ''' <summary>
    ''' Ergebniss der Überprüfung auf Schreibbar
    ''' </summary>
    ''' <returns></returns>
    Public Property Folder_Writeable_Result As Boolean

    ''' <summary>
    ''' Wichtigkeit/Dringlichkeit der Schreibbarkeit in dem Verzeichnis
    ''' </summary>
    ''' <returns></returns>
    Public Property Folder_Writeable_Severity As Boolean

End Class
