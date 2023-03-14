﻿<Serializable> _
Public Class FunctionResult

    ''' <summary>
    ''' Ergebnis des Aufrufs
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReturnValue() As [Object]

    ''' <summary>
    ''' Eine Sammlung von Fehlermeldungen, die während des Aufrufs aufgetreten sind
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ErrorMessages() As List(Of [String])

    Public Sub New()
        Me.New(New List(Of [String])())
    End Sub


    Public Sub New(Messages As List(Of [String]))
        ErrorMessages = Messages
    End Sub

    ''' <summary>
    ''' Hinzufügen einer neuer Fehlermeldung in die Auflistung
    ''' </summary>
    ''' <param name="errorMessage"></param>
    ''' <remarks></remarks>
    Public Sub New(errorMessage As [String])
        Me.New()
        ErrorMessages.Add(errorMessage)
    End Sub

    ''' <summary>
    ''' Abfragen, ob Fehler in dem Funktionsaufruf aufgetreten sind
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property HasErrors() As Boolean
        Get
            Return ErrorMessages.Count > 0
        End Get
    End Property

End Class
