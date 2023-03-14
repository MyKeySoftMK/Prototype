Imports MYKEY.FxCore.Common.ApplicationLogging

Public Class DatabaseSettings
    Public Shared Sub InitSettings()

        NLOGLOGGER.Debug("DATABASE_NAME    : " & DatabaseSettings.DATABASE_NAME)
        NLOGLOGGER.Debug("DATABASE_USERNAME: " & DatabaseSettings.DATABASE_USERNAME)
        NLOGLOGGER.Debug("DATABASE_PASSWORD: " & DatabaseSettings.DATABASE_PASSWORD)
        NLOGLOGGER.Debug("DATABASE_SERVER  : " & DatabaseSettings.DATABASE_SERVER)
        NLOGLOGGER.Debug("DATABASE_PORT    : " & DatabaseSettings.DATABASE_PORT)
        NLOGLOGGER.Debug("DATABASE_SOFTDEL : " & DatabaseSettings.DATABASE_SOFTDEL)

    End Sub

    Public Shared Property DATABASE_NAME As String

    Public Shared Property DATABASE_USERNAME As String

    Public Shared Property DATABASE_PASSWORD As String

    Public Shared Property DATABASE_SERVER As String

    Public Shared Property DATABASE_PORT As Integer

    Public Shared Property DATABASE_SOFTDEL As Boolean

End Class
