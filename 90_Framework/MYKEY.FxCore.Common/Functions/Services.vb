Imports System.ServiceProcess
Imports System.Management
Imports System.Management.ManagementObject


Public Class Services

    Public Enum Mode
        Start = 1
        Stoping = 2
        Restart = 3

    End Enum

    Public Shared Function ControlService(ServiceName As String, ProcMode As Mode) As FunctionResult

        Dim _result As New FunctionResult
        Dim sc As ServiceController


        Try

            sc = New ServiceController(ServiceName)


            Select Case ProcMode
                Case Mode.Start
                    If sc.Status <> ServiceControllerStatus.Running Then
                        sc.Start()
                    End If

                Case Mode.Stoping
                    If sc.Status = ServiceControllerStatus.Running Then
                        sc.Stop()
                    End If

                Case Mode.Restart
                    If sc.Status = ServiceControllerStatus.Running Then
                        sc.Stop()
                    End If
                    sc.Start()

            End Select

        Catch ex As Exception

        End Try


        Return _result

    End Function

    Public Shared Function ControlService(ServiceName As String, ProcMode As Mode, RemoteHost As String, UserName As String, Password As String)
        Dim _result As New FunctionResult
        Dim conOptions As New ConnectionOptions
        Dim scope As ManagementScope
        Dim selectQuery As SelectQuery
        Dim searcherWMI As ManagementObjectSearcher
        Dim searchResult As ManagementObjectCollection
        Dim WMIRoot As String

        Try

            With conOptions
                .Username = UserName
                .Password = Password
            End With


            WMIRoot = "\\" & RemoteHost & "\root\cimv2"
            scope = New ManagementScope(WMIRoot, conOptions)


            selectQuery = New SelectQuery("select * from Win32_Service where name = '" + ServiceName + "'")

            searcherWMI = New ManagementObjectSearcher(scope, selectQuery)

            searchResult = searcherWMI.Get()

            For Each sc As ManagementObject In searchResult


                Select Case ProcMode
                    Case Mode.Start

                        If sc("started").Equals(False) Then
                            sc.InvokeMethod("StartService", Nothing)
                        End If

                    Case Mode.Stoping
                        If sc("started").Equals(True) Then
                            sc.InvokeMethod("StopService", Nothing)
                        End If

                    Case Mode.Restart
                        If sc("started").Equals(True) Then
                            sc.InvokeMethod("StopService", Nothing)
                        End If
                        sc.InvokeMethod("StartService", Nothing)

                End Select


            Next



        Catch ex As Exception

        End Try


        Return _result

    End Function

End Class
