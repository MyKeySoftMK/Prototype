Imports System.Drawing.Printing
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.IO
Imports MYKEY.FxCore.Common.ApplicationLogging

''' <summary>
''' RawPrinterHelper.SendFileToPrinter("E:\XYZDocs\XYZ.pdf")
''' </summary>
''' <remarks>https://vishalsbsinha.wordpress.com/2014/05/06/how-to-programmatically-c-net-print-a-pdf-file-directly-to-the-printer/</remarks>
Public Class RawPrinting

    ' Structure and API declarions:
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)> _
    Private Class DOCINFOA
        <MarshalAs(UnmanagedType.LPStr)> _
        Public pDocName As String
        <MarshalAs(UnmanagedType.LPStr)> _
        Public pOutputFile As String
        <MarshalAs(UnmanagedType.LPStr)> _
        Public pDataType As String
    End Class

#Region "dll Wrappers"
    <DllImport("winspool.Drv", EntryPoint:="OpenPrinterA", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function OpenPrinter(<MarshalAs(UnmanagedType.LPStr)> szPrinter As String, ByRef hPrinter As IntPtr, pd As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="ClosePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function ClosePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="StartDocPrinterA", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function StartDocPrinter(hPrinter As IntPtr, level As Int32, <[In], MarshalAs(UnmanagedType.LPStruct)> di As DOCINFOA) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="EndDocPrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function EndDocPrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="StartPagePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function StartPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="EndPagePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function EndPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="WritePrinter", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
    Private Shared Function WritePrinter(hPrinter As IntPtr, pBytes As IntPtr, dwCount As Int32, ByRef dwWritten As Int32) As Boolean
    End Function

    <DllImport("winspool.drv", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function GetDefaultPrinter(pszBuffer As StringBuilder, ByRef size As Integer) As Boolean
    End Function

    Private Declare Auto Function DocumentProperties Lib "winspool.drv" _
     (ByVal hWnd As IntPtr, ByVal hPrinter As IntPtr, ByVal pDeviceName As String, _
      ByVal pDevModeOutput As IntPtr, ByVal pDevModeInput As IntPtr, ByVal fMode As Int32) As Integer

    Public Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" _
     (ByVal hpvDest As IntPtr, ByVal hpvSource As IntPtr, ByVal cbCopy As Long)

    Private Declare Function SetPrinter Lib "winspool.drv" Alias "SetPrinterA" _
     (ByVal hPrinter As IntPtr, ByVal level As Integer, ByVal pPrinterInfoIn As IntPtr, _
      ByVal command As Int32) As Boolean

#End Region

#Region "Methods"

    ''' <summary>
    ''' This function gets the pdf file name. This function opens the pdf file, 
    ''' gets all its bytes send and them to printer.
    ''' </summary>
    ''' <param name="pdfFileName"></param>
    ''' <returns></returns>
    ''' <remarks>Die Verwendung für PDFs nur bis PDFv1.4 - ab PDFv1.5 die PdfiumPrint-Methode verwenden</remarks>
    <Obsolete>
    Public Shared Function SendFileToPrinter(pdfFileName As String, Optional printerName As String = "", Optional printerTray As String = "") As Boolean
        Try
            '#Region "Get Connected Printer Name"
            Dim pd As New PrintDocument()
            Dim dp As New StringBuilder(256)
            Dim size As Integer = dp.Capacity
            Dim success As Boolean = False

            ' Ermitteln, welche Drucker installiert sind
            Dim printers As PrinterSettings.StringCollection = PrinterSettings.InstalledPrinters
            If (Not (printers) Is Nothing) Then
                For Each printer As String In printers
                    NLOGLOGGER.Debug("Printers: " & printer)
                Next
                ' Festelegen, welcher Drucker verwendet werden soll
                Try
                    If printerName.Length = 0 Then

                        ' Wenn kein Drucker angegeben wurde, dann den Standdarddrucker auswählen
                        If GetDefaultPrinter(dp, size) Then
                            pd.PrinterSettings.PrinterName = dp.ToString().Trim()
                        End If
                    Else
                        pd.PrinterSettings.PrinterName = printerName.Trim()

                    End If
                Catch ex As Exception
                End Try
                NLOGLOGGER.Debug("Printer selected: " & pd.PrinterSettings.PrinterName)

                ' Festlegen des Trays
                If printerTray.Length > 0 Then

                    For Each papSource As PaperSource In pd.PrinterSettings.PaperSources
                        NLOGLOGGER.Debug("Papersources: " & papSource.SourceName & " (RawKind = " & papSource.RawKind & ")")
                    Next

                    If pd.PrinterSettings.PaperSources.Count > 0 Then
                        Try
                            pd.DefaultPageSettings.PaperSource = pd.PrinterSettings.PaperSources(Integer.Parse(printerTray))
                            NLOGLOGGER.Debug("Printertray selected: " & pd.DefaultPageSettings.PaperSource.SourceName)
                        Catch ex As Exception

                        End Try
                    Else
                        NLOGLOGGER.Debug("Selected Printer has no papersources available")
                    End If
                End If

                '#End Region

                ' Open the PDF file.
                Dim fs As New FileStream(pdfFileName, FileMode.Open, FileAccess.Read)

                ' Create a BinaryReader on the file.
                Dim br As New BinaryReader(fs)
                Dim bytes As [Byte]() = New [Byte](fs.Length - 1) {}

                ' Unmanaged pointer.
                Dim ptrUnmanagedBytes As New IntPtr(0)
                Dim nLength As Integer = Convert.ToInt32(fs.Length)
                ' Read contents of the file into the array.
                bytes = br.ReadBytes(nLength)
                ' Allocate some unmanaged memory for those bytes.
                ptrUnmanagedBytes = Marshal.AllocCoTaskMem(nLength)
                ' Copy the managed byte array into the unmanaged array.
                Marshal.Copy(bytes, 0, ptrUnmanagedBytes, nLength)
                ' Send the unmanaged bytes to the printer.
                success = SendBytesToPrinter(pd.PrinterSettings.PrinterName, ptrUnmanagedBytes, nLength)
                ' Free the unmanaged memory that you allocated earlier.
                Marshal.FreeCoTaskMem(ptrUnmanagedBytes)

                ' Schließen des FileStreams
                fs.Close()

                Return success

            Else

                NLOGLOGGER.Fatal("No Printer detected")
                Return success
            End If

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Function

    ''' <summary>
    ''' This function gets the printer name and an unmanaged array of bytes, the function sends those bytes to the print queue.
    ''' </summary>
    ''' <param name="szPrinterName">Printer Name</param>
    ''' <param name="pBytes">No. of bytes in the pdf file</param>
    ''' <param name="dwCount">Word count</param>
    ''' <returns>True on success, false on failure</returns>
    Private Shared Function SendBytesToPrinter(szPrinterName As String, pBytes As IntPtr, dwCount As Int32) As Boolean
        Try
            Dim dwError As Int32 = 0, dwWritten As Int32 = 0
            Dim hPrinter As New IntPtr(0)
            Dim di As New DOCINFOA()
            Dim success As Boolean = False

            ' Assume failure unless you specifically succeed.
            di.pDocName = "PDF Document"
            di.pDataType = "RAW"

            ' Open the printer.
            If OpenPrinter(szPrinterName.Normalize(), hPrinter, IntPtr.Zero) Then
                ' Start a document.
                If StartDocPrinter(hPrinter, 1, di) Then
                    ' Start a page.
                    If StartPagePrinter(hPrinter) Then
                        ' Write the bytes.
                        success = WritePrinter(hPrinter, pBytes, dwCount, dwWritten)
                        EndPagePrinter(hPrinter)
                    End If
                    EndDocPrinter(hPrinter)
                End If
                ClosePrinter(hPrinter)
            End If

            ' If print did not succeed, GetLastError may give more information about the failure.
            If success = False Then
                dwError = Marshal.GetLastWin32Error()
            End If

            Return success

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Function
#End Region

#Region "Tray Handling"

    Private Const DM_OUT_BUFFER As Integer = 2
    Private Shared pOriginalDEVMODE As IntPtr

    Private Structure PRINTER_INFO_9
        Dim pDevMode As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
    Public Structure DEVMODE
        <MarshalAs(UnmanagedType.ByValTStr, Sizeconst:=32)> Public pDeviceName As String
        Public dmSpecVersion As Short
        Public dmDriverVersion As Short
        Public dmSize As Short
        Public dmDriverExtra As Short
        Public dmFields As Integer
        Public dmOrientation As Short
        Public dmPaperSize As Short
        Public dmPaperLength As Short
        Public dmPaperWidth As Short
        Public dmScale As Short
        Public dmCopies As Short
        Public dmDefaultSource As Short
        Public dmPrintQuality As Short
        Public dmColor As Short
        Public dmDuplex As Short
        Public dmYResolution As Short
        Public dmTTOption As Short
        Public dmCollate As Short
        <MarshalAs(UnmanagedType.ByValTStr, Sizeconst:=32)> Public dmFormName As String
        Public dmUnusedPadding As Short
        Public dmBitsPerPel As Integer
        Public dmPelsWidth As Integer
        Public dmPelsHeight As Integer
        Public dmNup As Integer
        Public dmDisplayFrequency As Integer
        Public dmICMMethod As Integer
        Public dmICMIntent As Integer
        Public dmMediaType As Integer
        Public dmDitherType As Integer
        Public dmReserved1 As Integer
        Public dmReserved2 As Integer
        Public dmPanningWidth As Integer
        Public dmPanningHeight As Integer

    End Structure

    Public Shared Sub SavePrinterSettings(ByVal printerName As String)

        Dim Needed As Integer
        Dim hWnd As IntPtr
        Dim hPrinter As IntPtr
        If printerName = "" Then Exit Sub

        Try
            If OpenPrinter(printerName, hPrinter, Nothing) = False Then Exit Sub
            'Save original printer settings data (DEVMODE structure)
            Needed = DocumentProperties(hWnd, hPrinter, printerName, Nothing, Nothing, 0)
            Dim pFullDevMode As IntPtr = Marshal.AllocHGlobal(Needed) 'buffer for DEVMODE structure
            DocumentProperties(hWnd, hPrinter, printerName, pFullDevMode, Nothing, DM_OUT_BUFFER)
            pOriginalDEVMODE = Marshal.AllocHGlobal(Needed)
            CopyMemory(pOriginalDEVMODE, pFullDevMode, Needed)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub


    Public Shared Sub SetTray(ByVal printerName As String, ByVal trayNumber As Integer)
        Dim hPrinter As IntPtr
        Dim Needed As Integer

        OpenPrinter(printerName, hPrinter, Nothing)

        'Get original printer settings data (DEVMODE structure)
        Needed = DocumentProperties(IntPtr.Zero, hPrinter, printerName, Nothing, Nothing, 0)
        Dim pFullDevMode As IntPtr = Marshal.AllocHGlobal(Needed) 'buffer for DEVMODE structure
        DocumentProperties(IntPtr.Zero, hPrinter, printerName, pFullDevMode, Nothing, DM_OUT_BUFFER)

        Dim pDevMode9 As DEVMODE = Marshal.PtrToStructure(pFullDevMode, GetType(DEVMODE))

        ' Tray change
        pDevMode9.dmDefaultSource = trayNumber

        Marshal.StructureToPtr(pDevMode9, pFullDevMode, True)

        Dim PI9 As New PRINTER_INFO_9
        PI9.pDevMode = pFullDevMode

        Dim pPI9 As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(PI9))
        Marshal.StructureToPtr(PI9, pPI9, True)
        SetPrinter(hPrinter, 9, pPI9, 0&)
        Marshal.FreeHGlobal(pPI9) 'pFullDevMode will be free too

        ClosePrinter(hPrinter)
    End Sub

    Public Shared Sub RestorePrinterSettings(ByVal printerName As String)
        Dim hPrinter As IntPtr
        If printerName = "" Then Exit Sub

        Try
            If OpenPrinter(printerName, hPrinter, Nothing) = False Then Exit Sub
            Dim PI9 As New PRINTER_INFO_9
            PI9.pDevMode = pOriginalDEVMODE
            Dim pPI9 As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(PI9))
            Marshal.StructureToPtr(PI9, pPI9, True)
            SetPrinter(hPrinter, 9, pPI9, 0&)
            Marshal.FreeHGlobal(pPI9) 'pOriginalDEVMODE will be free too
            ClosePrinter(hPrinter)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

#End Region

End Class
