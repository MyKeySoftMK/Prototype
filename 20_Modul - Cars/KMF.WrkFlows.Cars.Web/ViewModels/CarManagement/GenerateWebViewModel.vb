' ###################################################################
' #  T4-Name	: GenerateWebViewModel.tt                           #
' #  Date		: 2021-11-18                                        #
' #  Version	: 1                           (c) MyKey-Soft 2016   #
' ###################################################################
Imports System.Collections.ObjectModel

Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.Common.Application
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.Web.Application

Imports KMF.WrkFlows.Cars.Domain
Imports KMF.WrkFlows.Cars.DataAccess.Cars

Partial Class CarManagementViewModel

    Inherits WebViewModel
	Implements ICarManagement_ViewModelEntryPoints

    Private _CarManagement As CarManagement

#Region "Init"

    Public Sub New()

        Try

            NLOGLOGGER.Info("CarManagementViewModel will be initalized")

            _CarManagement = New CarManagement
            _Cars = GetCars(EntryPoint_DefineFullQueryParameter)

            NLOGLOGGER.Info("CarManagementViewModel is successfull initalized")

        Catch ex As Exception
            NLOGLOGGER.Fatal("CarManagementViewModel cannot initalized")
            NLOGLOGGER.Fatal(ex.Message)
            Console.WriteLine(ex.Message)
        End Try

    End Sub

#End Region

#Region "Events"

#End Region

#Region "Properties"

#Region "Common"

    ''' <summary>
    ''' Ob das Formular für die Eingabe aktiviert werden soll
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property EnableEditForm As Boolean
        Get
            If CurrentCar IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    ''' <summary>
    ''' Die Sammlung der Fahrzeuge
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Cars As ObservableCollection(Of Car)
        Set(value As ObservableCollection(Of Car))
            If value IsNot _Cars Then
                _Cars = value
                Me.OnPropertyChanged("Cars")
            End If
        End Set
        Get
            Return _Cars
        End Get
    End Property
    Private _Cars As ObservableCollection(Of Car)

    ''' <summary>
    ''' Die aktuell ausgewählte Fahrzeug incl. der Änderungen, die ein Benutzer vorgenommen hat,
    ''' bevor er diese in der Datenbank gespeichert hat
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CurrentCar As Car
        Set(value As Car)
            If value IsNot _currentCar Then
                If value IsNot Nothing Then

                    ' Wenn noch Änderungen an dem Eintrag nicht gespeichert wurden, dann den Benutzer vorher fragen
                    If _CarManagement.HasChanges = True Then

                        Dim userSelectYes As Boolean = False

                        ' Sicherheitsabfrage vor dem Löschen des Eintrags
                        ' <NOCH IMPLEMENTIEREN>

                        If userSelectYes = True Then

                            ' Speichern
                            ModifyCar(_currentCar)
							'Controller.Notification("Hinweis","Eintrag wurde gespeichert!")
                        Else

                            ' Verwerfen der Änderungen
                            _CarManagement.RejectChanges()
                            Me.OnPropertyChanged("Cars")
							'Controller.Notification("Hinweis","Änderungen wurden verworfen!")
                        End If
                    End If
                    _currentCar = value

                End If
                Me.OnPropertyChanged("CurrentCar")

                Me.OnPropertyChanged("EnableEditForm")

            End If
        End Set
        Get
            Return _currentCar
        End Get
    End Property
    Private _currentCar As Car

    ''' <summary>
    ''' Beinhaltet die Informationen über eine gewählte Fahrzeug, wie sie in der Datenabank
    ''' gespeichert sind
    ''' </summary>
    ''' <remarks></remarks>
    Private _savedCar As Car

#End Region

#End Region

#Region "Functions"

#Region "Common"

    ''' <summary>
    ''' Speichert eine neue Fahrzeug in der Datenbank
    ''' </summary>
    ''' <param name="CarEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateNewCar(CarEntity As Car) As ServerResult

        Dim _result As ServerResult
        _result = _CarManagement.CreateNewCar(CarEntity)
        Return _result

    End Function

	Public Function CopyCar(CarEntity As Car) As ServerResult

        Dim _result As ServerResult
        _result = _CarManagement.CopyCar(CarEntity)
        Return _result

    End Function


    Public Function ModifyCar(CarEntity As Car) As ServerResult

        Dim _result As ServerResult
        _result = _CarManagement.ModifyCar(CarEntity)
        Return _result

    End Function

    Public Function DeleteCar(CarGuid As String) As ServerResult

        Dim _result As ServerResult

        ' Löschen aus der Datenbank
        _result = _CarManagement.DeleteCar(CarGuid, (DatabaseSettings.DATABASE_SOFTDEL = False))

        ' Entfernen aus der Auflistung
        Cars.Remove(CurrentCar)

        Return _result

    End Function

#End Region

#End Region

#Region "Queries"

    Public Function GetCars(Optional QParam As QueryParameters = Nothing) As ObservableCollection(Of Car)
        Return _CarManagement.GetCars(QParam)
    End Function

    Public Function GetCar(CarGUID As String, Optional QParam As QueryParameters = Nothing) As Car
        Return _CarManagement.GetCar(CarGUID, QParam)
    End Function

#End Region

#Region "MVVM"

#Region "Common"

    ''' <summary>
    ''' Kann der gewählte Eintrag gelöscht werden
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CanDeleteCarCommand() As Boolean
        Return CurrentCar IsNot Nothing
    End Function

    ''' <summary>
    ''' Löschen des Eintrags mit einer Sicherheitsabfrage
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ExecuteDeleteCarCommand()

        Dim userSelectYes As Boolean = False
		Dim entryCopy as Car

		' Sicherheitsabfrage vor dem Löschen des Eintrags
        ' <NOCH IMPLEMENTIEREN>
        
		If userSelectYes = True Then
 			If CurrentCar.GUID.ToString IsNot Nothing Then
				entryCopy = CurrentCar
				If _CarManagement.DeleteCar(entryCopy.GUID.ToString).HasErrors = False then
					'Controller.Message("Der Eintrag mit der Id '" & entryCopy.GUID.ToString & "' wurde erfolgreich gelöscht","Hinweis")

					' Entfernen aus der lokalen Auflistung
					Cars.Remove(entryCopy)
					_currentCar = Nothing
					Me.OnPropertyChanged("CurrentCar")
					'Controller.Notification("Hinweis","Eintrag wurde gelöscht!")
				Else
					'Controller.Message("Der Eintrag mit der Id '" & entryCopy.GUID.ToString & "' konnte wegen eines Fehlers nicht gelöscht werden","Fehler",,MessageBoxImages.Error)
				End If
			End If
		End If

    End Sub

    ''' <summary>
    ''' Der der gewählte Eintrag gepeichert werden
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CanModifyCarCommand() As Boolean
        If CurrentCar Is Nothing Then
            Return False
        Else
            Return (_CarManagement.HasChanges = True)
        End If

    End Function

    ''' <summary>
    ''' Speichern des Eintrags in die Datenbank
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ExecuteModifyCarCommand()

        If CurrentCar.GUID.ToString Is Nothing Then
            CurrentCar = _CarManagement.GetCar(CreateNewCar(CurrentCar).ReturnValue)
        Else
            ModifyCar(CurrentCar)
        End If

		'Controller.Notification("Hinweis","Eintrag wurde gespeichert!")

    End Sub

	''' <summary>
    ''' Der der gewählte Eintrag kopiert werden
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CanCopyCarCommand() As Boolean
        If CurrentCar Is Nothing Then
            Return False
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' Kopieren des Eintrags 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ExecuteCopyCarCommand()

        If CurrentCar.GUID.ToString IsNot Nothing Then
            CopyCar(CurrentCar)
        End If

		'Controller.Notification("Hinweis","Eintrag wurde kopiert!")

    End Sub


    ''' <summary>
    ''' Können Änderungen an dem aktuellen Eintrag zurückgenommen werden
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CanCancelCarCommand() As Boolean
        If CurrentCar Is Nothing Then
            Return False
        End If
        Return False
    End Function

    ''' <summary>
    ''' Überschreibt die gemachten Änderungen des Eintrags wieder
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ExecuteCancelCarCommand()
        CurrentCar = _savedCar
    End Sub

    ''' <summary>
    ''' Kann ein neuer Eintrag erstellt werden
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CanNewCarCommand() As Boolean
        If CurrentCar Is Nothing Then
            Return True
        End If
        Return CurrentCar.GUID.ToString IsNot Nothing
    End Function

    ''' <summary>
    ''' Erstellen eines neuen Eintrags
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ExecuteNewCarCommand()

        Dim _result As ServerResult

        ' Erzeugen eines Eintrags auf dem Server
        _result = _CarManagement.CreateNewCar

		If _result.HasErrors = False Then
		    ' Hinzugefügten Eintrag lesen und in die aktuelle Liste einfügen
			Cars.Add(_CarManagement.GetCar(_result.ReturnValue.ToString))

			' Auswählen des neuen Eintrags
			CurrentCar = Cars.ElementAt(Cars.Count - 1)
		Else
			'Controller.Message(_result.ErrorMessages(0), "Fehler", MessageBoxButtons.OK, MessageBoxImages.Error)
		End If

    End Sub

#End Region

#End Region

End Class

