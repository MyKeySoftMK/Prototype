Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.ComponentModel.Composition
Imports System.Data.Objects

Imports MYKEY.FxCore
Imports MYKEY.FxCore.Common.Application
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTOlbMenu

Imports MYKEY.FxCore.Common
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess.FxNTUsers
Imports System.Data.Entity.Infrastructure


Partial Class OlbMenuGroupManagement

#Region "Functions"


#End Region

#Region "Queries"

    ' ''' <summary>
    ' ''' Gibt ein Objekt zurück, dass im MUI das Auswahlmenue erzeugt 
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Function GetMuiMenuLinkGroups() As FirstFloor.ModernUI.Presentation.LinkGroupCollection

    '    Dim _olbMenu As System.Collections.ObjectModel.ObservableCollection(Of MenuGroup)

    '    Dim _muiMenu As New FirstFloor.ModernUI.Presentation.LinkGroupCollection
    '    Dim _newMuiGroup As FirstFloor.ModernUI.Presentation.LinkGroup
    '    Dim _newMuiLink As FirstFloor.ModernUI.Presentation.Link


    '    _olbMenu = GetMenuGroups()

    '    For Each mnuGroup As MenuGroup In _olbMenu

    '        _newMuiGroup = New FirstFloor.ModernUI.Presentation.LinkGroup
    '        With _newMuiGroup

    '            ' Mit dieser Eigenschaft kann man die MenuLinkGruppen zusammenfassen, die zur gleichen Zeit angezeigt werden sollen
    '            ' Original: The groupname uniquely identifies a group of link groups to be displayed at a single time
    '            .GroupKey = "mnuMenu"

    '            .DisplayName = mnuGroup.Name

    '        End With

    '        For Each mnuEntry As MenuEntry In mnuGroup.MenuEntries

    '            _newMuiLink = New FirstFloor.ModernUI.Presentation.Link
    '            With _newMuiLink
    '                .DisplayName = mnuEntry.Text
    '                .Source = New Uri("/" & mnuEntry.LoadAssemblyName & ";component/Controls/" & mnuEntry.RunUserControlName & ".xaml", UriKind.Relative)
    '            End With

    '        Next

    '        _muiMenu.Add(_newMuiGroup)

    '    Next

    '    Return _muiMenu

    'End Function

#End Region

#Region "Proxy-Functions"

    Public Function GetUserPrincipal(UserName As String) As GenericPrincipal

        Dim _UserManagement As UserManagement
        Dim serverPrincipal As GenericPrincipal

        Try
            NLOGLOGGER.Info("Execute Proxyfunction 'GetUserPrincipal' ")

            ' Herstellen der Verbindung zur Artikel-Entität
            _UserManagement = New UserManagement(Me.EntityConnectionData)

            ' Lesen des benötigten Artikels
            serverPrincipal = _UserManagement.GetUserPrinicpal(UserName)

            NLOGLOGGER.Info("Execute Proxyfunction 'GetUserPrincipal' is successfull executed")

            Return serverPrincipal

        Catch ex As Exception

            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)
            Return Nothing
        End Try

    End Function

    Public Function GetUserByUserName(UserName As String) As User
        Dim _UserManagement As UserManagement
        Dim serverUser As User

        Try
            NLOGLOGGER.Info("Execute Proxyfunction 'GetUserByUserName' ")

            ' Herstellen der Verbindung zur Artikel-Entität
            _UserManagement = New UserManagement(Me.EntityConnectionData)

            ' Lesen des benötigten Artikels
            serverUser = _UserManagement.GetUserByUserName(UserName)

            NLOGLOGGER.Info("Execute Proxyfunction 'GetUserByUserName' is successfull executed")

            Return serverUser

        Catch ex As Exception

            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)
            Return Nothing
        End Try
    End Function

#End Region

End Class
