' ###################################################################
' #  T4-Name	: GenerateEDMExtensions.tt                          #
' #  Date		: 2015-12-11                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports System
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Namespace FxNTCheckLists

    Partial Public Class Entities
        Inherits DbContext

        Public Sub New(SqlConnectionSting As String)
            MyBase.New(SqlConnectionSting)
        End Sub
		
    End Class

End Namespace

