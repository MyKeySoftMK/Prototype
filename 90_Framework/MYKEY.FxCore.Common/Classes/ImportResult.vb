Public Class ImportResult

    Public Sub New()
        m_Added = New List(Of String)
        m_Errors = New List(Of String)
        m_Deleted = New List(Of String)
        m_Modified = New List(Of String)
        m_Exists = New List(Of String)
    End Sub

    ''' <summary>
    ''' Informationen zu neu / hinzugefügten Einträgen
    ''' </summary>
    ''' <returns></returns>
    Public Property Added As List(Of String)
        Get
            Return m_Added
        End Get
        Set(value As List(Of String))
            m_Added = value
        End Set
    End Property
    Private m_Added As List(Of String)

    ''' <summary>
    ''' Informationen zu fehlerhaften Einträgen
    ''' </summary>
    ''' <returns></returns>
    Public Property Errors As List(Of String)
        Get
            Return m_Errors
        End Get
        Set(value As List(Of String))
            m_Errors = value
        End Set
    End Property
    Private m_Errors As List(Of String)

    ''' <summary>
    ''' Informationen zu geänderte Einträgen
    ''' </summary>
    ''' <returns></returns>
    Public Property Modified As List(Of String)
        Get
            Return m_Modified
        End Get
        Set(value As List(Of String))
            m_Modified = value
        End Set
    End Property
    Private m_Modified As List(Of String)

    ''' <summary>
    ''' Informationen zu gelöschten Einträgen
    ''' </summary>
    ''' <returns></returns>
    Public Property Deleted As List(Of String)
        Get
            Return m_Deleted
        End Get
        Set(value As List(Of String))
            m_Deleted = value
        End Set
    End Property
    Private m_Deleted As List(Of String)


    ''' <summary>
    ''' Informationen zu bestehenden Einträgen
    ''' </summary>
    ''' <returns></returns>
    Public Property Exists As List(Of String)
        Get
            Return m_Exists
        End Get
        Set(value As List(Of String))
            m_Exists = value
        End Set
    End Property
    Private m_Exists As List(Of String)

    ''' <summary>
    ''' Gibt die Anzahl aller verarbeiteten Einträge zurück
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ResultSum As Integer
        Get
            Return m_Added.Count + m_Deleted.Count + m_Errors.Count + m_Modified.Count + m_Exists.Count
        End Get

    End Property

End Class
