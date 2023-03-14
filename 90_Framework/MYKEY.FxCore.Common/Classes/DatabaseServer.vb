''' <summary>
''' Diese Klasse beschreibt die Datenbank
''' </summary>
''' <remarks></remarks>
Public Class DatabaseServer

    ''' <summary>
    ''' Name oder IP des Datenbankservers
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Name As String

    ''' <summary>
    ''' Datenbankname
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Database As String

    ''' <summary>
    ''' Benutzername um sich mit der Datenbank zu verbinden
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property User As String

    ''' <summary>
    ''' Gültiges Kennwort des Datenbankbenutzers
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Password As String

    ''' <summary>
    ''' Mit welchem Datensatz begonnen werden soll
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StartID As Integer

    ''' <summary>
    ''' Definiert die Wartezeit bei Zugriffen, bis es zu einer Fehlermeldung kommt
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TimeOutTime As Integer = 360

    ''' <summary>
    ''' Standard-Kommunikationsport bei der Verwendung der Infrastruktur-Installation
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port As Integer = 1435

End Class
