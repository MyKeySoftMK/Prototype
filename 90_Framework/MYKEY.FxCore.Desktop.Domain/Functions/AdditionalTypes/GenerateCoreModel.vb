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
Imports MYKEY.FxCore.DataAccess.FxNTAdditionalTypes
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Class AdditionalTypeManagement

    Inherits ClassBase
    Implements IAdditionalTypeManagement
	Implements IAdditionalTypeManagement_ModelEntryPoints
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
        Return "FxNTAdditionalTypes.FxNTAdditionalTypesEDM"
    End Function

    Public Function EntityModelAssembly() As String
        Return "MYKEY.FxCore.DataAccess"
    End Function

    Private Sub Initialize()

        Try

            ' Einstellungen lesen
            NLOGLOGGER.Debug("=> Initialize Component 'AdditionalTypeManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'AdditionalTypeManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'AdditionalTypeManagement' cannot initialized. Check the database connection first.")

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
    Private _DbCtx As FxNTAdditionalTypes.Entities
    Private ReadOnly Property DbCtx As FxNTAdditionalTypes.Entities
        Get
            Try

                If _DbCtx Is Nothing Then
                    NLOGLOGGER.Debug("Try to create a new DbContext-Object")

                    ' Erzeugen eines DataContext 
                    _DbCtx = New FxNTAdditionalTypes.Entities(EntityConnectionData.EntityConnectionString)

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

    Private ReadOnly Property DefaultQueryAdditionalTypes As DbQuery(Of AdditionalType)
        Get

            Try

                Dim defaultQuery As DbQuery(Of AdditionalType) = DbCtx.AdditionalTypes
	            Dim _defaultQueryResult As DbQuery(Of AdditionalType)


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
    ''' Gibt eine unsortierte Liste der Zusatztypen aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAdditionalTypes(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of AdditionalType) Implements IAdditionalTypeManagement.GetAdditionalTypes
        Dim serverAdditionalTypes As DbQuery(Of AdditionalType)

        NLOGLOGGER.Info("Get List of AdditionalTypes ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverAdditionalTypes = DefaultQueryAdditionalTypes.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverAdditionalTypes = DefaultQueryAdditionalTypes
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverAdditionalTypes.Count & " AdditionalType/AdditionalTypes in database")

        Return New ObservableCollection(Of AdditionalType)(serverAdditionalTypes)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Zusatztypen aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAdditionalTypes(AdditionalTypeGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of AdditionalType) Implements IAdditionalTypeManagement.GetAdditionalTypes
        Dim serverAdditionalTypes As DbQuery(Of AdditionalType)

        NLOGLOGGER.Info("Get List of AdditionalTypes ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverAdditionalTypes = DefaultQueryAdditionalTypes.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverAdditionalTypes = DefaultQueryAdditionalTypes.Where(Function(_AdditionalType) AdditionalTypeGuids.Contains(_AdditionalType.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverAdditionalTypes.Count & " AdditionalType/AdditionalTypes in database")

        Return New ObservableCollection(Of AdditionalType)(serverAdditionalTypes)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Zusatztyp zurück
    ''' </summary>
    ''' <param name="AdditionalTypeGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetAdditionalType(AdditionalTypeGUID As String, Optional QParam As QueryParameters = Nothing) As AdditionalType Implements IAdditionalTypeManagement.GetAdditionalType
        Dim serverAdditionalType As AdditionalType = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get AdditionalType Informations ")

            guid = guidConvert.ConvertFromString(AdditionalTypeGUID)
            NLOGLOGGER.Debug("=> Search for AdditionalType with GUID '" & AdditionalTypeGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverAdditionalType = DefaultQueryAdditionalTypes.Single(Function(AdditionalTypes) AdditionalTypes.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found AdditionalType '" & serverAdditionalType.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverAdditionalType
    End Function

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Zusatztyp
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewAdditionalType() As ServerResult Implements IAdditionalTypeManagement.CreateNewAdditionalType
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewAdditionalType As AdditionalType

        Try
            NLOGLOGGER.Info("New AdditionalType will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewAdditionalType = EntryPoint_GenerateValidAdditionalTypeEntity()

			' Anpassen der Historieninformationen
			With _NewAdditionalType
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.AdditionalTypes.Add(_NewAdditionalType)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New AdditionalType is successfull create in database")
            _result.ReturnValue = _NewAdditionalType.GUID.ToString
            NLOGLOGGER.Info("=> AdditionalTypeGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewAdditionalType")
			_resultAfterAdd = EntryPoint_AfterAddNewAdditionalType(_NewAdditionalType)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewAdditionalType")

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
    ''' Erstellt einen neuen Zusatztyp aus einer Zusatztyp-Entität
    ''' </summary>
    ''' <param name="AdditionalTypeEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Zusatztyp-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewAdditionalType(AdditionalTypeEntity As AdditionalType) As ServerResult Implements IAdditionalTypeManagement.CreateNewAdditionalType
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim AdditionalTypeEntryPointResult as AdditionalType

        Try
            NLOGLOGGER.Info("New AdditionalType will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			AdditionalTypeEntryPointResult = EntryPoint_BeforeAddNewAdditionalType(AdditionalTypeEntity)
			If AdditionalTypeEntryPointResult IsNot Nothing then
				AdditionalTypeEntity = AdditionalTypeEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = EntryPoint_CheckAddNewAdditionalType(DbCtx, AdditionalTypeEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If


            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With AdditionalTypeEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.AdditionalTypes.Add(AdditionalTypeEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New AdditionalType is successfull create in database")
                _result.ReturnValue = AdditionalTypeEntity.GUID.ToString
                NLOGLOGGER.Info("=> AdditionalTypeGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewAdditionalType")
				_resultAfterAddNew = EntryPoint_AfterAddNewAdditionalType(AdditionalTypeEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewAdditionalType")

            Else
                NLOGLOGGER.Error("AdditionalType cannot added.")
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
    ''' Kopieren einer vorhanden Zusatztyp mit allen Eigenschaften
    ''' </summary>
    ''' <param name="AdditionalTypeEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyAdditionalType(AdditionalTypeEntity As AdditionalType) As ServerResult Implements IAdditionalTypeManagement.CopyAdditionalType

        Dim _copyAdditionalType As New AdditionalType
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.AdditionalTypes.Add(_copyAdditionalType)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(AdditionalTypeEntity).CurrentValues
			DbCtx.Entry(_copyAdditionalType).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyAdditionalType = EntryPoint_CopyAdditionalType(AdditionalTypeEntity)

			' Anpassen der Historieninformationen
			With _copyAdditionalType
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
    ''' <param name="AdditionalTypeEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyAdditionalType(AdditionalTypeEntity As AdditionalType) As ServerResult Implements IAdditionalTypeManagement.ModifyAdditionalType
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim AdditionalTypeEntryPointResult as AdditionalType

        Dim serverAdditionalType As AdditionalType

        Try
            NLOGLOGGER.Info("AdditionalType will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverAdditionalType = GetAdditionalType(AdditionalTypeEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyAdditionalType")
			_resultBeforeModify = EntryPoint_BeforeModifyAdditionalType(serverAdditionalType)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyAdditionalType")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyAdditionalType")
            AdditionalTypeEntryPointResult = EntryPoint_ModifyEntityBeforeModifyAdditionalType(AdditionalTypeEntity)
			If AdditionalTypeEntryPointResult IsNot Nothing then
				AdditionalTypeEntity = AdditionalTypeEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyAdditionalType")

			' Anpassen der Historieninformationen
			With AdditionalTypeEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverAdditionalType).CurrentValues.SetValues(AdditionalTypeEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyAdditionalType")
			_resultAfterModify = EntryPoint_AfterModifyAdditionalType(serverAdditionalType)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyAdditionalType")

            NLOGLOGGER.Debug("AdditionalType is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Zusatztyp 
    ''' </summary>
    ''' <param name="AdditionalTypeGUID">Die AdditionalTypeGUID des Zusatztyp, der gelöscht werden soll</param>
    ''' <returns>Wurde der Zusatztyp gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeleteAdditionalType(AdditionalTypeGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IAdditionalTypeManagement.DeleteAdditionalType

        Dim serverAdditionalTypes As List(Of AdditionalType)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete AdditionalType")

            NLOGLOGGER.Debug("=> Try to find AdditionalType-id '" & AdditionalTypeGUID & "'")
            guid = guidConvert.ConvertFromString(AdditionalTypeGUID)
            serverAdditionalTypes = (From AdditionalType In DbCtx.AdditionalTypes Where AdditionalType.GUID = guid).ToList

            If serverAdditionalTypes.Count = 1 Then
                NLOGLOGGER.Debug("=> AdditionalType-Id '" & AdditionalTypeGUID & "' found")
                If serverAdditionalTypes(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.AdditionalTypes.Remove(serverAdditionalTypes(0))
                    Else
						' Anpassen der Historieninformationen
						With serverAdditionalTypes(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeleteAdditionalType")
					_resultAfterDelete = EntryPoint_AfterDeleteAdditionalType(serverAdditionalTypes(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeleteAdditionalType")
			
                    NLOGLOGGER.Info("=> AdditionalType-Id '" & AdditionalTypeGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> AdditionalType-Id '" & AdditionalTypeGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> AdditionalType-Id '" & AdditionalTypeGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren Zusatztypen
    ''' </summary>
    ''' <param name="AdditionalTypes"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteAdditionalTypes(AdditionalTypes As AdditionalType(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IAdditionalTypeManagement.DeleteAdditionalTypes
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & AdditionalTypes.Count & " AdditionalTypes Collection")

            For Each _AdditionalType As AdditionalType In AdditionalTypes
                _resultAfterDelete = DeleteAdditionalType(_AdditionalType.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete AdditionalTypes Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete AdditionalTypes Collection with errors")
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

