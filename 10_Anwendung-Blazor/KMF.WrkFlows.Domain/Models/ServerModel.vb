Imports MYKEY.FxCore.Common

Public Class ServerModel

    Public Sub OpenLog2Console()

        Dim Exec As New MYKEY.FxCore.Common.Executions

        ' TODO: Kopieren der UserSettings.dat nach C:\Benutzer\kolowiczm\AppData\Local\Log2Console

        Exec.ExecuteCommandLine(Application.ApplicationSettings.EXE_FOLDER_NAME & "\Log2Console\Log2Console.exe", "")


    End Sub

End Class
