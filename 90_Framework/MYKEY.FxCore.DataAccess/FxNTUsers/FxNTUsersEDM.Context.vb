﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Der Code wurde von einer Vorlage generiert.
'
'     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
'     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Namespace FxNTUsers

    Partial Public Class Entities
        Inherits DbContext
    
        Public Sub New()
            MyBase.New("name=Entities")
        End Sub
    
    	Public Sub New(SqlConnectionSting As String)
            MyBase.New(SqlConnectionSting)
    	End Sub
    
        Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
            Throw New UnintentionalCodeFirstException()
        End Sub
    
        Public Overridable Property UserGroupRoles() As DbSet(Of UserGroupRole)
        Public Overridable Property UserGroups() As DbSet(Of UserGroup)
        Public Overridable Property UserGroupToUserGroupRoles() As DbSet(Of UserGroupToUserGroupRole)
        Public Overridable Property Users() As DbSet(Of User)
        Public Overridable Property UserToUserGroups() As DbSet(Of UserToUserGroup)
    
    End Class

End Namespace
