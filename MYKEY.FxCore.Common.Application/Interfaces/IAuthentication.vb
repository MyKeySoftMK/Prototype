''' <summary>
''' Schnittstelle für Authentifzierungs-Anbieter.
''' </summary>
Public Interface IAuthenticationProvider

    '#####################################################
    ' Source: http://zyan.codeplex.com/SourceControl/latest#source/Zyan.Communication/Security/IAuthenticationProvider.cs
    '#####################################################


    ''' <summary>
    ''' Authentifiziert einen bestimmten Benutzer anhand seiner Anmeldeinformationen.
    ''' </summary>
    ''' <param name="authRequest">Authentifizierungs-Anfragenachricht mit Anmeldeinformationen</param>
    ''' <returns>Antwortnachricht des Authentifizierungssystems</returns>
    Function Authenticate(authRequest As AuthRequestMessage) As AuthResponseMessage


End Interface