Imports System.DirectoryServices.AccountManagement
Imports System.DirectoryServices
Imports MYKEY.FxCore.Common.ApplicationLogging

Public Class ADFunctions

    ReadOnly Property ADRoot As DirectoryEntry
    Property SearchResult As DirectoryEntry

    Public Function Connect(Host As ADHost) As ServerResult
        Dim _result As New ServerResult

        Try

            _ADRoot = New DirectoryEntry
            With _ADRoot
                .Path = "LDAP://" & Host.Host & ":" & Host.HostPort & "/" & Host.AdDirectory
                .RefreshCache()
                If Host.UserName IsNot Nothing Then

                    If Host.UserName.Length > 0 Then
                        .AuthenticationType = AuthenticationTypes.Secure
                        .Username = Host.UserName
                        .Password = Host.UserNamePassword
                    End If

                End If
            End With

        Catch ex As Exception
            _result.ErrorMessages.Add(ex.Message)
        End Try

        Return _result

    End Function

    Public Function SearchTopCn(SearchRootDN As DirectoryEntry, CNName As String) As ServerResult
        Dim _result As New ServerResult
        Dim _DirectorySearcher As New DirectorySearcher
        Dim _searchResult As SearchResult
        Dim _ResultEntry As DirectoryEntry

        Try

            With _DirectorySearcher
                .SearchRoot = SearchRootDN
                .Filter = "(&(objectClass=top) (cn=" & CNName & "))"

                _searchResult = .FindOne
            End With

            If _searchResult IsNot Nothing Then
                _ResultEntry = New DirectoryEntry(_searchResult.Path)
                _result.ReturnValue = _ResultEntry
            End If

        Catch ex As Exception
            _result.ErrorMessages.Add(ex.Message)
        End Try


        Return _result

    End Function


    Public Function CreateTopContainer(RootDN As DirectoryEntry, CNData As ADEntry) As ServerResult
        Dim _result As New ServerResult
        Dim _newOU As DirectoryEntry
        Dim _RootPath As String
        Dim _RootServer As String

        Try

            _RootPath = RootDN.Path.Substring(RootDN.Path.LastIndexOf("/") + 1)
            _RootServer = RootDN.Path.Substring(0, RootDN.Path.LastIndexOf("/"))

            If DirectoryEntry.Exists(_RootServer & "/CN=" & CNData.Name & "," & _RootPath) = False Then
                _newOU = RootDN.Children.Add("CN=" & CNData.Name, "container")
                With _newOU
                    .Properties("Description").Add(CNData.Description)
                    .CommitChanges()
                End With
            Else
                NLOGLOGGER.Debug("=> CN " & CNData.Name & " exitst")
            End If


        Catch ex As Exception
            _result.ErrorMessages.Add(ex.Message)
        End Try

        Return _result

    End Function
End Class
