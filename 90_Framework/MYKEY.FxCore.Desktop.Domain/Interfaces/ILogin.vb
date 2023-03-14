Imports MYKEY.FxCore.Common.OperationStatusEnum
Imports MYKEY.FxCore.Common


Public Interface ILogin

    ''' <summary>
    ''' Stellt eine Verbindung zu einem Server her
    ''' </summary>
    ''' <remarks></remarks>
    Sub Connect()

    ''' <summary>
    ''' Trennt die Verbing zu einem Server
    ''' </summary>
    ''' <remarks></remarks>
    Sub Disconnect()

    ''' <summary>
    ''' Status ob die Verbindung hergestellt werden konnte
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property IsConnect As Boolean

    ''' <summary>
    ''' Diese Eigenschaft beinhaltet den Status der letzten Operation
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property OperationStatus As OperationStatus

    ''' <summary>
    ''' Ob der Server erreichbar ist
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property IsAvailable As Boolean

    ''' <summary>
    ''' Prüft, ob die derzeitige Schemaversion der Datenbank mit der Schemaversion der Anwendung zusammenpasst
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function CheckDataBaseShemaVersion() As ServerResult


End Interface
