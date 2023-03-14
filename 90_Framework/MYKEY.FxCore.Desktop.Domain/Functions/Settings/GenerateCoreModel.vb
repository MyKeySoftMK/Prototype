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
Imports MYKEY.FxCore.DataAccess.FxNTSettings
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Class SettingManagement

    Inherits ClassBase
    Implements ISettingManagement
	Implements ISettingManagement_ModelEntryPoints
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
        Return "FxNTSettings.FxNTSettingsEDM"
    End Function

    Public Function EntityModelAssembly() As String
        Return "MYKEY.FxCore.DataAccess"
    End Function

    Private Sub Initialize()

        Try

            ' Einstellungen lesen
            NLOGLOGGER.Debug("=> Initialize Component 'SettingManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'SettingManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'SettingManagement' cannot initialized. Check the database connection first.")

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
    Private _DbCtx As FxNTSettings.Entities
    Private ReadOnly Property DbCtx As FxNTSettings.Entities
        Get
            Try

                If _DbCtx Is Nothing Then
                    NLOGLOGGER.Debug("Try to create a new DbContext-Object")

                    ' Erzeugen eines DataContext 
                    _DbCtx = New FxNTSettings.Entities(EntityConnectionData.EntityConnectionString)

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

    Private ReadOnly Property DefaultQuerySettings As DbQuery(Of Setting)
        Get

            Try

                Dim defaultQuery As DbQuery(Of Setting) = DbCtx.Settings
	            Dim _defaultQueryResult As DbQuery(Of Setting)


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
    ''' Gibt eine unsortierte Liste der Einstellungen aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSettings(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Setting) Implements ISettingManagement.GetSettings
        Dim serverSettings As DbQuery(Of Setting)

        NLOGLOGGER.Info("Get List of Settings ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverSettings = DefaultQuerySettings.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverSettings = DefaultQuerySettings
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverSettings.Count & " Setting/Settings in database")

        Return New ObservableCollection(Of Setting)(serverSettings)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Einstellungen aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSettings(SettingGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Setting) Implements ISettingManagement.GetSettings
        Dim serverSettings As DbQuery(Of Setting)

        NLOGLOGGER.Info("Get List of Settings ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverSettings = DefaultQuerySettings.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverSettings = DefaultQuerySettings.Where(Function(_Setting) SettingGuids.Contains(_Setting.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverSettings.Count & " Setting/Settings in database")

        Return New ObservableCollection(Of Setting)(serverSettings)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Einstellung zurück
    ''' </summary>
    ''' <param name="SettingGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetSetting(SettingGUID As String, Optional QParam As QueryParameters = Nothing) As Setting Implements ISettingManagement.GetSetting
        Dim serverSetting As Setting = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get Setting Informations ")

            guid = guidConvert.ConvertFromString(SettingGUID)
            NLOGLOGGER.Debug("=> Search for Setting with GUID '" & SettingGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverSetting = DefaultQuerySettings.Single(Function(Settings) Settings.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found Setting '" & serverSetting.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverSetting
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Einstellung
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewSetting() As ServerResult Implements ISettingManagement.CreateNewSetting
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewSetting As Setting

        Try
            NLOGLOGGER.Info("New Setting will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewSetting = EntryPoint_GenerateValidSettingEntity()

			' Anpassen der Historieninformationen
			With _NewSetting
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.Settings.Add(_NewSetting)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New Setting is successfull create in database")
            _result.ReturnValue = _NewSetting.GUID.ToString
            NLOGLOGGER.Info("=> SettingGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewSetting")
			_resultAfterAdd = EntryPoint_AfterAddNewSetting(_NewSetting)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewSetting")

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
    ''' Erstellt einen neuen Einstellung aus einer Einstellung-Entität
    ''' </summary>
    ''' <param name="SettingEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Einstellung-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewSetting(SettingEntity As Setting) As ServerResult Implements ISettingManagement.CreateNewSetting
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim SettingEntryPointResult as Setting

        Try
            NLOGLOGGER.Info("New Setting will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			SettingEntryPointResult = EntryPoint_BeforeAddNewSetting(SettingEntity)
			If SettingEntryPointResult IsNot Nothing then
				SettingEntity = SettingEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = EntryPoint_CheckAddNewSetting(DbCtx, SettingEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If


            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With SettingEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.Settings.Add(SettingEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New Setting is successfull create in database")
                _result.ReturnValue = SettingEntity.GUID.ToString
                NLOGLOGGER.Info("=> SettingGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewSetting")
				_resultAfterAddNew = EntryPoint_AfterAddNewSetting(SettingEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewSetting")

            Else
                NLOGLOGGER.Error("Setting cannot added.")
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
    ''' Kopieren einer vorhanden Einstellung mit allen Eigenschaften
    ''' </summary>
    ''' <param name="SettingEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopySetting(SettingEntity As Setting) As ServerResult Implements ISettingManagement.CopySetting

        Dim _copySetting As New Setting
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.Settings.Add(_copySetting)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(SettingEntity).CurrentValues
			DbCtx.Entry(_copySetting).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copySetting = EntryPoint_CopySetting(SettingEntity)

			' Anpassen der Historieninformationen
			With _copySetting
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
    ''' <param name="SettingEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifySetting(SettingEntity As Setting) As ServerResult Implements ISettingManagement.ModifySetting
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim SettingEntryPointResult as Setting

        Dim serverSetting As Setting

        Try
            NLOGLOGGER.Info("Setting will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverSetting = GetSetting(SettingEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifySetting")
			_resultBeforeModify = EntryPoint_BeforeModifySetting(serverSetting)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifySetting")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifySetting")
            SettingEntryPointResult = EntryPoint_ModifyEntityBeforeModifySetting(SettingEntity)
			If SettingEntryPointResult IsNot Nothing then
				SettingEntity = SettingEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifySetting")

			' Anpassen der Historieninformationen
			With SettingEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverSetting).CurrentValues.SetValues(SettingEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifySetting")
			_resultAfterModify = EntryPoint_AfterModifySetting(serverSetting)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifySetting")

            NLOGLOGGER.Debug("Setting is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Einstellung 
    ''' </summary>
    ''' <param name="SettingGUID">Die SettingGUID des Einstellung, der gelöscht werden soll</param>
    ''' <returns>Wurde der Einstellung gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteSetting(SettingGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements ISettingManagement.DeleteSetting

        Dim serverSettings As List(Of Setting)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete Setting")

            NLOGLOGGER.Debug("=> Try to find Setting-id '" & SettingGUID & "'")
            guid = guidConvert.ConvertFromString(SettingGUID)
            serverSettings = (From Setting In DbCtx.Settings Where Setting.GUID = guid).ToList

            If serverSettings.Count = 1 Then
                NLOGLOGGER.Debug("=> Setting-Id '" & SettingGUID & "' found")
                If serverSettings(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.Settings.Remove(serverSettings(0))
                    Else
						' Anpassen der Historieninformationen
						With serverSettings(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteSetting")
					_resultAfterDelete = EntryPoint_AfterDeleteSetting(serverSettings(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteSetting")
			
                    NLOGLOGGER.Info("=> Setting-Id '" & SettingGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> Setting-Id '" & SettingGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> Setting-Id '" & SettingGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren Einstellungen
    ''' </summary>
    ''' <param name="Settings"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteSettings(Settings As Setting(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements ISettingManagement.DeleteSettings
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & Settings.Count & " Settings Collection")

            For Each _Setting As Setting In Settings
                _resultAfterDelete = DeleteSetting(_Setting.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete Settings Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete Settings Collection with errors")
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

#End Region
#End Region

End Class

