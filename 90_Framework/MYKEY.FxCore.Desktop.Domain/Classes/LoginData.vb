Imports System.ComponentModel
Imports MYKEY.FxCore.Common

''' <summary>
''' Diese Klasse beinhaltet die Eigenschaften für das Anmelden an einen Server
''' </summary>
''' <remarks></remarks>
Public Class LoginData

    Inherits ObservableObject
    Implements IDataErrorInfo

    Private ClientStartupSettingsXML As New ClientStartupSettings


    ''' <summary>
    ''' Der Benutzername, der zur Anmeldung verwendet werden soll
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Username As String
        Set(value As String)
            If value <> ClientStartupSettingsXML.LastUsername Then
                ClientStartupSettingsXML.LastUsername = value
                Me.OnPropertyChanged("Username")
            End If
        End Set
        Get
            Return ClientStartupSettingsXML.LastUsername
        End Get
    End Property


    ''' <summary>
    ''' Das Kennwort, was für den Anmeldeprozess verwendet werden soll
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Password As String
        Set(value As String)
            If value <> _Password Then
                _Password = value
                Me.OnPropertyChanged("Password")
            End If
        End Set
        Get
            Return _Password
        End Get
    End Property
    Private _Password As String = ""

    ''' <summary>
    ''' Mit welchem Server man sich verbinden möchte
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HostInstance As String
        Set(value As String)
            If value <> ClientStartupSettingsXML.LastHostInstance Then
                ClientStartupSettingsXML.LastHostInstance = value
                Me.OnPropertyChanged("HostInstance")
            End If
        End Set
        Get
            Return ClientStartupSettingsXML.LastHostInstance
        End Get
    End Property

    Public ReadOnly Property [Error] As String Implements System.ComponentModel.IDataErrorInfo.Error
        Get
            ' Wenn kein Fehler aufgetreten ist
            Return Nothing

        End Get
    End Property

    Default Public ReadOnly Property Item(columnName As String) As String Implements System.ComponentModel.IDataErrorInfo.Item
        Get
            Select Case columnName

                Case "Username"
                    If Me.Username.Length = 0 Then
                        Return "Benutzername darf nicht leer sein"
                    End If

                Case "Password"

                    If Me.Password.Length = 0 Then
                        Return "Kennwort darf nicht leer sein"
                    End If

                Case "HostInstance"
                    If Me.HostInstance.Length = 0 Then
                        Return "Serverbezeichnung darf nicht leer sein"
                    End If
            End Select

            ' Wenn kein Fehler aufgetreten ist
            Return Nothing

        End Get
    End Property
End Class
