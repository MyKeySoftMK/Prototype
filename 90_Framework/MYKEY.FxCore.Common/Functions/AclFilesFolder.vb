Imports System.Security.AccessControl
Imports System.IO.FileSystemAclExtensions
Imports System.DirectoryServices
Imports System.Text
Imports MYKEY.FxCore.Common.AclPermissions
Imports System.Security.Principal

Public Class AclFilesFolder

    ''' <summary>
    ''' Schreibt die Berechtigungen für ein Verzeichnis
    ''' </summary>
    ''' <param name="DirName"></param>
    ''' <param name="AclSetting"></param>
    ''' <returns></returns>
    ''' <example>var ds = new DirectorySecurity();
    '''ds.AddAccessRule(New FileSystemAccessRule(adminSI, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
    '''ds.SetAccessRuleProtection(true, false); // disable inheritance And clear any inherited permissions</example>
    Public Function SetAclDirectory(DirName As String, AclSetting As DirectorySecurity) As Boolean
        Dim _result As Boolean = False

        Try

            SetAccessControl(New IO.DirectoryInfo(DirName), AclSetting)

            _result = True
        Catch ex As Exception

        End Try

        Return _result

    End Function

    Public Function GetAclDirectory(DirName As String) As DirectorySecurity
        Dim _return As DirectorySecurity

        _return = New DirectorySecurity(DirName, AccessControlSections.Access)

        Return _return
    End Function
    Public Function SetDisableAndClearInheritance(ByRef AclSettings As DirectorySecurity)

        ' isProtected = True
        ' preserveInheritance = False
        AclSettings.SetAccessRuleProtection(True, False)

        Return AclSettings
    End Function

    ''' <summary>
    ''' Here is a helper function to show what ACL permissions you have for a directory
    ''' </summary>
    ''' <param name="Directory"></param>
    ''' <returns></returns>
    Public Function ListFolderACLs(Directory As String) As String

        ' http://justsomevbcode.blogspot.com/2012/11/acls-for-file-and-directory-access.html

        Dim oDirACLs As New DirectorySecurity(Directory, AccessControlSections.Access)
        Dim sbAccess As New StringBuilder()

        For Each oAccessRule As FileSystemAccessRule In oDirACLs.GetAccessRules(True, True, GetType(System.Security.Principal.NTAccount))
            sbAccess.AppendFormat("Account:     {0}", oAccessRule.IdentityReference.Value).AppendLine()
            sbAccess.AppendFormat("Type:        {0}", oAccessRule.AccessControlType).AppendLine()
            sbAccess.AppendFormat("Rights:      {0}", oAccessRule.FileSystemRights).AppendLine()
            sbAccess.AppendFormat("Inherited:   {0}", oAccessRule.IsInherited).AppendLine()
            sbAccess.AppendFormat("Inheritance: {0}", oAccessRule.InheritanceFlags).AppendLine()
            sbAccess.AppendFormat("Propagation: {0}", oAccessRule.PropagationFlags).AppendLine()
            sbAccess.AppendLine(New String("-"c, 25))
        Next

        Return sbAccess.ToString()
    End Function

    ''' <summary>
    ''' Here is a helper function to show what ACL permissions you have for a file
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <returns></returns>
    Public Function ListFileACLs(ByVal Filename As String) As String
        Dim oFileACLs As New FileSecurity(Filename, AccessControlSections.Access)
        Dim sbAccess As New StringBuilder()

        For Each oAccessRule As FileSystemAccessRule In oFileACLs.GetAccessRules(True, True, GetType(System.Security.Principal.NTAccount))
            sbAccess.AppendFormat("Account:     {0}", oAccessRule.IdentityReference.Value).AppendLine()
            sbAccess.AppendFormat("Type:        {0}", oAccessRule.AccessControlType).AppendLine()
            sbAccess.AppendFormat("Rights:      {0}", oAccessRule.FileSystemRights).AppendLine()
            sbAccess.AppendFormat("Inherited:   {0}", oAccessRule.IsInherited).AppendLine()
            sbAccess.AppendFormat("Inheritance: {0}", oAccessRule.InheritanceFlags).AppendLine()
            sbAccess.AppendFormat("Propagation: {0}", oAccessRule.PropagationFlags).AppendLine()
            sbAccess.AppendLine(New String("-"c, 25))
        Next

        Return sbAccess.ToString()
    End Function

    Public Sub AddDirectoryPermissions(DirName As String, Permissions As DirectoryPermission,
                                       Optional Domain As String = Nothing, Optional User As String = Nothing)


        Dim oACL As DirectorySecurity
        Dim oUserSid As Security.Principal.SecurityIdentifier
        Dim lRights As Long
        Dim lInheritance As Long
        Dim oRule As FileSystemAccessRule

        ' Get the ACL for the directory 
        oACL = GetAccessControl(New IO.DirectoryInfo(DirName), AccessControlSections.Access)

        ' Get Sid of given Username/-group
        If Not IsNothing(Domain) AndAlso Not IsNothing(User) Then
            oUserSid = New Security.Principal.NTAccount(Domain, User).Translate(GetType(Security.Principal.SecurityIdentifier))
        ElseIf Not IsNothing(User) Then
            oUserSid = New Security.Principal.NTAccount(User).Translate(GetType(Security.Principal.SecurityIdentifier))
        Else
            ' Create a security Identifier for the 
            ' BUILTIN\Users group to be passed to the new access rule
            oUserSid = New Security.Principal.SecurityIdentifier(Security.Principal.WellKnownSidType.BuiltinUsersSid, Nothing)
        End If

        ' Set the permission combination
        Select Case Permissions
            Case DirectoryPermission.Full
                lRights = FileSystemRights.FullControl
                lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit

            Case DirectoryPermission.Modify
                lRights = FileSystemRights.Modify Or FileSystemRights.Synchronize
                lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit

            Case DirectoryPermission.ReadAndExecute
                lRights = FileSystemRights.ReadAndExecute Or FileSystemRights.Synchronize
                lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit

            Case DirectoryPermission.AllExceptModifyAndFull
                lRights = FileSystemRights.Write Or FileSystemRights.ReadAndExecute Or FileSystemRights.Synchronize
                lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit

            Case DirectoryPermission.ListContents
                lRights = FileSystemRights.ReadAndExecute Or FileSystemRights.Synchronize
                lInheritance = InheritanceFlags.ContainerInherit

            Case DirectoryPermission.Read
                lRights = FileSystemRights.Read Or FileSystemRights.Synchronize
                lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit

            Case DirectoryPermission.Write
                lRights = FileSystemRights.Write Or FileSystemRights.Synchronize
                lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit

            Case Else
                ' No rights
                lRights = 0
                lInheritance = 0

        End Select

        ' Create the rule that needs to be added to the ACL
        oRule = New FileSystemAccessRule(oUserSid, lRights, lInheritance, PropagationFlags.None, AccessControlType.Allow)

        ' Add the new rule to our ACL
        oACL.AddAccessRule(oRule)

        ' Update the directory to include the new rules created
        SetAccessControl(New IO.DirectoryInfo(DirName), oACL)

    End Sub

    Public Sub SetDefaultDirectoryPermissions(DirName As String)
        Dim _DefaultAcl As New DirectorySecurity
        Dim oRule As FileSystemAccessRule
        Dim oUserSid As Security.Principal.SecurityIdentifier
        Dim lRights As Long
        Dim lInheritance As Long

        ' BUILTIN\Administrators group to be passed to the new access rule
        oUserSid = New Security.Principal.SecurityIdentifier(Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, Nothing)
        ' Full
        lRights = FileSystemRights.FullControl
        lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit
        ' Create the rule that needs to be added to the ACL
        oRule = New FileSystemAccessRule(oUserSid, lRights, lInheritance, PropagationFlags.None, AccessControlType.Allow)
        ' Add the new rule to our ACL
        _DefaultAcl.AddAccessRule(oRule)


        ' SYSTEM User to be passed to the new access rule
        oUserSid = New Security.Principal.SecurityIdentifier(Security.Principal.WellKnownSidType.LocalSystemSid, Nothing)
        ' Full
        lRights = FileSystemRights.FullControl
        lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit
        ' Create the rule that needs to be added to the ACL
        oRule = New FileSystemAccessRule(oUserSid, lRights, lInheritance, PropagationFlags.None, AccessControlType.Allow)
        ' Add the new rule to our ACL
        _DefaultAcl.AddAccessRule(oRule)


        ' Authenticates User to be passed to the new access rule
        oUserSid = New Security.Principal.SecurityIdentifier(Security.Principal.WellKnownSidType.AuthenticatedUserSid, Nothing)
        ' Modify
        lRights = FileSystemRights.Modify Or FileSystemRights.Synchronize
        lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit
        ' Create the rule that needs to be added to the ACL
        oRule = New FileSystemAccessRule(oUserSid, lRights, lInheritance, PropagationFlags.None, AccessControlType.Allow)
        ' Add the new rule to our ACL
        _DefaultAcl.AddAccessRule(oRule)


        ' BUILTIN\Users group to be passed to the new access rule
        oUserSid = New Security.Principal.SecurityIdentifier(Security.Principal.WellKnownSidType.BuiltinUsersSid, Nothing)
        ' Read and Execute
        lRights = FileSystemRights.ReadAndExecute Or FileSystemRights.Synchronize
        lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit
        ' Create the rule that needs to be added to the ACL
        oRule = New FileSystemAccessRule(oUserSid, lRights, lInheritance, PropagationFlags.None, AccessControlType.Allow)
        ' Add the new rule to our ACL
        _DefaultAcl.AddAccessRule(oRule)

        ' Remove Inheritance
        SetDisableAndClearInheritance(_DefaultAcl)

        ' Update the directory to include the new rules created
        SetAccessControl(New IO.DirectoryInfo(DirName), _DefaultAcl)

    End Sub

    Public Sub SetDefaultDirectoryPermissionsOnlyAV(DirName As String)
        Dim _DefaultAcl As New DirectorySecurity
        Dim oRule As FileSystemAccessRule
        Dim oUserSid As Security.Principal.SecurityIdentifier
        Dim oDomainSid As Security.Principal.SecurityIdentifier
        Dim lRights As Long
        Dim lInheritance As Long

        Try

            ' TODO: "Determine Domains SID" .net core
            oDomainSid = WindowsIdentity.GetCurrent.User.AccountDomainSid

            ' SYSTEM User to be passed to the new access rule
            oUserSid = New Security.Principal.SecurityIdentifier(Security.Principal.WellKnownSidType.LocalSystemSid, Nothing)
            ' Full
            lRights = FileSystemRights.FullControl
            lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit
            ' Create the rule that needs to be added to the ACL
            oRule = New FileSystemAccessRule(oUserSid, lRights, lInheritance, PropagationFlags.None, AccessControlType.Allow)
            ' Add the new rule to our ACL
            _DefaultAcl.AddAccessRule(oRule)


            '  Administrator user  to be passed to the new access rule
            oUserSid = New Security.Principal.SecurityIdentifier(Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, oDomainSid)
            ' Full
            lRights = FileSystemRights.FullControl
            lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit
            ' Create the rule that needs to be added to the ACL
            oRule = New FileSystemAccessRule(oUserSid, lRights, lInheritance, PropagationFlags.None, AccessControlType.Allow)
            ' Add the new rule to our ACL
            _DefaultAcl.AddAccessRule(oRule)


            ' Administrators group (Domain) to be passed to the new access rule
            oUserSid = New Security.Principal.SecurityIdentifier(Security.Principal.WellKnownSidType.AccountAdministratorSid, oDomainSid)
            ' Full
            lRights = FileSystemRights.FullControl
            lInheritance = InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit
            ' Create the rule that needs to be added to the ACL
            oRule = New FileSystemAccessRule(oUserSid, lRights, lInheritance, PropagationFlags.None, AccessControlType.Allow)
            ' Add the new rule to our ACL
            _DefaultAcl.AddAccessRule(oRule)


            ' Remove Inheritance
            SetDisableAndClearInheritance(_DefaultAcl)

            ' Update the directory to include the new rules created
            SetAccessControl(New IO.DirectoryInfo(DirName), _DefaultAcl)

        Catch ex As Exception

        End Try

    End Sub

End Class
