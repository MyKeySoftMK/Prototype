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

Partial Class PrintManagement

    Inherits ClassBase
    Implements IPrintManagement
	Implements IPrintManagement_ModelEntryPoints
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
            NLOGLOGGER.Debug("=> Initialize Component 'PrintManagement'")
            _EntityConnectionString = EntityConnectionData.EntityConnectionString
            NLOGLOGGER.Debug("=> Component 'PrintManagement' initialized")

            'NLOGLOGGER.Debug("=> Get Component Settings")
            'Me.Server_Documents_RootDirectory = GetSetting(CommonEnums.Settings.Server_Documents_RootDirectory)

        Catch ex As Exception
            NLOGLOGGER.Fatal("Component 'PrintManagement' cannot initialized. Check the database connection first.")

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

    Private ReadOnly Property DefaultQueryPrints As DbQuery(Of Print)
        Get

            Try

                Dim defaultQuery As DbQuery(Of Print) = DbCtx.Prints
	            Dim _defaultQueryResult As DbQuery(Of Print)


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
    ''' Gibt eine unsortierte Liste der Ausdrucke aus
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPrints(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Print) Implements IPrintManagement.GetPrints
        Dim serverPrints As DbQuery(Of Print)

        NLOGLOGGER.Info("Get List of Prints ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverPrints = DefaultQueryPrints.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverPrints = DefaultQueryPrints
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverPrints.Count & " Print/Prints in database")

        Return New ObservableCollection(Of Print)(serverPrints)

    End Function

    ''' <summary>
    ''' Gibt eine unsortierte Liste der Ausdrucke aus. Als Kriterium wird eine Liste von Guids mitgegeben
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPrints(PrintGuids As List(Of Guid), Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Print) Implements IPrintManagement.GetPrints
        Dim serverPrints As DbQuery(Of Print)

        NLOGLOGGER.Info("Get List of Prints ")

        ' Erzeugen eines leeren QueryParameter-Objekts
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        ' Abfrage vorbereiten und ausführen
        With QParam
            Includes = .Includes

            If .ItemsCount > 0 Then
                ' Skip geht nur auf sortierten listen. Daher in Standardreihenfolge
                serverPrints = DefaultQueryPrints.OrderBy(Function(e) e.Created).Skip(.StartIndex).Take(.ItemsCount)
            Else
                serverPrints = DefaultQueryPrints.Where(Function(_Print) PrintGuids.Contains(_Print.GUID))
            End If

        End With

        NLOGLOGGER.Debug("Found " & serverPrints.Count & " Print/Prints in database")

        Return New ObservableCollection(Of Print)(serverPrints)

    End Function

    ''' <summary>
    ''' Gibt die Informationen für einen Ausdruck zurück
    ''' </summary>
    ''' <param name="PrintGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetPrint(PrintGUID As String, Optional QParam As QueryParameters = Nothing) As Print Implements IPrintManagement.GetPrint
        Dim serverPrint As Print = Nothing
        Dim guidConvert As New GuidConverter
        Dim guid As Guid

        Try
            NLOGLOGGER.Info("Get Print Informations ")

            guid = guidConvert.ConvertFromString(PrintGUID)
            NLOGLOGGER.Debug("=> Search for Print with GUID '" & PrintGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverPrint = DefaultQueryPrints.Single(Function(Prints) Prints.GUID = guid)
            End With

            NLOGLOGGER.Debug("=> Found Print '" & serverPrint.GUID.ToString & "'")
        Catch ex As Exception
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverPrint
    End Function

#Region "AllocationListBox: FxNTPrintsPrintToInputControl"

    ''' <summary>
    ''' Gibt die Informationen für einen PrintToInputControls zurück
    ''' </summary>
    ''' <param name="FxNTPrintsPrintGUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetFxNTPrintsPrint(FxNTPrintsPrintGUID As String, Optional QParam As QueryParameters = Nothing) As FxNTPrints.Print
        Dim serverFxNTPrintsPrint As List (Of FxNTPrints.Print) 
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid

        Try
            NLOGLOGGER.Info("Get FxNTPrints.Print Informations ")

            _guid = guidConvert.ConvertFromString(FxNTPrintsPrintGUID)
            NLOGLOGGER.Debug("=> Search for FxNTPrints.Print with GUID '" & FxNTPrintsPrintGUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverFxNTPrintsPrint = (From FxNTPrintsPrint In DbCtx.Prints
                                    Where FxNTPrintsPrint.GUID = _guid
                                    Select FxNTPrintsPrint).ToList
            End With

            NLOGLOGGER.Debug("=> Found FxNTPrints.Print '" & serverFxNTPrintsPrint(0).GUID.ToString & "'")
        Catch ex As Exception
			serverFxNTPrintsPrint = New List (Of FxNTPrints.Print)
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return serverFxNTPrintsPrint(0)
    End Function

    ''' <summary>
    ''' Gibt die Informationen für alle Ausdrucke zurück
    ''' </summary>
    ''' <param name="GUID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
     Public Function GetFxNTPrintsPrintAllInputControlMembers(GUID As String, Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of FxNTPrints.PrintToInputControl)
        Dim serverInputControls As List(Of FxNTPrints.PrintToInputControl) 
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid

        Try
            NLOGLOGGER.Info("Get FxNTPrints.Print Informations")

            _guid = guidConvert.ConvertFromString(GUID)
            NLOGLOGGER.Debug("=> Search for FxNTPrints.Print with GUID '" & GUID & "'")

            ' Erzeugen eines leeren QueryParameter-Objekts
            If QParam Is Nothing Then
                QParam = New QueryParameters
            End If

            ' Abfrage vorbereiten und ausführen
            With QParam
                Includes = .Includes

                serverInputControls = (From FxNTPrintsPrint In DbCtx.Prints
                                    From InputControl In FxNTPrintsPrint.InputControls
                                    Where FxNTPrintsPrint.GUID = _guid
                                    Select InputControl).ToList

            End With

			NLOGLOGGER.Debug("=> Found " & serverInputControls.Count & " InputControls in database")

        Catch ex As Exception
			serverInputControls = New List(Of FxNTPrints.PrintToInputControl)
            NLOGLOGGER.Fatal(ex.Message)
        End Try

        Return New ObservableCollection(Of FxNTPrints.PrintToInputControl)(serverInputControls)
    End Function

#End Region

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Erstellt einen neuen leeren Eintrag vom Typ Ausdruck
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Als Ergebnis wird die erzeugte Versammlung-GUID zurückgegeben</remarks>
    Public Function CreateNewPrint() As ServerResult Implements IPrintManagement.CreateNewPrint
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAdd As New ServerResult
        Dim _NewPrint As Print

        Try
            NLOGLOGGER.Info("New Print will create in database")

            ' Erzeugt einen gültigen Eintrag, der hinzugefügt werden kann
            _NewPrint = EntryPoint_GenerateValidPrintEntity()

			' Anpassen der Historieninformationen
			With _NewPrint
                .Created = Now()
                .LastModified = Now()
                .CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
                .ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            ' Hinzufügen des Eintrags
            DbCtx.Prints.Add(_NewPrint)
            DbCtx.SaveChanges()

            NLOGLOGGER.Info("New Print is successfull create in database")
            _result.ReturnValue = _NewPrint.GUID.ToString
            NLOGLOGGER.Info("=> PrintGUID: " & _result.ReturnValue)

			NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewPrint")
			_resultAfterAdd = EntryPoint_AfterAddNewPrint(_NewPrint)
			NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewPrint")

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
    ''' Erstellt einen neuen Ausdruck aus einer Ausdruck-Entität
    ''' </summary>
    ''' <param name="PrintEntity"></param>
    ''' <returns>Als Ergebnis wird die erzeugte Ausdruck-GUID zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function CreateNewPrint(PrintEntity As Print) As ServerResult Implements IPrintManagement.CreateNewPrint
        Dim crypt As New CryptingHASH
        Dim _result As New ServerResult
		Dim _resultAfterAddNew As New ServerResult
        Dim canAddNew As Boolean
		Dim PrintEntryPointResult as Print

        Try
            NLOGLOGGER.Info("New Print will create in database")

           	' Zum Verändern der Entität vor dem Einfügen in die Datenbank
			PrintEntryPointResult = EntryPoint_BeforeAddNewPrint(PrintEntity)
			If PrintEntryPointResult IsNot Nothing then
				PrintEntity = PrintEntryPointResult
			End if

			' Vor dem Einfügen wird geprüft, ob der Eintrag hinzugefügt werden kann
			canAddNew = CheckAddNewPrint(PrintEntity)
			If canAddNew = True Then
			canAddNew = EntryPoint_CheckAddNewPrint(DbCtx, PrintEntity)
            If canAddNew = Nothing Then
                canAddNew = True
            End If

			End If

            If canAddNew = true Then

				' Anpassen der Historieninformationen
				With PrintEntity
					.Created = Now()
					.LastModified = Now()
					.CreatorGUID = Guid.Parse(CurrentUserSettings.UserGUID)
					.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
				End With

                DbCtx.Prints.Add(PrintEntity)
                DbCtx.SaveChanges()

                NLOGLOGGER.Info("New Print is successfull create in database")
                _result.ReturnValue = PrintEntity.GUID.ToString
                NLOGLOGGER.Info("=> PrintGUID: " & _result.ReturnValue)

				NLOGLOGGER.Debug("=> Execute additional process: AfterAddNewPrint")
				_resultAfterAddNew = EntryPoint_AfterAddNewPrint(PrintEntity)
                If _resultAfterAddNew IsNot Nothing Then
                    If _resultAfterAddNew.HasErrors = True Then
                        For Each ErrAfterAddNew As String In _resultAfterAddNew.ErrorMessages
                            _result.ErrorMessages.Add(ErrAfterAddNew)
                        Next
                    End If
                End If
				NLOGLOGGER.Debug("=> Finish additional process : AfterAddNewPrint")

            Else
                NLOGLOGGER.Error("Print cannot added.")
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
    ''' Kopieren einer vorhanden Ausdruck mit allen Eigenschaften
    ''' </summary>
    ''' <param name="PrintEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CopyPrint(PrintEntity As Print) As ServerResult Implements IPrintManagement.CopyPrint

        Dim _copyPrint As New Print
		Dim sourceValues As DbPropertyValues
		Dim _result As New ServerResult

		try

			' Create and add clone object to context before setting its values
			DbCtx.Prints.Add(_copyPrint)

			' Copy values from source to clone
			sourceValues = DbCtx.Entry(PrintEntity).CurrentValues
			DbCtx.Entry(_copyPrint).CurrentValues.SetValues(sourceValues)

			' Anpassen von Eigenschaften der Kopie
			_copyPrint = EntryPoint_CopyPrint(PrintEntity)

			' Anpassen der Historieninformationen
			With _copyPrint
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
    ''' <param name="PrintEntity">Benutzer-Entität, die veränderte Informationen enthält</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModifyPrint(PrintEntity As Print) As ServerResult Implements IPrintManagement.ModifyPrint
        Dim _result As New ServerResult
		Dim _resultAfterModify as New ServerResult
		Dim _resultBeforeModify as New ServerResult
		Dim PrintEntryPointResult as Print

        Dim serverPrint As Print

        Try
            NLOGLOGGER.Info("Print will modified in database")

            ' Die Artikelinformation wird zunächst aus dem EntityContext-Cache gelesen
            serverPrint = GetPrint(PrintEntity.GUID.ToString)

			NLOGLOGGER.Debug("=> Execute additional process: BeforeModifyPrint")
			_resultBeforeModify = EntryPoint_BeforeModifyPrint(serverPrint)
            If _resultBeforeModify IsNot Nothing Then
                If _resultBeforeModify.HasErrors = True Then
                    For Each ErrBeforeModify As String In _resultBeforeModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrBeforeModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : BeforeModifyPrint")

            NLOGLOGGER.Debug("=> Execute additional process: ModifyEntityBeforeModifyPrint")
            PrintEntryPointResult = EntryPoint_ModifyEntityBeforeModifyPrint(PrintEntity)
			If PrintEntryPointResult IsNot Nothing then
				PrintEntity = PrintEntryPointResult
			End if
            NLOGLOGGER.Debug("=> Finish additional process : ModifyEntityBeforeModifyPrint")

			' Anpassen der Historieninformationen
			With PrintEntity
				.LastModified = Now()
				.ModifierGUID = Guid.Parse(CurrentUserSettings.UserGUID)
			End With

            DbCtx.Entry(serverPrint).CurrentValues.SetValues(PrintEntity)
            DbCtx.SaveChanges()

			NLOGLOGGER.Debug("=> Execute additional process: AfterModifyPrint")
			_resultAfterModify = EntryPoint_AfterModifyPrint(serverPrint)
            If _resultAfterModify IsNot Nothing Then
                If _resultAfterModify.HasErrors = True Then
                    For Each ErrAfterModify As String In _resultAfterModify.ErrorMessages
                        _result.ErrorMessages.Add(ErrAfterModify)
                    Next
                End If
            End If
			NLOGLOGGER.Debug("=> Finish additional process : AfterModifyPrint")

            NLOGLOGGER.Debug("Print is successfull modiefied in database")

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
    ''' Diese Methode löscht einen Ausdruck 
    ''' </summary>
    ''' <param name="PrintGUID">Die PrintGUID des Ausdruck, der gelöscht werden soll</param>
    ''' <returns>Wurde der Ausdruck gelöscht, dann wird als Ergebniss TRUE zurückgegeben</returns>
    ''' <remarks></remarks>
    Public Function DeletePrint(PrintGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IPrintManagement.DeletePrint

        Dim serverPrints As List(Of Print)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
		Dim _resultAfterDelete As New ServerResult

        Try
            NLOGLOGGER.Info("Delete Print")

            NLOGLOGGER.Debug("=> Try to find Print-id '" & PrintGUID & "'")
            guid = guidConvert.ConvertFromString(PrintGUID)
            serverPrints = (From Print In DbCtx.Prints Where Print.GUID = guid).ToList

            If serverPrints.Count = 1 Then
                NLOGLOGGER.Debug("=> Print-Id '" & PrintGUID & "' found")
                If serverPrints(0).CanNotDelete = False Then

                    If PermanentlyDelete = True Then
                        DbCtx.Prints.Remove(serverPrints(0))
                    Else
						' Anpassen der Historieninformationen
						With serverPrints(0)
							.Deleted = Now()
							.DeleterGUID = Guid.Parse(CurrentUserSettings.UserGUID)
						End With
                    End If
                    DbCtx.SaveChanges()

					NLOGLOGGER.Debug("=> Execute additional process: AfterDeletePrint")
					_resultAfterDelete = EntryPoint_AfterDeletePrint(serverPrints(0))
					If _resultAfterDelete IsNot Nothing Then
						If _resultAfterDelete.HasErrors = True Then
							For Each ErrAfterDelete As String In _resultAfterDelete.ErrorMessages
								_result.ErrorMessages.Add(ErrAfterDelete)
							Next
						End If
					End If
					NLOGLOGGER.Debug("=> Finish additional process : AfterDeletePrint")
			
                    NLOGLOGGER.Info("=> Print-Id '" & PrintGUID & "' DELETED")

                Else
                    NLOGLOGGER.Debug("=> Print-Id '" & PrintGUID & "' FAILED to delete")
                End If

            Else
                NLOGLOGGER.Debug("=> Print-Id '" & PrintGUID & "' not in database")
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
    ''' Zum vereinfachten löschen von mehreren Ausdrucke
    ''' </summary>
    ''' <param name="Prints"></param>
    ''' <param name="PermanentlyDelete"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeletePrints(Prints As Print(), Optional PermanentlyDelete As Boolean = True) As ServerResult Implements IPrintManagement.DeletePrints
        Dim _result As New ServerResult
        Dim _resultAfterDelete As New ServerResult

        Try

            NLOGLOGGER.Info("Delete " & Prints.Count & " Prints Collection")

            For Each _Print As Print In Prints
                _resultAfterDelete = DeletePrint(_Print.GUID.ToString, PermanentlyDelete)

                If _resultAfterDelete.HasErrors = True Then
                    For Each msg As String In _resultAfterDelete.ErrorMessages
                        _result.ErrorMessages.Add(msg)
                    Next
                End If
            Next

            If _result.HasErrors = False Then
                NLOGLOGGER.Info("Successfull delete Prints Collection with no errors")
            Else
                NLOGLOGGER.Error("Delete Prints Collection with errors")
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
    ''' <param name="PrintEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckAddNewPrint(PrintEntity As Print) As Boolean

        Dim isUniquePrintName As Boolean

        ' Überprüfen auf Eindeutigkeit, da EF sowas nicht anbietet
        ' Any() stops at the first match and doesn't have to enumerate the entire sequence
        isUniquePrintName = Not DbCtx.Prints.Any(Function(u) u.Name = PrintEntity.Name)

        Return isUniquePrintName

    End Function
#End Region

#Region "AllocationListBoxes"

    ''' <summary>
    ''' Mit dieser Methode fügt man einen Eingabeelement einer Ausdruck hinzu.
    ''' </summary>
    ''' <param name="InputControlGUID">InputControl-GUID die hinzugefügt werden soll</param>
    ''' <param name="FxNTPrintsPrintGUID">Ausdruck-GUID der Gruppe die den Benutzer enthalten soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddInputControlToFxNTPrintsPrint(InputControlGUID As String, FxNTPrintsPrintGUID As String) As ServerResult

        Dim newInputControlToFxNTPrintsPrint As FxNTPrints.PrintToInputControl
        Dim _result As New ServerResult
        Dim guidConvert As New GuidConverter
        Dim _guid As Guid
		Dim qpara As New QueryParameters

        Try
            NLOGLOGGER.Info("InputControl will assign to a FxNTPrintsPrint")

            newInputControlToFxNTPrintsPrint = New FxNTPrints.PrintToInputControl
            With newInputControlToFxNTPrintsPrint
                ' Benötigte Informationen
                _guid = guidConvert.ConvertFromString(InputControlGUID)
                .InputControlGuid = _guid
                NLOGLOGGER.Debug("=> InputControlGuid: " & InputControlGUID)

                _guid = guidConvert.ConvertFromString(FxNTPrintsPrintGUID)
                .PrintGuid = _guid
                NLOGLOGGER.Debug("=> PrintGuid: " & FxNTPrintsPrintGUID)
            End With

            DbCtx.PrintToInputControls.Add(newInputControlToFxNTPrintsPrint)
            DbCtx.SaveChanges()

			' Frisches Einlesen der Entität
			qpara.Includes.Add("InputControls")
			TryCast(DbCtx, IObjectContextAdapter).ObjectContext.Detach(GetFxNTPrintsPrint(FxNTPrintsPrintGUID, qpara))
            Dim srvFxNTPrintsPrint As FxNTPrints.Print = DbCtx.Prints.Find(newInputControlToFxNTPrintsPrint.PrintGuid)


            NLOGLOGGER.Info("InputControl is successfull assign to FxNTPrints.Print")
            _result.ReturnValue = newInputControlToFxNTPrintsPrint.GUID.ToString
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
    ''' Mit dieser Methode entfernt man einen Eingabeelement aus einer Ausdruck .
    ''' </summary>
    ''' <param name="InputControlGUID">InputControl-GUID die hinzugefügt werden soll</param>
    ''' <param name="FxNTPrintsPrintGUID">Ausdruck-GUID der Gruppe die den Benutzer enthalten soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
	Public Function RemoveInputControlFromFxNTPrintsPrint(InputControlGUID As String, FxNTPrintsPrintGUID As String, Optional PermanentlyDelete As Boolean = True) As ServerResult

        Dim serverPrints As List(Of FxNTPrints.Print)
        Dim guidConvert As New GuidConverter
        Dim guid As Guid
        Dim _result As New ServerResult
        Dim InputControlFound As Boolean = False
		Dim qpara As New QueryParameters

        Try
            NLOGLOGGER.Info("Remove InputControl From FxNTPrints.Print")

            NLOGLOGGER.Debug("=> Try to find FxNTPrintsPrint-id '" & FxNTPrintsPrintGUID & "'")
            guid = guidConvert.ConvertFromString(FxNTPrintsPrintGUID)
            serverPrints = (From FxNTPrintsPrint In DbCtx.Prints.Include("InputControls") Where FxNTPrintsPrint.GUID = guid).ToList

            If serverPrints.Count = 1 Then
                NLOGLOGGER.Debug("=> FxNTPrintsPrint-Id '" & FxNTPrintsPrintGUID & "' found")

                NLOGLOGGER.Debug("=> Search for InputControl-id '" & InputControlGUID & "'")
                For Each entry As FxNTPrints.PrintToInputControl In serverPrints(0).InputControls

                    If entry.InputControlGuid.ToString = InputControlGUID Then

                        InputControlFound = True

                        If entry.CanNotDelete = False Then

                            If PermanentlyDelete = True Then
                                DbCtx.Entry(entry).State = Entity.EntityState.Deleted
                            Else
                                entry.Deleted = Now
                            End If
                            DbCtx.SaveChanges()

							' Frisches Einlesen der Entität
							qpara.Includes.Add("InputControls")
							TryCast(DbCtx, IObjectContextAdapter).ObjectContext.Detach(GetFxNTPrintsPrint(FxNTPrintsPrintGUID, qpara))
							Dim srvFxNTPrintsPrint As FxNTPrints.Print = DbCtx.Prints.Find(serverPrints(0).Guid)

                            NLOGLOGGER.Info("=> InputControl-Id '" & FxNTPrintsPrintGUID & "' REMOVED")

                        Else
                            NLOGLOGGER.Debug("=> InputControl-Id '" & FxNTPrintsPrintGUID & "' FAILED to delete")
                            _result.ErrorMessages.Add("InputControl  cannot delete - Delete not allowed")
                        End If
                        Exit For
                    End If
                Next
                If InputControlFound = False Then
                    NLOGLOGGER.Debug("=> InputControl-Id '" & FxNTPrintsPrintGUID & "' not in UserGroup")
                    _result.ErrorMessages.Add("InputControl cannot delete - InputControl not found in Group")
                End If

            Else
                NLOGLOGGER.Debug("=> FxNTPrints.Print-Id '" & FxNTPrintsPrintGUID & "' not in database")
                _result.ErrorMessages.Add("InputControl cannot delete from FxNTPrints.Print - FxNTPrints.Print not found in database")
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

