' ###################################################################
' #  T4-Name	: GenerateWebViewModelEntryPointsInterface.tt       #
' #  Date		: 2021-10-27                                        #
' #  Version	: 1                           (c) MyKey-Soft 2021   #
' ###################################################################
Imports MYKEY.FxCore.Common.Application
Imports System.Windows
Imports MYKEY.FxCore.Common
Imports KMF.WrkFlows.Cars.DataAccess.Cars

Public Interface ICarManagement_ViewModelEntryPoints

#Region "Functions"


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="QParam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function EntryPoint_DefineFullQueryParameter(Optional QParam As QueryParameters = Nothing) As QueryParameters

#End Region

#Region "Methods"
    

#End Region


End Interface

