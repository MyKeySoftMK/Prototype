Imports MYKEY.FxCore.Common.Application

Partial Public Class CarManagementViewModel

    Implements ICarManagement_ViewModelEntryPoints

    Public Function EntryPoint_DefineFullQueryParameter(Optional QParam As QueryParameters = Nothing) As QueryParameters Implements ICarManagement_ViewModelEntryPoints.EntryPoint_DefineFullQueryParameter
        If QParam Is Nothing Then
            QParam = New QueryParameters
        End If

        'With QParam
        '    .Includes.Add("Kingdomhall")
        '    .Includes.Add("ServiceGroups")
        '    .Includes.Add("Languages")
        'End With

        Return QParam
    End Function
End Class
