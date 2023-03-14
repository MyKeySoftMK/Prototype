Imports MYKEY.FxCore.Common.ApplicationLogging

Public Class ReportingServerSettings
    Public Shared Sub InitSettings()

        NLOGLOGGER.Debug("SSRS_SERVER           : " & ReportingServerSettings.SSRS_SERVER)
        NLOGLOGGER.Debug("SSRS_APPLICATION_ROOT : " & ReportingServerSettings.SSRS_APPLICATION_ROOT)
        NLOGLOGGER.Debug("SSRS_DATABASE_NAME    : " & ReportingServerSettings.SSRS_DATABASE_NAME)
        NLOGLOGGER.Debug("JASPER_SERVER_URL     : " & ReportingServerSettings.JASPER_SERVER_URL)
        NLOGLOGGER.Debug("JASPER_SERVER_USER    : " & ReportingServerSettings.JASPER_SERVER_USER)
        NLOGLOGGER.Debug("JASPER_SERVER_PASSWORD: " & ReportingServerSettings.JASPER_SERVER_PASSWORD)

    End Sub

    Public Shared Property SSRS_SERVER As String

    Public Shared Property SSRS_APPLICATION_ROOT As String

    Public Shared Property SSRS_DATABASE_NAME As String

    Public Shared Property JASPER_SERVER_URL As String

    Public Shared Property JASPER_SERVER_USER As String

    Public Shared Property JASPER_SERVER_PASSWORD As String

End Class
