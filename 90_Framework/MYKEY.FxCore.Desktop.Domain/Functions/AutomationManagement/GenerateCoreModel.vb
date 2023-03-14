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
Imports MYKEY.FxCore.DataAccess.FxNTAutomations
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Class AutomationManagement

    Inherits ClassBase
    Implements IAutomationManagement
	Implements IAutomationManagement_ModelEntryPoints
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
        Return "FxNTAutomations.FxNTAutomationsEDM"
    End Function

    Public Function EntityModelAssembly() As String
        Return "MYKEY.FxCore.DataAccess"
    End Function

    Private Sub Initialize()

        Try

            ' Einstellungen lesen
            NLOGLOGGER.Debug("=> Initialize Component 'AutomationManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'AutomationManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'AutomationManagement' cannot initialized. Check the database connection first.")

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
    Private _DbCtx As FxNTAutomations.Entities
    Private ReadOnly Property DbCtx As FxNTAutomations.Entities
        Get
            Try

                If _DbCtx Is Nothing Then
                    NLOGLOGGER.Debug("Try to create a new DbContext-Object")

                    ' Erzeugen eines DataContext 
                    _DbCtx = New FxNTAutomations.Entities(EntityConnectionData.EntityConnectionString)

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

    Private ReadOnly Property DefaultQueryAutomations As DbQuery(Of Automation)
        Get

            Try

                Dim defaultQuery As DbQuery(Of Automation) = DbCtx.Automations
	            Dim _defaultQueryResult As DbQuery(Of Automation)


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
    ''' Gibt eine unsortierte Liste der Automationen aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAutomations(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Automation) Implements IAutomationManagement.GetAutomations
        Dim serverAutomations As DbQuery(Of Automation)

        NLOGLOGGER.Info("Get List of Automations ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverAutomations = DefaultQueryAutomations.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverAutomations = DefaultQueryAutomations
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverAutomations.Count & " Automation/Automations in database")

        Return New ObservableCollection(Of Automation)(serverAutomations)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Automationen aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAutomations(AutomationGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Automation) Implements IAutomationManagement.GetAutomations
        Dim serverAutomations As DbQuery(Of Automation)

        NLOGLOGGER.Info("Get List of Automations ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverAutomations = DefaultQueryAutomations.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverAutomations = DefaultQueryAutomations.Where(Function(_Automation) AutomationGuids.Contains(_Automation.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverAutomations.Count & " Automation/Automations in database")

        Return New ObservableCollection(Of Automation)(serverAutomations)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Automation zurück
    ''' </summary>
    ''' <param name="AutomationGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetAutomation(AutomationGUID As String, Optional QParam As QueryParameters = Nothing) As Automation Implements IAutomationManagement.GetAutomation
        Dim serverAutomation As Automation = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get Automation Informations ")

            guid = guidConvert.ConvertFromString(AutomationGUID)
            NLOGLOGGER.Debug("=> Search for Automation with GUID '" & AutomationGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverAutomation = DefaultQueryAutomations.Single(Function(Automations) Automations.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found Automation '" & serverAutomation.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverAutomation
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Automation
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewAutomation() As ServerResult Implements IAutomationManagement.CreateNewAutomation
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewAutomation As Automation

        Try
            NLOGLOGGER.Info("New Automation will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewAutomation = EntryPoint_GenerateValidAutomationEntity()

			' Anpassen der Historieninformationen
			With _NewAutomation
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.Automations.Add(_NewAutomation)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New Automation is successfull create in database")
            _result.ReturnValue = _NewAutomation.GUID.ToString
            NLOGLOGGER.Info("=> AutomationGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewAutomation")
			_resultAfterAdd = EntryPoint_AfterAddNewAutomation(_NewAutomation)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewAutomation")

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
    ''' Erstellt einen neuen Automation aus einer Automation-Entität
    ''' </summary>
    ''' <param name="AutomationEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Automation-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewAutomation(AutomationEntity As Automation) As ServerResult Implements IAutomationManagement.CreateNewAutomation
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim AutomationEntryPointResult as Automation

        Try
            NLOGLOGGER.Info("New Automation will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			AutomationEntryPointResult = EntryPoint_BeforeAddNewAutomation(AutomationEntity)
			If AutomationEntryPointResult IsNot Nothing then
				AutomationEntity = AutomationEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = CheckAddNewAutomation(AutomationEntity)
			If canAddNew = True Then
			canAddNew = EntryPoint_CheckAddNewAutomation(DbCtx, AutomationEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If

			End If

            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With AutomationEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.Automations.Add(AutomationEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New Automation is successfull create in database")
                _result.ReturnValue = AutomationEntity.GUID.ToString
                NLOGLOGGER.Info("=> AutomationGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewAutomation")
				_resultAfterAddNew = EntryPoint_AfterAddNewAutomation(AutomationEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewAutomation")

            Else
                NLOGLOGGER.Error("Automation cannot added.")
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
    ''' Kopieren einer vorhanden Automation mit allen Eigenschaften
    ''' </summary>
    ''' <param name="AutomationEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyAutomation(AutomationEntity As Automation) As ServerResult Implements IAutomationManagement.CopyAutomation

        Dim _copyAutomation As New Automation
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.Automations.Add(_copyAutomation)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(AutomationEntity).CurrentValues
			DbCtx.Entry(_copyAutomation).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyAutomation = EntryPoint_CopyAutomation(AutomationEntity)

			' Anpassen der Historieninformationen
			With _copyAutomation
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
    ''' <param name="AutomationEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyAutomation(AutomationEntity As Automation) As ServerResult Implements IAutomationManagement.ModifyAutomation
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim AutomationEntryPointResult as Automation

        Dim serverAutomation As Automation

        Try
            NLOGLOGGER.Info("Automation will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverAutomation = GetAutomation(AutomationEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyAutomation")
			_resultBeforeModify = EntryPoint_BeforeModifyAutomation(serverAutomation)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyAutomation")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyAutomation")
            AutomationEntryPointResult = EntryPoint_ModifyEntityBeforeModifyAutomation(AutomationEntity)
			If AutomationEntryPointResult IsNot Nothing then
				AutomationEntity = AutomationEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyAutomation")

			' Anpassen der Historieninformationen
			With AutomationEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverAutomation).CurrentValues.SetValues(AutomationEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyAutomation")
			_resultAfterModify = EntryPoint_AfterModifyAutomation(serverAutomation)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyAutomation")

            NLOGLOGGER.Debug("Automation is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Automation 
    ''' </summary>
    ''' <param name="AutomationGUID">Die AutomationGUID des Automation, der gelöscht werden soll</param>
    ''' <returns>Wurde der Automation gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteAutomation(AutomationGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IAutomationManagement.DeleteAutomation

        Dim serverAutomations As List(Of Automation)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete Automation")

            NLOGLOGGER.Debug("=> Try to find Automation-id '" & AutomationGUID & "'")
            guid = guidConvert.ConvertFromString(AutomationGUID)
            serverAutomations = (From Automation In DbCtx.Automations Where Automation.GUID = guid).ToList

            If serverAutomations.Count = 1 Then
                NLOGLOGGER.Debug("=> Automation-Id '" & AutomationGUID & "' found")
                If serverAutomations(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.Automations.Remove(serverAutomations(0))
                    Else
						' Anpassen der Historieninformationen
						With serverAutomations(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteAutomation")
					_resultAfterDelete = EntryPoint_AfterDeleteAutomation(serverAutomations(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteAutomation")
			
                    NLOGLOGGER.Info("=> Automation-Id '" & AutomationGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> Automation-Id '" & AutomationGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> Automation-Id '" & AutomationGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren Automationen
    ''' </summary>
    ''' <param name="Automations"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteAutomations(Automations As Automation(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IAutomationManagement.DeleteAutomations
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & Automations.Count & " Automations Collection")

            For Each _Automation As Automation In Automations
                _resultAfterDelete = DeleteAutomation(_Automation.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete Automations Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete Automations Collection with errors")
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
    ''' <param name="AutomationEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckAddNewAutomation(AutomationEntity As Automation) As Boolean

        Dim isUniqueAutomationName As Boolean

        ' Überprüfen auf Eindeutigkeit, da EF sowas nicht anbietet
        ' Any() stops at the first match and doesn't have to enumerate the entire sequence
        isUniqueAutomationName = Not DbCtx.Automations.Any(Function(u) u.Name = AutomationEntity.Name)

        Return isUniqueAutomationName

    End Function
#End Region
#End Region

End Class

