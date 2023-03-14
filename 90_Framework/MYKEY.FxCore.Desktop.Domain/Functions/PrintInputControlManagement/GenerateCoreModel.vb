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
Imports MYKEY.FxCore.DataAccess.FxNTPrints
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Class PrintInputControlManagement

    Inherits ClassBase
    Implements IPrintInputControlManagement
	Implements IPrintInputControlManagement_ModelEntryPoints
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
        Return "FxNTPrints.FxNTPrintsEDM"
    End Function

    Public Function EntityModelAssembly() As String
        Return "MYKEY.FxCore.DataAccess"
    End Function

    Private Sub Initialize()

        Try

            ' Einstellungen lesen
            NLOGLOGGER.Debug("=> Initialize Component 'PrintInputControlManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'PrintInputControlManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'PrintInputControlManagement' cannot initialized. Check the database connection first.")

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
    Private _DbCtx As FxNTPrints.Entities
    Private ReadOnly Property DbCtx As FxNTPrints.Entities
        Get
            Try

                If _DbCtx Is Nothing Then
                    NLOGLOGGER.Debug("Try to create a new DbContext-Object")

                    ' Erzeugen eines DataContext 
                    _DbCtx = New FxNTPrints.Entities(EntityConnectionData.EntityConnectionString)

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

    Private ReadOnly Property DefaultQueryInputControls As DbQuery(Of InputControl)
        Get

            Try

                Dim defaultQuery As DbQuery(Of InputControl) = DbCtx.InputControls
	            Dim _defaultQueryResult As DbQuery(Of InputControl)


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
    ''' Gibt eine unsortierte Liste der Eingabeelemente aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInputControls(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of InputControl) Implements IPrintInputControlManagement.GetInputControls
        Dim serverInputControls As DbQuery(Of InputControl)

        NLOGLOGGER.Info("Get List of InputControls ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverInputControls = DefaultQueryInputControls.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverInputControls = DefaultQueryInputControls
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverInputControls.Count & " InputControl/InputControls in database")

        Return New ObservableCollection(Of InputControl)(serverInputControls)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Eingabeelemente aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInputControls(InputControlGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of InputControl) Implements IPrintInputControlManagement.GetInputControls
        Dim serverInputControls As DbQuery(Of InputControl)

        NLOGLOGGER.Info("Get List of InputControls ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverInputControls = DefaultQueryInputControls.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverInputControls = DefaultQueryInputControls.Where(Function(_InputControl) InputControlGuids.Contains(_InputControl.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverInputControls.Count & " InputControl/InputControls in database")

        Return New ObservableCollection(Of InputControl)(serverInputControls)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Eingabeelement zurück
    ''' </summary>
    ''' <param name="InputControlGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetInputControl(InputControlGUID As String, Optional QParam As QueryParameters = Nothing) As InputControl Implements IPrintInputControlManagement.GetInputControl
        Dim serverInputControl As InputControl = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get InputControl Informations ")

            guid = guidConvert.ConvertFromString(InputControlGUID)
            NLOGLOGGER.Debug("=> Search for InputControl with GUID '" & InputControlGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverInputControl = DefaultQueryInputControls.Single(Function(InputControls) InputControls.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found InputControl '" & serverInputControl.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverInputControl
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Eingabeelement
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewInputControl() As ServerResult Implements IPrintInputControlManagement.CreateNewInputControl
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewInputControl As InputControl

        Try
            NLOGLOGGER.Info("New InputControl will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewInputControl = EntryPoint_GenerateValidInputControlEntity()

			' Anpassen der Historieninformationen
			With _NewInputControl
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.InputControls.Add(_NewInputControl)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New InputControl is successfull create in database")
            _result.ReturnValue = _NewInputControl.GUID.ToString
            NLOGLOGGER.Info("=> InputControlGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewInputControl")
			_resultAfterAdd = EntryPoint_AfterAddNewInputControl(_NewInputControl)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewInputControl")

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
    ''' Erstellt einen neuen Eingabeelement aus einer Eingabeelement-Entität
    ''' </summary>
    ''' <param name="InputControlEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Eingabeelement-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewInputControl(InputControlEntity As InputControl) As ServerResult Implements IPrintInputControlManagement.CreateNewInputControl
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim InputControlEntryPointResult as InputControl

        Try
            NLOGLOGGER.Info("New InputControl will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			InputControlEntryPointResult = EntryPoint_BeforeAddNewInputControl(InputControlEntity)
			If InputControlEntryPointResult IsNot Nothing then
				InputControlEntity = InputControlEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = EntryPoint_CheckAddNewInputControl(DbCtx, InputControlEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If


            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With InputControlEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.InputControls.Add(InputControlEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New InputControl is successfull create in database")
                _result.ReturnValue = InputControlEntity.GUID.ToString
                NLOGLOGGER.Info("=> InputControlGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewInputControl")
				_resultAfterAddNew = EntryPoint_AfterAddNewInputControl(InputControlEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewInputControl")

            Else
                NLOGLOGGER.Error("InputControl cannot added.")
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
    ''' Kopieren einer vorhanden Eingabeelement mit allen Eigenschaften
    ''' </summary>
    ''' <param name="InputControlEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyInputControl(InputControlEntity As InputControl) As ServerResult Implements IPrintInputControlManagement.CopyInputControl

        Dim _copyInputControl As New InputControl
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.InputControls.Add(_copyInputControl)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(InputControlEntity).CurrentValues
			DbCtx.Entry(_copyInputControl).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyInputControl = EntryPoint_CopyInputControl(InputControlEntity)

			' Anpassen der Historieninformationen
			With _copyInputControl
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
    ''' <param name="InputControlEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyInputControl(InputControlEntity As InputControl) As ServerResult Implements IPrintInputControlManagement.ModifyInputControl
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim InputControlEntryPointResult as InputControl

        Dim serverInputControl As InputControl

        Try
            NLOGLOGGER.Info("InputControl will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverInputControl = GetInputControl(InputControlEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyInputControl")
			_resultBeforeModify = EntryPoint_BeforeModifyInputControl(serverInputControl)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyInputControl")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyInputControl")
            InputControlEntryPointResult = EntryPoint_ModifyEntityBeforeModifyInputControl(InputControlEntity)
			If InputControlEntryPointResult IsNot Nothing then
				InputControlEntity = InputControlEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyInputControl")

			' Anpassen der Historieninformationen
			With InputControlEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverInputControl).CurrentValues.SetValues(InputControlEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyInputControl")
			_resultAfterModify = EntryPoint_AfterModifyInputControl(serverInputControl)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyInputControl")

            NLOGLOGGER.Debug("InputControl is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Eingabeelement 
    ''' </summary>
    ''' <param name="InputControlGUID">Die InputControlGUID des Eingabeelement, der gelöscht werden soll</param>
    ''' <returns>Wurde der Eingabeelement gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteInputControl(InputControlGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IPrintInputControlManagement.DeleteInputControl

        Dim serverInputControls As List(Of InputControl)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete InputControl")

            NLOGLOGGER.Debug("=> Try to find InputControl-id '" & InputControlGUID & "'")
            guid = guidConvert.ConvertFromString(InputControlGUID)
            serverInputControls = (From InputControl In DbCtx.InputControls Where InputControl.GUID = guid).ToList

            If serverInputControls.Count = 1 Then
                NLOGLOGGER.Debug("=> InputControl-Id '" & InputControlGUID & "' found")
                If serverInputControls(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.InputControls.Remove(serverInputControls(0))
                    Else
						' Anpassen der Historieninformationen
						With serverInputControls(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteInputControl")
					_resultAfterDelete = EntryPoint_AfterDeleteInputControl(serverInputControls(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteInputControl")
			
                    NLOGLOGGER.Info("=> InputControl-Id '" & InputControlGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> InputControl-Id '" & InputControlGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> InputControl-Id '" & InputControlGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren Eingabeelemente
    ''' </summary>
    ''' <param name="InputControls"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteInputControls(InputControls As InputControl(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IPrintInputControlManagement.DeleteInputControls
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & InputControls.Count & " InputControls Collection")

            For Each _InputControl As InputControl In InputControls
                _resultAfterDelete = DeleteInputControl(_InputControl.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete InputControls Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete InputControls Collection with errors")
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

