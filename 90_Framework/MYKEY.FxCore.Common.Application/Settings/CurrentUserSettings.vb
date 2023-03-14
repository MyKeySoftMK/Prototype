Imports MYKEY.FxNT.Common.ApplicationLogging

Public Class CurrentUserSettings

    Public Shared Sub InitSettings()

    End Sub


    ''' <summary>
    ''' Benutzername des angemeldeten Benutzers
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property UserName As String = ""

    Public Shared ReadOnly Property UserGUID As String
        Get
            If UserEntity IsNot Nothing Then
                Return UserEntity.GUID.ToString
            Else
                Return Guid.Empty.ToString
            End If

        End Get
    End Property

    ''' <summary>
    ''' Angezeigter Benutzername
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property UserEntity As MYKEY.FxCore.DataAccess.FxNTUsers.User


    Public Shared Property UserPricipal As System.Security.Principal.GenericPrincipal


End Class
