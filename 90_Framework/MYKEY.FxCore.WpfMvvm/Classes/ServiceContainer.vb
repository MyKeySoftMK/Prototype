Imports System
Imports System.Collections.Generic

Public Class ServiceContainer

    Public Shared ReadOnly Instance As New ServiceContainer()

    Private Sub New()
        _serviceMap = New Dictionary(Of Type, Object)()
        _serviceMapLock = New Object()
    End Sub

    Public Sub AddService(Of TServiceContract As Class)(implementation As TServiceContract)
        SyncLock _serviceMapLock
            _serviceMap(GetType(TServiceContract)) = implementation
        End SyncLock
    End Sub

    Public Function GetService(Of TServiceContract As Class)() As TServiceContract
        Dim service As Object = Nothing
        SyncLock _serviceMapLock
            _serviceMap.TryGetValue(GetType(TServiceContract), service)
        End SyncLock
        Return TryCast(service, TServiceContract)
    End Function

    ReadOnly _serviceMap As Dictionary(Of Type, Object)
    ReadOnly _serviceMapLock As Object

End Class
