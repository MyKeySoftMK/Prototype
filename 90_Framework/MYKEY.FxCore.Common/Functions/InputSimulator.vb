Public Class InputSimulator

    ' Using InputSimulator

    'Before using Input Simulator you will need to include the required types at the top of your classes

    'Using WindowsInput.Native; 
    'Using WindowsInput;

    'Virtual code list

    'If you want To simulate the keypress Event Of any key, you will need it's keycode that you can get on this list. However you can just simply use the enum property of WindowsInput.Native.VirtualKeyCode that has a property for every key.
    'Simulate Keypress

    'To simulate a single keypress event, use the Keyboard.KeyPress method that expects the virtual key code of the key you want to simulate:

    'InputSimulator sim = New InputSimulator();

    '// Press 0 key
    'sim.Keyboard.KeyPress(VirtualKeyCode.VK_0);
    '// Press 1
    'sim.Keyboard.KeyPress(VirtualKeyCode.VK_1);
    '// Press b
    'sim.Keyboard.KeyPress(VirtualKeyCode.VK_B);
    '// Press v
    'sim.Keyboard.KeyPress(VirtualKeyCode.VK_V);
    '// Press enter
    'sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
    '// Press Left CTRL button
    'sim.Keyboard.KeyPress(VirtualKeyCode.LCONTROL);

    'As mentioned, this logic isn't limited to a single keypress event but too for KeyDown and KeyUp too.
    'Simulate Keystrokes

    '    If you want To simulate keystrokes Or keyboard shortcuts, you can use the ModifiedKeyStroke Function Of the Keyboard:

    'InputSimulator sim = New InputSimulator();

    '// CTRL + C (effectively a copy command in many situations)
    'sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);

    '// You can simulate chords with multiple modifiers
    '// For example CTRL + K + C whic Is simulated as
    '// CTRL-down, K, C, CTRL-up
    'sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, New[] {
    '    VirtualKeyCode.VK_K, VirtualKeyCode.VK_C
    '});

    '// You can simulate complex chords with multiple modifiers And key presses
    '// For example CTRL-ALT-SHIFT-ESC-K which Is simulated as
    '// CTRL + down, ALT + down, SHIFT + down, press ESC, press K, SHIFT-up, ALT-up, CTRL-up
    'sim.Keyboard.ModifiedKeyStroke(
    '    New[] { VirtualKeyCode.CONTROL, VirtualKeyCode.MENU, VirtualKeyCode.SHIFT },
    '    New[] { VirtualKeyCode.ESCAPE, VirtualKeyCode.VK_K }
    ');

    'Type entire words

    'The TextEntry method Of the Keyboard simulates uninterrupted text entry via the Keyboard:

    'InputSimulator Simulator = New InputSimulator();

    'Simulator.Keyboard.TextEntry("Hello World !");

    'The simulator API Is chainable, so you can use the Sleep method To wait some milliseconds before starting Or after typing something:

    'InputSimulator Simulator = New InputSimulator();

    '// Wait a second to start typing
    'Simulator.Keyboard.Sleep(1000)
    '// Type Hello World    
    '.TextEntry("Hello World !")
    '// Wait another second
    '.Sleep(1000)
    '// Type More text
    '.TextEntry("Type another thing")
    ';

    'Simulate typing Of text by characters

    'If you are lazy And want To create snippets that make your life easier, you may want To create some type Of roboscript that writes some text For you. Obviously the text needs To exist already so it can be used In projects Like videos where you can't type errors:

    '/// <summary>
    '/// Simulate typing of any text as you do when you write.
    '/// </summary>
    '/// <param name="Text">Text that will be written automatically by simulation.</param>
    '/// <param name="typingDelay">Time in ms to wait after 1 character Is written.</param>
    '/// <param name="startDelay"></param>
    'Private void simulateTypingText(String Text, int typingDelay = 100, int startDelay = 0)
    '{
    '    InputSimulator sim = New InputSimulator();

    '    // Wait the start delay time
    '    sim.Keyboard.Sleep(startDelay);

    '    // Split the text in lines in case it has
    '    String[] lines = Text.Split(New String[] { "\r\n", "\n" }, StringSplitOptions.None);

    '    // Some flags to calculate the percentage
    '    int maximum = lines.Length;
    '    int current = 1;

    '    foreach (string line in lines)
    '    {
    '        // Split line into characters
    '        Char[] words = line.ToCharArray();

    '        // Simulate typing of the char i.e a, e , i ,o ,u etc
    '        // Apply immediately the typing delay
    '        foreach (char word in words)
    '        {
    '            sim.Keyboard.TextEntry(word).Sleep(typingDelay);
    '        }

    '        float percentage = ((float)current / (float)maximum) * 100;

    '        current++;

    '        // Add a New line by pressing ENTER
    '        // Return to start of the line in your editor with HOME
    '        sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
    '        sim.Keyboard.KeyPress(VirtualKeyCode.HOME);

    '        // Show the percentage in the console
    '        Console.WriteLine("Percent : {0}", percentage.ToString());
    '    }
    '}

    'The method expects As first argument the text that will be typed As an human would Do. The Default typing delay Is Of 100 milliseconds which Is usually the typing delay Of an human after every keypress. The last argument Is Optional And provides only a time delay When To start typing:

    '// Simulate typing text of a textbox multiline
    'simulateTypingText(textBox1.Text);

    '// Simulate typing slowly by waiting half second after typing every character
    'simulateTypingText(textBox1.Text, 500);

    '// Simulate typing slowly by waiting half second after typing every character
    '// And wait 5 seconds before starting
    'simulateTypingText(textBox1.Text, 500, 5000);

End Class
