Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Common.Application.DatabaseSettings
Imports MYKEY.FxCore.Common
Imports System.Security.Principal
Imports MYKEY.FxCore.Common.Application
Imports System.Data.Objects
Imports System.Runtime.CompilerServices
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel


Public MustInherit Class ClassBaseMySQl

#Region "Entity Framework"

    Private _EntityConnectionData As ConnectionStringMySqlClientData

    Public Property EntityConnectionData As ConnectionStringMySqlClientData
        Set(value As ConnectionStringMySqlClientData)
            _EntityConnectionData = value
        End Set
        Get
            Return _EntityConnectionData
        End Get
    End Property

    ''' <summary>
    ''' Erstellen einen ConnectionString für die dynamische Benutzung des Entity Frameworks
    ''' </summary>
    ''' <param name="DataBaseModelName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateServerConn(DataBaseModelName As String, Optional DataServer As DatabaseServer = Nothing) As ConnectionStringMySqlClientData

        Dim ServerConn As New ConnectionStringMySqlClientData
        With ServerConn
            .DatabaseModelName = DataBaseModelName

            If DataServer Is Nothing Then
                .DatabaseName = DATABASE_NAME
                .DatabaseUserName = DATABASE_USERNAME
                .DatabasePassword = DATABASE_PASSWORD
                .ServerName = DATABASE_SERVER
                .DatabaseServerPort = DATABASE_PORT
            Else
                .ServerName = DataServer.Name
                .DatabaseName = DataServer.Database
                .DatabaseUserName = DataServer.User
                .DatabasePassword = DataServer.Password
                .DatabaseServerPort = DataServer.Port
            End If
        End With
        Return ServerConn

    End Function

    'Private _EntityConnectionDataMySql As ConnectionStringMySqlClientData

    'Public Property EntityConnectionDataMySql As ConnectionStringMySqlClientData
    '    Set(value As ConnectionStringMySqlClientData)
    '        _EntityConnectionDataMySql = value
    '    End Set
    '    Get
    '        Return _EntityConnectionDataMySql
    '    End Get
    'End Property

    ' ''' <summary>
    ' ''' Erstellen einen ConnectionString für die dynamische Benutzung des Entity Frameworks
    ' ''' </summary>
    ' ''' <param name="DataBaseModelName"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Function CreateServerConnMySql(DataBaseModelName As String, Optional DataServer As DatabaseServer = Nothing) As ConnectionStringMySqlClientData

    '    Dim ServerConn As New ConnectionStringMySqlClientData
    '    With ServerConn
    '        .DatabaseModelName = DataBaseModelName

    '        If DataServer Is Nothing Then
    '            .DatabaseName = DATABASE_NAME
    '            .DatabaseUserName = DATABASE_USERNAME
    '            .DatabasePassword = DATABASE_PASSWORD
    '            .ServerName = DATABASE_SERVER
    '        Else
    '            .ServerName = DataServer.Name
    '            .DatabaseName = DataServer.Database
    '            .DatabaseUserName = DataServer.User
    '            .DatabasePassword = DataServer.Password
    '        End If
    '    End With
    '    Return ServerConn

    'End Function



#End Region



End Class
