using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SinputSystems;

//Hi there,
// thanks for checking Sinput, as you can see from my dev notes below there's still a lot I need to do, but I think
// the system is already good for a few things so if you can get it working with your project easily you should be OK :)
// if you have any suggestions or Qs message me @S0phieH.
// - Love, Sophie. <3

//~~~~~~~~~~ Usage: ~~~~~~~~~~
// 1) Set up MainControlScheme asset in your project (it should be in the Sinput>Resources folder)
// 2) In your scripts, call Sinput.GetButton() or Sinput.GetAxis() as needed.
//     Use Sinput.GetVector() if you want multiple axis at once
// 3) Include the rebinding scene in your project somewhere to let your players rebind their controls.
 
// ~~~~~~~~~~ Credit: ~~~~~~~~~~
// You don't need to credit me for using this script (tho it'd be sweet if you did <3)
//  Sophie Houlden / @S0phieH
 
// ~~~~~~~~~~ Payment: ~~~~~~~~~~
// This system is 100% free in all circumstances,
// but if you want to leave a tip on itch or support me on patreon that would be awesome
//  http://sophieh.itch.io
//  http://patreon.com/SophieHoulden


/* ~~~~~~~~~~ Sophie's Dev Notes: ~~~~~~~~~~
 * priority to-do for future versions:
 * TODO: Get rebind screen scaling OK with various resolutions, it's no good if it's too tiny for people to read/use
 * TODO: MouseMotionLeft/MouseMotionRight/MouseMotionUp/MouseMotionDown to replace Mouse X/Mouse Y controls with Look Up/Look Down/etc controls that can be rebound, and a Look X & Look Y smart control. (also equivs for scroll)
 * TODO: tidy up the whole initialisation process, it's kind of a nightmare and is probably full of redundant stuff
 * TODO: when adding an input, automatically start listening for what its binding should be
 * TODO: get virtual inputs working
 * TODO: Sinput-version of unity's Standalone Input Module, so sinput inputs can navigate unity UI interactibes
 * TODO: Get rebind menu elements to have explicit navigation controls because the automatic ones are not exactly perfect
 * TODO: Find a way to deal with (or at least highlight) inputs that clash in the rebinding menu
 * TODO: if keyboard&mouse control are set to distinct, seperate them in the rebind menu
 * TODO: optimisation might be good to do, sometime
 * TODO: option to normalise GetVector() result if it's magnitude is >1
 * 
 * future feature wishlist/to-do
 * - GetDisplayName(controlName, PreferedDeviceType, defaultDisplayName) for user-facing text prompts
 * - GetDisplayIcon() like GetDisplayName() but returns an image for the button/key/whatever
 * - Multiple mice/keyboards
 * - VR input
 * - Microphone input
 * - force feedback
 * - find ways of supporting more gamepads?
 * 
 * other ideas:
 * - common shortcut to bring up rebind menu, loads the rebind scene additively and ignores other sinput calls until the menu is closed
 * 
 */

public static class Sinput {

	//Fixed number of gamepad things unity can handle, used mostly by GamepadDebug and InputManagerReplacementGenerator.
	//Sinput can handle as many of these as you want to throw at it buuuuut, unty can only handle so many and Sinput is wrapping unity input for now
	//You can try bumping up the range of these but you might have trouble
	//(EG, you can probably get axis of gamepads in slots over 8, but maybe not buttons?)
	public static int MAXCONNECTEDGAMEPADS { get {return 11; } }
	public static int MAXAXISPERGAMEPAD { get {return 28; } }
	public static int MAXBUTTONSPERGAMEPAD { get {return 20; } }


	//are keyboard & mouse used by two seperate players (distinct=true) or by a single player (distinct=false)
	private static bool keyboardAndMouseAreDistinct = false;

	
	//the control scheme, set it with SetControlScheme()
	private static Control[] controls;
	public static void SetControls(Control[] newControls){
		Init();
		controls = newControls;
	}
	public static Control[] GetControls(){
		Init();
		return controls;
	}

	private static SmartControl[] smartControls;

	//gamepads list is checked every GetButton/GetAxis call, when it updates all common binding inputs are reapplied appropriately
	static bool refreshGamepadsNow = true;
	static int nextGamepadCheck=-99;
	static string[] gamepads = new string[0];
	public static string[] GetGamepads(){
		UpdateGamepads();
		return gamepads;
	}

	//init
	private static bool loadCustomControls = true;
	private static bool initialised = false;
	static void Init(){
		if (initialised) return;
		initialised = true;

		LoadControlScheme("MainControlScheme", loadCustomControls);

	}

	public static ControlScheme controlScheme;
	public static void LoadControlScheme(string schemeName, bool loadCustom){
		loadCustomControls = loadCustom;
		Init();
		UnityEngine.Object[] projectControlSchemes = Resources.LoadAll("", typeof(ControlScheme));

		int schemeIndex = -1;
		for (int i=0; i<projectControlSchemes.Length; i++){
			if (projectControlSchemes[i].name == schemeName) schemeIndex = i;
		}
		if (schemeIndex==-1){
			Debug.LogError("Couldn't find control scheme \'" + schemeName + "\" in project resources.");
			return;
		}
		controlScheme = (ControlScheme)projectControlSchemes[schemeIndex];
		LoadControlScheme(controlScheme, loadCustomControls);
	}
	public static void LoadControlScheme(ControlScheme scheme, bool loadCustom){
		loadCustomControls = loadCustom;
		if (!initialised){
			Init();
			return;
		}

		//Debug.Log("Loading ControlScheme  - loadcustom set to: " + loadCustom.ToString());
		List<Control> loadedControls = new List<Control>();
		for (int i=0; i<scheme.controls.Count; i++){
			Control newControl = new Control(scheme.controls[i].name);

			for (int k=0; k<scheme.controls[i].keyboardInputs.Count; k++){
				newControl.AddKeyboardInput( (KeyCode)Enum.Parse(typeof(KeyCode), scheme.controls[i].keyboardInputs[k].ToString()) );
			}
			for (int k=0; k<scheme.controls[i].gamepadInputs.Count; k++){
				newControl.AddGamepadInput( scheme.controls[i].gamepadInputs[k] );
			}
			for (int k=0; k<scheme.controls[i].mouseInputs.Count; k++){
				newControl.AddMouseInput( scheme.controls[i].mouseInputs[k] );
			}
			for (int k=0; k<scheme.controls[i].virtualInputs.Count; k++){
				newControl.AddVirtualInput( scheme.controls[i].virtualInputs[k] );
			}

			loadedControls.Add(newControl);
		}
		controls = loadedControls.ToArray();

		List<SmartControl> loadedSmartControls = new List<SmartControl>();
		for (int i=0; i<scheme.smartControls.Count; i++){
			SmartControl newControl = new SmartControl(scheme.smartControls[i].name);

			newControl.positiveControl = scheme.smartControls[i].positiveControl;
			newControl.negativeControl = scheme.smartControls[i].negativeControl;
			newControl.gravity = scheme.smartControls[i].gravity;
			newControl.deadzone = scheme.smartControls[i].deadzone;
			newControl.speed = scheme.smartControls[i].speed;
			newControl.snap = scheme.smartControls[i].snap;
			newControl.scale = scheme.smartControls[i].scale;

			loadedSmartControls.Add(newControl);
		}
		smartControls = loadedSmartControls.ToArray();
		for (int i=0; i<smartControls.Length; i++) smartControls[i].Init();

		//now load any saved control scheme with custom rebound inputs
		if (loadCustom && SinputFileIO.SaveDataExists()){
			//Debug.Log("Found saved binding!");
			controls = SinputFileIO.LoadControls( controls );

			//also load any common gamepad bindings that might not be part of the saved binding
			refreshGamepadsNow = true;
			lastCheckedGamepadRefreshFrame=-1;//if we're doing stuff with control schemes, might want to do this more than once in a frame
			UpdateGamepads();
			lastCheckedGamepadRefreshFrame=-1;//if we're doing stuff with control schemes, might want to do this more than once in a frame
		}
	}

	//update gamepads
	static int lastCheckedGamepadRefreshFrame = -99;
	public static void UpdateGamepads(){
		if (Time.frameCount == lastCheckedGamepadRefreshFrame) return;
		lastCheckedGamepadRefreshFrame = Time.frameCount;

		Init();

		string[] inputGamepads = Input.GetJoystickNames();
		if (gamepads.Length!=inputGamepads.Length) refreshGamepadsNow = true; //number of connected gamepads has changed
		if (!refreshGamepadsNow && nextGamepadCheck < Time.frameCount){
			//this check is for the rare case gamepads get re-ordered in a single frame & the length of GetJoystickNames() stays the same
			nextGamepadCheck = Time.frameCount + 500;
			for (int i=0; i<gamepads.Length; i++){
				if (gamepads[i] != inputGamepads[i].ToUpper()) refreshGamepadsNow = true;
			}
		}
		if (refreshGamepadsNow){
			//Debug.Log("Refreshing gamepads");

			//connected gamepads have changed
			gamepads = new string[inputGamepads.Length];
			for (int i=0; i<gamepads.Length; i++){
				gamepads[i] = inputGamepads[i].ToUpper();
			}

			//reload common binding information
			CommonGamepadBindings.ReloadCommonMaps();
			refreshGamepadsNow = false;

			if (null != controls){
				

				//reapply common bindings
				for (int i=0; i<controls.Length; i++){
					controls[i].ReapplyCommonBindings();
				}
				//reset axis button states
				for (int i=0; i<controls.Length; i++){
					controls[i].ResetAxisButtonStates();
				}
					
				//if the input has a listed device name (i.e. it's custom bound, not common bound) it needs its applicable slots setting
				for (int c=0; c<controls.Length; c++){
					for (int i=0; i<controls[c].inputs.Count; i++){
						if (controls[c].inputs[i].isCustom){
							if (controls[c].inputs[i].inputType == InputDeviceType.GamepadAxis || controls[c].inputs[i].inputType == InputDeviceType.GamepadButton){
								//Debug.Log("Finding slot for gamepad: " + controls[c].inputs[i].displayName + " of " + controls[c].inputs[i].deviceName);
								//find applicable gamepad slots for this device
								List<int> allowedSlots = new List<int>();
								for (int g=0; g<gamepads.Length; g++){
									if (gamepads[g].ToUpper() == controls[c].inputs[i].deviceName.ToUpper()){
										allowedSlots.Add(g);
									}
								}
								controls[c].inputs[i].allowedSlots = allowedSlots.ToArray();
							}
						}
					}
				}
			}
			if (null != smartControls){
				for (int i=0; i<smartControls.Length; i++){
					smartControls[i].Init();
				}
			}




		}
	}

	//update buttonstates for axis
	static int lastCheckedAxisUpdateFrame = -99;
	static void UpdateAxisStates(){
		if (Time.frameCount == lastCheckedAxisUpdateFrame) return;

		UpdateGamepads();//check if connected gamepads have changed

		if (Time.frameCount > lastCheckedAxisUpdateFrame+5){
			//too many frames have passed without updating the states, reset them for safety
			for (int i=0; i<controls.Length; i++){
				controls[i].ResetAxisButtonStates();
			}
		}

		for (int i=0; i<controls.Length; i++){
			controls[i].UpdateAxisButtonStates();
		}

		lastCheckedAxisUpdateFrame = Time.frameCount;
	}


	public static InputDeviceSlot ListenForSlotPress(string controlName){
		//like GetButtonDown() but returns ~which~ keyboard/gamepad input slot pressed the control
		//use it for 'Pres A to join!' type multiplayer, and instantiate a player for the returned slot (if it isn't DeviceSlot.any)

		if (keyboardAndMouseAreDistinct){
			if (GetButtonDown(controlName, InputDeviceSlot.keyboard)) return InputDeviceSlot.keyboard;
			if (GetButtonDown(controlName, InputDeviceSlot.mouse)) return InputDeviceSlot.mouse;
		}else{
			if (GetButtonDown(controlName, InputDeviceSlot.keyboard)) return InputDeviceSlot.keyboardAndMouse;
			if (GetButtonDown(controlName, InputDeviceSlot.mouse)) return InputDeviceSlot.keyboardAndMouse;
		}
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad1)) return InputDeviceSlot.gamepad1;
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad2)) return InputDeviceSlot.gamepad2;
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad3)) return InputDeviceSlot.gamepad3;
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad4)) return InputDeviceSlot.gamepad4;
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad5)) return InputDeviceSlot.gamepad5;
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad6)) return InputDeviceSlot.gamepad6;
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad7)) return InputDeviceSlot.gamepad7;
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad7)) return InputDeviceSlot.gamepad7;
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad9)) return InputDeviceSlot.gamepad9;
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad10)) return InputDeviceSlot.gamepad10;
		if (GetButtonDown(controlName, InputDeviceSlot.gamepad11)) return InputDeviceSlot.gamepad11;

		return InputDeviceSlot.any;
	}


	//Button control checks
	public static bool GetButton(string controlName){ return ButtonCheck(controlName, InputDeviceSlot.any, ButtonAction.HELD); }
	public static bool GetButton(string controlName, InputDeviceSlot slot){ return ButtonCheck(controlName, slot, ButtonAction.HELD); }

	public static bool GetButtonDown(string controlName){ return ButtonCheck(controlName, InputDeviceSlot.any, ButtonAction.DOWN); }
	public static bool GetButtonDown(string controlName, InputDeviceSlot slot){ return ButtonCheck(controlName, slot, ButtonAction.DOWN); }

	public static bool GetButtonUp(string controlName){ return ButtonCheck(controlName, InputDeviceSlot.any, ButtonAction.UP); }
	public static bool GetButtonUp(string controlName, InputDeviceSlot slot){ return ButtonCheck(controlName, slot, ButtonAction.UP); }

	static bool ButtonCheck(string controlName, InputDeviceSlot slot, ButtonAction bAction){

		Init();

		UpdateAxisStates();

		bool controlFound = false;

		for (int i=0; i<controls.Length; i++){
			if (controls[i].name == controlName){
				controlFound=true;
				if (controls[i].ButtonCheck(bAction, slot)) return true;
			}
		}

		for (int i=0; i<smartControls.Length; i++){
			if (smartControls[i].name == controlName){
				controlFound=true;
				if (smartControls[i].ButtonCheck(bAction, slot)) return true;
			}
		}

		if (!controlFound) Debug.LogError("Sinput Error: Control \"" + controlName + "\" not found in list of controls or SmartControls.");

		return false;
	}


	//Axis control checks
	public static float GetAxis(string controlName){ return AxisCheck(controlName, InputDeviceSlot.any); }
	public static float GetAxis(string controlName, InputDeviceSlot slot){ return AxisCheck(controlName, slot); }

	static float AxisCheck(string controlName, InputDeviceSlot slot){

		Init();

		UpdateAxisStates();
		bool controlFound = false;

		if (controlName=="") return 0f;

		float returnV = 0f;
		for (int i=0; i<controls.Length; i++){
			if (controls[i].name == controlName){
				controlFound=true;
				float v = controls[i].AxisCheck(slot);
				if (Mathf.Abs(v) > returnV) returnV = v;
			}
		}

		for (int i=0; i<smartControls.Length; i++){
			if (smartControls[i].name == controlName){
				controlFound=true;
				float v = smartControls[i].GetValue(slot);
				if (Mathf.Abs(v) > returnV) returnV = v;
			}
		}

		if (!controlFound) Debug.LogError("Sinput Error: Control \"" + controlName + "\" not found in list of Controls or SmartControls.");

		return returnV;
	}

	//vector checks
	public static Vector2 GetVector(string controlNameA, string controlNameB){ return Vector2Check(controlNameA, controlNameB, InputDeviceSlot.any); }
	public static Vector2 GetVector(string controlNameA, string controlNameB, InputDeviceSlot slot){ return Vector2Check(controlNameA, controlNameB, slot); }

	static Vector2 Vector2Check(string controlNameA, string controlNameB, InputDeviceSlot slot){

		Init();

		UpdateAxisStates();

		Vector2 returnVec = Vector2.zero;
		returnVec.x = AxisCheck(controlNameA, slot);
		returnVec.y = AxisCheck(controlNameB, slot);
		return returnVec;
	}

	public static Vector3 GetVector(string controlNameA, string controlNameB, string controlNameC){ return Vector3Check(controlNameA, controlNameB, controlNameC, InputDeviceSlot.any); }
	public static Vector3 GetVector(string controlNameA, string controlNameB, string controlNameC, InputDeviceSlot slot){ return Vector3Check(controlNameA, controlNameB, controlNameC, slot); }

	static Vector3 Vector3Check(string controlNameA, string controlNameB, string controlNameC, InputDeviceSlot slot){

		Init();

		UpdateAxisStates();

		Vector3 returnVec = Vector3.zero;
		returnVec.x = AxisCheck(controlNameA, slot);
		returnVec.y = AxisCheck(controlNameB, slot);
		returnVec.z = AxisCheck(controlNameC, slot);
		return returnVec;
	}


	

}
namespace SinputSystems{








	public static class CommonGamepadBindings {


		static List<CommonBinding> commonBindings;
		static BindingSlots[] bindingSlots;

		public static void ReloadCommonMaps(){
			//called when gamepads are plugged in or removed, also when Sinput is first called

			//Debug.Log("Loading common mapping");

			OSFamily thisOS = OSFamily.Other;
			if (Application.platform == RuntimePlatform.OSXEditor) thisOS = OSFamily.MacOSX;
			if (Application.platform == RuntimePlatform.OSXPlayer) thisOS = OSFamily.MacOSX;
			if (Application.platform == RuntimePlatform.WindowsEditor) thisOS = OSFamily.Windows;
			if (Application.platform == RuntimePlatform.WindowsPlayer) thisOS = OSFamily.Windows;
			if (Application.platform == RuntimePlatform.LinuxEditor) thisOS = OSFamily.Linux;
			if (Application.platform == RuntimePlatform.LinuxPlayer) thisOS = OSFamily.Linux;
			if (Application.platform == RuntimePlatform.Android) thisOS = OSFamily.Android;
			if (Application.platform == RuntimePlatform.IPhonePlayer) thisOS = OSFamily.IOS;
			if (Application.platform == RuntimePlatform.PS4) thisOS = OSFamily.PS4;
			if (Application.platform == RuntimePlatform.PSP2) thisOS = OSFamily.PSVita;
			if (Application.platform == RuntimePlatform.XboxOne) thisOS = OSFamily.XboxOne;
			if (Application.platform == RuntimePlatform.WiiU) thisOS = OSFamily.WiiU;
			if (Application.platform == RuntimePlatform.Switch) thisOS = OSFamily.Switch;

			System.Object[] commonBindingAssets = Resources.LoadAll("", typeof(CommonBinding));
			commonBindings = new List<CommonBinding>();
			string[] gamepads = Sinput.GetGamepads();
			for (int i=0; i<commonBindingAssets.Length; i++){
				if (((CommonBinding)commonBindingAssets[i]).os == thisOS){
					bool gamepadConnected = false;

					for (int k=0; k<((CommonBinding)commonBindingAssets[i]).names.Count; k++){
						for (int g=0; g<gamepads.Length; g++){
							if (((CommonBinding)commonBindingAssets[i]).names[k].ToUpper() == gamepads[g]) gamepadConnected = true;
						}
					}

					if (gamepadConnected) commonBindings.Add( (CommonBinding)commonBindingAssets[i] );
				}
			}



			//for each commin binding, find which gamepad slots it applies to
			//inputs built from common bindings will only check slots which match
			bindingSlots = new BindingSlots[commonBindings.Count];
			for (int i=0; i<bindingSlots.Length; i++){
				bindingSlots[i].slots = new List<int>();
			}
			//string[] gamepads = Sinput.GetGamepads();
			for (int i=0; i<commonBindings.Count; i++){
				for (int k=0; k<commonBindings[i].names.Count; k++){
					for (int g=0; g<gamepads.Length; g++){
						if (gamepads[g] == commonBindings[i].names[k].ToUpper()){
							bindingSlots[i].slots.Add(g);
						}
					}
				}
			}



		}
		struct BindingSlots{
			public List<int> slots;
		}


		public static List<DeviceInput> GetApplicableMaps(CommonGamepadInputs t, string[] connectedGamepads){
			//builds input mapping of type t for all known connected gamepads


			List<DeviceInput> applicableInputs = new List<DeviceInput>();


			for (int i=0; i<commonBindings.Count; i++){

				//add any applicable button mappings
				for (int k=0; k<commonBindings[i].buttons.Count; k++){
					if (commonBindings[i].buttons[k].buttonType == t){
						//add this button input
						DeviceInput newInput = new DeviceInput(InputDeviceType.GamepadButton);
						//newInput.gamepadNames = commonBindings[i].names.ToArray();
						newInput.gamepadButtonNumber = commonBindings[i].buttons[k].buttonNumber;
						newInput.commonBindingType = t;
						newInput.displayName = commonBindings[i].buttons[k].displayName;

						newInput.allowedSlots = bindingSlots[i].slots.ToArray();

						applicableInputs.Add( newInput );
					}
				}
				//add any applicable axis bingings
				for (int k=0; k<commonBindings[i].axis.Count; k++){
					if (commonBindings[i].axis[k].buttonType == t){
						//add this axis input
						DeviceInput newInput = new DeviceInput(InputDeviceType.GamepadAxis);
						//newInput.gamepadNames = commonBindings[i].names.ToArray();
						newInput.gamepadAxisNumber = commonBindings[i].axis[k].axisNumber;
						newInput.commonBindingType = t;
						newInput.displayName = commonBindings[i].axis[k].displayName;
						newInput.invertAxis = commonBindings[i].axis[k].invert;
						newInput.clampAxis = commonBindings[i].axis[k].clamp;
						newInput.axisButtoncompareVal = commonBindings[i].axis[k].compareVal;
						newInput.defaultAxisValue = commonBindings[i].axis[k].defaultVal;

						newInput.allowedSlots = bindingSlots[i].slots.ToArray();

						if (commonBindings[i].axis[k].rescaleAxis){
							newInput.rescaleAxis = true;
							newInput.rescaleAxisMin = commonBindings[i].axis[k].rescaleAxisMin;
							newInput.rescaleAxisMax = commonBindings[i].axis[k].rescaleAxisMax;
						}

						applicableInputs.Add( newInput );
					}
				}

			}



			return applicableInputs;
		}

	}
}


