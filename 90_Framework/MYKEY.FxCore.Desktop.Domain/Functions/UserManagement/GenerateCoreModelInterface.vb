' ###################################################################
' #  T4-Name	: GenerateCOreModelInterface.tt                     #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports MYKEY.FxCore.Common
Imports System.Collections.ObjectModel
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.DataAccess.FxNTUsers
Imports MYKEY.FxCore.Common.Application

Public Interface IUserManagement

#Region "Functions"

    Function CreateNewUser() As ServerResult

    Function CreateNewUser(UserEntity As User) As ServerResult

	Function CopyUser(UserEntity As User) As ServerResult

    Function ModifyUser(UserEntity As User) As ServerResult

    Function DeleteUser(UserGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

	Function DeleteUsers(Users As User(), Optional PermanentlyDelete As Boolean = True) As ServerResult

#End Region

#Region "Queries"

    Function GetUsers(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of User)

	Function GetUsers(UserGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of User)

    Function GetUser(UserGUID As String, Optional QParam As QueryParameters = Nothing) As User

#End Region

End Interface
