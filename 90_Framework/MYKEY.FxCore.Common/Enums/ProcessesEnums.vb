Partial Public Class Processes

    ' Die Basis dieser Implementierung stammt von http://www.mycsharp.de/wbb2/thread.php?threadid=67479
    ' Sie wurde dann von C# nach VB.NET übersetzt.
    ' Die aktuelle Implementierung enthält nur die für den UWC-Agenten notwendigen Klassen und Funktionen


    ''' <summary> 
    ''' Logon option. 
    ''' </summary> 
    <FlagsAttribute()>
    Public Enum LogonFlags

        ''' <summary> 
        ''' Log on, then load the user's profile in the HKEY_USERS registry key. The function returns after the profile has been loaded. Loading the profile can be time-consuming, so it is best to use this value only if you must access the information in the HKEY_CURRENT_USER registry key. 
        ''' </summary> 
        WithProfile = 1

        ''' <summary> 
        ''' Log on, but use the specified credentials on the network only. The new process uses the same token as the caller, but the system creates a new logon session within LSA, and the process uses the specified credentials as the default credentials. 
        ''' </summary> 
        NetworkCredentialsOnly = 2

    End Enum


    ''' <summary> 
    ''' Controls how the process is created. The DefaultErrorMode, NewConsole, and NewProcessGroup flags are enabled by default— even if you do not set the flag, the system will function as if it were set. 
    ''' </summary> 
    <FlagsAttribute()>
    Public Enum CreationFlags
        ''' <summary> 
        ''' The primary thread of the new process is created in a suspended state, and does not run until the ResumeThread function is called. 
        ''' </summary> 
        Suspended = &H4

        ''' <summary> 
        ''' The new process has a new console, instead of inheriting the parent's console. 
        ''' </summary> 
        NewConsole = &H10

        ''' <summary> 
        ''' The new process is the root process of a new process group. 
        ''' </summary> 
        NewProcessGroup = &H200

        ''' <summary> 
        ''' This flag is only valid starting a 16-bit Windows-based application. If set, the new process runs in a private Virtual DOS Machine (VDM). By default, all 16-bit Windows-based applications run in a single, shared VDM. 
        ''' </summary> 
        SeperateWOWVDM = &H800

        ''' <summary> 
        ''' Indicates the format of the lpEnvironment parameter. If this flag is set, the environment block pointed to by lpEnvironment uses Unicode characters. 
        ''' </summary> 
        UnicodeEnvironment = &H400

        ''' <summary> 
        ''' The new process does not inherit the error mode of the calling process. 
        ''' </summary> 
        DefaultErrorMode = &H4000000

    End Enum

    Public Enum ShowWindowType As Short
        SW_HIDE = 0
        SW_NORMAL = 1
        SW_SHOWMINIMIZED = 2
        SW_MAXIMIZE = 3
        SW_SHOWNOACTIVATE = 4
        SW_SHOW = 5
        SW_MINIMIZE = 6
        SW_SHOWMINNOACTIVE = 7
        SW_SHOWNA = 8
        SW_SHOWDEFAULT = 10
    End Enum

    ''' <summary> 
    ''' Determines whether certain StartUpInfo members are used when the process creates a window. 
    ''' </summary> 
    <FlagsAttribute()>
    Public Enum StartUpInfoFlags As UInteger

        ''' <summary> 
        ''' If this value is not specified, the wShowWindow member is ignored. 
        ''' </summary> 
        UseShowWindow = &H1

        ''' <summary> 
        ''' If this value is not specified, the dwXSize and dwYSize members are ignored. 
        ''' </summary> 
        UseSize = &H2

        ''' <summary> 
        ''' If this value is not specified, the dwX and dwY members are ignored. 
        ''' </summary> 
        UsePosition = &H4

        ''' <summary> 
        ''' If this value is not specified, the dwXCountChars and dwYCountChars members are ignored. 
        ''' </summary> 
        UseCountChars = &H8

        ''' <summary> 
        ''' If this value is not specified, the dwFillAttribute member is ignored. 
        ''' </summary> 
        UseFillAttribute = &H10

        ''' <summary> 
        ''' Indicates that the process should be run in full-screen mode, rather than in windowed mode. 
        ''' </summary> 
        RunFullScreen = &H20

        ''' <summary> 
        ''' Indicates that the cursor is in feedback mode after CreateProcess is called. The system turns the feedback cursor off after the first call to GetMessage. 
        ''' </summary> 
        ForceOnFeedback = &H40

        ''' <summary> 
        ''' Indicates that the feedback cursor is forced off while the process is starting. The Normal Select cursor is displayed. 
        ''' </summary> 
        ForceOffFeedback = &H80

        ''' <summary> 
        ''' Sets the standard input, standard output, and standard error handles for the process to the handles specified in the hStdInput, hStdOutput, and hStdError members of the StartUpInfo structure. If this value is not specified, the hStdInput, hStdOutput, and hStdError members of the STARTUPINFO structure are ignored. 
        ''' </summary> 
        UseStandardHandles = &H100

        ''' <summary> 
        ''' When this flag is specified, the hStdInput member is to be used as the hotkey value instead of the standard-input pipe. 
        ''' </summary> 
        UseHotKey = &H200

        ''' <summary> 
        ''' When this flag is specified, the StartUpInfo's hStdOutput member is used to specify a handle to a monitor, on which to start the new process. This monitor handle can be obtained by any of the multiple-monitor display functions (i.e. EnumDisplayMonitors, MonitorFromPoint, MonitorFromWindow, etc...). 
        ''' </summary> 
        UseMonitor = &H400

        ''' <summary> 
        ''' Use the HICON specified in the hStdOutput member (incompatible with UseMonitor). 
        ''' </summary> 
        UseIcon = &H400

        ''' <summary> 
        ''' Program was started through a shortcut. The lpTitle contains the shortcut path. 
        ''' </summary> 
        TitleShortcut = &H800

        ''' <summary> 
        ''' The process starts with normal priority. After the first call to GetMessage, the priority is lowered to idle. 
        ''' </summary> 
        Screensaver = &H8000000

    End Enum

    Friend Enum SECURITY_IMPERSONATION_LEVEL
        SecurityAnonymous = 0
        SecurityIdentification = 1
        SecurityImpersonation = 2
        SecurityDelegation = 3
    End Enum

    Friend Enum TOKEN_TYPE
        TokenPrimary = 1
        TokenImpersonation = 2
    End Enum

End Class
