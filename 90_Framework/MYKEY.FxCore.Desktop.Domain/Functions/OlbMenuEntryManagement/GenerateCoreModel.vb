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
Imports MYKEY.FxCore.DataAccess.FxNTOlbMenu
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Class OlbMenuEntryManagement

    Inherits ClassBase
    Implements IOlbMenuEntryManagement
	Implements IOlbMenuEntryManagement_ModelEntryPoints
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
        Return "FxNTOlbMenu.FxNTOlbMenuEDM"
    End Function

    Public Function EntityModelAssembly() As String
        Return "MYKEY.FxCore.DataAccess"
    End Function

    Private Sub Initialize()

        Try

            ' Einstellungen lesen
            NLOGLOGGER.Debug("=> Initialize Component 'OlbMenuEntryManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'OlbMenuEntryManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'OlbMenuEntryManagement' cannot initialized. Check the database connection first.")

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
    Private _DbCtx As FxNTOlbMenu.Entities
    Private ReadOnly Property DbCtx As FxNTOlbMenu.Entities
        Get
            Try

                If _DbCtx Is Nothing Then
                    NLOGLOGGER.Debug("Try to create a new DbContext-Object")

                    ' Erzeugen eines DataContext 
                    _DbCtx = New FxNTOlbMenu.Entities(EntityConnectionData.EntityConnectionString)

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

    Private ReadOnly Property DefaultQueryMenuEntries As DbQuery(Of MenuEntry)
        Get

            Try

                Dim defaultQuery As DbQuery(Of MenuEntry) = DbCtx.MenuEntries
	            Dim _defaultQueryResult As DbQuery(Of MenuEntry)


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
    ''' Gibt eine unsortierte Liste der Menueeintraege aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMenuEntries(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of MenuEntry) Implements IOlbMenuEntryManagement.GetMenuEntries
        Dim serverMenuEntries As DbQuery(Of MenuEntry)

        NLOGLOGGER.Info("Get List of MenuEntries ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverMenuEntries = DefaultQueryMenuEntries.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverMenuEntries = DefaultQueryMenuEntries
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverMenuEntries.Count & " MenuEntry/MenuEntries in database")

        Return New ObservableCollection(Of MenuEntry)(serverMenuEntries)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Menueeintraege aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMenuEntries(MenuEntryGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of MenuEntry) Implements IOlbMenuEntryManagement.GetMenuEntries
        Dim serverMenuEntries As DbQuery(Of MenuEntry)

        NLOGLOGGER.Info("Get List of MenuEntries ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverMenuEntries = DefaultQueryMenuEntries.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverMenuEntries = DefaultQueryMenuEntries.Where(Function(_MenuEntry) MenuEntryGuids.Contains(_MenuEntry.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverMenuEntries.Count & " MenuEntry/MenuEntries in database")

        Return New ObservableCollection(Of MenuEntry)(serverMenuEntries)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Menueeintrag zurück
    ''' </summary>
    ''' <param name="MenuEntryGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetMenuEntry(MenuEntryGUID As String, Optional QParam As QueryParameters = Nothing) As MenuEntry Implements IOlbMenuEntryManagement.GetMenuEntry
        Dim serverMenuEntry As MenuEntry = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get MenuEntry Informations ")

            guid = guidConvert.ConvertFromString(MenuEntryGUID)
            NLOGLOGGER.Debug("=> Search for MenuEntry with GUID '" & MenuEntryGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverMenuEntry = DefaultQueryMenuEntries.Single(Function(MenuEntries) MenuEntries.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found MenuEntry '" & serverMenuEntry.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverMenuEntry
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Menueeintrag
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewMenuEntry() As ServerResult Implements IOlbMenuEntryManagement.CreateNewMenuEntry
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewMenuEntry As MenuEntry

        Try
            NLOGLOGGER.Info("New MenuEntry will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewMenuEntry = EntryPoint_GenerateValidMenuEntryEntity()

			' Anpassen der Historieninformationen
			With _NewMenuEntry
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.MenuEntries.Add(_NewMenuEntry)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New MenuEntry is successfull create in database")
            _result.ReturnValue = _NewMenuEntry.GUID.ToString
            NLOGLOGGER.Info("=> MenuEntryGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewMenuEntry")
			_resultAfterAdd = EntryPoint_AfterAddNewMenuEntry(_NewMenuEntry)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewMenuEntry")

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
    ''' Erstellt einen neuen Menueeintrag aus einer Menueeintrag-Entität
    ''' </summary>
    ''' <param name="MenuEntryEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Menueeintrag-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewMenuEntry(MenuEntryEntity As MenuEntry) As ServerResult Implements IOlbMenuEntryManagement.CreateNewMenuEntry
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim MenuEntryEntryPointResult as MenuEntry

        Try
            NLOGLOGGER.Info("New MenuEntry will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			MenuEntryEntryPointResult = EntryPoint_BeforeAddNewMenuEntry(MenuEntryEntity)
			If MenuEntryEntryPointResult IsNot Nothing then
				MenuEntryEntity = MenuEntryEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = EntryPoint_CheckAddNewMenuEntry(DbCtx, MenuEntryEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If


            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With MenuEntryEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.MenuEntries.Add(MenuEntryEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New MenuEntry is successfull create in database")
                _result.ReturnValue = MenuEntryEntity.GUID.ToString
                NLOGLOGGER.Info("=> MenuEntryGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewMenuEntry")
				_resultAfterAddNew = EntryPoint_AfterAddNewMenuEntry(MenuEntryEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewMenuEntry")

            Else
                NLOGLOGGER.Error("MenuEntry cannot added.")
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
    ''' Kopieren einer vorhanden Menueeintrag mit allen Eigenschaften
    ''' </summary>
    ''' <param name="MenuEntryEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyMenuEntry(MenuEntryEntity As MenuEntry) As ServerResult Implements IOlbMenuEntryManagement.CopyMenuEntry

        Dim _copyMenuEntry As New MenuEntry
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.MenuEntries.Add(_copyMenuEntry)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(MenuEntryEntity).CurrentValues
			DbCtx.Entry(_copyMenuEntry).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyMenuEntry = EntryPoint_CopyMenuEntry(MenuEntryEntity)

			' Anpassen der Historieninformationen
			With _copyMenuEntry
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
    ''' <param name="MenuEntryEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyMenuEntry(MenuEntryEntity As MenuEntry) As ServerResult Implements IOlbMenuEntryManagement.ModifyMenuEntry
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim MenuEntryEntryPointResult as MenuEntry

        Dim serverMenuEntry As MenuEntry

        Try
            NLOGLOGGER.Info("MenuEntry will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverMenuEntry = GetMenuEntry(MenuEntryEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyMenuEntry")
			_resultBeforeModify = EntryPoint_BeforeModifyMenuEntry(serverMenuEntry)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyMenuEntry")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyMenuEntry")
            MenuEntryEntryPointResult = EntryPoint_ModifyEntityBeforeModifyMenuEntry(MenuEntryEntity)
			If MenuEntryEntryPointResult IsNot Nothing then
				MenuEntryEntity = MenuEntryEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyMenuEntry")

			' Anpassen der Historieninformationen
			With MenuEntryEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverMenuEntry).CurrentValues.SetValues(MenuEntryEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyMenuEntry")
			_resultAfterModify = EntryPoint_AfterModifyMenuEntry(serverMenuEntry)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyMenuEntry")

            NLOGLOGGER.Debug("MenuEntry is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Menueeintrag 
    ''' </summary>
    ''' <param name="MenuEntryGUID">Die MenuEntryGUID des Menueeintrag, der gelöscht werden soll</param>
    ''' <returns>Wurde der Menueeintrag gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteMenuEntry(MenuEntryGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IOlbMenuEntryManagement.DeleteMenuEntry

        Dim serverMenuEntries As List(Of MenuEntry)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete MenuEntry")

            NLOGLOGGER.Debug("=> Try to find MenuEntry-id '" & MenuEntryGUID & "'")
            guid = guidConvert.ConvertFromString(MenuEntryGUID)
            serverMenuEntries = (From MenuEntry In DbCtx.MenuEntries Where MenuEntry.GUID = guid).ToList

            If serverMenuEntries.Count = 1 Then
                NLOGLOGGER.Debug("=> MenuEntry-Id '" & MenuEntryGUID & "' found")
                If serverMenuEntries(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.MenuEntries.Remove(serverMenuEntries(0))
                    Else
						' Anpassen der Historieninformationen
						With serverMenuEntries(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteMenuEntry")
					_resultAfterDelete = EntryPoint_AfterDeleteMenuEntry(serverMenuEntries(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteMenuEntry")
			
                    NLOGLOGGER.Info("=> MenuEntry-Id '" & MenuEntryGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> MenuEntry-Id '" & MenuEntryGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> MenuEntry-Id '" & MenuEntryGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren Menueeintraege
    ''' </summary>
    ''' <param name="MenuEntries"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteMenuEntries(MenuEntries As MenuEntry(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IOlbMenuEntryManagement.DeleteMenuEntries
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & MenuEntries.Count & " MenuEntries Collection")

            For Each _MenuEntry As MenuEntry In MenuEntries
                _resultAfterDelete = DeleteMenuEntry(_MenuEntry.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete MenuEntries Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete MenuEntries Collection with errors")
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

