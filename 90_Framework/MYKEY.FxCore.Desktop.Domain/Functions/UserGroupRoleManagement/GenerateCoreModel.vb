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

Partial Class UserGroupRoleManagement

    Inherits ClassBase
    Implements IUserGroupRoleManagement
	Implements IUserGroupRoleManagement_ModelEntryPoints
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
            NLOGLOGGER.Debug("=> Initialize Component 'UserGroupRoleManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'UserGroupRoleManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'UserGroupRoleManagement' cannot initialized. Check the database connection first.")

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

    Private ReadOnly Property DefaultQueryUserGroupRoles As DbQuery(Of UserGroupRole)
        Get

            Try

                Dim defaultQuery As DbQuery(Of UserGroupRole) = DbCtx.UserGroupRoles
	            Dim _defaultQueryResult As DbQuery(Of UserGroupRole)


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
    ''' Gibt eine unsortierte Liste der Benutzerrollen aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserGroupRoles(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of UserGroupRole) Implements IUserGroupRoleManagement.GetUserGroupRoles
        Dim serverUserGroupRoles As DbQuery(Of UserGroupRole)

        NLOGLOGGER.Info("Get List of UserGroupRoles ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverUserGroupRoles = DefaultQueryUserGroupRoles.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverUserGroupRoles = DefaultQueryUserGroupRoles
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverUserGroupRoles.Count & " UserGroupRole/UserGroupRoles in database")

        Return New ObservableCollection(Of UserGroupRole)(serverUserGroupRoles)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Benutzerrollen aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserGroupRoles(UserGroupRoleGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of UserGroupRole) Implements IUserGroupRoleManagement.GetUserGroupRoles
        Dim serverUserGroupRoles As DbQuery(Of UserGroupRole)

        NLOGLOGGER.Info("Get List of UserGroupRoles ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverUserGroupRoles = DefaultQueryUserGroupRoles.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverUserGroupRoles = DefaultQueryUserGroupRoles.Where(Function(_UserGroupRole) UserGroupRoleGuids.Contains(_UserGroupRole.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverUserGroupRoles.Count & " UserGroupRole/UserGroupRoles in database")

        Return New ObservableCollection(Of UserGroupRole)(serverUserGroupRoles)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Benutzerrolle zurück
    ''' </summary>
    ''' <param name="UserGroupRoleGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetUserGroupRole(UserGroupRoleGUID As String, Optional QParam As QueryParameters = Nothing) As UserGroupRole Implements IUserGroupRoleManagement.GetUserGroupRole
        Dim serverUserGroupRole As UserGroupRole = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get UserGroupRole Informations ")

            guid = guidConvert.ConvertFromString(UserGroupRoleGUID)
            NLOGLOGGER.Debug("=> Search for UserGroupRole with GUID '" & UserGroupRoleGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverUserGroupRole = DefaultQueryUserGroupRoles.Single(Function(UserGroupRoles) UserGroupRoles.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found UserGroupRole '" & serverUserGroupRole.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverUserGroupRole
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Benutzerrolle
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewUserGroupRole() As ServerResult Implements IUserGroupRoleManagement.CreateNewUserGroupRole
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewUserGroupRole As UserGroupRole

        Try
            NLOGLOGGER.Info("New UserGroupRole will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewUserGroupRole = EntryPoint_GenerateValidUserGroupRoleEntity()

			' Anpassen der Historieninformationen
			With _NewUserGroupRole
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.UserGroupRoles.Add(_NewUserGroupRole)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New UserGroupRole is successfull create in database")
            _result.ReturnValue = _NewUserGroupRole.GUID.ToString
            NLOGLOGGER.Info("=> UserGroupRoleGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewUserGroupRole")
			_resultAfterAdd = EntryPoint_AfterAddNewUserGroupRole(_NewUserGroupRole)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewUserGroupRole")

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
    ''' Erstellt einen neuen Benutzerrolle aus einer Benutzerrolle-Entität
    ''' </summary>
    ''' <param name="UserGroupRoleEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Benutzerrolle-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewUserGroupRole(UserGroupRoleEntity As UserGroupRole) As ServerResult Implements IUserGroupRoleManagement.CreateNewUserGroupRole
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim UserGroupRoleEntryPointResult as UserGroupRole

        Try
            NLOGLOGGER.Info("New UserGroupRole will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			UserGroupRoleEntryPointResult = EntryPoint_BeforeAddNewUserGroupRole(UserGroupRoleEntity)
			If UserGroupRoleEntryPointResult IsNot Nothing then
				UserGroupRoleEntity = UserGroupRoleEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = CheckAddNewUserGroupRole(UserGroupRoleEntity)
			If canAddNew = True Then
			canAddNew = EntryPoint_CheckAddNewUserGroupRole(DbCtx, UserGroupRoleEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If

			End If

            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With UserGroupRoleEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.UserGroupRoles.Add(UserGroupRoleEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New UserGroupRole is successfull create in database")
                _result.ReturnValue = UserGroupRoleEntity.GUID.ToString
                NLOGLOGGER.Info("=> UserGroupRoleGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewUserGroupRole")
				_resultAfterAddNew = EntryPoint_AfterAddNewUserGroupRole(UserGroupRoleEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewUserGroupRole")

            Else
                NLOGLOGGER.Error("UserGroupRole cannot added.")
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
    ''' Kopieren einer vorhanden Benutzerrolle mit allen Eigenschaften
    ''' </summary>
    ''' <param name="UserGroupRoleEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyUserGroupRole(UserGroupRoleEntity As UserGroupRole) As ServerResult Implements IUserGroupRoleManagement.CopyUserGroupRole

        Dim _copyUserGroupRole As New UserGroupRole
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.UserGroupRoles.Add(_copyUserGroupRole)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(UserGroupRoleEntity).CurrentValues
			DbCtx.Entry(_copyUserGroupRole).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyUserGroupRole = EntryPoint_CopyUserGroupRole(UserGroupRoleEntity)

			' Anpassen der Historieninformationen
			With _copyUserGroupRole
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
    ''' <param name="UserGroupRoleEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyUserGroupRole(UserGroupRoleEntity As UserGroupRole) As ServerResult Implements IUserGroupRoleManagement.ModifyUserGroupRole
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim UserGroupRoleEntryPointResult as UserGroupRole

        Dim serverUserGroupRole As UserGroupRole

        Try
            NLOGLOGGER.Info("UserGroupRole will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverUserGroupRole = GetUserGroupRole(UserGroupRoleEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyUserGroupRole")
			_resultBeforeModify = EntryPoint_BeforeModifyUserGroupRole(serverUserGroupRole)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyUserGroupRole")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyUserGroupRole")
            UserGroupRoleEntryPointResult = EntryPoint_ModifyEntityBeforeModifyUserGroupRole(UserGroupRoleEntity)
			If UserGroupRoleEntryPointResult IsNot Nothing then
				UserGroupRoleEntity = UserGroupRoleEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyUserGroupRole")

			' Anpassen der Historieninformationen
			With UserGroupRoleEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverUserGroupRole).CurrentValues.SetValues(UserGroupRoleEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyUserGroupRole")
			_resultAfterModify = EntryPoint_AfterModifyUserGroupRole(serverUserGroupRole)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyUserGroupRole")

            NLOGLOGGER.Debug("UserGroupRole is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Benutzerrolle 
    ''' </summary>
    ''' <param name="UserGroupRoleGUID">Die UserGroupRoleGUID des Benutzerrolle, der gelöscht werden soll</param>
    ''' <returns>Wurde der Benutzerrolle gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteUserGroupRole(UserGroupRoleGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IUserGroupRoleManagement.DeleteUserGroupRole

        Dim serverUserGroupRoles As List(Of UserGroupRole)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete UserGroupRole")

            NLOGLOGGER.Debug("=> Try to find UserGroupRole-id '" & UserGroupRoleGUID & "'")
            guid = guidConvert.ConvertFromString(UserGroupRoleGUID)
            serverUserGroupRoles = (From UserGroupRole In DbCtx.UserGroupRoles Where UserGroupRole.GUID = guid).ToList

            If serverUserGroupRoles.Count = 1 Then
                NLOGLOGGER.Debug("=> UserGroupRole-Id '" & UserGroupRoleGUID & "' found")
                If serverUserGroupRoles(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.UserGroupRoles.Remove(serverUserGroupRoles(0))
                    Else
						' Anpassen der Historieninformationen
						With serverUserGroupRoles(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteUserGroupRole")
					_resultAfterDelete = EntryPoint_AfterDeleteUserGroupRole(serverUserGroupRoles(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteUserGroupRole")
			
                    NLOGLOGGER.Info("=> UserGroupRole-Id '" & UserGroupRoleGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> UserGroupRole-Id '" & UserGroupRoleGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> UserGroupRole-Id '" & UserGroupRoleGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren Benutzerrollen
    ''' </summary>
    ''' <param name="UserGroupRoles"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteUserGroupRoles(UserGroupRoles As UserGroupRole(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IUserGroupRoleManagement.DeleteUserGroupRoles
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & UserGroupRoles.Count & " UserGroupRoles Collection")

            For Each _UserGroupRole As UserGroupRole In UserGroupRoles
                _resultAfterDelete = DeleteUserGroupRole(_UserGroupRole.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete UserGroupRoles Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete UserGroupRoles Collection with errors")
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
    ''' <param name="UserGroupRoleEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckAddNewUserGroupRole(UserGroupRoleEntity As UserGroupRole) As Boolean

        Dim isUniqueUserGroupRoleName As Boolean

        ' Überprüfen auf Eindeutigkeit, da EF sowas nicht anbietet
        ' Any() stops at the first match and doesn't have to enumerate the entire sequence
        isUniqueUserGroupRoleName = Not DbCtx.UserGroupRoles.Any(Function(u) u.Name = UserGroupRoleEntity.Name)

        Return isUniqueUserGroupRoleName

    End Function
#End Region
#End Region

End Class

