Public Class UserManagementEnums

    Public Enum CheckCredentialsResult
        Success = 0
        UserNameNotFound = 1
        PasswordNotCorrect = 2
        NotChecked = 255
        GeneralFailure = 254
    End Enum


End Class
