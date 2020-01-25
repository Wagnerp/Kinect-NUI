Copyleft (c) 2011, Kevin Connolly
http://en.wikipedia.org/wiki/Copyleft
http://research.microsoft.com/en-us/um/legal/kinectsdk-tou_noncommercial.htm
http://www.kconnolly.net/
mailto:kevin@kconnolly.net


_____
Overhead and Introduction

1. I didn't want to make this such a legal document, but Microsoft requires a crap-ton of legal erratta. See "Legal erratta" below.
2. I do not own at the time of this writing, nor have I ever owned, an XBox of any derivations thereof. Therefore some of my verbiage and assumptions may be incorrect.
3. If you'd like to contribute to this project, contact me and include your CodePlex username, list of applicable skill sets, and what you hope to add to the project.

_____
Requirements:
1. Microsoft Windows. I have only tested this software on Windows 7 Ultimate x64. Your mileage may vary.
2. Kinect sensor plugged into and recognized by the computer.
3. You may need to install the Kinect SDK itself from http://research.microsoft.com/en-us/um/redmond/projects/kinectsdk/download.aspx
	I believe that's where the drivers come from.  This project does not include the drivers. There is also additional legal crapola there which you may need to agree to.
4. This is a personal project. I have no idea what minimum hardware you need, other than a PC and a Kinect. Works on my "Dev & Test" system below.
5. Microsoft .NET Framework 4.0 or higher: http://www.microsoft.com/download/en/details.aspx?id=17851
6. Must be run "As Administrator". It manages external processes and performs Win32 API calls and hardware hooks to complete many of its tasks. Limited users may not have 
	access to do everything; therefore, limited users may lose functionality and see unexpected UAC prompts and errors.
7. Must be run in Aero Glass mode. Failure to do so will result in the loss of some features which implicitly use it.
8. There are some sensitivity settings and other constants or variables within the code that you may need to tweak for your particular hardware configuration.
	I have tried to make these easy to find. Look near the top of each .cs file. For example, your desktop probably isn't over 7000 pixels wide.
9. For best results, position yourself two to three meters from the Kinect sensor, and/or in accordance with the guidelines that came with your Kinect sensor.
10. For best results, position yourself standing upright with no obstructions between you and the sensor. Use the preview windows in the UI to estimate what the sensor can see.

Dev & Test System:
AMD Phenom 2 1055T CPU (6 cores)
16GB Ripjaw DDR3 
(3x) ATI Radeon HD4670 video cards
(6x) Asus 24" Widescreen Monitors
(1x) Vizio 26" Widescreen HDTV
Desktop Resolution: 7680x2160 (Not a typo)
I bet I've dropped a few jaws at the office where they review the Steam Hardware Survey :)

_____
How to use Gestures:

(Axis Definitions): X = horizontal; Y = vertical; Z = depth (distance from sensor).

1. Always position yourself directly in front of the Kinect camera when starting a gesture. If you fall out of the detectable range, you may 
	lose some functionality. Use the three included preview images as feedback as to how the project can see you. Skeleton preview is the most 
	important, as this project tracks your body movements. This technology has a long way to go and sometimes fails to realize how fluid human 
	motion can be. Even when you're trying to move like a robot. Keep in mind that some gestures may interfere and/or race with each other. 
	Gestures start disabled. The first gesture you do must be the "Enable Gestures" gesture.
2. When making motions from side to side (X axis), try to keep your hands nearby their origin in YZ axis plane.
	When making motions up and down (Y axis), try to keep your hands nearby their origin in XZ axis plane.
	When making motions from in and out (Z axis), try to keep your hands nearby their origin in XY axis plane.e
	When making motions that make use of multiple axes (XY, YZ, XZ), try to keep your hands nearby their origin in the remaining axis line.
3. Both hands slowly move up to Enable Gestures. Move up about 18 inches in a one-second timeframe.
4. Both hands slowly move down to Disable Gestures. Move down about 18 inches in a one-second timeframe.
5. Both hands start from the belt area and move outward and up (like in Disco) to Zoom In via Windows Magnifier.
	- Sometimes this doesn't work if Magnifier is already running. The Project will attempt to kill any previously running instances of Magnifier.
6. Both hands start at the top outer corners and move quickly toward the belt area to Zoom Out via Windows Magnifier. (this one sometimes acts wonky)
	- Considering the previous point, when you zoom out, the project will attempt to kill any newly instantiated instances of Magnifier after you zoom out. This is why "zoom out" is less "smooth".
7. While in Magnified (zoomed in) mode:
	- Moving your pelvis from side to side (i.e. walking) will scroll horizontally.
	- Moving your left hand up and down will scroll vertically.
8. With your hands horizontally aligned (same X axis), move swiftly inward to a center point directly in front of you to enter Flip3D mode.
9. While in Flip3D mode:
	- The selected window will track your right hand, but in relation to the left hand's position. Both have to move to scroll.
	- Move left hand back and right hand forward to "push" the window stack back.
	- Move right hand back and left hand forward to "pull" the window stack toward you.
	- Place both hands together directly in front of you and push away along the X axis (sideways) to exit Flip3D mode.
10. With your left hand about 6" left of your left shoulder, move your left hand forward about 18 inches to open the Pie Menu.
	Note the Pie Menu really doesn't do anything yet. Forthcoming feature.
11. While not in Magnified, Flip3D or other special modes, push your right hand forward about 18 inches to "grab" the currently active window.
	- Move your right hand around to move the "grabbed" window. Remember this is in XY space in relation to the Kinect sensor, not the monitor(s).
	- Move your right hand back (away from sensor) about 18 inches to "release" the "grabbed" window. It should stay where you put it.

Watch my YouTube videos for practical "hands-on" (pardon the pun. Technically, hands are "off") demonstrations on how the gestures work.
http://www.youtube.com/tsilb

_____
Additional Features
- The UI will include three video previews: Video, Depth, and Skeleton.
- For reasons I haven't figured out yet, the Depth preview looks like what I'm calling an "LSD Rainbow"; but it still seems to work.
- If you can't see the Skeleton preview, the program can't see you. That's how it detects gestures.
- The UI will include circles and trails to track the positions of your hands. The radius of the circles and width of the trails indicates the 
	Z dimension (distance from sensor). 
- The app will work when minimized or resized to your preference; However, doing so may impact the feedback it provides you as to what it's doing and why.
- The UI includes a textbox which outputs information on what it detects and what it's doing.

_____
Known Issues
- You may notice the infrared / depth sensor transmitter remains on after the program closes. This appears to be normal and can be reset by unplugging it or rebooting the computer.
- Sometimes when you use the Flip3D or Zoom/Magnifier features, the app may leave the "Windows" key pressed. Test this by hitting the 'E' button and seeing if that opens an Explorer window. 
	If it does, simply press and release the Windows key on your keyboard to correct the issue.
- Sometimes when you exit the application, certain keys on the keyboard stop functioning. These include T, H, C, X, and V. Rebooting seems to be the only way to correct this issue.
- The LED indicator on the Kinect is always blinking green. The Kinect SDK does not appear to talk to the Kinect sensor in exactly the same way as an XBox 360 or XBox 360 S does.
- This was originally a dirty hack, so I really don't like the architecture of the solution (SLN).  Refactoring would be awesome. Maybe someday.

_____
Legal Erratta

Portions of this code have been modified based upon the "Code Samples" in the Kinect SDK.
The Kinect SDK is licensed under the following agreement:
http://research.microsoft.com/en-us/um/legal/kinectsdk-tou_noncommercial.htm

Amusingly, the SDK itself has a readme which links you there. That page says you agree to it simply by downloading the SDK.  
Well, if I hadn't downloaded it in the first place, how would I even know that link existed?

This project is not endorsed nor created by Microsoft; though it may include code based on the "Code Samples" provided in the Kinect SDK. 

Microsoft arbitrarily and ridiculously requires that I enforce the following EULA.  I don't like that, but it's my legal responsibility 
to do so. All text below the underscore line below is the Microsoft-enforced and Microsoft-required EULA in its entirety, verbatim, 
except formatting and clause (4). I added that little gem myself.

The way I read Clause (3) and the linked agreement(s), I'm allowed to give you this source code, but you're not allowed to make your 
own derivative works and make THAT code available to others.  I may be wrong; but in that case, see clause (4). If you want to do that,
you'll have to create your own derivative works directly from the Kinect SDK "Code Samples" themselves. Feel free to use this project
as "inspiration" as long as you do not otherwise violate this joke of a EULA, nor any other jokes of EULAs.

I'll take this project down if Microsoft requires I do so; but I'll bitch and moan about it all over the Internet.

____________________________________________________________________________________________________________
License Restrictions
1.	The software may not be used for any commercial purpose;
2.	The software may not be used or run on any software platform other than Microsoft Windows; and
3.	You may not do anything with or to the software in any way that would subject it to the terms of an Excluded License. 
	An Excluded License is a license that requires, as a condition of use, modification, or distribution of code subject to that license, that: 
		(i) the code be disclosed or distributed in source code form; or 
		(ii) others have the right to modify the code.
4.	WHEREAS, I have created this project as "personal experimentation" for the purposes of linked agreement(s), and
	WHEREAS, This project functions as a Proof of Concept; thus, one of its intentions is to show Microsoft where I want them to go with Kinect, and 
	WHEREAS, Microsoft has shown themselves to excel at writing agreements that often expressly infuriate anyone who reads and actually understands them, and
	WHEREAS, This project is not "commercial" for the purposes of linked agreement(s);
		Therefore, Thuswise, and Henceforth, notwithstanding the aforementioned, nor the forementioned, and wherein the previously stipulated agreement 
		has authorized creators of derivative works to amend their license restrictions, as indicated in Attachment 2 of the aforementioned agreement 
		link; Henceforth, thuswise, and thenby, The following is hereby included and thenceforth enforced, where applicable and not otherwise prohibited, 
		superceded, or overruled:
		(i) Clause 4 in its entirety hereby overrules and supercedes all other clauses within this agreement and all linked agreements to an infinite 
			level of linking recursion, where applicable, where not prohibited by law, and where not prohibited or superceded by a higher or legally 
			overruling authority.
		(ii) You are free to redistribute and modify this code as much as you want, as long as you:
			(a) Include a link to the original, non-modified version of the complete code and accompanying files, including this agreement:
				http://kinectnui.codeplex.com/
			(b) Include a link to the original, non-modified version of the Kinect SDK: http://research.microsoft.com/en-us/um/redmond/projects/kinectsdk/download.aspx
			(c) Give credit to the people and organizations whose work you are building upon, including me and Microsoft;
			(d) Comply with all of Microsoft's BS.
		(iii) I hereby give to Microsoft, without charge, the right to use, share, and commercialize these modifications in any way and for any purpose. In fact, 
			I hereby PLEAD to Microsoft to include gesture recognition technology similar to the herein included code as integrated features within future versions of Windows 
			and/or other Microsoft operating systems and platforms as applicable. We've had this kind of technology for years. What the hell have you been waiting for?
		(iv) I believe this code is different enough from the "Code Samples" to count as totally different code.
		(v) This is a software project based on the Kinect SDK. The Kinect SDK, as of this writing, is "research software" and "pre-release beta software".  Therefore, I promise nothing.
		(vi) The Kinect SDK software may connect to Microsoft CEIP, likely to "improve" future Microsoft software. http://www.microsoft.com/products/ceip/en-us/privacypolicy.mspx
		(vii) Exporting this software outside the United States may fall under jurisdictions and laws I am unfamiliar with. Be sure to learn on your own how this works.
			Be careful; this software is really cool and techy, so the military might want it; so it may be classifiable as a munition.  I don't know. Do not download or use this project
			if it's illegal on either side of the download. http://www.microsoft.com/exporting/default.htm
		(viii) You must comply with all legal documentation from Microsoft where applicable, including that which came with your Kinect.
		(ix) If you violate any clause of this agreement or any agreements linked therein, I probably have no way of knowing.
		(x) All copyrights, trademarks, and everything else is property of their respective owners, where applicable.

Feedback
	If you provide feedback, input, or suggestions (“Feedback”) on the software, you grant to Kevin Connolly and its licensors, under all intellectual 
	property rights, the right to make, use, modify, distribute, and otherwise commercialize your Feedback in any way and for any purpose.

DISCLAIMER
	This software is provided by the Kevin Connolly and its licensors “as is” and any express or implied warranties, including, but not 
	limited to, the implied warranties of merchantability and fitness for a particular purpose are disclaimed. In no event will the Kevin Connolly 
	or its licensors be liable for any direct, indirect, incidental, special, exemplary, or consequential damages (including, but not limited to, 
	procurement of substitute goods or services; loss of use, data, or profits; or business interruption) however caused and on any theory of liability, 
	whether in contract, strict liability, or tort (including negligence or otherwise) arising in any way out of the use of this software, even if advised 
	of the possibility of such damage. You can recover from Kevin Connolly and its licensors only direct damages up to U.S. $5.00.
