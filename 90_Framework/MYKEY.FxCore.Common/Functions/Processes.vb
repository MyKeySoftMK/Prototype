Imports System.Runtime.InteropServices
Imports System.Text

Public Class Processes

    Declare Function OpenProcessToken Lib "advapi32.dll" (ByVal ProcessHandle As IntPtr, ByVal DesiredAccess As Integer, ByRef TokenHandle As IntPtr) As Boolean

    Declare Function CreateEnvironmentBlock Lib "userenv.dll" (ByRef lpEnvironment As IntPtr, ByVal hToken As IntPtr, ByVal bInherit As Boolean) As Boolean

    Declare Function DuplicateTokenEx Lib "advapi32.dll" (ByVal hExistingToken As IntPtr, ByVal dwDesiredAccess As System.UInt32, ByRef lpThreadAttributes As SECURITY_ATTRIBUTES, ByVal ImpersonationLevel As Int32, ByVal dwTokenType As Int32, ByRef phNewToken As IntPtr) As Boolean

    Private Declare Function GetExitCodeProcess Lib "kernel32" (ByVal hProcess As Long, ByVal lpExitCode As Long) As Long


    <DllImport("user32.dll")>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowThreadProcessId(ByVal hwnd As IntPtr, ByRef lpdwProcessId As Integer) As Integer
    End Function

    <DllImport("USER32.DLL", CharSet:=CharSet.Unicode)>
    Public Shared Function FindWindow(ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr

    End Function

    <DllImport("USER32.DLL")>
    Public Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As Boolean

    End Function

    ''' <summary> 
    ''' The CreateProcessWithLogonW function creates a new process and its primary thread. The new process then runs the specified executable file in the security context of the specified credentials (user, domain, and password). It can optionally load the user profile for the specified user. 
    ''' </summary> 
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function CreateProcessWithLogonW(ByVal lpszUsername As [String], ByVal lpszDomain As [String], ByVal lpszPassword As [String], ByVal dwLogonFlags As Integer, ByVal applicationName As String, ByVal commandLine As StringBuilder,
    ByVal creationFlags As UInteger, ByVal environment As IntPtr, ByVal currentDirectory As String, ByRef sui As RunAsStartUpInfo, ByRef processInfo As ProcessInformation) As Boolean
    End Function

    <DllImport("kernel32", CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
    Public Shared Function WaitForSingleObject(ByVal hHandle As IntPtr, ByVal dwMilliseconds As Integer) As Integer
    End Function

    ''' <summary> 
    ''' Closes an open object handle. 
    ''' </summary> 
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function CloseHandle(ByVal handle As IntPtr) As Boolean
    End Function

    ''' <summary> 
    ''' Creates a new process and its primary thread. The new process then runs the 
    ''' specified executable file in the security context of the specified 
    ''' credentials (user, domain, and password). It can optionally load the user 
    ''' profile for the specified user. 
    ''' </summary> 
    ''' <remarks> 
    ''' This method is untested. 
    ''' </remarks> 
    ''' <param name="userName"> 
    ''' This is the name of the user account to log on to. If you use the UPN format, 
    ''' user@domain, the Domain parameter must be NULL. The user account must have 
    ''' the Log On Locally permission on the local computer. 
    ''' </param> 
    ''' <param name="domain"> 
    ''' Specifies the name of the domain or server whose account database contains the 
    ''' user account. If this parameter is NULL, the user name must be specified in 
    ''' UPN format. 
    ''' </param> 
    ''' <param name="password"> 
    ''' Specifies the clear-text password for the user account. 
    ''' </param> 
    ''' <param name="logonFlags"> 
    ''' Logon option. This parameter can be zero or one value from the LogonFlags enum. 
    ''' </param> 
    ''' <param name="applicationName"> 
    ''' Specifies the module to execute. The specified module can be a Windows-based 
    ''' application. It can be some other type of module (for example, MS-DOS or OS/2) 
    ''' if the appropriate subsystem is available on the local computer. The string 
    ''' can specify the full path and file name of the module to execute or it can 
    ''' specify a partial name. In the case of a partial name, the function uses the 
    ''' current drive and current directory to complete the specification. The function 
    ''' will not use the search path. If the file name does not contain an extension, 
    ''' .exe is assumed. Therefore, if the file name extension is .com, this parameter 
    ''' must include the .com extension. The appname parameter can be NULL. In that 
    ''' case, the module name must be the first white space-delimited token in the 
    ''' commandline string. If the executable module is a 16-bit application, appname 
    ''' should be NULL, and the string pointed to by commandline should specify the 
    ''' executable module as well as its arguments. 
    ''' </param> 
    ''' <param name="commandLine"> 
    ''' Specifies the command line to execute. The maximum length of this string is 
    ''' 32,000 characters. The commandline parameter can be NULL. In that case, the 
    ''' function uses the string pointed to by appname as the command line. If the 
    ''' file name does not contain an extension, .exe is appended. Therefore, if the 
    ''' file name extension is .com, this parameter must include the .com extension. 
    ''' If the file name ends in a period with no extension, or if the file name 
    ''' contains a path, .exe is not appended. If the file name does not contain a 
    ''' directory path, the system searches for the executable file. 
    ''' </param> 
    ''' <param name="creationFlags"> 
    ''' Use CreationFlags and PriorityFlags enums. Controls how the process is created. 
    ''' Also controls the new process's priority class, which is used to determine the 
    ''' scheduling priorities of the process's threads. 
    ''' </param> 
    ''' <param name="currentDirectory"> 
    ''' Specifies the full path to the current directory for the process. The string 
    ''' can also specify a UNC path. If this parameter is NULL, the new process will 
    ''' have the same current drive and directory as the calling process. 
    ''' </param> 
    ''' <returns> 
    ''' Returns a System.Diagnostic.Process which will be null if the call failed. 
    ''' </returns> 
    ''' <exception cref="System.ComponentModel.Win32Exception"> 
    ''' Throws a System.ComponentModel.Win32Exception containing the last error if the 
    ''' call failed. 
    ''' </exception> 
    Public Shared Function StartProcess(ByVal userName As String, ByVal domain As String, ByVal password As String, ByVal logonFlags As LogonFlags, ByVal applicationName As String, ByVal commandLine As String,
    ByVal creationFlags As CreationFlags, ByVal currentDirectory As String, Optional ByVal ShowWindow As ShowWindowType = ShowWindowType.SW_NORMAL, Optional ByVal CopyCurrentEnvBlock As Boolean = True) As System.Diagnostics.Process
        Dim processInfo As ProcessInformation
        Dim startupInfo As New RunAsStartUpInfo()

        Dim enviromentBlock As IntPtr

        startupInfo.cb = Marshal.SizeOf(startupInfo)
        startupInfo.lpTitle = Nothing
        startupInfo.wShowWindow = ShowWindow
        startupInfo.dwFlags = CInt(StartUpInfoFlags.UseCountChars + StartUpInfoFlags.UseShowWindow)

        startupInfo.dwYCountChars = 50

        If CopyCurrentEnvBlock = True Then
            Dim ps As Process() = Process.GetProcessesByName("explorer")
            Dim processId As Integer = -1
            If ps.Length > 0 Then
                processId = ps(0).Id
            End If
            If processId > 1 Then
                Dim token As IntPtr = GetPrimaryToken(processId)
                If Not (token.Equals(IntPtr.Zero)) Then
                    enviromentBlock = GetEnvironmentBlock(token)
                End If
            End If
        Else
            enviromentBlock = IntPtr.Zero
        End If

        Return StartProcess(userName, domain, password, logonFlags, applicationName, commandLine,
        creationFlags, enviromentBlock, currentDirectory, startupInfo, processInfo)
    End Function

    ''' <summary> 
    ''' Creates a new process and its primary thread. The new process then runs the 
    ''' specified executable file in the security context of the specified 
    ''' credentials (user, domain, and password). It can optionally load the user 
    ''' profile for the specified user. 
    ''' </summary> 
    ''' <remarks> 
    ''' This method is untested. 
    ''' </remarks> 
    ''' <param name="userName"> 
    ''' This is the name of the user account to log on to. If you use the UPN format, 
    ''' user@domain, the Domain parameter must be NULL. The user account must have 
    ''' the Log On Locally permission on the local computer. 
    ''' </param> 
    ''' <param name="domain"> 
    ''' Specifies the name of the domain or server whose account database contains the 
    ''' user account. If this parameter is NULL, the user name must be specified in 
    ''' UPN format. 
    ''' </param> 
    ''' <param name="password"> 
    ''' Specifies the clear-text password for the user account. 
    ''' </param> 
    ''' <param name="logonFlags"> 
    ''' Logon option. This parameter can be zero or one value from the LogonFlags enum. 
    ''' </param> 
    ''' <param name="applicationName"> 
    ''' Specifies the module to execute. The specified module can be a Windows-based 
    ''' application. It can be some other type of module (for example, MS-DOS or OS/2) 
    ''' if the appropriate subsystem is available on the local computer. The string 
    ''' can specify the full path and file name of the module to execute or it can 
    ''' specify a partial name. In the case of a partial name, the function uses the 
    ''' current drive and current directory to complete the specification. The function 
    ''' will not use the search path. If the file name does not contain an extension, 
    ''' .exe is assumed. Therefore, if the file name extension is .com, this parameter 
    ''' must include the .com extension. The appname parameter can be NULL. In that 
    ''' case, the module name must be the first white space-delimited token in the 
    ''' commandline string. If the executable module is a 16-bit application, appname 
    ''' should be NULL, and the string pointed to by commandline should specify the 
    ''' executable module as well as its arguments. 
    ''' </param> 
    ''' <param name="commandLine"> 
    ''' Specifies the command line to execute. The maximum length of this string is 
    ''' 32,000 characters. The commandline parameter can be NULL. In that case, the 
    ''' function uses the string pointed to by appname as the command line. If the 
    ''' file name does not contain an extension, .exe is appended. Therefore, if the 
    ''' file name extension is .com, this parameter must include the .com extension. 
    ''' If the file name ends in a period with no extension, or if the file name 
    ''' contains a path, .exe is not appended. If the file name does not contain a 
    ''' directory path, the system searches for the executable file. 
    ''' </param> 
    ''' <param name="creationFlags"> 
    ''' Use CreationFlags and PriorityFlags enums. Controls how the process is created. 
    ''' Also controls the new process's priority class, which is used to determine the 
    ''' scheduling priorities of the process's threads. 
    ''' </param> 
    ''' <param name="currentDirectory"> 
    ''' Specifies the full path to the current directory for the process. The string 
    ''' can also specify a UNC path. If this parameter is NULL, the new process will 
    ''' have the same current drive and directory as the calling process. 
    ''' </param> 
    ''' <param name="environment"> 
    ''' Pointer to an environment block for the new process. If this parameter is NULL, 
    ''' the new process uses the environment of the specified user instead of the 
    ''' environment of the calling process. 
    ''' </param> 
    ''' <param name="startupInfo"> 
    ''' Specifies the window station, desktop, standard handles, and appearance of the 
    ''' main window for the new process. 
    ''' </param> 
    ''' <param name="processInfo"> 
    ''' ProcessInformation structure that receives identification information for the 
    ''' new process, including a handle to the process. 
    ''' </param> 
    ''' <returns> 
    ''' Returns a System.Diagnostic.Process which will be null if the call failed. 
    ''' </returns> 
    ''' <exception cref="System.ComponentModel.Win32Exception"> 
    ''' Throws a System.ComponentModel.Win32Exception containing the last error if the 
    ''' call failed. 
    ''' </exception> 
    Public Shared Function StartProcess(ByVal userName As String, ByVal domain As String, ByVal password As String, ByVal logonFlags As LogonFlags, ByVal applicationName As String, ByVal commandLine As String,
    ByVal creationFlags As CreationFlags, ByVal environment As IntPtr, ByVal currentDirectory As String, ByRef startupInfo As RunAsStartUpInfo, ByRef processInfo As ProcessInformation) As System.Diagnostics.Process
        Dim cl As New StringBuilder(commandLine.Length)
        Const INFINITE = -1&
        Dim ret As Long

        Try
            cl.Append(commandLine)
            Dim retval As Boolean = CreateProcessWithLogonW(userName, domain, password, CInt(logonFlags), applicationName, cl, CUInt(creationFlags), environment, currentDirectory, startupInfo, processInfo)

            If Not retval Then
                Throw New System.ComponentModel.Win32Exception()
            Else

                ret = WaitForSingleObject(processInfo.hProcess, INFINITE)
                'GetExitCodeProcess(processInfo.hProcess, ret)

                CloseHandle(processInfo.hProcess)
                CloseHandle(processInfo.hThread)
                'Return System.Diagnostics.Process.GetProcessById(processInfo.dwProcessId)
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Private Shared Function GetPrimaryToken(ByVal processId As Integer) As IntPtr
        Dim token As IntPtr = IntPtr.Zero
        Dim primaryToken As IntPtr = IntPtr.Zero
        Dim retVal As Boolean = False
        Dim p As Process = Nothing
        Try
            p = Process.GetProcessById(processId)
        Catch generatedExceptionVariable0 As ArgumentException
            Dim details As String = String.Format("ProcessID {0} Not Available", processId)
            Debug.WriteLine(details)
            Return primaryToken
        End Try
        retVal = OpenProcessToken(p.Handle, TOKEN_DUPLICATE, token)
        If retVal = True Then
            Dim sa As SECURITY_ATTRIBUTES = New SECURITY_ATTRIBUTES
            sa.nLength = Convert.ToUInt32(Marshal.SizeOf(sa))
            retVal = DuplicateTokenEx(token, Convert.ToUInt32(TOKEN_ASSIGN_PRIMARY Or TOKEN_DUPLICATE Or TOKEN_QUERY), sa, CType(SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, Integer), CType(TOKEN_TYPE.TokenPrimary, Integer), primaryToken)
            If retVal = False Then
                Dim message As String = String.Format("DuplicateTokenEx Error: {0}", Marshal.GetLastWin32Error)
                Debug.WriteLine(message)
            End If
        Else
            Dim message As String = String.Format("OpenProcessToken Error: {0}", Marshal.GetLastWin32Error)
            Debug.WriteLine(message)
        End If
        Return primaryToken
    End Function

    Private Shared Function GetEnvironmentBlock(ByVal token As IntPtr) As IntPtr
        Dim envBlock As IntPtr = IntPtr.Zero
        Dim retVal As Boolean = CreateEnvironmentBlock(envBlock, token, False)
        If retVal = False Then
            Dim message As String = String.Format("CreateEnvironmentBlock Error: {0}", Marshal.GetLastWin32Error)
            Debug.WriteLine(message)
        End If
        Return envBlock
    End Function

    Public Shared Function GetListOfRunningProcesses() As List(Of String)
        Dim _result As New List(Of String)
        Dim _localAll As Process()

        _localAll = Process.GetProcesses
        For Each proc As Process In _localAll
            _result.Add(proc.ProcessName)
        Next

        Return _result
    End Function

    Public Shared Function GetCurrentForegroundProccess() As String
        Dim _result As String = ""
        Dim lngPid As Integer
        Dim _ActiveProcess As Process

        Dim hwnd As IntPtr = GetForegroundWindow()
        If hwnd <> Nothing Then
            GetWindowThreadProcessId(hwnd, lngPid)
            _ActiveProcess = Process.GetProcessById(lngPid)
            _result = _ActiveProcess.ProcessName
        End If

        Return _result

    End Function


    Public Shared Function GetProcessByName(ProcessName As String) As Process()

        Dim _result As Process()

        _result = Process.GetProcessesByName(ProcessName)


        Return _result

    End Function


    Public Shared Function SetForegroundWindowByName(ProcessName As String) As Boolean

        Dim _result As Boolean = False

        Dim _ProcHandle As IntPtr = GetProcessByName(ProcessName)(0).MainWindowHandle

        If _ProcHandle = IntPtr.Zero Then
            Exit Function
        End If

        SetForegroundWindow(_ProcHandle)

        _result = True

        Return _result

    End Function

End Class
