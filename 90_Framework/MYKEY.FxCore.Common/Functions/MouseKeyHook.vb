Public Class MouseKeyHook

End Class


'Private IKeyboardMouseEvents m_GlobalHook;

'Public void Subscribe()
'{
'    // Note: For the application hook, use the Hook.AppEvents() instead
'    m_GlobalHook = Hook.GlobalEvents();

'    m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
'    m_GlobalHook.KeyPress += GlobalHookKeyPress;
'}

'Private void GlobalHookKeyPress(Object sender, KeyPressEventArgs e)
'{
'    Console.WriteLine("KeyPress: \t{0}", e.KeyChar);
'}

'Private void GlobalHookMouseDownExt(Object sender, MouseEventExtArgs e)
'{
'    Console.WriteLine("MouseDown: \t{0}; \t System Timestamp: \t{1}", e.Button, e.Timestamp);

'    // uncommenting the following line will suppress the middle mouse button click
'    // if (e.Buttons == MouseButtons.Middle) { e.Handled = true; }
'}

'Public void Unsubscribe()
'{
'    m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
'    m_GlobalHook.KeyPress -= GlobalHookKeyPress;

'    //It Is recommened to dispose it
'    m_GlobalHook.Dispose();
'}