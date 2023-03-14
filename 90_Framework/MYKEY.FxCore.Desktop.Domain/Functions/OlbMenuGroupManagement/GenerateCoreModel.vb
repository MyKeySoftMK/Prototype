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

Partial Class OlbMenuGroupManagement

    Inherits ClassBase
    Implements IOlbMenuGroupManagement
	Implements IOlbMenuGroupManagement_ModelEntryPoints
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
            NLOGLOGGER.Debug("=> Initialize Component 'OlbMenuGroupManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'OlbMenuGroupManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'OlbMenuGroupManagement' cannot initialized. Check the database connection first.")

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

    Private ReadOnly Property DefaultQueryMenuGroups As DbQuery(Of MenuGroup)
        Get

            Try

                Dim defaultQuery As DbQuery(Of MenuGroup) = DbCtx.MenuGroups
	            Dim _defaultQueryResult As DbQuery(Of MenuGroup)


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
    ''' Gibt eine unsortierte Liste der Menuegruppen aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMenuGroups(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of MenuGroup) Implements IOlbMenuGroupManagement.GetMenuGroups
        Dim serverMenuGroups As DbQuery(Of MenuGroup)

        NLOGLOGGER.Info("Get List of MenuGroups ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverMenuGroups = DefaultQueryMenuGroups.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverMenuGroups = DefaultQueryMenuGroups
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverMenuGroups.Count & " MenuGroup/MenuGroups in database")

        Return New ObservableCollection(Of MenuGroup)(serverMenuGroups)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Menuegruppen aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMenuGroups(MenuGroupGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of MenuGroup) Implements IOlbMenuGroupManagement.GetMenuGroups
        Dim serverMenuGroups As DbQuery(Of MenuGroup)

        NLOGLOGGER.Info("Get List of MenuGroups ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverMenuGroups = DefaultQueryMenuGroups.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverMenuGroups = DefaultQueryMenuGroups.Where(Function(_MenuGroup) MenuGroupGuids.Contains(_MenuGroup.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverMenuGroups.Count & " MenuGroup/MenuGroups in database")

        Return New ObservableCollection(Of MenuGroup)(serverMenuGroups)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Menuegruppe zurück
    ''' </summary>
    ''' <param name="MenuGroupGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetMenuGroup(MenuGroupGUID As String, Optional QParam As QueryParameters = Nothing) As MenuGroup Implements IOlbMenuGroupManagement.GetMenuGroup
        Dim serverMenuGroup As MenuGroup = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get MenuGroup Informations ")

            guid = guidConvert.ConvertFromString(MenuGroupGUID)
            NLOGLOGGER.Debug("=> Search for MenuGroup with GUID '" & MenuGroupGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverMenuGroup = DefaultQueryMenuGroups.Single(Function(MenuGroups) MenuGroups.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found MenuGroup '" & serverMenuGroup.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverMenuGroup
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Menuegruppe
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewMenuGroup() As ServerResult Implements IOlbMenuGroupManagement.CreateNewMenuGroup
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewMenuGroup As MenuGroup

        Try
            NLOGLOGGER.Info("New MenuGroup will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewMenuGroup = EntryPoint_GenerateValidMenuGroupEntity()

			' Anpassen der Historieninformationen
			With _NewMenuGroup
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.MenuGroups.Add(_NewMenuGroup)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New MenuGroup is successfull create in database")
            _result.ReturnValue = _NewMenuGroup.GUID.ToString
            NLOGLOGGER.Info("=> MenuGroupGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewMenuGroup")
			_resultAfterAdd = EntryPoint_AfterAddNewMenuGroup(_NewMenuGroup)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewMenuGroup")

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
    ''' Erstellt einen neuen Menuegruppe aus einer Menuegruppe-Entität
    ''' </summary>
    ''' <param name="MenuGroupEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Menuegruppe-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewMenuGroup(MenuGroupEntity As MenuGroup) As ServerResult Implements IOlbMenuGroupManagement.CreateNewMenuGroup
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim MenuGroupEntryPointResult as MenuGroup

        Try
            NLOGLOGGER.Info("New MenuGroup will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			MenuGroupEntryPointResult = EntryPoint_BeforeAddNewMenuGroup(MenuGroupEntity)
			If MenuGroupEntryPointResult IsNot Nothing then
				MenuGroupEntity = MenuGroupEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = EntryPoint_CheckAddNewMenuGroup(DbCtx, MenuGroupEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If


            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With MenuGroupEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.MenuGroups.Add(MenuGroupEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New MenuGroup is successfull create in database")
                _result.ReturnValue = MenuGroupEntity.GUID.ToString
                NLOGLOGGER.Info("=> MenuGroupGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewMenuGroup")
				_resultAfterAddNew = EntryPoint_AfterAddNewMenuGroup(MenuGroupEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewMenuGroup")

            Else
                NLOGLOGGER.Error("MenuGroup cannot added.")
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
    ''' Kopieren einer vorhanden Menuegruppe mit allen Eigenschaften
    ''' </summary>
    ''' <param name="MenuGroupEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyMenuGroup(MenuGroupEntity As MenuGroup) As ServerResult Implements IOlbMenuGroupManagement.CopyMenuGroup

        Dim _copyMenuGroup As New MenuGroup
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.MenuGroups.Add(_copyMenuGroup)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(MenuGroupEntity).CurrentValues
			DbCtx.Entry(_copyMenuGroup).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyMenuGroup = EntryPoint_CopyMenuGroup(MenuGroupEntity)

			' Anpassen der Historieninformationen
			With _copyMenuGroup
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
    ''' <param name="MenuGroupEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyMenuGroup(MenuGroupEntity As MenuGroup) As ServerResult Implements IOlbMenuGroupManagement.ModifyMenuGroup
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim MenuGroupEntryPointResult as MenuGroup

        Dim serverMenuGroup As MenuGroup

        Try
            NLOGLOGGER.Info("MenuGroup will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverMenuGroup = GetMenuGroup(MenuGroupEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyMenuGroup")
			_resultBeforeModify = EntryPoint_BeforeModifyMenuGroup(serverMenuGroup)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyMenuGroup")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyMenuGroup")
            MenuGroupEntryPointResult = EntryPoint_ModifyEntityBeforeModifyMenuGroup(MenuGroupEntity)
			If MenuGroupEntryPointResult IsNot Nothing then
				MenuGroupEntity = MenuGroupEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyMenuGroup")

			' Anpassen der Historieninformationen
			With MenuGroupEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverMenuGroup).CurrentValues.SetValues(MenuGroupEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyMenuGroup")
			_resultAfterModify = EntryPoint_AfterModifyMenuGroup(serverMenuGroup)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyMenuGroup")

            NLOGLOGGER.Debug("MenuGroup is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Menuegruppe 
    ''' </summary>
    ''' <param name="MenuGroupGUID">Die MenuGroupGUID des Menuegruppe, der gelöscht werden soll</param>
    ''' <returns>Wurde der Menuegruppe gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteMenuGroup(MenuGroupGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IOlbMenuGroupManagement.DeleteMenuGroup

        Dim serverMenuGroups As List(Of MenuGroup)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete MenuGroup")

            NLOGLOGGER.Debug("=> Try to find MenuGroup-id '" & MenuGroupGUID & "'")
            guid = guidConvert.ConvertFromString(MenuGroupGUID)
            serverMenuGroups = (From MenuGroup In DbCtx.MenuGroups Where MenuGroup.GUID = guid).ToList

            If serverMenuGroups.Count = 1 Then
                NLOGLOGGER.Debug("=> MenuGroup-Id '" & MenuGroupGUID & "' found")
                If serverMenuGroups(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.MenuGroups.Remove(serverMenuGroups(0))
                    Else
						' Anpassen der Historieninformationen
						With serverMenuGroups(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteMenuGroup")
					_resultAfterDelete = EntryPoint_AfterDeleteMenuGroup(serverMenuGroups(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteMenuGroup")
			
                    NLOGLOGGER.Info("=> MenuGroup-Id '" & MenuGroupGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> MenuGroup-Id '" & MenuGroupGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> MenuGroup-Id '" & MenuGroupGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren Menuegruppen
    ''' </summary>
    ''' <param name="MenuGroups"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteMenuGroups(MenuGroups As MenuGroup(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IOlbMenuGroupManagement.DeleteMenuGroups
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & MenuGroups.Count & " MenuGroups Collection")

            For Each _MenuGroup As MenuGroup In MenuGroups
                _resultAfterDelete = DeleteMenuGroup(_MenuGroup.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete MenuGroups Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete MenuGroups Collection with errors")
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

