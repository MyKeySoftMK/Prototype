Imports System
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Security.Principal
Imports System.Security.Permissions
Imports System.Threading
Imports System.Reflection
Imports MYKEY.FxCore.Common.ApplicationLogging
Imports MYKEY.FxCore.Common.Processes
Imports MYKEY.FxCore.Common.ProcessEvents
Imports System.Management
Imports Microsoft.Win32
Imports Microsoft.VisualBasic.FileIO

<Assembly: SecurityPermissionAttribute(SecurityAction.RequestMinimum, UnmanagedCode:=True)>
<SecurityPermission(SecurityAction.Demand, ControlPrincipal:=True)>
Public Class Runtime

    Shared UNSECURE_LOGGING = 1

    ' http://csharptest.net/532/using-processstart-to-capture-console-output/
    ' http://csharptest.net/321/how-to-use-systemdiagnosticsprocess-correctly/


    Shared mreOut As New ManualResetEvent(False)
    Shared mreErr As New ManualResetEvent(False)

    ''' <summary>
    ''' Ob die Anwendung als 32-bit oder 64-bit Version läuft
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Die Struktur System.IntPtr ist plattformabhängig. Bei 64-bit ist sie acht Byte groß, bei 32-bit nur 4 Byte</remarks>
    Public Shared Function Is64Bit() As Boolean
        Return (IntPtr.Size = 8)
    End Function

    ''' <summary>
    ''' Ruft eine Datei auf und führt das dazugehörige Programm auf
    ''' </summary>
    ''' <param name="ExeCommand"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function WaitForShelledApplication(ExeCommand As Command) As FunctionResult
        Dim _result As New FunctionResult
        Dim pInfo As New ProcessStartInfo
        Dim pProcess As Process
        Dim sPassword As Security.SecureString = New Security.SecureString
        Dim BatchFile As String
        Dim BatchCommands As String()


        ' Beim Dateinamen kann absoluter Pfad oder auch nur der Name der Datei angegeben werden, wenn sich das Programm
        ' im Suchpfad befindet
        If System.IO.File.Exists(ExeCommand.Application) = True Then

            Try

                With pInfo

                    If ExeCommand.ApplicationIsBatch = False Then

                        ' Script-Ablauf
                        .FileName = ExeCommand.Application
                        .Arguments = String.Format("{0}", ExeCommand.Arguments)

                    Else

                        ' Batch
                        .FileName = "cmd.exe"

                    End If


                    ' Soll eine Fehlermeldung angezeigt werden, wenn der Prozess nicht gestartet werden kann
                    .ErrorDialog = False

                    ' Umleiten der Ein-/Ausgaben
                    .RedirectStandardError = True
                    .StandardErrorEncoding = Encoding.UTF8

                    .RedirectStandardInput = True

                    .RedirectStandardOutput = True
                    .StandardOutputEncoding = Encoding.UTF8

                    '' Aufrufen mit erhöhten Rechten
                    'If ExeCommand.AdminPassword.Length > 0 Then
                    '    .Verb = "runas"

                    '    ' Umwandlung von Text nach SecureString
                    '    For Each txt As Char In ExeCommand.AdminPassword
                    '        sPassword.AppendChar(txt)
                    '    Next
                    '    .Password = sPassword

                    '    .UserName = ExeCommand.AdminUser
                    '    .Domain = ExeCommand.AdminDomain
                    'End If

                    ' Wie die gestartete Anwendung angezeigt werden soll
                    .WindowStyle = ExeCommand.WindowsState

                    ' Soll beim Starten die Betriebssystemshell verwendet werden
                    ' Bei Verwendung von UWC ist dieser Wert immer auf FALSE zu setzen
                    .UseShellExecute = False

                    ' Ob der Prozess in einem eigenem Fenster gestartet werden soll
                    .CreateNoWindow = True

                End With

                ' Starten des Prozesses
                pProcess = Process.Start(pInfo)
                NLOGLOGGER.Info("Process is started successfull")

                With pProcess

                    ' Ob nach dem Beenden das Exited-Event ausgelöst werden soll
                    .EnableRaisingEvents = True

                    ' Set up the event handler to call back with 
                    ' each line of output 
                    AddHandler .OutputDataReceived, AddressOf OnDataReceived
                    AddHandler .Exited, AddressOf ProcessOnExited
                    AddHandler .ErrorDataReceived, AddressOf ErrorDataReceived


                    ' Startet das Auslesen des Output
                    .BeginOutputReadLine()

                    If ExeCommand.ApplicationIsBatch = False Then

                        NLOGLOGGER.Debug("Process ApplicationIsBatch is false")

                        ' Beendet die Eingabe
                        .StandardInput.Close()

                        .BeginErrorReadLine()

                        ' Warten bis der Prozess beendet wurde
                        .WaitForExit()

                        ' Sicherstellen, das die Output-Streams geschrieben werden
                        mreOut.WaitOne()
                        mreErr.WaitOne()

                        _result.ReturnValue = .ExitCode
                    Else

                        NLOGLOGGER.Debug("Process ApplicationIsBatch is true")

                        ' Die Befehle in einzelne Elemente eines Arrays packen
                        BatchFile = FileSystem.ReadAllText(ExeCommand.Application)
                        BatchCommands = BatchFile.Split(vbCrLf)

                        If BatchCommands.Length > 0 Then

                            For Each cmd As String In BatchCommands
                                .StandardInput.WriteLine(cmd)
                                NLOGLOGGER.Debug("ProcessInput:" & cmd)
                                .StandardInput.Flush()
                            Next

                        End If

                        ' Beendet den Eingabe
                        .StandardInput.Close()
                    End If

                End With



            Catch ex As System.ComponentModel.Win32Exception
                _result.ErrorMessages.Add(ex.Message)
                NLOGLOGGER.Fatal("Message   :" + ex.Message)
                NLOGLOGGER.Fatal("TargetSite: " + ex.TargetSite.ToString)
                NLOGLOGGER.Fatal("Source    : " + ex.Source.ToString)
                NLOGLOGGER.Fatal(ex.InnerException)
                NLOGLOGGER.Fatal(ex.StackTrace)
            End Try
        Else

            _result.ErrorMessages.Add("Filename not found: " & ExeCommand.Application)
            NLOGLOGGER.Fatal("Filename not found: " & ExeCommand.Application)

        End If

        Return _result
    End Function


    ''' <summary>
    ''' Ruft eine Datei auf und führt das dazugehörige Programm auf
    ''' </summary>
    ''' <param name="ExeCommand"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function WaitForShelledApplicationEx(ExeCommand As Command) As FunctionResult
        Dim _result As New FunctionResult

        Dim REGISTRY_UAC_EnableLUA As String
        Dim REGISTRY_UAC_ConsentPromptBehaviorAdmin As String
        Dim REGISTRY_UAC_EnableLinkedConnections As String
        Dim UAC_PROPER As Boolean

        ' Damit die Ausführung mit den Administrationsberechtigungen erfolgen kann, muss das UAC (User Account Control – Benutzerkontensteuerung) angepasst werden.
        ' Zunächst ist es notwendig, dass man allen Administratoren erlaubt, jegliche Scripte ohne Ausführungsbestätigung zu starten
        '     - Direkte Veränderung der Registry
        '     - Script-Befehl ausführen 
        REGISTRY_UAC_EnableLUA = Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", "1")
        NLOGLOGGER.Debug("REGISTRY_UWC_EnableLUA = " & REGISTRY_UAC_EnableLUA)

        REGISTRY_UAC_ConsentPromptBehaviorAdmin = Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", "1")
        NLOGLOGGER.Debug("REGISTRY_UWC_ConsentPromptBehaviorAdmin = " & REGISTRY_UAC_ConsentPromptBehaviorAdmin)

        REGISTRY_UAC_EnableLinkedConnections = Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLinkedConnections", "0")
        NLOGLOGGER.Debug("REGISTRY_UWC_EnableLinkedConnections = " & REGISTRY_UAC_EnableLinkedConnections)

        If REGISTRY_UAC_EnableLUA = "0" And REGISTRY_UAC_ConsentPromptBehaviorAdmin = "0" And REGISTRY_UAC_EnableLinkedConnections = "1" Then
            UAC_PROPER = True
            NLOGLOGGER.Debug("UAC_PROPER = " & UAC_PROPER)
        Else
            UAC_PROPER = False
            NLOGLOGGER.Error("UAC_PROPER = " & UAC_PROPER)
        End If


        'Dim pProcess As Process
        'Dim BatchFile As String
        'Dim BatchCommands As String()

        ' Beim Dateinamen kann absoluter Pfad oder auch nur der Name der Datei angegeben werden, wenn sich das Programm
        ' im Suchpfad befindet
        If System.IO.File.Exists(ExeCommand.Application) = True Then

            Try

                With ExeCommand

                    NLOGLOGGER.Trace("Username     : " & .AdminUser)
                    NLOGLOGGER.Trace("Userndomain  : " & .AdminDomain)
                    If UNSECURE_LOGGING = 1 Then
                        NLOGLOGGER.Trace("Userpassword : " & .AdminPassword)
                    End If
                    NLOGLOGGER.Trace("Application  : " & .Application)
                    NLOGLOGGER.Trace("Arguments    : " & .Arguments)

                    If UAC_PROPER = False Then
                        NLOGLOGGER.Error("UAC_PROPER   : " & UAC_PROPER)
                        NLOGLOGGER.Error("* UWC_EnableLUA                  : " & REGISTRY_UAC_EnableLUA & " <= should be '0'")
                        NLOGLOGGER.Error("* UWC_ConsentPromptBehaviorAdmin : " & REGISTRY_UAC_ConsentPromptBehaviorAdmin & " <= should be '0'")
                        NLOGLOGGER.Error("* UWC_EnableLinkedConnections    : " & REGISTRY_UAC_EnableLinkedConnections & " <= should be '1'")
                    Else
                        NLOGLOGGER.Trace("UAC_PROPER   : " & UAC_PROPER)
                    End If

                    StartProcess(.AdminUser, .AdminDomain, .AdminPassword, Processes.LogonFlags.WithProfile,
                                 .Application, .Arguments, Processes.CreationFlags.UnicodeEnvironment, Nothing, Processes.ShowWindowType.SW_HIDE, True)
                    NLOGLOGGER.Info("Process is started successfull")

                End With

                'With pProcess.StartInfo

                '    ' Soll eine Fehlermeldung angezeigt werden, wenn der Prozess nicht gestartet werden kann
                '    .ErrorDialog = False

                '    ' Umleiten der Ein-/Ausgaben
                '    .RedirectStandardError = True
                '    .StandardErrorEncoding = Encoding.UTF8

                '    .RedirectStandardInput = True

                '    .RedirectStandardOutput = True
                '    .StandardOutputEncoding = Encoding.UTF8

                'End With

                'With pProcess

                '    ' Ob nach dem Beenden das Exited-Event ausgelöst werden soll
                '    .EnableRaisingEvents = True

                '    ' Set up the event handler to call back with 
                '    ' each line of output 
                '    AddHandler .OutputDataReceived, AddressOf OnDataReceived
                '    AddHandler .Exited, AddressOf ProcessOnExited
                '    AddHandler .ErrorDataReceived, AddressOf ErrorDataReceived


                '    ' Startet das Auslesen des Output
                '    .BeginOutputReadLine()

                '    If ExeCommand.ApplicationIsBatch = False Then
                '        ' Beendet die Eingabe
                '        .StandardInput.Close()

                '        .BeginErrorReadLine()

                '        ' Warten bis der Prozess beendet wurde
                '        .WaitForExit()

                '        ' Sicherstellen, das die Output-Streams geschrieben werden
                '        mreOut.WaitOne()
                '        mreErr.WaitOne()

                '        _result.ReturnValue = .ExitCode
                '    Else

                '        ' Die Befehle in einzelne Elemente eines Arrays packen
                '        BatchFile = My.Computer.FileSystem.ReadAllText(ExeCommand.Application)
                '        BatchCommands = BatchFile.Split(vbCrLf)

                '        If BatchCommands.Length > 0 Then

                '            For Each cmd As String In BatchCommands
                '                .StandardInput.WriteLine(cmd)
                '                NLOGLOGGER.Debug("ProcessInput:" & cmd)
                '                .StandardInput.Flush()
                '            Next

                '        End If

                '        ' Beendet den Eingabe
                '        .StandardInput.Close()
                '    End If

                'End With

            Catch ex As System.ComponentModel.Win32Exception
                _result.ErrorMessages.Add(ex.Message)
                NLOGLOGGER.Fatal("Message   :" + ex.Message)
                NLOGLOGGER.Fatal("TargetSite: " + ex.TargetSite.ToString)
                NLOGLOGGER.Fatal("Source    : " + ex.Source.ToString)
                NLOGLOGGER.Fatal(ex.InnerException)
                NLOGLOGGER.Fatal(ex.StackTrace)

            End Try
        Else

            _result.ErrorMessages.Add("Filename not found: " & ExeCommand.Application)
            NLOGLOGGER.Fatal("Filename not found: " & ExeCommand.Application)

        End If

        Return _result
    End Function


    ' Called asynchronously with a line of data 
    Private Shared Sub OnDataReceived(ByVal Sender As Object, ByVal e As DataReceivedEventArgs)
        Dim _message As String

        If e.Data IsNot Nothing Then
            If Mid(e.Data, 2, 1) <> ":" And e.Data.Trim.Length > 0 Then
                _message = e.Data
                NLOGLOGGER.Trace("ProcessOutput: " & _message)
                If _message.StartsWith("Installing") = True Or _message.StartsWith("You are going to uninstall") = True Then
                    RAISE_SERVICE_STATE(ServiceStates.States.WORKING, _message)
                End If

            End If
        Else
            mreOut.Set()
        End If

    End Sub

    Private Shared Sub ProcessOnExited(ByVal Sender As Object, ByVal e As EventArgs)

        ' Die OutputStreams sind komplett auszugeben
        mreOut.WaitOne()
        mreErr.WaitOne()

        NLOGLOGGER.Info("Process is closed")

    End Sub

    Private Shared Sub ErrorDataReceived(sender As Object, e As DataReceivedEventArgs)

        If e.Data IsNot Nothing Then
            NLOGLOGGER.Error("ProcessErrorOutput: " & e.Data)
        Else
            mreErr.Set()
        End If

    End Sub

    ''' <summary>
    ''' Stop the executing thread for given amount of time
    ''' </summary>
    ''' <param name="TimeSeconds"></param>
    Public Shared Sub Wait(TimeSeconds As Integer)
        Thread.Sleep(TimeSeconds * 1000)
    End Sub
End Class
