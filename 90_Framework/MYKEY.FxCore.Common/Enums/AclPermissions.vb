Public Class AclPermissions

    Public Enum DirectoryPermission
        Full
        Modify
        AllExceptModifyAndFull
        ReadAndExecute
        ListContents
        Read
        Write
        None
        ' "None" in the above list doesn't remove the security rule,
        ' but sets it to no available permissions
    End Enum
End Class
