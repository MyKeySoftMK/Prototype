Imports System.Windows.Input

Public Class DelegateCommand

    Implements ICommand

#Region "Fields"

    Private ReadOnly _execute As Action
    Private ReadOnly _executeParam As Action(Of Object)
    Private ReadOnly _canExecute As Func(Of Boolean)

#End Region ' Fields 


#Region "Constructors"

    ''' <summary> 
    ''' Creates a new command that can always execute. 
    ''' </summary> 
    ''' <param name="execute">The execution logic.</param> 
    Public Sub New(ByVal execute As Action)
        Me.New(execute, Nothing)
    End Sub

    ''' <summary> 
    ''' Creates a new command. 
    ''' </summary> 
    ''' <param name="execute">The execution logic.</param> 
    ''' <param name="canExecute">The execution status logic.</param> 
    Public Sub New(ByVal execute As Action, ByVal canExecute As Func(Of Boolean))
        If execute Is Nothing Then
            Throw New ArgumentNullException("execute")
        End If

        _execute = execute
        _executeParam = Nothing
        _canExecute = canExecute

    End Sub

    ''' <summary> 
    ''' Creates a new command that can always execute. 
    ''' </summary> 
    ''' <param name="execute">The execution logic.</param> 
    Public Sub New(ByVal execute As Action(Of Object))
        Me.New(execute, Nothing)
    End Sub

    ''' <summary> 
    ''' Creates a new command. 
    ''' </summary> 
    ''' <param name="execute">The execution logic.</param> 
    ''' <param name="canExecute">The execution status logic.</param> 
    Public Sub New(ByVal execute As Action(Of Object), ByVal canExecute As Func(Of Boolean))
        If execute Is Nothing Then
            Throw New ArgumentNullException("execute")
        End If

        _execute = Nothing
        _executeParam = execute
        _canExecute = canExecute
    End Sub
#End Region

#Region "ICommand Members"

    <DebuggerStepThrough()> _
    Public Function CanExecute(ByVal parameter As Object) As Boolean Implements ICommand.CanExecute
        Return If(_canExecute Is Nothing, True, _canExecute())
    End Function

    Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
        AddHandler(ByVal value As EventHandler)
            AddHandler CommandManager.RequerySuggested, value
        End AddHandler
        RemoveHandler(ByVal value As EventHandler)
            RemoveHandler CommandManager.RequerySuggested, value
        End RemoveHandler
        RaiseEvent(ByVal sender As System.Object, ByVal e As System.EventArgs)
        End RaiseEvent
    End Event

    Public Sub Execute(ByVal parameter As Object) Implements ICommand.Execute
        If _execute IsNot Nothing Then
            _execute()
        Else
            _executeParam(parameter)
        End If

    End Sub

#End Region ' ICommand Members 

End Class

Public Class DelegateCommand(Of T)

    Implements ICommand

    Private executeMethod As Action(Of T)
    Private canExecuteMethod As Func(Of T, Boolean)

    ''' <SUMMARY>         
    ''' Occurs when changes occur that affect whether or not the command should execute.         
    ''' </SUMMARY>         
    ''' <REMARKS></REMARKS>         
    Public Event CanExecuteChanged As EventHandler Implements System.Windows.Input.ICommand.CanExecuteChanged

    ''' <SUMMARY>         
    ''' Initializes a new instance of <SEE cref="DelegateCommand(Of T)"> class.         
    ''' </SEE></SUMMARY>         
    ''' <PARAM name="executeMethod" />Method that execute when command is invoked.         
    Public Sub New(ByVal executeMethod As Action(Of T))
        MyClass.New(executeMethod, Nothing)
    End Sub

    ''' <SUMMARY>         
    ''' Initializes a new instance of <SEE cref="DelegateCommand(Of T)"> class.         
    ''' </SEE></SUMMARY>         
    ''' <PARAM name="executeMethod" />Method that execute when command is invoked.         
    ''' <PARAM name="canExecuteMethod" />Method that determines whether the command can execute in its current state.         
    Public Sub New(ByVal executeMethod As Action(Of T), ByVal canExecuteMethod As Func(Of T, Boolean))
        If executeMethod Is Nothing AndAlso canExecuteMethod Is Nothing Then
            Throw New ArgumentNullException("executeMethod")
        End If
        Me.executeMethod = executeMethod
        Me.canExecuteMethod = canExecuteMethod
    End Sub

    ''' <SUMMARY>         
    '''  Determines whether the command can execute in its current state.         
    ''' </SUMMARY>         
    ''' <PARAM name="parameter" />Data used by the command. If the command does not require data to be passed, this object can be set to null.         
    ''' <RETURNS>True if this command can be executed; otherwise, False.</RETURNS>         
    Public Function CanExecute(ByVal parameter As Object) As Boolean Implements System.Windows.Input.ICommand.CanExecute
        If canExecuteMethod Is Nothing Then
            Return True
        End If
        If parameter Is Nothing Then
            Return canExecuteMethod(Nothing)
        End If
        Return canExecuteMethod(CType(parameter, T))
    End Function

    ''' <SUMMARY>         
    ''' Called when the command is invoked.         
    ''' </SUMMARY>         
    ''' <PARAM name="parameter" />Data used by the command. If the command does not require data to be passed, this object can be set to null.         
    Public Sub Execute(ByVal parameter As Object) Implements System.Windows.Input.ICommand.Execute
        If executeMethod Is Nothing Then
            Return
        End If
        If parameter Is Nothing Then
            executeMethod(Nothing)
        Else
            executeMethod(CType(parameter, T))
        End If
    End Sub

End Class
