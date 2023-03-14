Public Class Executions

    Public Sub ExecuteCommandLine(PathToExe As String, Args As String)
        Dim pInfo As New ProcessStartInfo
        Dim pProcess As Process

        With pInfo
            .Arguments = String.Format("{0}", Args)
            .FileName = PathToExe

            ' .NET Framework hat der Default UseShellExecute = True
            ' Bei .NET Core und .NET 5 ist der Defaul UseShellExecute = False
            ' TODO: Prüfen, ob bei *.EXE auch funktioniert
            .UseShellExecute = True
        End With

        pProcess = Process.Start(pInfo)

    End Sub

End Class
