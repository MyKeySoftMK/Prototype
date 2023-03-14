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

Partial Class PrintGroupManagement

    Inherits ClassBase
    Implements IPrintGroupManagement
	Implements IPrintGroupManagement_ModelEntryPoints
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
            NLOGLOGGER.Debug("=> Initialize Component 'PrintGroupManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'PrintGroupManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'PrintGroupManagement' cannot initialized. Check the database connection first.")

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

    Private ReadOnly Property DefaultQueryPrintGroups As DbQuery(Of PrintGroup)
        Get

            Try

                Dim defaultQuery As DbQuery(Of PrintGroup) = DbCtx.PrintGroups
	            Dim _defaultQueryResult As DbQuery(Of PrintGroup)


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
    ''' Gibt eine unsortierte Liste der AusdrucksGroupen aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPrintGroups(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of PrintGroup) Implements IPrintGroupManagement.GetPrintGroups
        Dim serverPrintGroups As DbQuery(Of PrintGroup)

        NLOGLOGGER.Info("Get List of PrintGroups ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverPrintGroups = DefaultQueryPrintGroups.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverPrintGroups = DefaultQueryPrintGroups
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverPrintGroups.Count & " PrintGroup/PrintGroups in database")

        Return New ObservableCollection(Of PrintGroup)(serverPrintGroups)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der AusdrucksGroupen aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPrintGroups(PrintGroupGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of PrintGroup) Implements IPrintGroupManagement.GetPrintGroups
        Dim serverPrintGroups As DbQuery(Of PrintGroup)

        NLOGLOGGER.Info("Get List of PrintGroups ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverPrintGroups = DefaultQueryPrintGroups.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverPrintGroups = DefaultQueryPrintGroups.Where(Function(_PrintGroup) PrintGroupGuids.Contains(_PrintGroup.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverPrintGroups.Count & " PrintGroup/PrintGroups in database")

        Return New ObservableCollection(Of PrintGroup)(serverPrintGroups)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Ausdrucksgruppe zurück
    ''' </summary>
    ''' <param name="PrintGroupGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetPrintGroup(PrintGroupGUID As String, Optional QParam As QueryParameters = Nothing) As PrintGroup Implements IPrintGroupManagement.GetPrintGroup
        Dim serverPrintGroup As PrintGroup = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get PrintGroup Informations ")

            guid = guidConvert.ConvertFromString(PrintGroupGUID)
            NLOGLOGGER.Debug("=> Search for PrintGroup with GUID '" & PrintGroupGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverPrintGroup = DefaultQueryPrintGroups.Single(Function(PrintGroups) PrintGroups.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found PrintGroup '" & serverPrintGroup.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverPrintGroup
    End Function

#Region "AllocationListBox: FxNTPrintsPrintToPrintGroup"

    ''' <summary>
    ''' Gibt die Informationen für einen PrintToPrintGroups zurück
    ''' </summary>
    ''' <param name="FxNTPrintsPrintGroupGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetFxNTPrintsPrintGroup(FxNTPrintsPrintGroupGUID As String, Optional QParam As QueryParameters = Nothing) As FxNTPrints.PrintGroup
        Dim serverFxNTPrintsPrintGroup As List (Of FxNTPrints.PrintGroup) 
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid

        Try
            NLOGLOGGER.Info("Get FxNTPrints.PrintGroup Informations ")

            _guid = guidConvert.ConvertFromString(FxNTPrintsPrintGroupGUID)
            NLOGLOGGER.Debug("=> Search for FxNTPrints.PrintGroup with GUID '" & FxNTPrintsPrintGroupGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverFxNTPrintsPrintGroup = (From FxNTPrintsPrintGroup In DbCtx.PrintGroups
                                    Where FxNTPrintsPrintGroup.GUID = _guid
                                    Select FxNTPrintsPrintGroup).ToList
            End With

            NLOGLOGGER.Debug("=> Found FxNTPrints.PrintGroup '" & serverFxNTPrintsPrintGroup(0).GUID.ToString & "'")
        Catch ex As Exception
			serverFxNTPrintsPrintGroup = New List (Of FxNTPrints.PrintGroup)
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverFxNTPrintsPrintGroup(0)
    End Function

    ''' <summary>
    ''' Gibt die Informationen für alle AusdrucksGroupen zurück
    ''' </summary>
    ''' <param name="GUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetFxNTPrintsPrintGroupAllPrintMembers(GUID As String, Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTPrints.PrintToPrintGroup)
        Dim serverPrints As List(Of FxNTPrints.PrintToPrintGroup) 
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid

        Try
            NLOGLOGGER.Info("Get FxNTPrints.PrintGroup Informations")

            _guid = guidConvert.ConvertFromString(GUID)
            NLOGLOGGER.Debug("=> Search for FxNTPrints.PrintGroup with GUID '" & GUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverPrints = (From FxNTPrintsPrintGroup In DbCtx.PrintGroups
                                    From Print In FxNTPrintsPrintGroup.Prints
                                    Where FxNTPrintsPrintGroup.GUID = _guid
                                    Select Print).ToList

            End With

			NLOGLOGGER.Debug("=> Found " & serverPrints.Count & " Prints in database")

        Catch ex As Exception
			serverPrints = New List(Of FxNTPrints.PrintToPrintGroup)
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return New ObservableCollection(Of FxNTPrints.PrintToPrintGroup)(serverPrints)
    End Function

#End Region

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Ausdrucksgruppe
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewPrintGroup() As ServerResult Implements IPrintGroupManagement.CreateNewPrintGroup
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewPrintGroup As PrintGroup

        Try
            NLOGLOGGER.Info("New PrintGroup will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewPrintGroup = EntryPoint_GenerateValidPrintGroupEntity()

			' Anpassen der Historieninformationen
			With _NewPrintGroup
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.PrintGroups.Add(_NewPrintGroup)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New PrintGroup is successfull create in database")
            _result.ReturnValue = _NewPrintGroup.GUID.ToString
            NLOGLOGGER.Info("=> PrintGroupGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewPrintGroup")
			_resultAfterAdd = EntryPoint_AfterAddNewPrintGroup(_NewPrintGroup)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewPrintGroup")

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
    ''' Erstellt einen neuen Ausdrucksgruppe aus einer Ausdrucksgruppe-Entität
    ''' </summary>
    ''' <param name="PrintGroupEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Ausdrucksgruppe-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewPrintGroup(PrintGroupEntity As PrintGroup) As ServerResult Implements IPrintGroupManagement.CreateNewPrintGroup
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim PrintGroupEntryPointResult as PrintGroup

        Try
            NLOGLOGGER.Info("New PrintGroup will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			PrintGroupEntryPointResult = EntryPoint_BeforeAddNewPrintGroup(PrintGroupEntity)
			If PrintGroupEntryPointResult IsNot Nothing then
				PrintGroupEntity = PrintGroupEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = CheckAddNewPrintGroup(PrintGroupEntity)
			If canAddNew = True Then
			canAddNew = EntryPoint_CheckAddNewPrintGroup(DbCtx, PrintGroupEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If

			End If

            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With PrintGroupEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.PrintGroups.Add(PrintGroupEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New PrintGroup is successfull create in database")
                _result.ReturnValue = PrintGroupEntity.GUID.ToString
                NLOGLOGGER.Info("=> PrintGroupGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewPrintGroup")
				_resultAfterAddNew = EntryPoint_AfterAddNewPrintGroup(PrintGroupEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewPrintGroup")

            Else
                NLOGLOGGER.Error("PrintGroup cannot added.")
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
    ''' Kopieren einer vorhanden Ausdrucksgruppe mit allen Eigenschaften
    ''' </summary>
    ''' <param name="PrintGroupEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyPrintGroup(PrintGroupEntity As PrintGroup) As ServerResult Implements IPrintGroupManagement.CopyPrintGroup

        Dim _copyPrintGroup As New PrintGroup
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.PrintGroups.Add(_copyPrintGroup)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(PrintGroupEntity).CurrentValues
			DbCtx.Entry(_copyPrintGroup).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyPrintGroup = EntryPoint_CopyPrintGroup(PrintGroupEntity)

			' Anpassen der Historieninformationen
			With _copyPrintGroup
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
    ''' <param name="PrintGroupEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyPrintGroup(PrintGroupEntity As PrintGroup) As ServerResult Implements IPrintGroupManagement.ModifyPrintGroup
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim PrintGroupEntryPointResult as PrintGroup

        Dim serverPrintGroup As PrintGroup

        Try
            NLOGLOGGER.Info("PrintGroup will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverPrintGroup = GetPrintGroup(PrintGroupEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyPrintGroup")
			_resultBeforeModify = EntryPoint_BeforeModifyPrintGroup(serverPrintGroup)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyPrintGroup")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyPrintGroup")
            PrintGroupEntryPointResult = EntryPoint_ModifyEntityBeforeModifyPrintGroup(PrintGroupEntity)
			If PrintGroupEntryPointResult IsNot Nothing then
				PrintGroupEntity = PrintGroupEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyPrintGroup")

			' Anpassen der Historieninformationen
			With PrintGroupEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverPrintGroup).CurrentValues.SetValues(PrintGroupEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyPrintGroup")
			_resultAfterModify = EntryPoint_AfterModifyPrintGroup(serverPrintGroup)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyPrintGroup")

            NLOGLOGGER.Debug("PrintGroup is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Ausdrucksgruppe 
    ''' </summary>
    ''' <param name="PrintGroupGUID">Die PrintGroupGUID des Ausdrucksgruppe, der gelöscht werden soll</param>
    ''' <returns>Wurde der Ausdrucksgruppe gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeletePrintGroup(PrintGroupGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IPrintGroupManagement.DeletePrintGroup

        Dim serverPrintGroups As List(Of PrintGroup)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete PrintGroup")

            NLOGLOGGER.Debug("=> Try to find PrintGroup-id '" & PrintGroupGUID & "'")
            guid = guidConvert.ConvertFromString(PrintGroupGUID)
            serverPrintGroups = (From PrintGroup In DbCtx.PrintGroups Where PrintGroup.GUID = guid).ToList

            If serverPrintGroups.Count = 1 Then
                NLOGLOGGER.Debug("=> PrintGroup-Id '" & PrintGroupGUID & "' found")
                If serverPrintGroups(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.PrintGroups.Remove(serverPrintGroups(0))
                    Else
						' Anpassen der Historieninformationen
						With serverPrintGroups(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeletePrintGroup")
					_resultAfterDelete = EntryPoint_AfterDeletePrintGroup(serverPrintGroups(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeletePrintGroup")
			
                    NLOGLOGGER.Info("=> PrintGroup-Id '" & PrintGroupGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> PrintGroup-Id '" & PrintGroupGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> PrintGroup-Id '" & PrintGroupGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren AusdrucksGroupen
    ''' </summary>
    ''' <param name="PrintGroups"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeletePrintGroups(PrintGroups As PrintGroup(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IPrintGroupManagement.DeletePrintGroups
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & PrintGroups.Count & " PrintGroups Collection")

            For Each _PrintGroup As PrintGroup In PrintGroups
                _resultAfterDelete = DeletePrintGroup(_PrintGroup.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete PrintGroups Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete PrintGroups Collection with errors")
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
    ''' <param name="PrintGroupEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckAddNewPrintGroup(PrintGroupEntity As PrintGroup) As Boolean

        Dim isUniquePrintGroupName As Boolean

        ' Überprüfen auf Eindeutigkeit, da EF sowas nicht anbietet
        ' Any() stops at the first match and doesn't have to enumerate the entire sequence
        isUniquePrintGroupName = Not DbCtx.PrintGroups.Any(Function(u) u.Name = PrintGroupEntity.Name)

        Return isUniquePrintGroupName

    End Function
#End Region

#Region "AllocationListBoxes"

    ''' <summary>
    ''' Mit dieser Methode fügt man einen Ausdrucke einer Ausdrucksgruppe hinzu.
    ''' </summary>
    ''' <param name="PrintGUID">Print-GUID die hinzugefügt werden soll</param>
    ''' <param name="FxNTPrintsPrintGroupGUID">Ausdrucksgruppe-GUID der Gruppe die den Benutzer enthalten soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddPrintToFxNTPrintsPrintGroup(PrintGUID As String, FxNTPrintsPrintGroupGUID As String) As ServerResult

        Dim newPrintToFxNTPrintsPrintGroup As FxNTPrints.PrintToPrintGroup
        Dim _result As New ServerResult
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid
		Dim qpara As New QueryParameters

        Try
            NLOGLOGGER.Info("Print will assign to a FxNTPrintsPrintGroup")

            newPrintToFxNTPrintsPrintGroup = New FxNTPrints.PrintToPrintGroup
            With newPrintToFxNTPrintsPrintGroup
                ' Benötigte Informationen
                _guid = guidConvert.ConvertFromString(PrintGUID)
                .PrintGUID = _guid
                NLOGLOGGER.Debug("=> PrintGUID: " & PrintGUID)

                _guid = guidConvert.ConvertFromString(FxNTPrintsPrintGroupGUID)
                .PrintGroupGUID = _guid
                NLOGLOGGER.Debug("=> PrintGroupGUID: " & FxNTPrintsPrintGroupGUID)
            End With

            DbCtx.PrintToPrintGroups.Add(newPrintToFxNTPrintsPrintGroup)
            DbCtx.SaveChanges()

			' Frisches Einlesen der Entität
			qpara.Includes.Add("Prints")
			TryCast(DbCtx, IObjectContextAdapter).ObjectContext.Detach(GetFxNTPrintsPrintGroup(FxNTPrintsPrintGroupGUID, qpara))
            Dim srvFxNTPrintsPrintGroup As FxNTPrints.PrintGroup = DbCtx.PrintGroups.Find(newPrintToFxNTPrintsPrintGroup.PrintGroupGUID)


            NLOGLOGGER.Info("Print is successfull assign to FxNTPrints.PrintGroup")
            _result.ReturnValue = newPrintToFxNTPrintsPrintGroup.GUID.ToString
            NLOGLOGGER.Info("=> EntryGUID: " & _result.ReturnValue)

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
    ''' Mit dieser Methode entfernt man einen Ausdrucke aus einer Ausdrucksgruppe .
    ''' </summary>
    ''' <param name="PrintGUID">Print-GUID die hinzugefügt werden soll</param>
    ''' <param name="FxNTPrintsPrintGroupGUID">Ausdrucksgruppe-GUID der Gruppe die den Benutzer enthalten soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Public Function RemovePrintFromFxNTPrintsPrintGroup(PrintGUID As String, FxNTPrintsPrintGroupGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

        Dim serverPrintGroups As List(Of FxNTPrints.PrintGroup)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
        Dim PrintFound As Boolean = False
		Dim qpara As New QueryParameters

        Try
            NLOGLOGGER.Info("Remove Print From FxNTPrints.PrintGroup")

            NLOGLOGGER.Debug("=> Try to find FxNTPrintsPrintGroup-id '" & FxNTPrintsPrintGroupGUID & "'")
            guid = guidConvert.ConvertFromString(FxNTPrintsPrintGroupGUID)
            serverPrintGroups = (From FxNTPrintsPrintGroup In DbCtx.PrintGroups.Include("Prints") Where FxNTPrintsPrintGroup.GUID = guid).ToList

            If serverPrintGroups.Count = 1 Then
                NLOGLOGGER.Debug("=> FxNTPrintsPrintGroup-Id '" & FxNTPrintsPrintGroupGUID & "' found")

                NLOGLOGGER.Debug("=> Search for Print-id '" & PrintGUID & "'")
                For Each entry As FxNTPrints.PrintToPrintGroup In serverPrintGroups(0).Prints

                    If entry.PrintGUID.ToString = PrintGUID Then

                        PrintFound = True

                        If entry.CanNotDelete = False Then

                            If PermanentlyDelete = True Then
                                DbCtx.Entry(entry).State = Entity.EntityState.Deleted
                            Else
                                entry.Deleted = Now
                            End If
                            DbCtx.SaveChanges()

							' Frisches Einlesen der Entität
							qpara.Includes.Add("Prints")
							TryCast(DbCtx, IObjectContextAdapter).ObjectContext.Detach(GetFxNTPrintsPrintGroup(FxNTPrintsPrintGroupGUID, qpara))
							Dim srvFxNTPrintsPrintGroup As FxNTPrints.PrintGroup = DbCtx.PrintGroups.Find(serverPrintGroups(0).Guid)

                            NLOGLOGGER.Info("=> Print-Id '" & FxNTPrintsPrintGroupGUID & "' REMOVED")

                        Else
                            NLOGLOGGER.Debug("=> Print-Id '" & FxNTPrintsPrintGroupGUID & "' FAILED to delete")
                            _result.ErrorMessages.Add("Print  cannot delete - Delete not allowed")
                        End If
                        Exit For
                    End If
                Next
                If PrintFound = False Then
                    NLOGLOGGER.Debug("=> Print-Id '" & FxNTPrintsPrintGroupGUID & "' not in UserGroup")
                    _result.ErrorMessages.Add("Print cannot delete - Print not found in Group")
                End If

            Else
                NLOGLOGGER.Debug("=> FxNTPrints.PrintGroup-Id '" & FxNTPrintsPrintGroupGUID & "' not in database")
                _result.ErrorMessages.Add("Print cannot delete from FxNTPrints.PrintGroup - FxNTPrints.PrintGroup not found in database")
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
 
  #End Region
#End Region

End Class

