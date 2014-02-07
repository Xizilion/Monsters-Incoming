#pragma strict

import System.Collections.Generic;

// This script will only work with the TNG skin
var TNGSkin : GUISkin;
var Image1 : Texture2D;

// Some info about our loading animation
var LoadingAnimation1 : Texture2D;
private var LoadingAnimation1TileX : int = 5; //Here you can place the number of columns of your sheet. 
private var LoadingAnimation1TileY : int = 6; //Here you can place the number of rows of your sheet. 
private var LoadingAnimation1FPS : float = 30.0;
private var LoadingAnimation1TexOffset : Vector2 = Vector2(0, 0);
private var LoadingAnimation1TexSize : Vector2 = Vector2(1.0 / LoadingAnimation1TileX, 1.0 / LoadingAnimation1TileY);
private var LoadingAnimation1Start : boolean = true;
private var LoadingAnimation1Percentage : float = 0.0;
private var LA1PUT : float = 0.100;
private var LA1NPUT : float = 0.0;

// Scaling the GUI
private var originalWidth : float = 1920;
private var originalHeight : float = 1200;
private var scale: Vector3;

// Windows
private var Windows : WindowsInfo[] = new WindowsInfo[4];

// The first window info
Windows[0].rect = Rect(0, 0, 491, 723);
Windows[0].Alpha = 0.0;
Windows[0].UIAlpha = 0.0;
Windows[0].Show = true;
Windows[0].TimeToWait = 0.0;
Windows[0].Speed = 2.0;

// The second window info
Windows[1].rect = Rect(491, 0, 491, 723);
Windows[1].Alpha = 0.0;
Windows[1].UIAlpha = 0.0;
Windows[1].Show = true;
Windows[1].TimeToWait = 0.0;
Windows[1].Speed = 2.0;

// The third window info
Windows[2].rect = Rect(982, 0, 491, 723);
Windows[2].Alpha = 0.0;
Windows[2].UIAlpha = 0.0;
Windows[2].Show = true;
Windows[2].TimeToWait = 0.0;
Windows[2].Speed = 2.0;

// The login window
Windows[3].rect = Rect(1473, 0, 414, 485);
Windows[3].Alpha = 0.0;
Windows[3].UIAlpha = 0.0;
Windows[3].Show = true;
Windows[3].TimeToWait = 1.0; // If you wish to delay the Fade execution, set this value to the UI time when it should execute, eg: Time.time +_2.0;
Windows[3].Speed = 2.0;

// Toggle booleans
private var rememberme = false;

private var textAreaStr : String = "Fusce ac justo ornare, tempor purus eu, sagittis diam. Donec eu erat eget odio ullamcorper iaculis. Proin placerat tincidunt velit, id pharetra dolor tempor vitae.Donec eu erat eget odio ullamcorper iaculis.";
private var textFieldStr : String = "Click to edit this text input field";
private var textFieldStr1 : String = "Click to edit this text input field";
private var textFieldStr2 : String = "Please enter username";
private var textFieldStr3 : String = "Please enter password";

private var scrollPosition : Vector2 = Vector2.zero;
private var scrollViewText : String = "Fusce ac justo ornare, tempor purus eu, sagittis diam. Donec eu erat eget odio ullamcorper iaculis. Proin placerat tincidunt velit, id pharetra dolor tempor vitae.Donec eu erat eget odio ullamcorper iaculis. Fusce ac justo ornare, tempor purus eu, sagittis diam. Donec eu erat eget odio ullamcorper iaculis. Fusce ac justo ornare, tempor purus eu, sagittis diam. Donec eu erat eget odio ullamcorper iaculis. Proin placerat tincidunt velit, id pharetra dolor tempor vitae.Donec eu erat eget odio ullamcorper iaculis.";

private var radioSelected : int = 0;

private var sliderValue0 : float = 0.5;
private var sliderValue1 : float = 0.5;

function Update()
{
	// Window Animations
	// Loop trough our windows
	for (var i : int = 0; i < Windows.Length; i++)
	{
		// Check if delay is set
		if (Time.time >= Windows[i].TimeToWait)
		{
			var newAlpha : float;
			
			// Determine wether to fadeIn or fadeOut
			if (Windows[i].Show)
			{
				// FadeIn
				if (Windows[i].Alpha < 1.0)
		    	{
		    		newAlpha = Windows[i].Alpha + (Time.deltaTime * Windows[i].Speed);
					Windows[i].Alpha = newAlpha;
					Windows[i].UIAlpha = newAlpha;
				}
				else
				{
					Windows[i].Alpha = 1.0; // Accounts for Time.deltaTime variance
					Windows[i].UIAlpha = 1.0;
				}
			}
			else
			{
				// FadeOut
				if (Windows[i].Alpha > 0.0)
		    	{
		    		newAlpha = Windows[i].Alpha - (Time.deltaTime * Windows[i].Speed);
					Windows[i].Alpha = newAlpha;
					Windows[i].UIAlpha = newAlpha - (Time.deltaTime * Windows[i].Speed) * 2; // On FadeOut we must increase the UI fading speed abit
				}
				else
				{
					Windows[i].Alpha = 0.0; // Accounts for Time.deltaTime variance
					Windows[i].UIAlpha = 0.0;
				}
			}
		}
	}
    
    // Loading Animation 1
    if (LoadingAnimation1Start && LoadingAnimation1)
    {
	    // Calculate index
		var index : int = Time.time * LoadingAnimation1FPS;
		// repeat when exhausting all frames
		index = index % (LoadingAnimation1TileX * LoadingAnimation1TileY);
	 
		// Size of every tile
		LoadingAnimation1TexSize = Vector2 (1.0 / LoadingAnimation1TileX, 1.0 / LoadingAnimation1TileY);
	 
		// split into horizontal and vertical index
		var uIndex = index % LoadingAnimation1TileX;
		var vIndex = index / LoadingAnimation1TileX;
	 	
		// build offset
		// v coordinate is the bottom of the image in opengl so we need to invert.
		LoadingAnimation1TexOffset = Vector2(uIndex * LoadingAnimation1TexSize.x, 1.0 - LoadingAnimation1TexSize.y - vIndex * LoadingAnimation1TexSize.y);
		
		// Calculate the percentage
		var totalFrames : int = LoadingAnimation1TileX * LoadingAnimation1TileY;
		var percentage  : float;
		percentage = (1.0 * index) / (1.0 * totalFrames);
		percentage = percentage * 100;
		
		// Set the percentage for usage outside of this functions scope
		if (Time.time >= LA1NPUT)
		{
			LoadingAnimation1Percentage = Mathf.Round(percentage);
			LA1NPUT += LA1PUT;
		}
	}
}

function OnGUI ()
{
	GUI.skin = TNGSkin;

	// Do the windows
	GUI.color.a = Windows[0].Alpha;
	Windows[0].rect = GUI.Window(0, Windows[0].rect, DoWindow0, "");
	
	GUI.color.a = Windows[1].Alpha;
	Windows[1].rect = GUI.Window(1, Windows[1].rect, DoWindow1, "");
	
	GUI.color.a = Windows[2].Alpha;
	Windows[2].rect = GUI.Window(2, Windows[2].rect, DoWindow2, "");

	GUI.color.a = Windows[3].Alpha;
	Windows[3].rect = GUI.Window(3, Windows[3].rect, DoWindow3, "");
	
	//now adjust to the group. (0,0) is the topleft corner of the group.
	GUI.BeginGroup(Rect(0,0,100,100));
	// End the group we started above. This is very important to remember!
	GUI.EndGroup ();
}

function DoWindow0 (windowID : int) 
{
	GUI.color.a = Windows[0].UIAlpha;
	
	// Do the window title
	DoWindowTitle("Window Title");
	
	// Do the window action buttons
	GUI.Button(Rect(395, 28, 26, 25), "", "WindowButtonInfo");
	GUI.Button(Rect(417, 28, 26, 25), "", "WindowButtonClose");
	
	// Do a text with the first style
	DoTextStyle1(Rect(95, 142, 303, 76), "In sed auctor erat. Phasellus mauris elit, accumsan tempor elit eget, faucibus tempor elit. Sed vitae mi eget tortor tempor molestie at ac magna.");
	
	// Do an image
	DoImage(Vector2(71, 218), Image1);
	
	// Do a separator
	DoSeparator(Vector2(85, 369), false);
	
	// Do some toggles
	Windows[3].Show = DoToggle(Vector2(90, 387), Windows[3].Show, "Display the login window?");
	Windows[1].Show = DoToggle(Vector2(90, 427), Windows[1].Show, "Display the second window?");
	Windows[2].Show = DoToggle(Vector2(90, 467), Windows[2].Show, "Display the thrid window?");
	
	// Do a fliped separator
	DoSeparator(Vector2(85, 505), true);
	
	// Do a text with the second style
	DoTextStyle2(Rect(86, 548, 322, 87), "Aliquam semper facilisis tellus sit amet tincidunt. Phasellus eu sodales leo, quis porta nisl. Vivamus imperdiet rhoncus odio, eget vestibulum ligula dictum at.");

	// Make the windows be draggable.
	GUI.DragWindow (Rect (0,0,10000,10000));
}

function DoWindow1 (windowID : int) 
{
	GUI.color.a = Windows[1].UIAlpha;
	
	// Do the window title
	DoWindowTitle("Second window");
	
	// Do the window action buttons
	GUI.Button(Rect(395, 28, 26, 25), "", "WindowButtonInfo");
	
	if (GUI.Button(Rect(417, 28, 26, 25), "", "WindowButtonClose"))
	{
		Windows[1].Show = false;
	}
	
	// Do a button
	DoButton(Rect(140, 160, 208, 49), "Button");
	
	// Do a text area
	textAreaStr = GUI.TextArea(Rect(99, 264, 294, 146), textAreaStr);
	
	// Do a text field
	textFieldStr = GUI.TextField(Rect(96, 457, 300, 28), textFieldStr);
	
	var radios : String[] = new String[3];
	radios[0] = "Semper facilisis tellus ?";
	radios[1] = "Phasellus eu sodales leo!";
	radios[2] = "Aliquam semper facilisis tellus...";
	
	// Do a group of radio style toggles
	radioSelected = ToggleList(Vector2(80, 512), radioSelected, radios);
	
	// Make the windows be draggable.
	GUI.DragWindow (Rect (0,0,10000,10000));
}

function DoWindow2 (windowID : int) 
{
	GUI.color.a = Windows[2].UIAlpha;
	
	// Do the window title
	DoWindowTitle("Third window");
	
	// Do the window action buttons
	GUI.Button(Rect(395, 28, 26, 25), "", "WindowButtonInfo");
	
	if (GUI.Button(Rect(417, 28, 26, 25), "", "WindowButtonClose"))
	{
		Windows[2].Show = false;
	}
		
	// Do a scroll view
	scrollPosition = GUI.BeginScrollView(Rect(99, 148, 304, 146), scrollPosition, Rect(0, 0, 496, 146));
	// Do a text with the first style
	DoTextStyle1(Rect(0, 0, 494, 146), scrollViewText);
	// End the scroll view that we began above.
	GUI.EndScrollView();
	
	// Do a horizontal slider
	sliderValue0 = GUI.HorizontalSlider(Rect(84, 327, 324, 10), sliderValue0, 0.0, 1.0);
	
	// Do a vertical slider
	sliderValue1 = GUI.VerticalSlider(Rect(85, 362, 8, 175), sliderValue1, 0.0, 1.0);
	
	// Do the loading animation
	DoAnimation1(Vector2(117, 366));
	
	// Do a label
	DoLabel(Rect(156, 557, 180, 20), "Input label comes here", true, true);
	
	// Do a text field
	textFieldStr1 = GUI.TextField(Rect(99, 593, 300, 28), textFieldStr1);

	// Make the windows be draggable.
	GUI.DragWindow (Rect (0,0,10000,10000));
}

function DoWindow3 (windowID : int) 
{
	GUI.color.a = Windows[3].UIAlpha;
	
	// Do the window title, scaled down to 83% with adjusted position Y to 70
	// DoWindowTitle("Login", 0.83, 70);
	
	// Do a label
	DoLabel(Rect(97, 85, 140, 20), "Username / Email", false, true);
	
	// Do a text field
	textFieldStr2 = GUI.TextField(Rect(111, 122, 194, 28), textFieldStr2);
	
	// Do a label
	DoLabel(Rect(97, 180, 75, 20), "Password", false, true);
	
	// Do a text field
	textFieldStr3 = GUI.TextField(Rect(111, 216, 194, 28), textFieldStr3);
	
	// Do a toggle
	rememberme = DoToggle(Vector2(106, 275), rememberme, "Remember me?");
	
	// Do a button
	DoButton(Rect(104, 337, 211, 49), "Login");
	
	// Make the windows be draggable.
	GUI.DragWindow (Rect (0,0,10000,10000));
}

function DoLabel(r : Rect, text : String)
{
	DoLabel(r, text, false, false);
}

function DoLabel(r : Rect, text : String, leftImage : boolean, rightImage : boolean)
{
	var LabelStyle : GUIStyle = GUI.skin.GetStyle("label");
	var LabelShadowStyle : GUIStyle = GUI.skin.GetStyle("LabelTextShadow");
	
	if (leftImage)
		GUI.Label(Rect((r.x - 70), (r.y + 1), 65, 16), "", "LabelImageLeft");
	
	if (rightImage)
		GUI.Label(Rect((r.x + r.width) + 5, (r.y + 1), 65, 16), "", "LabelImageRight");
	
	DoTextWithShadow(r, GUIContent(text), LabelStyle, LabelStyle.normal.textColor, LabelShadowStyle.normal.textColor, Vector2(1.0, 1.0));
}

function DoWindowTitle(text : String)
{
	DoWindowTitle(text, 1.0, 0.0);
}

function DoWindowTitle(text : String, scale : float)
{
	DoWindowTitle(text, scale, 0.0);
}

function DoWindowTitle(text : String, scale : float, adjustY : float)
{
	// Define styles
	var WindowTitleStyle : GUIStyle = GUI.skin.GetStyle("WindowTitle");
	var WindowTitleShadowStyle : GUIStyle = GUI.skin.GetStyle("WindowTitleShadow");
	
	var windowTitleRect0 = ScaleRect(Rect(0, 80, 491, 21), scale);
	var windowTitleBGRect0 = ScaleRect(Rect(55, 57, 381, 130), scale);
	var windowTitleOLRect0 = ScaleRect(Rect(58, 66, 374, 50), scale);
	
	if (adjustY > 0.0)
		windowTitleRect0.y = adjustY;
	
	// Lay the title background
	GUI.Label(windowTitleBGRect0, "", "WindowTitleBackground");
	// Draw a label with shadow
	DoTextWithShadow(windowTitleRect0, GUIContent(text), WindowTitleStyle, WindowTitleStyle.normal.textColor, WindowTitleShadowStyle.normal.textColor, Vector2(2.0, 2.0));
	// Lay the title overlay
	GUI.Label(windowTitleOLRect0, "", "WindowTitleOverlay");
}

function ScaleRect(rect : Rect, scale : float) : Rect
{
	scale = scale * 100;
	
	var newRect : Rect = new Rect(0, 0, 0, 0);
	newRect.x = Mathf.CeilToInt((rect.x / 100) * scale);
	newRect.y = Mathf.CeilToInt((rect.y / 100) * scale);
	newRect.width = Mathf.CeilToInt((rect.width / 100) * scale);
	newRect.height = Mathf.CeilToInt((rect.height / 100) * scale);
	
	return newRect;
}

function DoTextWithShadow(rect : Rect, content : GUIContent, style : GUIStyle, txtColor : Color, shadowColor : Color, direction : Vector2)
{
    var backupStyle : GUIStyle = new GUIStyle(style);

    style.normal.textColor = shadowColor;
    rect.x += direction.x;
    rect.y += direction.y;
    GUI.Label(rect, content, style);

    style.normal.textColor = txtColor;
    rect.x -= direction.x;
    rect.y -= direction.y;
    GUI.Label(rect, content, style);

    style = backupStyle;
}

function DoTextStyle1(r : Rect, text : String)
{
	var TextStyle : GUIStyle = GUI.skin.GetStyle("TextStyle1");
	var TextStyleShadow : GUIStyle = GUI.skin.GetStyle("TextStyle1Shadow");
	
	DoTextWithShadow(r, GUIContent(text), TextStyle, TextStyle.normal.textColor, TextStyleShadow.normal.textColor, Vector2(1.0, 1.0));
}

function DoTextStyle2(r : Rect, text : String)
{
	var TextStyle : GUIStyle = GUI.skin.GetStyle("TextStyle2");
	var TextStyleShadow : GUIStyle = GUI.skin.GetStyle("TextStyle2Shadow");
	
	DoTextWithShadow(r, GUIContent(text), TextStyle, TextStyle.normal.textColor, TextStyleShadow.normal.textColor, Vector2(1.0, 1.0));
}

function DoButton(r : Rect, content : String) : boolean
{
	var ButtonTextStyle : GUIStyle = GUI.skin.GetStyle("ButtonText");
	var backupStyle : GUIStyle = new GUIStyle(ButtonTextStyle);
	var ShadowStyle : GUIStyle = GUI.skin.GetStyle("ButtonTextShadow");
	var direction : Vector2 = Vector2(2.0, 1.0);
	
    var result : boolean = GUI.Button(r, "");
	
    var color : Color = (r.Contains(Event.current.mousePosition) && Input.GetMouseButton(0)) ? ButtonTextStyle.active.textColor : (r.Contains(Event.current.mousePosition) ? ButtonTextStyle.hover.textColor : ButtonTextStyle.normal.textColor);

    DoTextWithShadow(r, GUIContent(content), ButtonTextStyle, color, ShadowStyle.normal.textColor, direction);
	
	ButtonTextStyle.normal.textColor = backupStyle.normal.textColor;
	
    return result;
}

function DoImage(offset : Vector2, imageTexture : Texture2D)
{
	GUI.BeginGroup(Rect(offset.x, offset.y, 349, 146));
	GUI.Label(Rect(0, 0, 349, 146), "", "ImageFrame");
	GUI.DrawTexture(Rect(23, 24, 303, 100), imageTexture);
	GUI.EndGroup();
}

function DoSeparator(offset : Vector2, flip : boolean)
{
	var SeparatorStyle : GUIStyle = GUI.skin.GetStyle((flip ? "Separator2" : "Separator"));

	GUI.DrawTexture(Rect(offset.x, offset.y, 322, 17), SeparatorStyle.normal.background);
}

function DoToggle(offset : Vector2, toggle : boolean, text : String) : boolean
{
	var ToggleTextStyle : GUIStyle = GUI.skin.GetStyle("ToggleText");
	var ToggleTextShadowStyle : GUIStyle = GUI.skin.GetStyle("ToggleTextShadow");
	
	GUI.BeginGroup(Rect(offset.x, offset.y, 349, 146));
	toggle = GUI.Toggle(Rect(0, 0, 32, 32), toggle, "");
	DoTextWithShadow(Rect(39, 2, 278, 32), GUIContent(text), ToggleTextStyle, ToggleTextStyle.normal.textColor, ToggleTextShadowStyle.normal.textColor, Vector2(1.0, 1.0));
	GUI.EndGroup();
	
	return toggle;
}

// Displays a vertical list of toggles and returns the index of the selected item.
function ToggleList(offset : Vector2, selected : int, items : String[]) : int
{
    // Keep the selected index within the bounds of the items array
    selected = (selected < 0) ? 0 : (selected >= items.Length ? items.Length - 1 : selected);
	
	// Get the radio toggles style
	var radioStyle : GUIStyle = GUI.skin.GetStyle("RadioToggle");
	var ToggleTextStyle : GUIStyle = GUI.skin.GetStyle("ToggleText");
	var ToggleTextShadowStyle : GUIStyle = GUI.skin.GetStyle("ToggleTextShadow");
	
	// Get the toggles height
	var height : int = radioStyle.fixedWidth;
	
	GUI.BeginGroup(Rect(offset.x, offset.y, 314, (height * items.Length) + height));
    GUILayout.BeginVertical();
    
    for (var i : int = 0; i < items.Length; i++)
    {
        // Display toggle. Get if toggle changed.
        var change : boolean = GUILayout.Toggle(selected == i, "", radioStyle);
		DoTextWithShadow(Rect(49, ((i + 1) * height) - 12, 278, height), GUIContent(items[i]), ToggleTextStyle, ToggleTextStyle.normal.textColor, ToggleTextShadowStyle.normal.textColor, Vector2(1.0, 1.0));
		
        // If changed, set selected to current index.
        if (change)
            selected = i;
    }

    GUILayout.EndVertical();
	GUI.EndGroup();
	
    // Return the currently selected item's index
    return selected;
}

function DoAnimation1(offset : Vector2)
{
	GUI.BeginGroup(Rect(offset.x, offset.y, 271, 160));
	
	// Set the background
	GUI.Label(Rect(0, 0, 271, 160), "", "Animation1Background");
	
	// Draw the texture
	var position : Rect = new Rect(95, 40, 84, 84);
	var texCoords : Rect = new Rect(LoadingAnimation1TexOffset.x, LoadingAnimation1TexOffset.y, LoadingAnimation1TexSize.x, LoadingAnimation1TexSize.y);
	var alpha : boolean = true;
	
	GUI.DrawTextureWithTexCoords(position, LoadingAnimation1, texCoords, alpha);
	
	// Do the percentage
	var PercentageStyle : GUIStyle = GUI.skin.GetStyle("Animation1Percentage");
	var TextStyle : GUIStyle = GUI.skin.GetStyle("Animation1Text");
	var TextShadowStyle : GUIStyle = GUI.skin.GetStyle("Animation1TextShadow");

	DoTextWithShadow(Rect(108, 63, 60, 25), GUIContent(LoadingAnimation1Percentage + "%"), PercentageStyle, PercentageStyle.normal.textColor, TextShadowStyle.normal.textColor, Vector2(1.0, 1.0));
	DoTextWithShadow(Rect(108, 84, 56, 12), GUIContent("Loading"), TextStyle, TextStyle.normal.textColor, TextShadowStyle.normal.textColor, Vector2(1.0, 1.0));
	
	GUI.EndGroup();
}

class WindowsInfo extends System.ValueType
{
	var rect : Rect;
	var Alpha : float;
	var UIAlpha : float;
	var Show : boolean;
	var TimeToWait : float;
	var Speed : float;
	
	public function WindowsInfo(rect : Rect, alpha : float, uialpha : float, show : boolean, TimeToWait : int, speed : float)
	{
		this.rect = rect;
		this.Alpha = alpha;
		this.UIAlpha = uialpha;
		this.Show = show;
		this.TimeToWait = TimeToWait;
		this.Speed = speed;
	}
}