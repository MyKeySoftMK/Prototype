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
Imports MYKEY.FxCore.DataAccess.FxNTGroupLists
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Class GroupListManagement

    Inherits ClassBase
    Implements IGroupListManagement
	Implements IGroupListManagement_ModelEntryPoints
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
        Return "FxNTGroupLists.FxNTGroupListsEDM"
    End Function

    Public Function EntityModelAssembly() As String
        Return "MYKEY.FxCore.DataAccess"
    End Function

    Private Sub Initialize()

        Try

            ' Einstellungen lesen
            NLOGLOGGER.Debug("=> Initialize Component 'GroupListManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'GroupListManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'GroupListManagement' cannot initialized. Check the database connection first.")

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
    Private _DbCtx As FxNTGroupLists.Entities
    Private ReadOnly Property DbCtx As FxNTGroupLists.Entities
        Get
            Try

                If _DbCtx Is Nothing Then
                    NLOGLOGGER.Debug("Try to create a new DbContext-Object")

                    ' Erzeugen eines DataContext 
                    _DbCtx = New FxNTGroupLists.Entities(EntityConnectionData.EntityConnectionString)

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

    Private ReadOnly Property DefaultQueryGroupLists As DbQuery(Of GroupList)
        Get

            Try

                Dim defaultQuery As DbQuery(Of GroupList) = DbCtx.GroupLists
	            Dim _defaultQueryResult As DbQuery(Of GroupList)

				' Order-By Einstellungen festlegen
				defaultQuery = defaultQuery.OrderBy(Function (entity) entity.Value) 

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
    ''' Gibt eine unsortierte Liste der Gruppenlisten aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetGroupLists(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of GroupList) Implements IGroupListManagement.GetGroupLists
        Dim serverGroupLists As DbQuery(Of GroupList)

        NLOGLOGGER.Info("Get List of GroupLists ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverGroupLists = DefaultQueryGroupLists.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverGroupLists = DefaultQueryGroupLists
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverGroupLists.Count & " GroupList/GroupLists in database")

        Return New ObservableCollection(Of GroupList)(serverGroupLists)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Gruppenlisten aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetGroupLists(GroupListGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of GroupList) Implements IGroupListManagement.GetGroupLists
        Dim serverGroupLists As DbQuery(Of GroupList)

        NLOGLOGGER.Info("Get List of GroupLists ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverGroupLists = DefaultQueryGroupLists.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverGroupLists = DefaultQueryGroupLists.Where(Function(_GroupList) GroupListGuids.Contains(_GroupList.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverGroupLists.Count & " GroupList/GroupLists in database")

        Return New ObservableCollection(Of GroupList)(serverGroupLists)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Gruppenliste zurück
    ''' </summary>
    ''' <param name="GroupListGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetGroupList(GroupListGUID As String, Optional QParam As QueryParameters = Nothing) As GroupList Implements IGroupListManagement.GetGroupList
        Dim serverGroupList As GroupList = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get GroupList Informations ")

            guid = guidConvert.ConvertFromString(GroupListGUID)
            NLOGLOGGER.Debug("=> Search for GroupList with GUID '" & GroupListGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverGroupList = DefaultQueryGroupLists.Single(Function(GroupLists) GroupLists.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found GroupList '" & serverGroupList.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverGroupList
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Gruppenliste
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewGroupList() As ServerResult Implements IGroupListManagement.CreateNewGroupList
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewGroupList As GroupList

        Try
            NLOGLOGGER.Info("New GroupList will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewGroupList = EntryPoint_GenerateValidGroupListEntity()

			' Anpassen der Historieninformationen
			With _NewGroupList
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.GroupLists.Add(_NewGroupList)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New GroupList is successfull create in database")
            _result.ReturnValue = _NewGroupList.GUID.ToString
            NLOGLOGGER.Info("=> GroupListGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewGroupList")
			_resultAfterAdd = EntryPoint_AfterAddNewGroupList(_NewGroupList)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewGroupList")

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
    ''' Erstellt einen neuen Gruppenliste aus einer Gruppenliste-Entität
    ''' </summary>
    ''' <param name="GroupListEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Gruppenliste-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewGroupList(GroupListEntity As GroupList) As ServerResult Implements IGroupListManagement.CreateNewGroupList
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim GroupListEntryPointResult as GroupList

        Try
            NLOGLOGGER.Info("New GroupList will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			GroupListEntryPointResult = EntryPoint_BeforeAddNewGroupList(GroupListEntity)
			If GroupListEntryPointResult IsNot Nothing then
				GroupListEntity = GroupListEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = EntryPoint_CheckAddNewGroupList(DbCtx, GroupListEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If


            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With GroupListEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.GroupLists.Add(GroupListEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New GroupList is successfull create in database")
                _result.ReturnValue = GroupListEntity.GUID.ToString
                NLOGLOGGER.Info("=> GroupListGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewGroupList")
				_resultAfterAddNew = EntryPoint_AfterAddNewGroupList(GroupListEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewGroupList")

            Else
                NLOGLOGGER.Error("GroupList cannot added.")
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
    ''' Kopieren einer vorhanden Gruppenliste mit allen Eigenschaften
    ''' </summary>
    ''' <param name="GroupListEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyGroupList(GroupListEntity As GroupList) As ServerResult Implements IGroupListManagement.CopyGroupList

        Dim _copyGroupList As New GroupList
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.GroupLists.Add(_copyGroupList)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(GroupListEntity).CurrentValues
			DbCtx.Entry(_copyGroupList).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyGroupList = EntryPoint_CopyGroupList(GroupListEntity)

			' Anpassen der Historieninformationen
			With _copyGroupList
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
    ''' <param name="GroupListEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyGroupList(GroupListEntity As GroupList) As ServerResult Implements IGroupListManagement.ModifyGroupList
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim GroupListEntryPointResult as GroupList

        Dim serverGroupList As GroupList

        Try
            NLOGLOGGER.Info("GroupList will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverGroupList = GetGroupList(GroupListEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyGroupList")
			_resultBeforeModify = EntryPoint_BeforeModifyGroupList(serverGroupList)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyGroupList")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyGroupList")
            GroupListEntryPointResult = EntryPoint_ModifyEntityBeforeModifyGroupList(GroupListEntity)
			If GroupListEntryPointResult IsNot Nothing then
				GroupListEntity = GroupListEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyGroupList")

			' Anpassen der Historieninformationen
			With GroupListEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverGroupList).CurrentValues.SetValues(GroupListEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyGroupList")
			_resultAfterModify = EntryPoint_AfterModifyGroupList(serverGroupList)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyGroupList")

            NLOGLOGGER.Debug("GroupList is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Gruppenliste 
    ''' </summary>
    ''' <param name="GroupListGUID">Die GroupListGUID des Gruppenliste, der gelöscht werden soll</param>
    ''' <returns>Wurde der Gruppenliste gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteGroupList(GroupListGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IGroupListManagement.DeleteGroupList

        Dim serverGroupLists As List(Of GroupList)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete GroupList")

            NLOGLOGGER.Debug("=> Try to find GroupList-id '" & GroupListGUID & "'")
            guid = guidConvert.ConvertFromString(GroupListGUID)
            serverGroupLists = (From GroupList In DbCtx.GroupLists Where GroupList.GUID = guid).ToList

            If serverGroupLists.Count = 1 Then
                NLOGLOGGER.Debug("=> GroupList-Id '" & GroupListGUID & "' found")
                If serverGroupLists(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.GroupLists.Remove(serverGroupLists(0))
                    Else
						' Anpassen der Historieninformationen
						With serverGroupLists(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteGroupList")
					_resultAfterDelete = EntryPoint_AfterDeleteGroupList(serverGroupLists(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteGroupList")
			
                    NLOGLOGGER.Info("=> GroupList-Id '" & GroupListGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> GroupList-Id '" & GroupListGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> GroupList-Id '" & GroupListGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren Gruppenlisten
    ''' </summary>
    ''' <param name="GroupLists"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteGroupLists(GroupLists As GroupList(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IGroupListManagement.DeleteGroupLists
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & GroupLists.Count & " GroupLists Collection")

            For Each _GroupList As GroupList In GroupLists
                _resultAfterDelete = DeleteGroupList(_GroupList.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete GroupLists Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete GroupLists Collection with errors")
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

