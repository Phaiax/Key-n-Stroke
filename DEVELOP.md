# PxKeystrokesForScreencasts
(aka PxKS)

This is a little documentation about how the source code is organized and works

## Main (Program.cs)

The main function is found in Program.cs.

It initializes the classes decribed below.
 * new KeyboardHook()
 * new KeystrokeParser()
 * new SettingsStore()
 * new KeystrokeDisplay()

The keypress information is intercepted in KeyboardHook, passed to KeystrokeParser and then passed to KeystrokeDisplay.


## How key events are intercepted from the system

In Windows, you can register a callback (also known as hook) for certain process messages like [keyboard][LowLevelKeyboardProc] and mouse events.

Callbacks are registered using the function [SetWindowsHookEx(event type, callback function, ...)][SetWindowsHookEx].
This function is not part of PxKS but part of the system library user32.dll. In PxKS we only need to define how each of those system functions looks. This is done in the files NativeMethodsKeyboard.cs, NativeMethodsMouse.cs, NativeMethodsGWL.cs and NativeMethodsSWP.cs.

Sometimes special data structures and constants are needed for these system functions (for example [KBDLLHOOKSTRUCT][KBDLLHOOKSTRUCT]).They are also defined in these files.


### System calls (KeyboardHook.cs KeyboardRawEvents.cs)

Now back to the key events. The class that encapsulates the calls to the system functions is KeyboardHook in KeyboardHook.cs. It registers the keyboard event system callback on creation and unregisters it on disposing/deconstructing. The related functions are RegisterKeyboardHook() and UnregisterKeyboardHook().

KeyboardHook exposes the C# event <code>KeyEvent</code> that takes methods of delegate type <code>KeyboardRawEventHandler</code>. (via <code>interface IKeyboardRawEventProvider</code> in KeyboardRawEvent.cs).

The idea is, that you just do this nice pure C# thing

```	void hook_KeyEvent(KeyboardRawEventArgs raw_e)
	{
		// process hook
	}
	IKeyboardRawEventProvider myKeyboardHook = new KeyboardHook();
	hook.KeyEvent += hook_KeyEvent; ```

... instead of dealing with the raw system library calls.

The KeyboardHook class does a little bit more. It executes multiple system calls to find out which modifier keys (shift, ...) are currently pressed and appends this information to <code>raw_e</code>.

### Key event processing (KeystrokeParser.cs KeystrokeEvent.cs)

Next, the <code>KeyboardRawEventArgs raw_e</code> are converted into Keystrokes. This is happening in a similar interface/event pattern.

The idea is, that the KeystrokeParser gets the RawEvents as input, determines what should be displayed to the user (for example a simple letter or a more complex information like CTRL + A), and forwards the result to the next program part using a C# event.

Input: The KeystrokeParser registers itself on the <code>KeyEvent</code> of the KeyboardHook in the constructor.

Output: The KeystrokeParser exposes the C# event <code>KeystrokeEvent</code> that takes methods of delegate type KeystrokeEventHandler. (via interface IKeystrokeEventProvider in KeystrokeEvent.cs).

During Calculation, the KeystrokeParser uses the static methods in <code>KeyboardLayoutParser</code> and <code>SpecialkeysParser</code> (can be found in the two .cs files) to do some of the conversion.

The <code>KeyboardLayoutParser</code> wraps some other system library functions that convert raw key information to corresponding letters with respect to the chosen keyboard layout by the user (for example us QWERTY vs german QWERTZ).

The <code>SpecialkeysParser</code> is simply a big switch statement that converts special function keys on keyboards like 'volume up' and normal keys like 'ESC' and 'tab' to text or unicode symbols like 🔊, which are then displayed in the UI.


The output of the KeystrokeParser is used by the KeystrokesDisplay.

[SetWindowsHookEx]: https://msdn.microsoft.com/en-us/library/windows/desktop/ms644990%28v=vs.85%29.aspx "SetWindowsHookEx function"
[LowLevelKeyboardProc]: https://msdn.microsoft.com/en-us/library/windows/desktop/ms644985%28v=vs.85%29.aspx "LowLevelKeyboardProc callback function"
[TranslateMessage]: https://msdn.microsoft.com/en-us/library/windows/desktop/ms644955%28v=vs.85%29.aspx "TranslateMessage function"
[KBDLLHOOKSTRUCT]: https://msdn.microsoft.com/en-us/library/windows/desktop/ms644967%28v=vs.85%29.aspx "KBDLLHOOKSTRUCT structure"

## Displaying Keystrokes (KeystrokesDisplay.cs code (F7))

 * displays new keys as wished by the information in KeystrokeEventArgs (<code>k_KeystrokeEvent()</code>)
 * Checks if resizing mode is activated (<code>CheckForSettingsMode()</code>)
 * allows resizing and moving of window
 * keeps track of the History of keystrokes (<code>List&lt;TweenLabel&gt; tweenLabels</code>)

### TweenLabel (TweenLabel.cs code (F7))

The TweenLabel is a C# System.Windows.Forms.Control that displays a line of keystrokes in the UI and is capable of fading in and out and nicely moving around in the window.

## Settings

Are stored in the C# Application.UserAppDataRegistry using the wrapper functions in SettingsStore.cs.

The Settings are changed in the UI window Settings.cs

## NativeMethodsSWP.cs

A wrapper for calling the system library function SetWindowPos.
The class NativeMethodsSWP provides a method to pin the UI on top of everything.

## NativeMethodsGWL.cs

Some wrappers for calling system library functions.
They provide methods to make the UI through clickable and clickable again.

## UrlOpener.cs

Open a browser.