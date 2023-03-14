Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Configuration
Imports System.Data
Imports System.Drawing

Imports MYKEY.FxCore.Common
Imports MYKEY.FxCore.Common.ApplicationLogging

''' <summary>
''' Class for managing configuration persistence
''' </summary>
''' <remarks></remarks>
Public Class Settings

    ''' <summary>
    ''' This method has to be invoked before using
    ''' any other method of FXConfiguration class
    ''' ConfigFile parameter is the name of the config file to be read
    ''' </summary>
    ''' <param name="ConfigName">Name der Konfigurationsdatei</param>
    ''' <remarks>If that file doesn't exists, the method
    ''' simply initialize the data structure
    '''and the ConfigFileName property </remarks>
    Public Shared Function Initialize(ByVal ConfigName As String, ByVal ConfigPath As String) As DataSet
        Dim mConfigFileName As String
        Dim DSOptions As DataSet
        Dim XMLEncrypt As XMLEncryptor

        ' Prüfen und gegenbenfalls erzeugen der Verzeichnisstruktur
        If Directory.Exists(ConfigPath) = False Then
            Directory.CreateDirectory(ConfigPath)
        End If

        mConfigFileName = ConfigPath & "\" & ConfigName & ".fxcfg"

        If File.Exists(mConfigFileName) Then
            NLOGLOGGER.Debug("Open XML-Config: " & mConfigFileName)
            XMLEncrypt = New XMLEncryptor(ApplicationSettings.CRYPTOKEY, ApplicationSettings.CRYPTOKEY)
            DSOptions = XMLEncrypt.ReadEncryptedXML(mConfigFileName)
        Else
            NLOGLOGGER.Debug("Create a new Settings-DataSet")
            NLOGLOGGER.Debug("=> ConfigName: " & ConfigName)
            NLOGLOGGER.Debug("=> XML-Name  : " & mConfigFileName)


            ' If the specified config file doesn't exists, 
            ' the DataSet is simply initialized (and left empty):
            ' the ConfigValues DataTable is created
            ' with two fields (to hold key/values pairs)
            Dim dt As New DataTable("ConfigValues")

            DSOptions = New DataSet(ConfigName)
            dt.Columns.Add("OptionName", System.Type.GetType("System.String"))
            dt.Columns.Add("OptionValue", System.Type.GetType("System.String"))
            dt.Columns("OptionName").ColumnMapping = MappingType.Attribute
            dt.Columns("OptionValue").ColumnMapping = MappingType.SimpleContent
            DSOptions.Tables.Add(dt)
        End If

        Return DSOptions

    End Function

    Public Shared Sub DeInitialize(ByVal ConfigName As String, ByVal ConfigPath As String)

        Dim mConfigFileName As String = ConfigPath & "\" & ConfigName & ".fxcfg"
        NLOGLOGGER.Debug("Close Config " & mConfigFileName)
        FilesIOFunctions.DeleteFile(mConfigFileName)

    End Sub

    ''' <summary>
    ''' Same as Store() method, but with the ability to serialize on a different filename
    ''' </summary>
    ''' <param name="ConfigDS"></param>
    ''' <param name="ConfigFileName"></param>
    ''' <remarks></remarks>
    Public Shared Sub Store(ByVal ConfigDS As DataSet, ByVal ConfigFileName As String, ByVal ConfigPath As String)

        Dim XMLEncrypt As New XMLEncryptor(ApplicationSettings.CRYPTOKEY, ApplicationSettings.CRYPTOKEY)
        NLOGLOGGER.Debug("Store Settings in XML-Config: " & ConfigPath & "\" & ConfigFileName)
        XMLEncrypt.WriteEncryptedXML(ConfigDS, ConfigPath & "\" & ConfigFileName)

    End Sub

    ''' <summary>
    ''' Read a configuration Value, given its name and section name 
    ''' (szOptionName). If the Key is not defined, the default value is 
    ''' returned, and the entry is added to the data set. 
    ''' string 
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function GetOption(ByVal ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal szDefault As String) As String
        Dim bAddOption As Boolean = True
        Dim szReturn As String = szDefault
        If ConfigDS.Tables(szSectionName) IsNot Nothing Then
            Dim dv As DataView = ConfigDS.Tables(szSectionName).DefaultView
            dv.RowFilter = "OptionName='" + szOptionName + "'"
            If dv.Count > 0 Then
                szReturn = dv(0)("OptionValue").ToString()
                bAddOption = False
            End If
        End If

        If bAddOption = True Then
            SetOption(ConfigDS, szSectionName, szOptionName, szDefault)
        End If

        Return szReturn
    End Function

    ''' <summary>
    ''' Overload for getting integer values 
    ''' int 
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function GetOption(ByVal ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal iDefault As Integer) As Integer
        Return Integer.Parse(GetOption(ConfigDS, szSectionName, szOptionName, iDefault.ToString()))
    End Function

    ''' <summary>
    ''' Overload for getting double values 
    ''' double
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function GetOption(ByVal ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal dDefault As Double) As Double
        Return Double.Parse(GetOption(ConfigDS, szSectionName, szOptionName, dDefault.ToString()))
    End Function

    ''' <summary>
    ''' Overload for getting color values 
    ''' double
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function GetOption(ByVal ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal dDefault As Drawing.Color) As Integer
        Return Integer.Parse(GetOption(ConfigDS, szSectionName, szOptionName, dDefault.ToArgb))
    End Function

    ''' <summary>
    ''' Overload for getting boolean values 
    ''' double
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function GetOption(ByVal ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal dDefault As Boolean) As Boolean
        Return Boolean.Parse(GetOption(ConfigDS, szSectionName, szOptionName, dDefault.ToString))
    End Function

    ''' <summary>
    ''' Overload for getting string array values 
    ''' double
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function GetOption(ByVal ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal dDefault() As String) As String()
        Return GetOption(ConfigDS, szSectionName, szOptionName, String.Join("@", dDefault)).Split(New Char() {"@"})
    End Function


    ''' <summary>
    ''' Write in the memory data structure a Key/Value pair for a 
    ''' configuration setting. If the Key already exists, the Value is 
    ''' simply updated, else the Key/Value pair is added. Warning: to update 
    ''' the written Key/Value pair on the config file, you need to call 
    ''' Store 
    ''' string
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub SetOption(ByRef ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal szOptionValue As String)
        If ConfigDS.Tables(szSectionName) Is Nothing Then
            Dim dt As New DataTable(szSectionName)
            dt.Columns.Add("OptionName", System.Type.[GetType]("System.String"))
            dt.Columns.Add("OptionValue", System.Type.[GetType]("System.String"))
            ' dt.Columns.Add("OptionType", 
            ConfigDS.Tables.Add(dt)
        End If

        Dim dv As DataView = ConfigDS.Tables(szSectionName).DefaultView
        dv.RowFilter = "OptionName='" + szOptionName + "'"
        If dv.Count > 0 Then
            dv(0)("OptionValue") = szOptionValue
        Else
            Dim dr As DataRow = ConfigDS.Tables(szSectionName).NewRow()
            dr("OptionName") = szOptionName
            dr("OptionValue") = szOptionValue
            ConfigDS.Tables(szSectionName).Rows.Add(dr)
        End If
    End Sub

    ''' <summary>
    ''' Overload of SetOption that takes an integer option value 
    ''' int
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub SetOption(ByRef ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal iOptionValue As Integer)
        SetOption(ConfigDS, szSectionName, szOptionName, iOptionValue.ToString())
    End Sub

    ''' <summary>
    ''' Overload of SetOption that takes a double option value 
    ''' double
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub SetOption(ByRef ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal dOptionValue As Double)
        SetOption(ConfigDS, szSectionName, szOptionName, dOptionValue.ToString())
    End Sub

    ''' <summary>
    ''' Overload of SetOption that takes a color option value 
    ''' double
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub SetOption(ByRef ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal dOptionValue As Drawing.Color)
        SetOption(ConfigDS, szSectionName, szOptionName, dOptionValue.ToArgb())
    End Sub

    ''' <summary>
    ''' Overload of SetOption that takes a boolean option value 
    ''' double
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub SetOption(ByRef ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal dOptionValue As Boolean)
        SetOption(ConfigDS, szSectionName, szOptionName, dOptionValue.ToString)
    End Sub

    ''' <summary>
    ''' Overload of SetOption that takes a String-Array option value 
    ''' double
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub SetOption(ByRef ConfigDS As DataSet, ByVal szSectionName As String, ByVal szOptionName As String, ByVal dOptionValue() As String)
        SetOption(ConfigDS, szSectionName, szOptionName, String.Join("@", dOptionValue))
    End Sub


End Class

