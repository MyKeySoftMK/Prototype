''' <summary>
''' Zum Kennzeichnen einer Klasse als Konfigurationsobjekt
''' </summary>
''' <remarks></remarks>
Public Interface IApplicationSettings

    ''' <summary>
    ''' Initialisieren und Bereitstellen von Anwendungseinstellungen in einer XML-Datei
    ''' </summary>
    ''' <param name="ConfigNameInit">Name der Konfiguration</param>
    ''' <param name="ConfigPathInit">Verzeichnispfad, in der sich die </param>
    ''' <remarks>Es wird nicht der Konfigurationsdateiname angegeben, da autm. an den Namen ein .fxcfg
    ''' angehängt wird</remarks>
    Sub Initialize(ConfigNameInit As String, ConfigPathInit As String)


    ''' <summary>
    ''' Speichern und Freigeben der Einstellungen
    ''' </summary>
    ''' <remarks>Da die einzelnen Informationen (ConfigName/ConfigFilePath) beim Implementieren in lokalen
    ''' Eigenschaften hinterlegt werden, kann man durch Aufrufen von dieser Methode die Informationen
    ''' schreiben und die Resourcen wieder freigeben.</remarks>
    Sub DeInitialize()

End Interface