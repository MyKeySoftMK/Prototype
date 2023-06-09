﻿Option Strict On
Option Explicit On

Imports System.IO
Imports System.Collections.Specialized

Public Class FileSearch

    Private Const DefaultFileMask As String = "*.*"
    Private Const DefaultDirectoryMask As String = "*"

#Region " Member Variables "

    Private _InitialDirectory As DirectoryInfo
    Private _DirectoryMasks As StringCollection
    Private _FileMasks As StringCollection

    '
    Private _Directories As New ArrayList
    Private _Files As New ArrayList

    Private _Recursive As Boolean = True

#End Region

#Region " Properites "

    Public Property InitialDirectory() As DirectoryInfo
        Get
            Return _InitialDirectory
        End Get
        Set(ByVal Value As DirectoryInfo)
            _InitialDirectory = Value
        End Set
    End Property
    Public Property DirectoryMask() As StringCollection
        Get
            Return _DirectoryMasks
        End Get
        Set(ByVal Value As StringCollection)
            _DirectoryMasks = Value
        End Set
    End Property
    Public Property FileMask() As StringCollection
        Get
            Return _FileMasks
        End Get
        Set(ByVal Value As StringCollection)
            _FileMasks = Value
        End Set
    End Property

    Public ReadOnly Property Directories() As ArrayList
        Get
            Return _Directories
        End Get
    End Property
    Public ReadOnly Property Files() As ArrayList
        Get
            Return _Files
        End Get
    End Property

    Public Property RecursiveSearch As Boolean
        Get
            Return _Recursive
        End Get
        Set(value As Boolean)
            _Recursive = value
        End Set
    End Property
#End Region

#Region " Constructors "

    Public Sub New()

    End Sub

    Public Sub New( _
        ByVal BaseDirectory As String)
        Me.New(New DirectoryInfo(BaseDirectory))
    End Sub

    Public Sub New( _
        ByVal InitialDirectory As DirectoryInfo)

        _InitialDirectory = InitialDirectory
    End Sub

#End Region

    Public Overloads Sub Search(ByVal InitalDirectory As String,
        Optional ByVal FileMask As String = Nothing,
        Optional ByVal DirectoryMask As String = Nothing,
        Optional ByVal RecursiveSearch As Boolean = True)

        Search(
            New DirectoryInfo(InitalDirectory),
            FileMask,
            DirectoryMask,
            RecursiveSearch
            )

    End Sub




    Public Overloads Sub Search(
        Optional ByVal InitalDirectory As DirectoryInfo = Nothing,
        Optional ByVal FileMask As String = Nothing,
        Optional ByVal DirectoryMask As String = Nothing,
        Optional ByVal RecursiveSearch As Boolean = True)

        _Recursive = RecursiveSearch
        _Files = New ArrayList
        _Directories = New ArrayList

        If Not IsNothing(InitalDirectory) Then
            _InitialDirectory = InitalDirectory
        End If

        If IsNothing(_InitialDirectory) Then
            Throw New ArgumentException("A Directory Must be specified!", "Directory")
        End If

        If IsNothing(FileMask) OrElse FileMask.Length = 0 Then
            _FileMasks = New StringCollection
            _FileMasks.Add(DefaultFileMask)
        Else
            _FileMasks = ParseMask(FileMask)
        End If

        If IsNothing(DirectoryMask) OrElse DirectoryMask.Length > 0 Then
            _DirectoryMasks = New StringCollection
            _DirectoryMasks.Add(DefaultDirectoryMask)
        Else
            _DirectoryMasks = ParseMask(DirectoryMask)
        End If

        DoSearch(_InitialDirectory)
    End Sub

    Private Sub DoSearch(ByVal BaseDirectory As DirectoryInfo)

        Try
            For Each fm As String In _FileMasks
                Files.AddRange(BaseDirectory.GetFiles(fm))
            Next
            _
        Catch u As UnauthorizedAccessException
            'Siliently Ignore this error, there isnt any simple
            'way to avoid this error.

        End Try

        If _Recursive = True Then

            Try
                Dim Directories As New ArrayList

                For Each dm As String In _DirectoryMasks
                    Directories.AddRange(BaseDirectory.GetDirectories(dm))
                    _Directories.AddRange(Directories)
                Next

                For Each di As DirectoryInfo In Directories
                    DoSearch(di)
                Next


            Catch u As UnauthorizedAccessException
                'Siliently Ignore this error, there isnt any simple
                'way to avoid this error.

            End Try

        End If

    End Sub

    'Masks are formated like *.jpeg;*.jpg
    Private Shared Function ParseMask(ByVal Mask As String) As StringCollection
        If IsNothing(Mask) Then
            Return Nothing
        End If
        Mask = Mask.Trim(";"c)
        If Mask.Length = 0 Then
            Return Nothing
        End If
        Dim Masks As New StringCollection

        Masks.AddRange(Mask.Split(";"c))

        Return Masks
    End Function


    Protected Overrides Sub Finalize()
        _Files = Nothing
        _Directories = Nothing
        MyBase.Finalize()
    End Sub

End Class