Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Data
Imports System.Security
Imports System.Security.Cryptography
Imports System.Xml

''' <summary> 
''' Summary: 
''' -------- 
''' The XMLEncryptor class provides methods to save and read a DataSet as an encrypted XML file. 
''' 
''' Main functionality: 
''' ------------------- 
''' 1. Save any DataSet to an encrypted XML file. 
''' 2. Read the encrypted XML file back and present it to any program as a DataSet, 
''' without creating temporary files in the process 
''' 3. The encrypted XML files are marked with a signature that identifies them as being encrypted files. 
''' 
''' Limitations: 
''' ------------ 
''' 1. The class uses a user name and password to generate a combined key which is used for encryption. 
''' No methods are provided to manage the secure storage of these data. 
''' 2. Theoretically, the class can handle files up to 2GB in size. In practice, since conversions 
''' are handled in memory to avoid having temporary (decrypted) files being written to the drive, 
''' the practical size may be limited by available system resources. 
''' </summary> 
Public Class XMLEncryptor

    Private signature As Byte()
    Private username As String
    Private password As String
    Const BIN_SIZE As Integer = 4096
    Private md5Key As Byte()
    Private md5IV As Byte()
    Private validParameters As Boolean

    Public Sub New(ByVal username As String, ByVal password As String)
        Me.username = username
        Me.password = password
        If username.Length + password.Length < 6 Then
            validParameters = False
            ' abort the constructor. Calls to public functions will not work. 
            Return
        Else
            validParameters = True
        End If
        GenerateSignature()
        GenerateKey()
        GenerateIV()
    End Sub

#Region "Helper functions called from constructor only"
    ''' <summary> 
    ''' Generates a standard signature for the file. The signature may be longer than 16 bytes if deemed necessary. 
    ''' The signature, which is NOT ENCRYPTED, serves two purposes. 
    ''' 1. It allows to recognize the file as one that has been encrypted with the XMLEncryptor class. 
    ''' 2. The first bytes of each XML file are quite similar (xml version="1.0" encoding="utf-8"). 
    ''' This can be exploite to "guess" the key the file has been encrypted with. Adding a signature of a reasonably 
    ''' large number of bytes can be used to overcome this limitation. 
    ''' </summary> 
    Private Sub GenerateSignature()
        signature = New Byte(15) {123, 78, 99, 166, 0, 43, _
        244, 8, 5, 89, 239, 255, _
        45, 188, 7, 33}
    End Sub

    ''' <summary> 
    ''' Generates an MD5 key for encryption/decryption. This method is only called during construction. 
    ''' </summary> 
    Private Sub GenerateKey()
        Dim md5 As MD5 = New MD5CryptoServiceProvider()
        Dim hash As New StringBuilder(username + password)
        For i As Integer = 1 To hash.Length - 1 Step 2

            ' Manipulate the hash string - not strictly necessary. 
            Dim c As Char = hash(i - 1)
            hash(i - 1) = hash(i)
            hash(i) = c
        Next

        ' Convert the string into a byte array. 
        Dim unicode As Encoding = Encoding.Unicode
        Dim unicodeBytes As Byte() = unicode.GetBytes(hash.ToString())
        ' Compute the key from the byte array 
        md5Key = md5.ComputeHash(unicodeBytes)
    End Sub

    ''' <summary> 
    ''' Generates an MD5 Initiakization Vector for encryption/decryption. This method is only called during construction. 
    ''' </summary> 
    Private Sub GenerateIV()
        Dim md5 As MD5 = New MD5CryptoServiceProvider()
        Dim hash As String = password + username

        ' Convert the string into a byte array. 
        Dim unicode As Encoding = Encoding.Unicode
        Dim unicodeBytes As Byte() = unicode.GetBytes(hash)

        ' Compute the IV from the byte array 
        md5IV = md5.ComputeHash(unicodeBytes)
    End Sub


#End Region

#Region "Methods to write and verify the signature"

    Private Sub WriteSignature(ByVal fOut As FileStream)
        fOut.Position = 0
        fOut.Write(signature, 0, 16)
    End Sub

    Private Function VerifySignature(ByVal fIn As FileStream) As Boolean
        Dim bin As Byte() = New Byte(15) {}
        fIn.Read(bin, 0, 16)
        For i As Integer = 0 To 15
            If bin(i) <> signature(i) Then
                Return False
            End If
        Next
        ' Reset file pointer. 
        fIn.Position = 0
        Return True
    End Function


#End Region

#Region "Public Functions"

    ''' <summary> 
    ''' Reads an encrypted XML file into a DataSet. 
    ''' </summary> 
    ''' <param name="fileName">The path to the XML file.</param> 
    ''' <returns>The DataSet, or null if an error occurs.</returns> 
    Public Function ReadEncryptedXML(ByVal fileName As String) As DataSet
        Dim fi As New FileInfo(fileName)
        Dim inFile As FileStream

        ' Check for possible errors (includes verification of the signature).
        If Not validParameters Then
            Trace.WriteLine("Invalid parameters - cannot perform requested action")
            Return Nothing
        End If
        If Not fi.Exists Then
            Trace.WriteLine("Cannot perform decryption: File " + fileName + " does not exist.")
            Return Nothing
        End If
        If fi.Length > Int32.MaxValue Then
            Trace.WriteLine("This decryption method can only handle files up to 2GB in size.")
            Return Nothing
        End If

        Try
            inFile = New FileStream(fi.FullName, FileMode.Open)
        Catch exc As Exception
            Trace.WriteLine(exc.Message + "Cannot perform decryption")
            Return Nothing
        End Try
        If Not VerifySignature(inFile) Then
            Trace.WriteLine("Invalid signature - file was not encrypted using this program")
            Return Nothing
        End If

        Dim rijn As New RijndaelManaged()
        rijn.Padding = PaddingMode.Zeros
        Dim decryptor As ICryptoTransform = rijn.CreateDecryptor(md5Key, md5IV)
        ' Allocate byte array buffer to read only the xml part of the file (ie everything following the signature). 
        Dim encryptedXmlData As Byte() = New Byte(CInt(fi.Length) - signature.Length - 1) {}
        inFile.Position = signature.Length
        inFile.Read(encryptedXmlData, 0, encryptedXmlData.Length)

        ' Convert the byte array to a MemoryStream object so that it can be passed on to the CryptoStream 
        Dim encryptedXmlStream As New MemoryStream(encryptedXmlData)
        ' Create a CryptoStream, bound to the MemoryStream containing the encrypted xml data 
        Dim csDecrypt As New CryptoStream(encryptedXmlStream, decryptor, CryptoStreamMode.Read)

        ' Read in the DataSet from the CryptoStream 
        Dim data As New DataSet()
        Try
            data.ReadXml(csDecrypt, XmlReadMode.Auto)
        Catch exc As Exception
            Trace.WriteLine(exc.Message, "Error decrypting XML")
            Return Nothing
        End Try

        ' flush & close files. 
        encryptedXmlStream.Flush()
        encryptedXmlStream.Close()
        inFile.Close()
        Return data
    End Function

    ''' <summary> 
    ''' Writes a DataSet to an encrypted XML file. 
    ''' </summary> 
    ''' <param name="dataset">The DataSet to encrypt.</param> 
    ''' <param name="encFileName">The name of the encrypted file. Existing files will be overwritten.</param> 
    Public Sub WriteEncryptedXML(ByVal dataset As DataSet, ByVal encFileName As String)
        Dim fOut As FileStream

        ' Check for possible errors
        If Not validParameters Then
            Trace.WriteLine("Invalid parameters - cannot perform requested action")
            Return
        End If

        ' Create a MemoryStream and write the DataSet to it. 
        Dim xmlStream As New MemoryStream()
        dataset.WriteXml(xmlStream)
        ' Reset the pointer of the MemoryStream (which is at the EOF after the WriteXML function). 
        xmlStream.Position = 0

        ' Create a write FileStream and write the signature to it (unencrypted). 
        fOut = New FileStream(encFileName, FileMode.Create)
        WriteSignature(fOut)

        ' Encryption objects
        Dim rijn As New RijndaelManaged()
        rijn.Padding = PaddingMode.Zeros
        Dim encryptor As ICryptoTransform = rijn.CreateEncryptor(md5Key, md5IV)
        Dim csEncrypt As New CryptoStream(fOut, encryptor, CryptoStreamMode.Write)

        'Create variables to help with read and write. 
        Dim bin As Byte() = New Byte(BIN_SIZE - 1) {}
        ' Intermediate storage for the encryption. 
        Dim rdlen As Integer = 0
        ' The total number of bytes written. 
        Dim totlen As Integer = CInt(xmlStream.Length)
        ' The total length of the input stream. 
        Dim len As Integer
        ' The number of bytes to be written at a time. 
        'Read from the input file, then encrypt and write to the output file. 
        While rdlen < totlen
            len = xmlStream.Read(bin, 0, bin.Length)
            If len = 0 AndAlso rdlen = 0 Then
                Trace.WriteLine("No read")
                Exit While
            End If
            csEncrypt.Write(bin, 0, len)
            rdlen += len
        End While
        csEncrypt.FlushFinalBlock()
        csEncrypt.Close()
        fOut.Close()
        xmlStream.Close()
    End Sub
#End Region

End Class
