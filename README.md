# PxKeystrokesForScreencasts

Displays Keystrokes in an overlay window. (Windows 8)

Press <code>Ctrl + Shift + Alt</code> to reveal configuration options, resize, move or close the window.

Demo Video: https://www.youtube.com/watch?v=Ud3tqB8TMVc

Screenshots below.

This is Open Source. If you miss a feature, go for it and send a Pull-Request afterwards!

# Features
 - Displays special keys (volume up: 🔊⏫) and shortcuts
 - Click Through
 - Opacity
 - Customizable: Colors, Font, Size and Position, Text Position and Orientation
 - Displays history of pressed keys
 - Icon in notification area
 - One small file, no installation required
 - Visually indicate mouse cursor position
 - Visually indicate pressed buttons

# Download

<a href="https://github.com/Phaiax/PxKeystrokesForScreencasts/raw/master/Releases/v0.3.1/PxKeystrokesUi.exe">Download PxKeystrokesUi.exe 0.3.1</a> (most recent, 2016-01-31, Win10 tested)


This security alert will appear. Click the <code>More Info</code> link, and then <code>Execute anyway</code>.

<img src="https://raw.githubusercontent.com/Phaiax/PxKeystrokesForScreencasts/master/Screenshots/Smartscreen1.png" alt="SmartScreen filter dialog.">

This dialog is ugly. To fix this, i need a validated security certificate. Certum offers a low priced <a href="http://www.certum.eu/certum/cert,offer_en_open_source_cs.xml"> 1 year certificate for open source projects </a> for € 17.22 per annum. If someone funds at least three years of certification (€ 51.66), i will use that money to buy a new certificate each year. Then I will also implement a secure autoupdater. <a href="https://github.com/Phaiax/PxKeystrokesForScreencasts/issues/15">Get in contact with me.</a> For transparency: Currently total raised money: € 0.


## Releases
You can find the exe in the <code>Releases</code> folder

- v0.0.1 (Text positioning not implemented yet)
- v0.1.1 (Looks fine)
- v0.1.2 (More Whitespace between special chars)
- v0.1.3 (fix bug with Win+? shortcuts)
- v0.2.0 (indicator for cursor position, history timeout, bugfixes, documentation)
- v0.3.0 (indicator for pressed buttons, deadkey fixes, backspace functionality)
- v0.3.1 (new icons, little fixes and internal improvements)

## Requirements
Windows 8 or 10

Windows 7 requires you to change the default font, otherwise you will see little squares instead of symbols.

# Screenshots

<a href="https://raw.githubusercontent.com/Phaiax/PxKeystrokesForScreencasts/master/Screenshots/mouse.png">
	<img src="https://raw.githubusercontent.com/Phaiax/PxKeystrokesForScreencasts/master/Screenshots/mouse.png">
</a>
<a href="https://raw.githubusercontent.com/Phaiax/PxKeystrokesForScreencasts/master/Screenshots/bottom_right.png">
	<img src="https://raw.githubusercontent.com/Phaiax/PxKeystrokesForScreencasts/master/Screenshots/bottom_right.png">
</a>
<a href="https://raw.githubusercontent.com/Phaiax/PxKeystrokesForScreencasts/master/Screenshots/bottom_center.png">
	<img src="https://raw.githubusercontent.com/Phaiax/PxKeystrokesForScreencasts/master/Screenshots/bottom_center.png">
</a>
<a href="https://raw.githubusercontent.com/Phaiax/PxKeystrokesForScreencasts/master/Screenshots/settings.png">
	<img src="https://raw.githubusercontent.com/Phaiax/PxKeystrokesForScreencasts/master/Screenshots/settings.png">
</a>


# TODO
 - Make some things a little bit more beautiful
 - Documentation, Refactoring, Testing



## Known Issues
 - Centering of Text if many arrow keys (or other special keys) have been pressed
 - Will not work in Front of Win 8 Apps


# License

Apache Version 2, refer to the file LICENSE for details