Imports System.Security.Cryptography


Public Class CryptingHASH

    ''' <summary>
    ''' Definiert die Salt-Länge
    ''' </summary>
    ''' <remarks></remarks>
    Private saltLength As Integer = 6

    ''' <summary>
    ''' Erzeugt ein Kennwort mit einem Saltwert um es in der Datenbank zu hinterlegen
    ''' </summary>
    ''' <param name="Password">Kennwort, dass gespeichert werden soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePasswordForDb(ByVal Password As String) As Byte()
        ' Für das Umwandeln eines String in ein Byte-Array
        Dim enc As New System.Text.ASCIIEncoding

        Dim sha1 As SHA1 = sha1.Create

        'Create a salt value.
        Dim saltValue(saltLength - 1) As Byte

        Dim rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()
        rng.GetBytes(saltValue)

        Return CreateSaltedPassword(saltValue, sha1.ComputeHash(enc.GetBytes(Password)))
    End Function

    ''' <summary>
    ''' Erzeugt ein Kennwort mit einem Saltwert
    ''' </summary>
    ''' <param name="saltValue">Zufälliger Saltwert um ein Kennwort zu hashen/salten</param>
    ''' <param name="unsaltedPassword">Kennwort, dass gespeichert werden soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateSaltedPassword(ByVal saltValue As Byte(), ByVal unsaltedPassword As Byte()) As Byte()
        ' Add the salt to the hash.
        Dim rawSalted(unsaltedPassword.Length + saltValue.Length - 1) As Byte
        unsaltedPassword.CopyTo(rawSalted, 0)
        saltValue.CopyTo(rawSalted, unsaltedPassword.Length)

        'Create the salted hash.         
        Dim sha1 As SHA1 = sha1.Create()
        Dim saltedPassword As Byte() = sha1.ComputeHash(rawSalted)

        ' Add the salt value to the salted hash.
        Dim dbPassword(saltedPassword.Length + saltValue.Length - 1) As Byte
        saltedPassword.CopyTo(dbPassword, 0)
        saltValue.CopyTo(dbPassword, saltedPassword.Length)

        Return dbPassword
    End Function

    ''' <summary>
    ''' Vergleicht das gehashte Kennwort gegen ein gespeichertes Kennwort
    ''' </summary>
    ''' <param name="storedPassword">Gespeichertes Kennwort</param>
    ''' <param name="Password">Kennwort das überprüft werden soll</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ComparePasswords(ByVal storedPassword As Byte(), ByVal Password As String) As Boolean
        Dim sha1 As SHA1 = sha1.Create
        Dim hashedPassword As Byte()
        ' Für das Umwandeln eines String in ein Byte-Array
        Dim enc As New System.Text.ASCIIEncoding

        hashedPassword = sha1.ComputeHash(enc.GetBytes(Password))

        If ((storedPassword Is Nothing) Or (hashedPassword Is Nothing) Or (hashedPassword.Length <> storedPassword.Length - saltLength)) Then
            Return False
        End If

        ' Get the saved saltValue.
        Dim saltValue(saltLength - 1) As Byte
        Dim saltOffset As Integer = storedPassword.Length - saltLength
        Dim i As Integer = 0
        For i = 0 To saltLength - 1
            saltValue(i) = storedPassword(saltOffset + i)
        Next

        Dim saltedPassword As Byte() = CreateSaltedPassword(saltValue, hashedPassword)

        ' Compare the values.
        Return CompareByteArray(storedPassword, saltedPassword)
    End Function

    ''' <summary>
    ''' Vergleicht den Inhalt von zwei Byte-Arrays
    ''' </summary>
    ''' <param name="array1"></param>
    ''' <param name="array2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CompareByteArray(ByVal array1 As Byte(), ByVal array2 As Byte()) As Boolean
        If (array1.Length <> array2.Length) Then
            Return False
        End If

        Dim i As Integer
        For i = 0 To array1.Length - 1
            If (array1(i) <> array2(i)) Then
                Return False
            End If
        Next

        Return True
    End Function

End Class
