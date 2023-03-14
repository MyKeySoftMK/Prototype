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

Partial Class UserManagement

    Inherits ClassBase
    Implements IUserManagement
	Implements IUserManagement_ModelEntryPoints
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
            NLOGLOGGER.Debug("=> Initialize Component 'UserManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'UserManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'UserManagement' cannot initialized. Check the database connection first.")

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

    Private ReadOnly Property DefaultQueryUsers As DbQuery(Of User)
        Get

            Try

                Dim defaultQuery As DbQuery(Of User) = DbCtx.Users
	            Dim _defaultQueryResult As DbQuery(Of User)


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
    ''' Gibt eine unsortierte Liste der Benutzer aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUsers(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of User) Implements IUserManagement.GetUsers
        Dim serverUsers As DbQuery(Of User)

        NLOGLOGGER.Info("Get List of Users ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverUsers = DefaultQueryUsers.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverUsers = DefaultQueryUsers
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverUsers.Count & " User/Users in database")

        Return New ObservableCollection(Of User)(serverUsers)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Benutzer aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUsers(UserGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of User) Implements IUserManagement.GetUsers
        Dim serverUsers As DbQuery(Of User)

        NLOGLOGGER.Info("Get List of Users ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverUsers = DefaultQueryUsers.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverUsers = DefaultQueryUsers.Where(Function(_User) UserGuids.Contains(_User.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverUsers.Count & " User/Users in database")

        Return New ObservableCollection(Of User)(serverUsers)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Benutzer zurück
    ''' </summary>
    ''' <param name="UserGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetUser(UserGUID As String, Optional QParam As QueryParameters = Nothing) As User Implements IUserManagement.GetUser
        Dim serverUser As User = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get User Informations ")

            guid = guidConvert.ConvertFromString(UserGUID)
            NLOGLOGGER.Debug("=> Search for User with GUID '" & UserGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverUser = DefaultQueryUsers.Single(Function(Users) Users.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found User '" & serverUser.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverUser
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Benutzer
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewUser() As ServerResult Implements IUserManagement.CreateNewUser
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewUser As User

        Try
            NLOGLOGGER.Info("New User will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewUser = EntryPoint_GenerateValidUserEntity()

			' Anpassen der Historieninformationen
			With _NewUser
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.Users.Add(_NewUser)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New User is successfull create in database")
            _result.ReturnValue = _NewUser.GUID.ToString
            NLOGLOGGER.Info("=> UserGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewUser")
			_resultAfterAdd = EntryPoint_AfterAddNewUser(_NewUser)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewUser")

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
    ''' Erstellt einen neuen Benutzer aus einer Benutzer-Entität
    ''' </summary>
    ''' <param name="UserEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Benutzer-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewUser(UserEntity As User) As ServerResult Implements IUserManagement.CreateNewUser
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim UserEntryPointResult as User

        Try
            NLOGLOGGER.Info("New User will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			UserEntryPointResult = EntryPoint_BeforeAddNewUser(UserEntity)
			If UserEntryPointResult IsNot Nothing then
				UserEntity = UserEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = CheckAddNewUser(UserEntity)
			If canAddNew = True Then
			canAddNew = EntryPoint_CheckAddNewUser(DbCtx, UserEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If

			End If

            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With UserEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.Users.Add(UserEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New User is successfull create in database")
                _result.ReturnValue = UserEntity.GUID.ToString
                NLOGLOGGER.Info("=> UserGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewUser")
				_resultAfterAddNew = EntryPoint_AfterAddNewUser(UserEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewUser")

            Else
                NLOGLOGGER.Error("User cannot added.")
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
    ''' Kopieren einer vorhanden Benutzer mit allen Eigenschaften
    ''' </summary>
    ''' <param name="UserEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyUser(UserEntity As User) As ServerResult Implements IUserManagement.CopyUser

        Dim _copyUser As New User
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.Users.Add(_copyUser)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(UserEntity).CurrentValues
			DbCtx.Entry(_copyUser).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyUser = EntryPoint_CopyUser(UserEntity)

			' Anpassen der Historieninformationen
			With _copyUser
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
    ''' <param name="UserEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyUser(UserEntity As User) As ServerResult Implements IUserManagement.ModifyUser
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim UserEntryPointResult as User

        Dim serverUser As User

        Try
            NLOGLOGGER.Info("User will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverUser = GetUser(UserEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyUser")
			_resultBeforeModify = EntryPoint_BeforeModifyUser(serverUser)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyUser")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyUser")
            UserEntryPointResult = EntryPoint_ModifyEntityBeforeModifyUser(UserEntity)
			If UserEntryPointResult IsNot Nothing then
				UserEntity = UserEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyUser")

			' Anpassen der Historieninformationen
			With UserEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverUser).CurrentValues.SetValues(UserEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyUser")
			_resultAfterModify = EntryPoint_AfterModifyUser(serverUser)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyUser")

            NLOGLOGGER.Debug("User is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Benutzer 
    ''' </summary>
    ''' <param name="UserGUID">Die UserGUID des Benutzer, der gelöscht werden soll</param>
    ''' <returns>Wurde der Benutzer gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteUser(UserGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IUserManagement.DeleteUser

        Dim serverUsers As List(Of User)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete User")

            NLOGLOGGER.Debug("=> Try to find User-id '" & UserGUID & "'")
            guid = guidConvert.ConvertFromString(UserGUID)
            serverUsers = (From User In DbCtx.Users Where User.GUID = guid).ToList

            If serverUsers.Count = 1 Then
                NLOGLOGGER.Debug("=> User-Id '" & UserGUID & "' found")
                If serverUsers(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.Users.Remove(serverUsers(0))
                    Else
						' Anpassen der Historieninformationen
						With serverUsers(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteUser")
					_resultAfterDelete = EntryPoint_AfterDeleteUser(serverUsers(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteUser")
			
                    NLOGLOGGER.Info("=> User-Id '" & UserGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> User-Id '" & UserGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> User-Id '" & UserGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren Benutzer
    ''' </summary>
    ''' <param name="Users"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteUsers(Users As User(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IUserManagement.DeleteUsers
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & Users.Count & " Users Collection")

            For Each _User As User In Users
                _resultAfterDelete = DeleteUser(_User.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete Users Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete Users Collection with errors")
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
    ''' <param name="UserEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckAddNewUser(UserEntity As User) As Boolean

        Dim isUniqueUserName As Boolean

        ' Überprüfen auf Eindeutigkeit, da EF sowas nicht anbietet
        ' Any() stops at the first match and doesn't have to enumerate the entire sequence
        isUniqueUserName = Not DbCtx.Users.Any(Function(u) u.Name = UserEntity.Name)

        Return isUniqueUserName

    End Function
#End Region
#End Region

End Class

