Imports MYKEY.FxCore.Common.ApplicationLogging

Public Class ApplicationSettings

    Public Shared Sub InitSettings()

        NLOGLOGGER.Debug("EXE_FOLDER_NAME    : " & ApplicationSettings.PUBLICPRODUCTNAME)

    End Sub


    Public Shared Property PUBLICPRODUCTNAME As String = "KM-F Workflows"
    Public Shared Property PUBLICPRODUCTNAMESHORT As String = "WrkFlows"

    Public Shared Property PRODUCTASSEMBLIES As String = "KMF.WrkFlows"

End Class
