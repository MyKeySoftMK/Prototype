Imports MYKEY.FxCore.Common.ApplicationLogging
Imports System.Speech.Synthesis

Public Class ClientMachineApplicationSettings

    Public Shared Sub InitSettings()

    End Sub

    ''' <summary>
    ''' Zur Benutzung des Serial-COM-Ports
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property AppSerialComPort As MYKEY.FxCore.Common.SerialPortCommunication = Nothing

    ''' <summary>
    ''' Zur Benutzung der Sprachausgabe
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property SpeechVoice As SpeechSynthesizer = Nothing

End Class
