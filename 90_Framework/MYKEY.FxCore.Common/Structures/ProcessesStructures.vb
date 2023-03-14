Imports System.Runtime.InteropServices

Partial Public Class Processes

    ''' <summary> 
    ''' The ProcessInformation structure is filled in by either the CreateProcess, CreateProcessAsUser, CreateProcessWithLogonW, or CreateProcessWithTokenW function with information about the newly created process and its primary thread. 
    ''' </summary> 
    <StructLayout(LayoutKind.Sequential)>
    Public Structure ProcessInformation

        ''' <summary> 
        ''' Handle to the newly created process. The handle is used to specify the process in all functions that perform operations on the process object. 
        ''' </summary> 
        Public hProcess As IntPtr

        ''' <summary> 
        ''' Handle to the primary thread of the newly created process. The handle is used to specify the thread in all functions that perform operations on the thread object. 
        ''' </summary> 
        Public hThread As IntPtr

        ''' <summary> 
        ''' Value that can be used to identify a process. The value is valid from the time the process is created until the time the process is terminated. 
        ''' </summary> 
        Public dwProcessId As Integer

        ''' <summary> 
        ''' Value that can be used to identify a thread. The value is valid from the time the thread is created until the time the thread is terminated. 
        ''' </summary> 
        Public dwThreadId As Integer

    End Structure

    ''' <summary> 
    ''' The STARTUPINFO structure is used with the CreateProcess, CreateProcessAsUser, and CreateProcessWithLogonW functions to specify the window station, desktop, standard handles, and appearance of the main window for the new process. 
    ''' </summary> 
    <StructLayout(LayoutKind.Sequential)>
    Public Structure RunAsStartUpInfo

        ''' <summary> 
        ''' Size of the structure, in bytes. 
        ''' </summary> 
        Public cb As Integer

        ''' <summary> 
        ''' Reserved. Set this member to NULL before passing the structure to CreateProcess. 
        ''' </summary> 
        <MarshalAs(UnmanagedType.LPTStr)>
        Public lpReserved As String

        ''' <summary> 
        ''' Pointer to a null-terminated string that specifies either the name of the desktop, or the name of both the desktop and window station for this process. A backslash in the string indicates that the string includes both the desktop and window station names. 
        ''' </summary> 
        <MarshalAs(UnmanagedType.LPTStr)>
        Public lpDesktop As String

        ''' <summary> 
        ''' For console processes, this is the title displayed in the title bar if a new console window is created. If NULL, the name of the executable file is used as the window title instead. This parameter must be NULL for GUI or console processes that do not create a new console window. 
        ''' </summary> 
        <MarshalAs(UnmanagedType.LPTStr)>
        Public lpTitle As String

        ''' <summary> 
        ''' The x offset of the upper left corner of a window if a new window is created, in pixels. 
        ''' </summary> 
        Public dwX As Integer

        ''' <summary> 
        ''' The y offset of the upper left corner of a window if a new window is created, in pixels. 
        ''' </summary> 
        Public dwY As Integer

        ''' <summary> 
        ''' The width of the window if a new window is created, in pixels. 
        ''' </summary> 
        Public dwXSize As Integer

        ''' <summary> 
        ''' The height of the window if a new window is created, in pixels. 
        ''' </summary> 
        Public dwYSize As Integer

        ''' <summary> 
        ''' If a new console window is created in a console process, this member specifies the screen buffer width, in character columns. 
        ''' </summary> 
        Public dwXCountChars As Integer

        ''' <summary> 
        ''' If a new console window is created in a console process, this member specifies the screen buffer height, in character rows. 
        ''' </summary> 
        Public dwYCountChars As Integer

        ''' <summary> 
        ''' The initial text and background colors if a new console window is created in a console application. 
        ''' </summary> 
        Public dwFillAttribute As Integer

        ''' <summary> 
        ''' Bit field that determines whether certain StartUpInfo members are used when the process creates a window. 
        ''' </summary> 
        Public dwFlags As Integer

        ''' <summary> 
        ''' This member can be any of the SW_ constants defined in Winuser.h. 
        ''' </summary> 
        Public wShowWindow As Short

        ''' <summary> 
        ''' Reserved for use by the C Runtime; must be zero. 
        ''' </summary> 
        Public cbReserved2 As Short

        ''' <summary> 
        ''' Reserved for use by the C Runtime; must be null. 
        ''' </summary> 
        Public lpReserved2 As IntPtr

        ''' <summary> 
        ''' A handle to be used as the standard input handle for the process. 
        ''' </summary> 
        Public hStdInput As IntPtr

        ''' <summary> 
        ''' A handle to be used as the standard output handle for the process. 
        ''' </summary> 
        Public hStdOutput As IntPtr

        ''' <summary> 
        ''' A handle to be used as the standard error handle for the process. 
        ''' </summary> 
        Public hStdError As IntPtr

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure SECURITY_ATTRIBUTES
        Public nLength As System.UInt32
        Public lpSecurityDescriptor As IntPtr
        Public bInheritHandle As Boolean
    End Structure

End Class
