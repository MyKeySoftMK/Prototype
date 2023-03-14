Imports Microsoft.Win32.TaskScheduler

Public Class TaskScheduler

    ' https://github.com/dahall/TaskScheduler/wiki
    ' https://github.com/dahall/TaskScheduler/wiki/Examples


    Private Sub Sample()
        Using ts As New TaskService()
            ' Create a new task definition and assign properties
            Dim td As TaskDefinition = ts.NewTask
            td.RegistrationInfo.Description = "Does something"

            ' Add a trigger that will, starting tomorrow, fire every other week on Monday
            ' and Saturday and repeat every 10 minutes for the following 11 hours
            Dim wt As New WeeklyTrigger()
            wt.StartBoundary = DateTime.Today.AddDays(1)
            wt.DaysOfWeek = DaysOfTheWeek.Monday Or DaysOfTheWeek.Saturday
            wt.WeeksInterval = 2
            wt.Repetition.Duration = TimeSpan.FromHours(11)
            wt.Repetition.Interval = TimeSpan.FromMinutes(10)
            td.Triggers.Add(wt)

            ' Add an action (shorthand) that runs Notepad
            td.Actions.Add(New ExecAction("notepad.exe", "c:\test.log"))

            ' Register the task in the root folder
            ts.RootFolder.RegisterTaskDefinition("Test", td)
        End Using
    End Sub



End Class
