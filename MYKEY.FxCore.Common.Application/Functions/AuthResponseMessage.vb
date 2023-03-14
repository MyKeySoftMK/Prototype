
Imports System.Security.Principal
Imports System.Security

'#####################################################
' Source: http://zyan.codeplex.com/SourceControl/latest#source/Zyan.Communication/Security/AuthResponseMessage.cs
'#####################################################

''' <summary>
''' Reply message of the authentication system.
''' </summary>
<Serializable> _
Public Class AuthResponseMessage

    ''' <summary>
    ''' Gets or sets value indicating whether the authentication procedure completed successfully.
    ''' </summary>
    Public Property Success() As Boolean
        Get
            Return m_Success
        End Get
        Set(value As Boolean)
            m_Success = value
        End Set
    End Property
    Private m_Success As Boolean

    ''' <summary>
    ''' Gets or sets error message.
    ''' </summary>
    Public Property ErrorMessage() As String
        Get
            Return m_ErrorMessage
        End Get
        Set(value As String)
            m_ErrorMessage = value
        End Set
    End Property
    Private m_ErrorMessage As String

    ''' <summary>
    ''' Gets or sets authenticated user's identity object.
    ''' </summary>
    Public Property AuthenticatedIdentity() As IIdentity
        Get
            Return m_AuthenticatedIdentity
        End Get
        Set(value As IIdentity)
            m_AuthenticatedIdentity = value
        End Set
    End Property
    Private m_AuthenticatedIdentity As IIdentity

    ''' <summary>
    ''' Gets or sets security exception thrown on authentication failure.
    ''' </summary>
    Public Property Exception() As SecurityException
        Get
            Return m_Exception
        End Get
        Set(value As SecurityException)
            m_Exception = value
        End Set
    End Property
    Private m_Exception As SecurityException

End Class



