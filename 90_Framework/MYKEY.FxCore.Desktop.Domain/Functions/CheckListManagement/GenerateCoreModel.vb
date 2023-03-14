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
Imports MYKEY.FxCore.DataAccess.FxNTCheckLists
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Class CheckListManagement

    Inherits ClassBase
    Implements ICheckListManagement
	Implements ICheckListManagement_ModelEntryPoints
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
        Return "FxNTCheckLists.FxNTCheckListsEDM"
    End Function

    Public Function EntityModelAssembly() As String
        Return "MYKEY.FxCore.DataAccess"
    End Function

    Private Sub Initialize()

        Try

            ' Einstellungen lesen
            NLOGLOGGER.Debug("=> Initialize Component 'CheckListManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'CheckListManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'CheckListManagement' cannot initialized. Check the database connection first.")

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
    Private _DbCtx As FxNTCheckLists.Entities
    Private ReadOnly Property DbCtx As FxNTCheckLists.Entities
        Get
            Try

                If _DbCtx Is Nothing Then
                    NLOGLOGGER.Debug("Try to create a new DbContext-Object")

                    ' Erzeugen eines DataContext 
                    _DbCtx = New FxNTCheckLists.Entities(EntityConnectionData.EntityConnectionString)

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

    Private ReadOnly Property DefaultQueryCheckLists As DbQuery(Of CheckList)
        Get

            Try

                Dim defaultQuery As DbQuery(Of CheckList) = DbCtx.CheckLists
	            Dim _defaultQueryResult As DbQuery(Of CheckList)


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
    ''' Gibt eine unsortierte Liste der CheckListen aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCheckLists(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of CheckList) Implements ICheckListManagement.GetCheckLists
        Dim serverCheckLists As DbQuery(Of CheckList)

        NLOGLOGGER.Info("Get List of CheckLists ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverCheckLists = DefaultQueryCheckLists.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverCheckLists = DefaultQueryCheckLists
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverCheckLists.Count & " CheckList/CheckLists in database")

        Return New ObservableCollection(Of CheckList)(serverCheckLists)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der CheckListen aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCheckLists(CheckListGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of CheckList) Implements ICheckListManagement.GetCheckLists
        Dim serverCheckLists As DbQuery(Of CheckList)

        NLOGLOGGER.Info("Get List of CheckLists ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverCheckLists = DefaultQueryCheckLists.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverCheckLists = DefaultQueryCheckLists.Where(Function(_CheckList) CheckListGuids.Contains(_CheckList.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverCheckLists.Count & " CheckList/CheckLists in database")

        Return New ObservableCollection(Of CheckList)(serverCheckLists)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen CheckListe zurück
    ''' </summary>
    ''' <param name="CheckListGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetCheckList(CheckListGUID As String, Optional QParam As QueryParameters = Nothing) As CheckList Implements ICheckListManagement.GetCheckList
        Dim serverCheckList As CheckList = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get CheckList Informations ")

            guid = guidConvert.ConvertFromString(CheckListGUID)
            NLOGLOGGER.Debug("=> Search for CheckList with GUID '" & CheckListGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverCheckList = DefaultQueryCheckLists.Single(Function(CheckLists) CheckLists.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found CheckList '" & serverCheckList.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverCheckList
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ CheckListe
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewCheckList() As ServerResult Implements ICheckListManagement.CreateNewCheckList
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewCheckList As CheckList

        Try
            NLOGLOGGER.Info("New CheckList will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewCheckList = EntryPoint_GenerateValidCheckListEntity()

			' Anpassen der Historieninformationen
			With _NewCheckList
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.CheckLists.Add(_NewCheckList)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New CheckList is successfull create in database")
            _result.ReturnValue = _NewCheckList.GUID.ToString
            NLOGLOGGER.Info("=> CheckListGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewCheckList")
			_resultAfterAdd = EntryPoint_AfterAddNewCheckList(_NewCheckList)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewCheckList")

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
    ''' Erstellt einen neuen CheckListe aus einer CheckListe-Entität
    ''' </summary>
    ''' <param name="CheckListEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte CheckListe-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewCheckList(CheckListEntity As CheckList) As ServerResult Implements ICheckListManagement.CreateNewCheckList
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim CheckListEntryPointResult as CheckList

        Try
            NLOGLOGGER.Info("New CheckList will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			CheckListEntryPointResult = EntryPoint_BeforeAddNewCheckList(CheckListEntity)
			If CheckListEntryPointResult IsNot Nothing then
				CheckListEntity = CheckListEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = CheckAddNewCheckList(CheckListEntity)
			If canAddNew = True Then
			canAddNew = EntryPoint_CheckAddNewCheckList(DbCtx, CheckListEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If

			End If

            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With CheckListEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.CheckLists.Add(CheckListEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New CheckList is successfull create in database")
                _result.ReturnValue = CheckListEntity.GUID.ToString
                NLOGLOGGER.Info("=> CheckListGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewCheckList")
				_resultAfterAddNew = EntryPoint_AfterAddNewCheckList(CheckListEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewCheckList")

            Else
                NLOGLOGGER.Error("CheckList cannot added.")
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
    ''' Kopieren einer vorhanden CheckListe mit allen Eigenschaften
    ''' </summary>
    ''' <param name="CheckListEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyCheckList(CheckListEntity As CheckList) As ServerResult Implements ICheckListManagement.CopyCheckList

        Dim _copyCheckList As New CheckList
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.CheckLists.Add(_copyCheckList)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(CheckListEntity).CurrentValues
			DbCtx.Entry(_copyCheckList).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyCheckList = EntryPoint_CopyCheckList(CheckListEntity)

			' Anpassen der Historieninformationen
			With _copyCheckList
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
    ''' <param name="CheckListEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyCheckList(CheckListEntity As CheckList) As ServerResult Implements ICheckListManagement.ModifyCheckList
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim CheckListEntryPointResult as CheckList

        Dim serverCheckList As CheckList

        Try
            NLOGLOGGER.Info("CheckList will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverCheckList = GetCheckList(CheckListEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyCheckList")
			_resultBeforeModify = EntryPoint_BeforeModifyCheckList(serverCheckList)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyCheckList")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyCheckList")
            CheckListEntryPointResult = EntryPoint_ModifyEntityBeforeModifyCheckList(CheckListEntity)
			If CheckListEntryPointResult IsNot Nothing then
				CheckListEntity = CheckListEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyCheckList")

			' Anpassen der Historieninformationen
			With CheckListEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverCheckList).CurrentValues.SetValues(CheckListEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyCheckList")
			_resultAfterModify = EntryPoint_AfterModifyCheckList(serverCheckList)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyCheckList")

            NLOGLOGGER.Debug("CheckList is successfull modiefied in database")

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
    ''' Diese Methode löscht einen CheckListe 
    ''' </summary>
    ''' <param name="CheckListGUID">Die CheckListGUID des CheckListe, der gelöscht werden soll</param>
    ''' <returns>Wurde der CheckListe gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteCheckList(CheckListGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements ICheckListManagement.DeleteCheckList

        Dim serverCheckLists As List(Of CheckList)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete CheckList")

            NLOGLOGGER.Debug("=> Try to find CheckList-id '" & CheckListGUID & "'")
            guid = guidConvert.ConvertFromString(CheckListGUID)
            serverCheckLists = (From CheckList In DbCtx.CheckLists Where CheckList.GUID = guid).ToList

            If serverCheckLists.Count = 1 Then
                NLOGLOGGER.Debug("=> CheckList-Id '" & CheckListGUID & "' found")
                If serverCheckLists(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.CheckLists.Remove(serverCheckLists(0))
                    Else
						' Anpassen der Historieninformationen
						With serverCheckLists(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteCheckList")
					_resultAfterDelete = EntryPoint_AfterDeleteCheckList(serverCheckLists(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteCheckList")
			
                    NLOGLOGGER.Info("=> CheckList-Id '" & CheckListGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> CheckList-Id '" & CheckListGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> CheckList-Id '" & CheckListGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren CheckListen
    ''' </summary>
    ''' <param name="CheckLists"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteCheckLists(CheckLists As CheckList(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements ICheckListManagement.DeleteCheckLists
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & CheckLists.Count & " CheckLists Collection")

            For Each _CheckList As CheckList In CheckLists
                _resultAfterDelete = DeleteCheckList(_CheckList.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete CheckLists Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete CheckLists Collection with errors")
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
    ''' <param name="CheckListEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckAddNewCheckList(CheckListEntity As CheckList) As Boolean

        Dim isUniqueCheckListName As Boolean

        ' Überprüfen auf Eindeutigkeit, da EF sowas nicht anbietet
        ' Any() stops at the first match and doesn't have to enumerate the entire sequence
        isUniqueCheckListName = Not DbCtx.CheckLists.Any(Function(u) u.Name = CheckListEntity.Name)

        Return isUniqueCheckListName

    End Function
#End Region
#End Region

End Class

