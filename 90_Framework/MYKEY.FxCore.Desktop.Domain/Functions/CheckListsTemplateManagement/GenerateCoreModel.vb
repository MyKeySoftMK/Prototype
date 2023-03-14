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
Imports MYKEY.FxCore.DataAccess.FxNTCheckListsTemplates
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Class CheckListsTemplateManagement

    Inherits ClassBase
    Implements ICheckListsTemplateManagement
	Implements ICheckListsTemplateManagement_ModelEntryPoints
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
        Return "FxNTCheckListsTemplate.FxNTCheckListsTemplateEDM"
    End Function

    Public Function EntityModelAssembly() As String
        Return "MYKEY.FxCore.DataAccess"
    End Function

    Private Sub Initialize()

        Try

            ' Einstellungen lesen
            NLOGLOGGER.Debug("=> Initialize Component 'CheckListsTemplateManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'CheckListsTemplateManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'CheckListsTemplateManagement' cannot initialized. Check the database connection first.")

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
    Private _DbCtx As FxNTCheckListsTemplates.Entities
    Private ReadOnly Property DbCtx As FxNTCheckListsTemplates.Entities
        Get
            Try

                If _DbCtx Is Nothing Then
                    NLOGLOGGER.Debug("Try to create a new DbContext-Object")

                    ' Erzeugen eines DataContext 
                    _DbCtx = New FxNTCheckListsTemplates.Entities(EntityConnectionData.EntityConnectionString)

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

    Private ReadOnly Property DefaultQueryCheckListsTemplates As DbQuery(Of CheckListsTemplate)
        Get

            Try

                Dim defaultQuery As DbQuery(Of CheckListsTemplate) = DbCtx.CheckListsTemplates
	            Dim _defaultQueryResult As DbQuery(Of CheckListsTemplate)


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
    ''' Gibt eine unsortierte Liste der CheckListvorlagen aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCheckListsTemplates(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of CheckListsTemplate) Implements ICheckListsTemplateManagement.GetCheckListsTemplates
        Dim serverCheckListsTemplates As DbQuery(Of CheckListsTemplate)

        NLOGLOGGER.Info("Get List of CheckListsTemplates ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverCheckListsTemplates = DefaultQueryCheckListsTemplates.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverCheckListsTemplates = DefaultQueryCheckListsTemplates
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverCheckListsTemplates.Count & " CheckListsTemplate/CheckListsTemplates in database")

        Return New ObservableCollection(Of CheckListsTemplate)(serverCheckListsTemplates)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der CheckListvorlagen aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCheckListsTemplates(CheckListsTemplateGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of CheckListsTemplate) Implements ICheckListsTemplateManagement.GetCheckListsTemplates
        Dim serverCheckListsTemplates As DbQuery(Of CheckListsTemplate)

        NLOGLOGGER.Info("Get List of CheckListsTemplates ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverCheckListsTemplates = DefaultQueryCheckListsTemplates.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverCheckListsTemplates = DefaultQueryCheckListsTemplates.Where(Function(_CheckListsTemplate) CheckListsTemplateGuids.Contains(_CheckListsTemplate.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverCheckListsTemplates.Count & " CheckListsTemplate/CheckListsTemplates in database")

        Return New ObservableCollection(Of CheckListsTemplate)(serverCheckListsTemplates)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen CheckListvorlage zurück
    ''' </summary>
    ''' <param name="CheckListsTemplateGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetCheckListsTemplate(CheckListsTemplateGUID As String, Optional QParam As QueryParameters = Nothing) As CheckListsTemplate Implements ICheckListsTemplateManagement.GetCheckListsTemplate
        Dim serverCheckListsTemplate As CheckListsTemplate = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get CheckListsTemplate Informations ")

            guid = guidConvert.ConvertFromString(CheckListsTemplateGUID)
            NLOGLOGGER.Debug("=> Search for CheckListsTemplate with GUID '" & CheckListsTemplateGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverCheckListsTemplate = DefaultQueryCheckListsTemplates.Single(Function(CheckListsTemplates) CheckListsTemplates.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found CheckListsTemplate '" & serverCheckListsTemplate.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverCheckListsTemplate
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ CheckListvorlage
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewCheckListsTemplate() As ServerResult Implements ICheckListsTemplateManagement.CreateNewCheckListsTemplate
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewCheckListsTemplate As CheckListsTemplate

        Try
            NLOGLOGGER.Info("New CheckListsTemplate will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewCheckListsTemplate = EntryPoint_GenerateValidCheckListsTemplateEntity()

			' Anpassen der Historieninformationen
			With _NewCheckListsTemplate
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.CheckListsTemplates.Add(_NewCheckListsTemplate)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New CheckListsTemplate is successfull create in database")
            _result.ReturnValue = _NewCheckListsTemplate.GUID.ToString
            NLOGLOGGER.Info("=> CheckListsTemplateGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewCheckListsTemplate")
			_resultAfterAdd = EntryPoint_AfterAddNewCheckListsTemplate(_NewCheckListsTemplate)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewCheckListsTemplate")

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
    ''' Erstellt einen neuen CheckListvorlage aus einer CheckListvorlage-Entität
    ''' </summary>
    ''' <param name="CheckListsTemplateEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte CheckListvorlage-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As ServerResult Implements ICheckListsTemplateManagement.CreateNewCheckListsTemplate
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim CheckListsTemplateEntryPointResult as CheckListsTemplate

        Try
            NLOGLOGGER.Info("New CheckListsTemplate will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			CheckListsTemplateEntryPointResult = EntryPoint_BeforeAddNewCheckListsTemplate(CheckListsTemplateEntity)
			If CheckListsTemplateEntryPointResult IsNot Nothing then
				CheckListsTemplateEntity = CheckListsTemplateEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = CheckAddNewCheckListsTemplate(CheckListsTemplateEntity)
			If canAddNew = True Then
			canAddNew = EntryPoint_CheckAddNewCheckListsTemplate(DbCtx, CheckListsTemplateEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If

			End If

            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With CheckListsTemplateEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.CheckListsTemplates.Add(CheckListsTemplateEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New CheckListsTemplate is successfull create in database")
                _result.ReturnValue = CheckListsTemplateEntity.GUID.ToString
                NLOGLOGGER.Info("=> CheckListsTemplateGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewCheckListsTemplate")
				_resultAfterAddNew = EntryPoint_AfterAddNewCheckListsTemplate(CheckListsTemplateEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewCheckListsTemplate")

            Else
                NLOGLOGGER.Error("CheckListsTemplate cannot added.")
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
    ''' Kopieren einer vorhanden CheckListvorlage mit allen Eigenschaften
    ''' </summary>
    ''' <param name="CheckListsTemplateEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As ServerResult Implements ICheckListsTemplateManagement.CopyCheckListsTemplate

        Dim _copyCheckListsTemplate As New CheckListsTemplate
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.CheckListsTemplates.Add(_copyCheckListsTemplate)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(CheckListsTemplateEntity).CurrentValues
			DbCtx.Entry(_copyCheckListsTemplate).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyCheckListsTemplate = EntryPoint_CopyCheckListsTemplate(CheckListsTemplateEntity)

			' Anpassen der Historieninformationen
			With _copyCheckListsTemplate
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
    ''' <param name="CheckListsTemplateEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As ServerResult Implements ICheckListsTemplateManagement.ModifyCheckListsTemplate
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim CheckListsTemplateEntryPointResult as CheckListsTemplate

        Dim serverCheckListsTemplate As CheckListsTemplate

        Try
            NLOGLOGGER.Info("CheckListsTemplate will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverCheckListsTemplate = GetCheckListsTemplate(CheckListsTemplateEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyCheckListsTemplate")
			_resultBeforeModify = EntryPoint_BeforeModifyCheckListsTemplate(serverCheckListsTemplate)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyCheckListsTemplate")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyCheckListsTemplate")
            CheckListsTemplateEntryPointResult = EntryPoint_ModifyEntityBeforeModifyCheckListsTemplate(CheckListsTemplateEntity)
			If CheckListsTemplateEntryPointResult IsNot Nothing then
				CheckListsTemplateEntity = CheckListsTemplateEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyCheckListsTemplate")

			' Anpassen der Historieninformationen
			With CheckListsTemplateEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverCheckListsTemplate).CurrentValues.SetValues(CheckListsTemplateEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyCheckListsTemplate")
			_resultAfterModify = EntryPoint_AfterModifyCheckListsTemplate(serverCheckListsTemplate)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyCheckListsTemplate")

            NLOGLOGGER.Debug("CheckListsTemplate is successfull modiefied in database")

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
    ''' Diese Methode löscht einen CheckListvorlage 
    ''' </summary>
    ''' <param name="CheckListsTemplateGUID">Die CheckListsTemplateGUID des CheckListvorlage, der gelöscht werden soll</param>
    ''' <returns>Wurde der CheckListvorlage gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteCheckListsTemplate(CheckListsTemplateGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements ICheckListsTemplateManagement.DeleteCheckListsTemplate

        Dim serverCheckListsTemplates As List(Of CheckListsTemplate)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete CheckListsTemplate")

            NLOGLOGGER.Debug("=> Try to find CheckListsTemplate-id '" & CheckListsTemplateGUID & "'")
            guid = guidConvert.ConvertFromString(CheckListsTemplateGUID)
            serverCheckListsTemplates = (From CheckListsTemplate In DbCtx.CheckListsTemplates Where CheckListsTemplate.GUID = guid).ToList

            If serverCheckListsTemplates.Count = 1 Then
                NLOGLOGGER.Debug("=> CheckListsTemplate-Id '" & CheckListsTemplateGUID & "' found")
                If serverCheckListsTemplates(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.CheckListsTemplates.Remove(serverCheckListsTemplates(0))
                    Else
						' Anpassen der Historieninformationen
						With serverCheckListsTemplates(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteCheckListsTemplate")
					_resultAfterDelete = EntryPoint_AfterDeleteCheckListsTemplate(serverCheckListsTemplates(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteCheckListsTemplate")
			
                    NLOGLOGGER.Info("=> CheckListsTemplate-Id '" & CheckListsTemplateGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> CheckListsTemplate-Id '" & CheckListsTemplateGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> CheckListsTemplate-Id '" & CheckListsTemplateGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren CheckListvorlagen
    ''' </summary>
    ''' <param name="CheckListsTemplates"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteCheckListsTemplates(CheckListsTemplates As CheckListsTemplate(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements ICheckListsTemplateManagement.DeleteCheckListsTemplates
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & CheckListsTemplates.Count & " CheckListsTemplates Collection")

            For Each _CheckListsTemplate As CheckListsTemplate In CheckListsTemplates
                _resultAfterDelete = DeleteCheckListsTemplate(_CheckListsTemplate.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete CheckListsTemplates Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete CheckListsTemplates Collection with errors")
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
    ''' <param name="CheckListsTemplateEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckAddNewCheckListsTemplate(CheckListsTemplateEntity As CheckListsTemplate) As Boolean

        Dim isUniqueCheckListsTemplateName As Boolean

        ' Überprüfen auf Eindeutigkeit, da EF sowas nicht anbietet
        ' Any() stops at the first match and doesn't have to enumerate the entire sequence
        isUniqueCheckListsTemplateName = Not DbCtx.CheckListsTemplates.Any(Function(u) u.Name = CheckListsTemplateEntity.Name)

        Return isUniqueCheckListsTemplateName

    End Function
#End Region
#End Region

End Class

