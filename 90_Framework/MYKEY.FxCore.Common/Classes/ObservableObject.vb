Imports System.ComponentModel

''' <summary> 
''' This is the abstract base class for any object that provides property change notifications.   
''' </summary> 
<Serializable()> _
Public Class ObservableObject

    Implements INotifyPropertyChanged

#Region "OnPropertyChanged"

    ''' <summary> 
    ''' Raises this object's PropertyChanged event. 
    ''' </summary> 
    ''' <param name="propertyName">The property that has a new value.</param> 
    Protected Sub OnPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#End Region

#Region "Debugging Aides"

    ''' <summary> 
    ''' Warns the developer if this object does not have 
    ''' a public property with the specified name. This  
    ''' method does not exist in a Release build. 
    ''' </summary> 
    <Conditional("DEBUG")> _
    <DebuggerStepThrough()> _
    Public Sub VerifyPropertyName(ByVal propertyName As String)

        ' If you raise PropertyChanged and do not specify a property name, 
        ' all properties on the object are considered to be changed by the binding system. 
        If [String].IsNullOrEmpty(propertyName) Then
            Return
        End If

        ' Verify that the property name matches a real,   
        ' public, instance property on this object. 
        If TypeDescriptor.GetProperties(Me)(propertyName) Is Nothing Then
            Dim msg As String = "Invalid property name: " & propertyName

            If Me.ThrowOnInvalidPropertyName Then
                Throw New ArgumentException(msg)
            Else
                Debug.Fail(msg)
            End If
        End If
    End Sub

    ''' <summary> 
    ''' Returns whether an exception is thrown, or if a Debug.Fail() is used 
    ''' when an invalid property name is passed to the VerifyPropertyName method. 
    ''' The default value is false, but subclasses used by unit tests might  
    ''' override this property's getter to return true. 
    ''' </summary> 
    Protected Property ThrowOnInvalidPropertyName() As Boolean
        Get
            Return m_ThrowOnInvalidPropertyName
        End Get
        Private Set(ByVal value As Boolean)
            m_ThrowOnInvalidPropertyName = value
        End Set
    End Property
    Private m_ThrowOnInvalidPropertyName As Boolean

#End Region

#Region "INotifyPropertyChanged Members"

    ''' <summary> 
    ''' Raised when a property on this object has a new value. 
    ''' </summary> 
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

#End Region

End Class
