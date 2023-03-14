' ###################################################################
' #  T4-Name	: GenerateCoreModel.tt                              #
' #  Date		: 2021-11-03                                        #
' #  Version	: 1                           (c) MyKey-Soft 2015   #
' ###################################################################
Imports System.Collections.ObjectModel
Imports System.ComponentModel

Imports MYKEY.FxCore
Imports MYKEY.FxCore.Common.Application
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.DataAccess
Imports MYKEY.FxCore.Desktop.Domain

Imports MYKEY.FxCore.Common
Imports System.Security.Principal
Imports MYKEY.FxCore.DataAccess.FxNTUsers
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Class UserGroupManagement

    Inherits ClassBase
    Implements IUserGroupManagement
	Implements IUserGroupManagement_ModelEntryPoints
    Implements IDisposable

#Region "Constructor"

    Public Sub New()

        Me.EntityConnectionData = CreateServerConn(EntityModelDomain)

        Initialize()

    End Sub

    ''' <summary>
    ''' </summary>
    ''' <param name="EntityConnection"></param>
    ''' <remarks>Dieser Konstruktur wird bei den Testklassen verwendet</remarks>
    Public Sub New(EntityConnection As ConnectionStringSqlClientData)

        Me.EntityConnectionData = EntityConnection
        Me.EntityConnectionData.DatabaseModelName = Me.EntityModelDomain
        Me.EntityConnectionData.DatabaseModelAssembly = Me.EntityModelAssembly

        Initialize()

    End Sub

    Public Function EntityModelDomain() As String
        Return "FxNTUsers.FxNTUsersEDM"
    End Function

    Public Function EntityModelAssembly() As String
        Return "MYKEY.FxCore.DataAccess"
    End Function

    Private Sub Initialize()

        Try

            ' Einstellungen lesen
            NLOGLOGGER.Debug("=> Initialize Component 'UserGroupManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'UserGroupManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'UserGroupManagement' cannot initialized. Check the database connection first.")

        End Try
    End Sub

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then

                If _DbCtx IsNot Nothing Then
                    _DbCtx.Dispose()
                End If

            End If

        End If
        Me.disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(ByVal disposing As Boolean) oben über Code zum Freigeben von nicht verwalteten Ressourcen verfügt.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(ByVal disposing As Boolean) Bereinigungscode ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(ByVal disposing As Boolean) Bereinigungscode ein.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

#Region "Properties"

    Private _EntityConnectionString As String

	 ' Eine lokale Variabel für das lokale Auslesen von Einstellungen
    Private ClientServerSettingsXML As New ServerApplicationSettings

    <ThreadStatic()>
    Private _DbCtx As FxNTUsers.Entities
    Private ReadOnly Property DbCtx As FxNTUsers.Entities
        Get
            Try

                If _DbCtx Is Nothing Then
                    NLOGLOGGER.Debug("Try to create a new DbContext-Object")

                    ' Erzeugen eines DataContext 
                    _DbCtx = New FxNTUsers.Entities(EntityConnectionData.EntityConnectionString)

                    ' Aktivierung des EF6-Loggings
                    If ClientServerSettingsXML.EF6Logging = True Then
                        _DbCtx.Database.Log = Sub(s) NLOGLOGGER.Debug(s)
                    End If

                    _DbCtx.Database.SqlQuery(Of Object)("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;").FirstOrDefault()

                    NLOGLOGGER.Debug("=> successfull")
                End If

            Catch ex As Exception

                Dim _exception As Exception = ex
                While _exception.InnerException IsNot Nothing
                    _exception = _exception.InnerException
                End While

                NLOGLOGGER.Fatal(ex.Message)

            End Try

            Return _DbCtx
        End Get
    End Property

    ''' <summary>
    ''' Diese Eigenschaft speichert einmalig die Werte für den Include bei Eager Loading
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Wenn der Wer</remarks>
    Private Property Includes As List(Of String)


	    ''' <summary>
    ''' Damit kann geprüft werden, ob Änderungen vorgenommen wurden, die noch nicht gespeichert sind
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Die Aussage bezieht sich auf den gesamten Kontext. Möchte man daher Datensatzweise speichern, ist es 
    ''' notwendig, dass vor jedem Wechsel des Datensatzes ein Verwerfen oder ein Speichern staffinden muss</remarks>
    Public ReadOnly Property HasChanges As Boolean
        Get
            Return _DbCtx.ChangeTracker.HasChanges
        End Get
    End Property

#End Region

#Region "Queries"

#Region "DefaultQueries"

    Private ReadOnly Property DefaultQueryUserGroups As DbQuery(Of UserGroup)
        Get

            Try

                Dim defaultQuery As DbQuery(Of UserGroup) = DbCtx.UserGroups
	            Dim _defaultQueryResult As DbQuery(Of UserGroup)


                ' Nachladen der Includes für Entitäten-Childs
                If Includes IsNot Nothing Then
                    For Each prop As String In Includes
                        defaultQuery = defaultQuery.Include(prop)
                        NLOGLOGGER.Debug("=> Include Entities-Childs for: " & prop)
                    Next
                    Includes = Nothing
                End If

                ' Anpassen der Standardabfrage durch den Benutzer
                _defaultQueryResult = EntryPoint_DefineDefaultQuery(defaultQuery)
                If _defaultQueryResult IsNot Nothing Then
                    defaultQuery = _defaultQueryResult
                End If

                Return defaultQuery

            Catch ex As Exception
                NLOGLOGGER.Fatal(ex.Message)
                Return Nothing
            End Try

        End Get
    End Property


#End Region


    ''' <summary>
    ''' Gibt eine unsortierte Liste der Benutzergruppen aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserGroups(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of UserGroup) Implements IUserGroupManagement.GetUserGroups
        Dim serverUserGroups As DbQuery(Of UserGroup)

        NLOGLOGGER.Info("Get List of UserGroups ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverUserGroups = DefaultQueryUserGroups.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverUserGroups = DefaultQueryUserGroups
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverUserGroups.Count & " UserGroup/UserGroups in database")

        Return New ObservableCollection(Of UserGroup)(serverUserGroups)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Benutzergruppen aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserGroups(UserGroupGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of UserGroup) Implements IUserGroupManagement.GetUserGroups
        Dim serverUserGroups As DbQuery(Of UserGroup)

        NLOGLOGGER.Info("Get List of UserGroups ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverUserGroups = DefaultQueryUserGroups.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverUserGroups = DefaultQueryUserGroups.Where(Function(_UserGroup) UserGroupGuids.Contains(_UserGroup.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverUserGroups.Count & " UserGroup/UserGroups in database")

        Return New ObservableCollection(Of UserGroup)(serverUserGroups)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Benutzergruppe zurück
    ''' </summary>
    ''' <param name="UserGroupGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetUserGroup(UserGroupGUID As String, Optional QParam As QueryParameters = Nothing) As UserGroup Implements IUserGroupManagement.GetUserGroup
        Dim serverUserGroup As UserGroup = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get UserGroup Informations ")

            guid = guidConvert.ConvertFromString(UserGroupGUID)
            NLOGLOGGER.Debug("=> Search for UserGroup with GUID '" & UserGroupGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverUserGroup = DefaultQueryUserGroups.Single(Function(UserGroups) UserGroups.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found UserGroup '" & serverUserGroup.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverUserGroup
    End Function

#Region "AllocationListBox: FxNTUsersUserToUserGroup"

    ''' <summary>
    ''' Gibt die Informationen für einen UserToUserGroups zurück
    ''' </summary>
    ''' <param name="FxNTUsersUserGroupGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetFxNTUsersUserGroup(FxNTUsersUserGroupGUID As String, Optional QParam As QueryParameters = Nothing) As FxNTUsers.UserGroup
        Dim serverFxNTUsersUserGroup As List (Of FxNTUsers.UserGroup) 
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid

        Try
            NLOGLOGGER.Info("Get FxNTUsers.UserGroup Informations ")

            _guid = guidConvert.ConvertFromString(FxNTUsersUserGroupGUID)
            NLOGLOGGER.Debug("=> Search for FxNTUsers.UserGroup with GUID '" & FxNTUsersUserGroupGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverFxNTUsersUserGroup = (From FxNTUsersUserGroup In DbCtx.UserGroups
                                    Where FxNTUsersUserGroup.GUID = _guid
                                    Select FxNTUsersUserGroup).ToList
            End With

            NLOGLOGGER.Debug("=> Found FxNTUsers.UserGroup '" & serverFxNTUsersUserGroup(0).GUID.ToString & "'")
        Catch ex As Exception
			serverFxNTUsersUserGroup = New List (Of FxNTUsers.UserGroup)
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverFxNTUsersUserGroup(0)
    End Function

    ''' <summary>
    ''' Gibt die Informationen für alle Benutzergruppen zurück
    ''' </summary>
    ''' <param name="GUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetFxNTUsersUserGroupAllUserMembers(GUID As String, Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTUsers.UserToUserGroup)
        Dim serverUsers As List(Of FxNTUsers.UserToUserGroup) 
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid

        Try
            NLOGLOGGER.Info("Get FxNTUsers.UserGroup Informations")

            _guid = guidConvert.ConvertFromString(GUID)
            NLOGLOGGER.Debug("=> Search for FxNTUsers.UserGroup with GUID '" & GUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverUsers = (From FxNTUsersUserGroup In DbCtx.UserGroups
                                    From User In FxNTUsersUserGroup.Users
                                    Where FxNTUsersUserGroup.GUID = _guid
                                    Select User).ToList

            End With

			NLOGLOGGER.Debug("=> Found " & serverUsers.Count & " Users in database")

        Catch ex As Exception
			serverUsers = New List(Of FxNTUsers.UserToUserGroup)
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return New ObservableCollection(Of FxNTUsers.UserToUserGroup)(serverUsers)
    End Function

#End Region

#Region "AllocationListBox: FxNTUsersUserGroupToUserGroupRole"


    ''' <summary>
    ''' Gibt die Informationen für alle Benutzergruppen zurück
    ''' </summary>
    ''' <param name="GUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetFxNTUsersUserGroupAllUserGroupRoleMembers(GUID As String, Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTUsers.UserGroupToUserGroupRole)
        Dim serverUserGroupRoles As List(Of FxNTUsers.UserGroupToUserGroupRole) 
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid

        Try
            NLOGLOGGER.Info("Get FxNTUsers.UserGroup Informations")

            _guid = guidConvert.ConvertFromString(GUID)
            NLOGLOGGER.Debug("=> Search for FxNTUsers.UserGroup with GUID '" & GUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverUserGroupRoles = (From FxNTUsersUserGroup In DbCtx.UserGroups
                                    From UserGroupRole In FxNTUsersUserGroup.UserGroupRoles
                                    Where FxNTUsersUserGroup.GUID = _guid
                                    Select UserGroupRole).ToList

            End With

			NLOGLOGGER.Debug("=> Found " & serverUserGroupRoles.Count & " UserGroupRoles in database")

        Catch ex As Exception
			serverUserGroupRoles = New List(Of FxNTUsers.UserGroupToUserGroupRole)
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return New ObservableCollection(Of FxNTUsers.UserGroupToUserGroupRole)(serverUserGroupRoles)
    End Function

#End Region

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Benutzergruppe
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewUserGroup() As ServerResult Implements IUserGroupManagement.CreateNewUserGroup
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewUserGroup As UserGroup

        Try
            NLOGLOGGER.Info("New UserGroup will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewUserGroup = EntryPoint_GenerateValidUserGroupEntity()

			' Anpassen der Historieninformationen
			With _NewUserGroup
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.UserGroups.Add(_NewUserGroup)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New UserGroup is successfull create in database")
            _result.ReturnValue = _NewUserGroup.GUID.ToString
            NLOGLOGGER.Info("=> UserGroupGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewUserGroup")
			_resultAfterAdd = EntryPoint_AfterAddNewUserGroup(_NewUserGroup)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewUserGroup")

        Catch ex As Entity.Validation.DbEntityValidationException

            ' Retrieve the error messages as a list of strings.
            Dim errorMessages = ex.EntityValidationErrors.SelectMany(Function(x) x.ValidationErrors).[Select](Function(x) x.ErrorMessage)

            ' Join the list to a single string.
            Dim fullErrorMessage = String.Join("; ", errorMessages)

            ' Combine the original exception message with the new one.
            Dim exceptionMessage = String.Concat(ex.Message, " Die Validierung ist fehlgeschlagen: ", fullErrorMessage)

            _result.ErrorMessages.Add(exceptionMessage)

        Catch ex As Exception

            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException

                NLOGLOGGER.Fatal(_exception.Message)
                _result.ErrorMessages.Add(_exception.Message)
            End While

        End Try

        Return _result
    End Function


    ''' <summary>
    ''' Erstellt einen neuen Benutzergruppe aus einer Benutzergruppe-Entität
    ''' </summary>
    ''' <param name="UserGroupEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Benutzergruppe-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewUserGroup(UserGroupEntity As UserGroup) As ServerResult Implements IUserGroupManagement.CreateNewUserGroup
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim UserGroupEntryPointResult as UserGroup

        Try
            NLOGLOGGER.Info("New UserGroup will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			UserGroupEntryPointResult = EntryPoint_BeforeAddNewUserGroup(UserGroupEntity)
			If UserGroupEntryPointResult IsNot Nothing then
				UserGroupEntity = UserGroupEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = CheckAddNewUserGroup(UserGroupEntity)
			If canAddNew = True Then
			canAddNew = EntryPoint_CheckAddNewUserGroup(DbCtx, UserGroupEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If

			End If

            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With UserGroupEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.UserGroups.Add(UserGroupEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New UserGroup is successfull create in database")
                _result.ReturnValue = UserGroupEntity.GUID.ToString
                NLOGLOGGER.Info("=> UserGroupGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewUserGroup")
				_resultAfterAddNew = EntryPoint_AfterAddNewUserGroup(UserGroupEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewUserGroup")

            Else
                NLOGLOGGER.Error("UserGroup cannot added.")
                _result.ErrorMessages.Add("The check of uniquenes is failed. Maybe a Entity with same data is exists")
            End If


        Catch ex As Entity.Validation.DbEntityValidationException

            ' Retrieve the error messages as a list of strings.
            Dim errorMessages = ex.EntityValidationErrors.SelectMany(Function(x) x.ValidationErrors).[Select](Function(x) x.ErrorMessage)

            ' Join the list to a single string.
            Dim fullErrorMessage = String.Join("; ", errorMessages)

            ' Combine the original exception message with the new one.
            Dim exceptionMessage = String.Concat(ex.Message, " Die Validierung ist fehlgeschlagen: ", fullErrorMessage)

            _result.ErrorMessages.Add(exceptionMessage)

        Catch ex As Exception

            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException

                NLOGLOGGER.Fatal(_exception.Message)
                _result.ErrorMessages.Add(_exception.Message)
            End While

        End Try

        Return _result

    End Function

	''' <summary>
    ''' Kopieren einer vorhanden Benutzergruppe mit allen Eigenschaften
    ''' </summary>
    ''' <param name="UserGroupEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyUserGroup(UserGroupEntity As UserGroup) As ServerResult Implements IUserGroupManagement.CopyUserGroup

        Dim _copyUserGroup As New UserGroup
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.UserGroups.Add(_copyUserGroup)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(UserGroupEntity).CurrentValues
			DbCtx.Entry(_copyUserGroup).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyUserGroup = EntryPoint_CopyUserGroup(UserGroupEntity)

			' Anpassen der Historieninformationen
			With _copyUserGroup
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

			' Insert clone with changes into database
			DbCtx.SaveChanges()

        Catch ex As Entity.Validation.DbEntityValidationException

            ' Retrieve the error messages as a list of strings.
            Dim errorMessages = ex.EntityValidationErrors.SelectMany(Function(x) x.ValidationErrors).[Select](Function(x) x.ErrorMessage)

            ' Join the list to a single string.
            Dim fullErrorMessage = String.Join("; ", errorMessages)

            ' Combine the original exception message with the new one.
            Dim exceptionMessage = String.Concat(ex.Message, " Die Validierung ist fehlgeschlagen: ", fullErrorMessage)

            _result.ErrorMessages.Add(exceptionMessage)

        Catch ex As Exception

            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)
            _result.ErrorMessages.Add(_exception.Message)

        End Try

		Return _result

    End Function

    ''' <summary>
    ''' Die Methode übeträgt die Veränderungen eines Benutzes an die Datenbank
    ''' </summary>
    ''' <param name="UserGroupEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyUserGroup(UserGroupEntity As UserGroup) As ServerResult Implements IUserGroupManagement.ModifyUserGroup
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim UserGroupEntryPointResult as UserGroup

        Dim serverUserGroup As UserGroup

        Try
            NLOGLOGGER.Info("UserGroup will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverUserGroup = GetUserGroup(UserGroupEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyUserGroup")
			_resultBeforeModify = EntryPoint_BeforeModifyUserGroup(serverUserGroup)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyUserGroup")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyUserGroup")
            UserGroupEntryPointResult = EntryPoint_ModifyEntityBeforeModifyUserGroup(UserGroupEntity)
			If UserGroupEntryPointResult IsNot Nothing then
				UserGroupEntity = UserGroupEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyUserGroup")

			' Anpassen der Historieninformationen
			With UserGroupEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverUserGroup).CurrentValues.SetValues(UserGroupEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyUserGroup")
			_resultAfterModify = EntryPoint_AfterModifyUserGroup(serverUserGroup)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyUserGroup")

            NLOGLOGGER.Debug("UserGroup is successfull modiefied in database")

        Catch ex As Entity.Validation.DbEntityValidationException

            ' Retrieve the error messages as a list of strings.
            Dim errorMessages = ex.EntityValidationErrors.SelectMany(Function(x) x.ValidationErrors).[Select](Function(x) x.ErrorMessage)

            ' Join the list to a single string.
            Dim fullErrorMessage = String.Join("; ", errorMessages)

            ' Combine the original exception message with the new one.
            Dim exceptionMessage = String.Concat(ex.Message, " Die Validierung ist fehlgeschlagen: ", fullErrorMessage)

            _result.ErrorMessages.Add(exceptionMessage)

        Catch ex As Exception

            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)
            _result.ErrorMessages.Add(_exception.Message)

        End Try

        Return _result

    End Function

	''' <summary>
    ''' Diese Methode löscht einen Benutzergruppe 
    ''' </summary>
    ''' <param name="UserGroupGUID">Die UserGroupGUID des Benutzergruppe, der gelöscht werden soll</param>
    ''' <returns>Wurde der Benutzergruppe gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteUserGroup(UserGroupGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IUserGroupManagement.DeleteUserGroup

        Dim serverUserGroups As List(Of UserGroup)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete UserGroup")

            NLOGLOGGER.Debug("=> Try to find UserGroup-id '" & UserGroupGUID & "'")
            guid = guidConvert.ConvertFromString(UserGroupGUID)
            serverUserGroups = (From UserGroup In DbCtx.UserGroups Where UserGroup.GUID = guid).ToList

            If serverUserGroups.Count = 1 Then
                NLOGLOGGER.Debug("=> UserGroup-Id '" & UserGroupGUID & "' found")
                If serverUserGroups(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.UserGroups.Remove(serverUserGroups(0))
                    Else
						' Anpassen der Historieninformationen
						With serverUserGroups(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteUserGroup")
					_resultAfterDelete = EntryPoint_AfterDeleteUserGroup(serverUserGroups(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteUserGroup")
			
                    NLOGLOGGER.Info("=> UserGroup-Id '" & UserGroupGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> UserGroup-Id '" & UserGroupGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> UserGroup-Id '" & UserGroupGUID & "' not in database")
            End If

        Catch ex As Exception
            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)

            _result.ErrorMessages.Add(_exception.Message)
        End Try

        Return _result

    End Function

	''' <summary>
    ''' Zum vereinfachten löschen von mehreren Benutzergruppen
    ''' </summary>
    ''' <param name="UserGroups"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteUserGroups(UserGroups As UserGroup(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IUserGroupManagement.DeleteUserGroups
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & UserGroups.Count & " UserGroups Collection")

            For Each _UserGroup As UserGroup In UserGroups
                _resultAfterDelete = DeleteUserGroup(_UserGroup.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete UserGroups Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete UserGroups Collection with errors")
            End If


        Catch ex As Exception
            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)

            _result.ErrorMessages.Add(_exception.Message)
        End Try

        Return _result
    End Function

	''' <summary>
    ''' Setzt alle Einträge die verändert wurden wieder in den ursprünglichen Zustand zurück
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RejectChanges()
        For Each entry As Object In _DbCtx.ChangeTracker.Entries()
            Select Case entry.State
                Case EntityState.Modified
                    If True Then
                        entry.CurrentValues.SetValues(entry.OriginalValues)
                        entry.State = EntityState.Unchanged
                        Exit Select
                    End If
                Case EntityState.Deleted
                    If True Then
                        entry.State = EntityState.Unchanged
                        Exit Select
                    End If
                Case EntityState.Added
                    If True Then
                        entry.State = EntityState.Detached
                        Exit Select
                    End If
            End Select
        Next
    End Sub

	''' <summary>
    ''' Prüft ob der Eintrag hinzugefüt werden kann
    ''' </summary>
    ''' <param name="UserGroupEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckAddNewUserGroup(UserGroupEntity As UserGroup) As Boolean

        Dim isUniqueUserGroupName As Boolean

        ' Überprüfen auf Eindeutigkeit, da EF sowas nicht anbietet
        ' Any() stops at the first match and doesn't have to enumerate the entire sequence
        isUniqueUserGroupName = Not DbCtx.UserGroups.Any(Function(u) u.Name = UserGroupEntity.Name)

        Return isUniqueUserGroupName

    End Function
#End Region

#Region "AllocationListBoxes"

    ''' <summary>
    ''' Mit dieser Methode fügt man einen Benutzer einer Benutzergruppe hinzu.
    ''' </summary>
    ''' <param name="UserGUID">User-GUID die hinzugefügt werden soll</param>
    ''' <param name="FxNTUsersUserGroupGUID">Benutzergruppe-GUID der Gruppe die den Benutzer enthalten soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddUserToFxNTUsersUserGroup(UserGUID As String, FxNTUsersUserGroupGUID As String) As ServerResult

        Dim newUserToFxNTUsersUserGroup As FxNTUsers.UserToUserGroup
        Dim _result As New ServerResult
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid
		Dim qpara As New QueryParameters

        Try
            NLOGLOGGER.Info("User will assign to a FxNTUsersUserGroup")

            newUserToFxNTUsersUserGroup = New FxNTUsers.UserToUserGroup
            With newUserToFxNTUsersUserGroup
                ' Benötigte Informationen
                _guid = guidConvert.ConvertFromString(UserGUID)
                .UserGUID = _guid
                NLOGLOGGER.Debug("=> UserGUID: " & UserGUID)

                _guid = guidConvert.ConvertFromString(FxNTUsersUserGroupGUID)
                .UserGroupGUID = _guid
                NLOGLOGGER.Debug("=> UserGroupGUID: " & FxNTUsersUserGroupGUID)
            End With

            DbCtx.UserToUserGroups.Add(newUserToFxNTUsersUserGroup)
            DbCtx.SaveChanges()

			' Frisches Einlesen der Entität
			qpara.Includes.Add("Users")
			TryCast(DbCtx, IObjectContextAdapter).ObjectContext.Detach(GetFxNTUsersUserGroup(FxNTUsersUserGroupGUID, qpara))
            Dim srvFxNTUsersUserGroup As FxNTUsers.UserGroup = DbCtx.UserGroups.Find(newUserToFxNTUsersUserGroup.UserGroupGUID)


            NLOGLOGGER.Info("User is successfull assign to FxNTUsers.UserGroup")
            _result.ReturnValue = newUserToFxNTUsersUserGroup.GUID.ToString
            NLOGLOGGER.Info("=> EntryGUID: " & _result.ReturnValue)

        Catch ex As Entity.Validation.DbEntityValidationException

            ' Retrieve the error messages as a list of strings.
            Dim errorMessages = ex.EntityValidationErrors.SelectMany(Function(x) x.ValidationErrors).[Select](Function(x) x.ErrorMessage)

            ' Join the list to a single string.
            Dim fullErrorMessage = String.Join("; ", errorMessages)

            ' Combine the original exception message with the new one.
            Dim exceptionMessage = String.Concat(ex.Message, " Die Validierung ist fehlgeschlagen: ", fullErrorMessage)

            _result.ErrorMessages.Add(exceptionMessage)

        Catch ex As Exception
            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)

            _result.ErrorMessages.Add(_exception.Message)
        End Try

        Return _result
		
    End Function

    ''' <summary>
    ''' Mit dieser Methode entfernt man einen Benutzer aus einer Benutzergruppe .
    ''' </summary>
    ''' <param name="UserGUID">User-GUID die hinzugefügt werden soll</param>
    ''' <param name="FxNTUsersUserGroupGUID">Benutzergruppe-GUID der Gruppe die den Benutzer enthalten soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Public Function RemoveUserFromFxNTUsersUserGroup(UserGUID As String, FxNTUsersUserGroupGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

        Dim serverUserGroups As List(Of FxNTUsers.UserGroup)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
        Dim UserFound As Boolean = False
		Dim qpara As New QueryParameters

        Try
            NLOGLOGGER.Info("Remove User From FxNTUsers.UserGroup")

            NLOGLOGGER.Debug("=> Try to find FxNTUsersUserGroup-id '" & FxNTUsersUserGroupGUID & "'")
            guid = guidConvert.ConvertFromString(FxNTUsersUserGroupGUID)
            serverUserGroups = (From FxNTUsersUserGroup In DbCtx.UserGroups.Include("Users") Where FxNTUsersUserGroup.GUID = guid).ToList

            If serverUserGroups.Count = 1 Then
                NLOGLOGGER.Debug("=> FxNTUsersUserGroup-Id '" & FxNTUsersUserGroupGUID & "' found")

                NLOGLOGGER.Debug("=> Search for User-id '" & UserGUID & "'")
                For Each entry As FxNTUsers.UserToUserGroup In serverUserGroups(0).Users

                    If entry.UserGUID.ToString = UserGUID Then

                        UserFound = True

                        If entry.CanNotDelete = False Then

                            If PermanentlyDelete = True Then
                                DbCtx.Entry(entry).State = Entity.EntityState.Deleted
                            Else
                                entry.Deleted = Now
                            End If
                            DbCtx.SaveChanges()

							' Frisches Einlesen der Entität
							qpara.Includes.Add("Users")
							TryCast(DbCtx, IObjectContextAdapter).ObjectContext.Detach(GetFxNTUsersUserGroup(FxNTUsersUserGroupGUID, qpara))
							Dim srvFxNTUsersUserGroup As FxNTUsers.UserGroup = DbCtx.UserGroups.Find(serverUserGroups(0).Guid)

                            NLOGLOGGER.Info("=> User-Id '" & FxNTUsersUserGroupGUID & "' REMOVED")

                        Else
                            NLOGLOGGER.Debug("=> User-Id '" & FxNTUsersUserGroupGUID & "' FAILED to delete")
                            _result.ErrorMessages.Add("User  cannot delete - Delete not allowed")
                        End If
                        Exit For
                    End If
                Next
                If UserFound = False Then
                    NLOGLOGGER.Debug("=> User-Id '" & FxNTUsersUserGroupGUID & "' not in UserGroup")
                    _result.ErrorMessages.Add("User cannot delete - User not found in Group")
                End If

            Else
                NLOGLOGGER.Debug("=> FxNTUsers.UserGroup-Id '" & FxNTUsersUserGroupGUID & "' not in database")
                _result.ErrorMessages.Add("User cannot delete from FxNTUsers.UserGroup - FxNTUsers.UserGroup not found in database")
            End If

        Catch ex As Exception
            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)

            _result.ErrorMessages.Add(_exception.Message)
        End Try

        Return _result

    End Function
 
 
    ''' <summary>
    ''' Mit dieser Methode fügt man einen Benutzergruppenrolle einer Benutzergruppe hinzu.
    ''' </summary>
    ''' <param name="UserGroupRoleGUID">UserGroupRole-GUID die hinzugefügt werden soll</param>
    ''' <param name="FxNTUsersUserGroupGUID">Benutzergruppe-GUID der Gruppe die den Benutzer enthalten soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddUserGroupRoleToFxNTUsersUserGroup(UserGroupRoleGUID As String, FxNTUsersUserGroupGUID As String) As ServerResult

        Dim newUserGroupRoleToFxNTUsersUserGroup As FxNTUsers.UserGroupToUserGroupRole
        Dim _result As New ServerResult
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid
		Dim qpara As New QueryParameters

        Try
            NLOGLOGGER.Info("UserGroupRole will assign to a FxNTUsersUserGroup")

            newUserGroupRoleToFxNTUsersUserGroup = New FxNTUsers.UserGroupToUserGroupRole
            With newUserGroupRoleToFxNTUsersUserGroup
                ' Benötigte Informationen
                _guid = guidConvert.ConvertFromString(UserGroupRoleGUID)
                .RoleGUID = _guid
                NLOGLOGGER.Debug("=> RoleGUID: " & UserGroupRoleGUID)

                _guid = guidConvert.ConvertFromString(FxNTUsersUserGroupGUID)
                .UserGroupGUID = _guid
                NLOGLOGGER.Debug("=> UserGroupGUID: " & FxNTUsersUserGroupGUID)
            End With

            DbCtx.UserGroupToUserGroupRoles.Add(newUserGroupRoleToFxNTUsersUserGroup)
            DbCtx.SaveChanges()

			' Frisches Einlesen der Entität
			qpara.Includes.Add("UserGroupRoles")
			TryCast(DbCtx, IObjectContextAdapter).ObjectContext.Detach(GetFxNTUsersUserGroup(FxNTUsersUserGroupGUID, qpara))
            Dim srvFxNTUsersUserGroup As FxNTUsers.UserGroup = DbCtx.UserGroups.Find(newUserGroupRoleToFxNTUsersUserGroup.UserGroupGUID)


            NLOGLOGGER.Info("UserGroupRole is successfull assign to FxNTUsers.UserGroup")
            _result.ReturnValue = newUserGroupRoleToFxNTUsersUserGroup.GUID.ToString
            NLOGLOGGER.Info("=> EntryGUID: " & _result.ReturnValue)

        Catch ex As Entity.Validation.DbEntityValidationException

            ' Retrieve the error messages as a list of strings.
            Dim errorMessages = ex.EntityValidationErrors.SelectMany(Function(x) x.ValidationErrors).[Select](Function(x) x.ErrorMessage)

            ' Join the list to a single string.
            Dim fullErrorMessage = String.Join("; ", errorMessages)

            ' Combine the original exception message with the new one.
            Dim exceptionMessage = String.Concat(ex.Message, " Die Validierung ist fehlgeschlagen: ", fullErrorMessage)

            _result.ErrorMessages.Add(exceptionMessage)

        Catch ex As Exception
            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)

            _result.ErrorMessages.Add(_exception.Message)
        End Try

        Return _result
		
    End Function

    ''' <summary>
    ''' Mit dieser Methode entfernt man einen Benutzergruppenrolle aus einer Benutzergruppe .
    ''' </summary>
    ''' <param name="UserGroupRoleGUID">UserGroupRole-GUID die hinzugefügt werden soll</param>
    ''' <param name="FxNTUsersUserGroupGUID">Benutzergruppe-GUID der Gruppe die den Benutzer enthalten soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Public Function RemoveUserGroupRoleFromFxNTUsersUserGroup(UserGroupRoleGUID As String, FxNTUsersUserGroupGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

        Dim serverUserGroups As List(Of FxNTUsers.UserGroup)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
        Dim UserGroupRoleFound As Boolean = False
		Dim qpara As New QueryParameters

        Try
            NLOGLOGGER.Info("Remove UserGroupRole From FxNTUsers.UserGroup")

            NLOGLOGGER.Debug("=> Try to find FxNTUsersUserGroup-id '" & FxNTUsersUserGroupGUID & "'")
            guid = guidConvert.ConvertFromString(FxNTUsersUserGroupGUID)
            serverUserGroups = (From FxNTUsersUserGroup In DbCtx.UserGroups.Include("UserGroupRoles") Where FxNTUsersUserGroup.GUID = guid).ToList

            If serverUserGroups.Count = 1 Then
                NLOGLOGGER.Debug("=> FxNTUsersUserGroup-Id '" & FxNTUsersUserGroupGUID & "' found")

                NLOGLOGGER.Debug("=> Search for UserGroupRole-id '" & UserGroupRoleGUID & "'")
                For Each entry As FxNTUsers.UserGroupToUserGroupRole In serverUserGroups(0).UserGroupRoles

                    If entry.RoleGUID.ToString = UserGroupRoleGUID Then

                        UserGroupRoleFound = True

                        If entry.CanNotDelete = False Then

                            If PermanentlyDelete = True Then
                                DbCtx.Entry(entry).State = Entity.EntityState.Deleted
                            Else
                                entry.Deleted = Now
                            End If
                            DbCtx.SaveChanges()

							' Frisches Einlesen der Entität
							qpara.Includes.Add("UserGroupRoles")
							TryCast(DbCtx, IObjectContextAdapter).ObjectContext.Detach(GetFxNTUsersUserGroup(FxNTUsersUserGroupGUID, qpara))
							Dim srvFxNTUsersUserGroup As FxNTUsers.UserGroup = DbCtx.UserGroups.Find(serverUserGroups(0).Guid)

                            NLOGLOGGER.Info("=> UserGroupRole-Id '" & FxNTUsersUserGroupGUID & "' REMOVED")

                        Else
                            NLOGLOGGER.Debug("=> UserGroupRole-Id '" & FxNTUsersUserGroupGUID & "' FAILED to delete")
                            _result.ErrorMessages.Add("UserGroupRole  cannot delete - Delete not allowed")
                        End If
                        Exit For
                    End If
                Next
                If UserGroupRoleFound = False Then
                    NLOGLOGGER.Debug("=> UserGroupRole-Id '" & FxNTUsersUserGroupGUID & "' not in UserGroup")
                    _result.ErrorMessages.Add("UserGroupRole cannot delete - UserGroupRole not found in Group")
                End If

            Else
                NLOGLOGGER.Debug("=> FxNTUsers.UserGroup-Id '" & FxNTUsersUserGroupGUID & "' not in database")
                _result.ErrorMessages.Add("UserGroupRole cannot delete from FxNTUsers.UserGroup - FxNTUsers.UserGroup not found in database")
            End If

        Catch ex As Exception
            Dim _exception As Exception = ex
            While _exception.InnerException IsNot Nothing
                _exception = _exception.InnerException
            End While

            NLOGLOGGER.Fatal(_exception.Message)

            _result.ErrorMessages.Add(_exception.Message)
        End Try

        Return _result

    End Function
 
  #End Region
#End Region

End Class

