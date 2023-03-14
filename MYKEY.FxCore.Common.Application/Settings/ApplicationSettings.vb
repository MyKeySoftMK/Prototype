Imports MYKEY.FxCore.Common.ApplicationLogging

Public Class ApplicationSettings

    Public Shared Sub InitSettings()

        NLOGLOGGER.Debug("EXE_FOLDER_NAME    : " & ApplicationSettings.EXE_FOLDER_NAME)
        NLOGLOGGER.Debug("TEMP_FOLDER_NAME   : " & ApplicationSettings.TEMP_FOLDER_NAME)
        NLOGLOGGER.Debug("APPDATA_FOLDER_NAME: " & ApplicationSettings.APPDATA_FOLDER_NAME)
        NLOGLOGGER.Debug("INTERNALAPPDATA    : " & ApplicationSettings.INTERNALAPPDATA)

    End Sub

    Public Shared Sub ClearSettings()

        ' Erzeugen eine Pfades zur Speicherung der temporären Daten
        ' Zuvor wird dieser gelöscht, damit bei einem Absturz keine defekten Konfigurationen übrigbleiben
        If System.IO.Directory.Exists(ApplicationSettings.INTERNALAPPDATA) Then
            Try
                NLOGLOGGER.Debug("Delete Temp-Folder : " & ApplicationSettings.INTERNALAPPDATA)
                System.IO.Directory.Delete(ApplicationSettings.INTERNALAPPDATA, True)
            Catch ex As Exception

            End Try
        End If
        If Not System.IO.Directory.Exists(ApplicationSettings.INTERNALAPPDATA) Then
            Try
                NLOGLOGGER.Debug("Create Temp-Folder : " & ApplicationSettings.INTERNALAPPDATA)
                System.IO.Directory.CreateDirectory(ApplicationSettings.INTERNALAPPDATA)
            Catch ex As Exception

            End Try
        End If


    End Sub

    ''' <summary>
    ''' Zum ver- und entschlüsseln von Streams und Daten
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property CRYPTOKEY As String = "igepa78"

    Public Shared Property INTERNALCOMPANY As String = System.Windows.Forms.Application.CompanyName

    Public Shared Property INTERNALPRODUCTNAME As String = System.Windows.Forms.Application.ProductName

    Public Shared Property INTERNALPRODUCTVERSION As String = System.Windows.Forms.Application.ProductVersion

#Region "Platzhalter"

    ''' <summary>
    ''' Verzeichnis in dem sich die ausführende Datei befindet
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property EXE_FOLDER_NAME As String = System.AppDomain.CurrentDomain.BaseDirectory

    ''' <summary>
    ''' Das Verzeichnis, in dem temporäre Dateien abgelegt werden können
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property TEMP_FOLDER_NAME As String = System.IO.Path.GetTempPath

    ''' <summary>
    ''' Verzeichnis für die Daten, die von Anwendungen benötigt werden können
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property APPDATA_FOLDER_NAME As String = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)

    Public Shared Property INTERNALAPPDATA As String = ApplicationSettings.APPDATA_FOLDER_NAME & "\" & ApplicationSettings.INTERNALCOMPANY & "\" & ApplicationSettings.INTERNALPRODUCTNAME

#End Region

#Region "Runtime"

    ''' <summary>
    ''' Damit ist es mögliche den Login-Screen zu umgehen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property NOLOGIN As Boolean = False

    ''' <summary>
    ''' Ob man das Logging der Applikation starten möchte
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property LOGAPP As Boolean = False

    ''' <summary>
    ''' Wenn diese Eigenschaft auf TRUE gesetzt wird, dann wird ein lokaler fakeSMTP-Server angesteuert. Dies ist nützlich beim
    ''' Debuggen der Anwendung
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property FAKESMTP As Boolean = False

    ''' <summary>
    ''' Ob der Verbindungstest beim Login ausgeführt werden soll
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property NOSQLTEST As Boolean = False

#End Region

End Class
