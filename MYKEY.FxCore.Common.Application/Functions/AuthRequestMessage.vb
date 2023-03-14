Imports System.Collections

'#####################################################
' Source: http://zyan.codeplex.com/SourceControl/latest#source/Zyan.Communication/Security/AuthRequestMessage.cs
'#####################################################

''' <summary>
''' Authentication request message.
''' </summary>
<Serializable> _
Public Class AuthRequestMessage

    ''' <summary>
    ''' User name constant.
    ''' </summary>
    Public Const CREDENTIAL_USERNAME As String = "username"

    ''' <summary>
    ''' Password constant.
    ''' </summary>
    Public Const CREDENTIAL_PASSWORD As String = "password"

    ''' <summary>
    ''' Domain constant.
    ''' </summary>
    Public Const CREDENTIAL_DOMAIN As String = "domain"

    ''' <summary>
    ''' Security token name constant.
    ''' </summary>
    Public Const CREDENTIAL_WINDOWS_SECURITY_TOKEN As String = "windowssecuritytoken"

    ''' <summary>
    ''' Gets or sets user's credentials.
    ''' </summary>
    Public Property Credentials() As Hashtable
        Get
            Return m_Credentials
        End Get
        Set(value As Hashtable)
            m_Credentials = value
        End Set
    End Property
    Private m_Credentials As Hashtable

    ''' <summary>
    ''' Gets or sets the IP Address of the calling client.
    ''' </summary>
    Public Property ClientAddress() As String
        Get
            Return m_ClientAddress
        End Get
        Set(value As String)
            m_ClientAddress = value
        End Set
    End Property
    Private m_ClientAddress As String

End Class


