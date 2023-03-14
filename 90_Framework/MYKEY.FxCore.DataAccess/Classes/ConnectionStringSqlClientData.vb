Imports System.Data.SqlClient
Imports System.Data.Entity.Core.EntityClient

Public Class ConnectionStringSqlClientData

    ' Initialize the EntityConnectionStringBuilder.
    Private entityBuilder As New EntityConnectionStringBuilder

    ' Initialize the connection string builder for the
    ' underlying provider.
    Private sqlBuilder As New SqlConnectionStringBuilder

    Public ReadOnly Property ProviderName As String
        Get
            Return "System.Data.SqlClient"
            'Return "System.Data.EntityClient"
        End Get
    End Property

    Public Property ServerName As String = "."

    Public Property DatabaseName = ""

    Public Property DatabaseUserName As String = ""

    Public Property DatabasePassword As String = ""

    Public Property DatabaseModelName As String = ""

    Public Property DatabaseModelAssembly As String = "*"

    Public Property DatabaseServerPort As Integer = 1435

    Public ReadOnly Property SqlConnectionString As String
        Get
            With sqlBuilder

                .DataSource = ServerName & "," & DatabaseServerPort.ToString
                .InitialCatalog = DatabaseName

                If DatabaseUserName.Length = 0 Then
                    .IntegratedSecurity = True
                Else
                    .IntegratedSecurity = False
                    .UserID = DatabaseUserName
                    .Password = DatabasePassword
                End If

                sqlBuilder.MultipleActiveResultSets = True

            End With

            Return sqlBuilder.ToString & ";App=EntityFramework"
        End Get
    End Property

    Public ReadOnly Property EntityConnectionString As String
        Get
            Dim _metaData As String = ""

            With entityBuilder
                .Provider = ProviderName
                .ProviderConnectionString = SqlConnectionString

                _metaData = "res://" & DatabaseModelAssembly & "/" & DatabaseModelName & ".csdl|"
                _metaData = _metaData & "res://" & DatabaseModelAssembly & "/" & DatabaseModelName & ".ssdl|"
                _metaData = _metaData & "res://" & DatabaseModelAssembly & "/" & DatabaseModelName & ".msl"

                .Metadata = _metaData
            End With

            Return entityBuilder.ToString

        End Get
    End Property

    'Public Function ConnectionTest() As Boolean
    '    Try

    '        Dim conn As New EntityConnection(EntityConnectionString)
    '        conn.Open()
    '        Console.WriteLine("Just testing the connection.")
    '        conn.Close()

    '        Return True

    '    Catch ex As Exception

    '        Return False

    '    End Try

    'End Function


End Class
