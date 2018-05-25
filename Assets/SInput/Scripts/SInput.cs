using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SinputSystems;


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

	//how many devices can be connected
	private static int _totalPossibleDeviceSlots;
	public static int totalPossibleDeviceSlots { get { return _totalPossibleDeviceSlots; } }

	//overall mouse sensitivity
	public static float mouseSensitivity = 1f;


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
		CheckGamepads();
		return gamepads;
	}

	//init
	private static bool loadCustomControls = true;
	private static bool initialised = false;
	static void Init(){
		if (initialised) return;
		initialised = true;

		_totalPossibleDeviceSlots = System.Enum.GetValues(typeof(InputDeviceSlot)).Length;

		zeroInputWaits = new float[_totalPossibleDeviceSlots];
		zeroInputs = new bool[_totalPossibleDeviceSlots];

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
			Debug.LogError("Couldn't find control scheme \"" + schemeName + "\" in project resources.");
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
			//newControl.scale = scheme.smartControls[i].scale;

			newControl.inversion = new bool[_totalPossibleDeviceSlots];
			newControl.scales = new float[_totalPossibleDeviceSlots];
			for (int k = 0; k <_totalPossibleDeviceSlots; k++) newControl.scales[k] = scheme.smartControls[i].scale;

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
			CheckGamepads();
			lastCheckedGamepadRefreshFrame=-1;//if we're doing stuff with control schemes, might want to do this more than once in a frame
		}
	}

	static int lastUpdateFrame = -99;
	//static int lastCheckedAxisUpdateFrame = -99;
	static bool resetAxisButtonStates = false;
	public static void SinputUpdate() {
		if (lastUpdateFrame == Time.frameCount) return;

		resetAxisButtonStates = false;
		if (Time.frameCount > lastUpdateFrame + 5) resetAxisButtonStates = true;

		lastUpdateFrame = Time.frameCount;

		//make sure everything is set up
		Init();

		//check if connected gamepads have changed
		CheckGamepads();

		//update controls
		if (null != controls) {
			for (int i = 0; i < controls.Length; i++) {
				controls[i].Update(resetAxisButtonStates);
			}
		}

		//update our smart controls
		if (null != smartControls) {
			for (int i = 0; i < smartControls.Length; i++) {
				smartControls[i].Update();
			}
		}

		//count down till we can stop zeroing inputs
		for (int i = 0; i < _totalPossibleDeviceSlots; i++) {
			if (zeroInputs[i]) {
				zeroInputWaits[i] -= Time.deltaTime;
				if (zeroInputWaits[i] <= 0f) zeroInputs[i] = false;
			}
		}
	}


	//tells sinput to return false/0f for any input checks until the wait time has passed
	static float[] zeroInputWaits;
	static bool[] zeroInputs;
	public static void ResetInputs(InputDeviceSlot slot = InputDeviceSlot.any) { ResetInputs(0.5f, slot); } //default wait is half a second
	public static void ResetInputs(float waitTime, InputDeviceSlot slot=InputDeviceSlot.any) {
		SinputUpdate();
		
		if (slot == InputDeviceSlot.any) {
			//reset all slots' input
			for (int i=0; i<_totalPossibleDeviceSlots; i++) {
				zeroInputWaits[i] = waitTime;
				zeroInputs[i] = true;
			}
		} else {
			//reset only a specific slot's input
			zeroInputWaits[(int)slot] = waitTime;
			zeroInputs[(int)slot] = true;
		}
		
		//reset smartControl values
		if (smartControls != null) {
			for (int i = 0; i < smartControls.Length; i++) {
				smartControls[i].ResetAllValues(slot);
			}
		}
	}
    

	//update gamepads
	static int lastCheckedGamepadRefreshFrame = -99;
	public static void CheckGamepads(){
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


	
	
	

	public static InputDeviceSlot GetSlotPress(string controlName){
		//like GetButtonDown() but returns ~which~ keyboard/gamepad input slot pressed the control
		//use it for 'Pres A to join!' type multiplayer, and instantiate a player for the returned slot (if it isn't DeviceSlot.any)

		SinputUpdate();

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

	public static bool GetButtonRaw(string controlName) { return ButtonCheck(controlName, InputDeviceSlot.any, ButtonAction.HELD, true); }
	public static bool GetButtonRaw(string controlName, InputDeviceSlot slot) { return ButtonCheck(controlName, slot, ButtonAction.HELD, true); }
	public static bool GetButtonDownRaw(string controlName) { return ButtonCheck(controlName, InputDeviceSlot.any, ButtonAction.DOWN, true); }
	public static bool GetButtonDownRaw(string controlName, InputDeviceSlot slot) { return ButtonCheck(controlName, slot, ButtonAction.DOWN, true); }
	public static bool GetButtonUpRaw(string controlName) { return ButtonCheck(controlName, InputDeviceSlot.any, ButtonAction.UP, true); }
	public static bool GetButtonUpRaw(string controlName, InputDeviceSlot slot) { return ButtonCheck(controlName, slot, ButtonAction.UP, true); }

	public static float buttonRepeatWait = 0.5f;
	public static float buttonRepeat = 0.1f;
	public static bool GetButtonDownRepeating(string controlName) { return ButtonCheck(controlName, InputDeviceSlot.any, ButtonAction.REPEATING); }
	public static bool GetButtonDownRepeating(string controlName, InputDeviceSlot slot) { return ButtonCheck(controlName, slot, ButtonAction.REPEATING); }

	static bool ButtonCheck(string controlName, InputDeviceSlot slot, ButtonAction bAction, bool getRawValue = false){
		
		SinputUpdate();
		if (zeroInputs[(int)slot]) return false;

		bool controlFound = false;

		for (int i=0; i<controls.Length; i++){
			if (controls[i].name == controlName){
				controlFound=true;
				if (controls[i].GetButtonState(bAction, slot, getRawValue)) return true;
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
	public static float GetAxis(string controlName) { return AxisCheck(controlName, InputDeviceSlot.any); }
	public static float GetAxis(string controlName, InputDeviceSlot slot) { return AxisCheck(controlName, slot); }

	public static float GetAxisRaw(string controlName) { return AxisCheck(controlName, InputDeviceSlot.any, true); }
	public static float GetAxisRaw(string controlName, InputDeviceSlot slot) { return AxisCheck(controlName, slot, true); }

	static float AxisCheck(string controlName, InputDeviceSlot slot, bool getRawValue=false){

		SinputUpdate();
		if (zeroInputs[(int)slot]) return 0f;

		bool controlFound = false;

		if (controlName=="") return 0f;

		float returnV = 0f;
		for (int i=0; i<controls.Length; i++){
			if (controls[i].name == controlName){
				controlFound=true;
				float v = controls[i].GetAxisState(slot);
				if (Mathf.Abs(v) > returnV) returnV = v;
			}
		}

		for (int i=0; i<smartControls.Length; i++){
			if (smartControls[i].name == controlName){
				controlFound=true;
				float v = smartControls[i].GetValue(slot, getRawValue);
				if (Mathf.Abs(v) > returnV) returnV = v;
			}
		}

		if (!controlFound) Debug.LogError("Sinput Error: Control \"" + controlName + "\" not found in list of Controls or SmartControls.");

		return returnV;
	}

	//vector checks
	public static Vector2 GetVector(string controlNameA, string controlNameB) { return Vector2Check(controlNameA, controlNameB, InputDeviceSlot.any, true); }
	public static Vector2 GetVector(string controlNameA, string controlNameB, bool normalClip) { return Vector2Check(controlNameA, controlNameB, InputDeviceSlot.any, normalClip); }
	public static Vector2 GetVector(string controlNameA, string controlNameB, InputDeviceSlot slot) { return Vector2Check(controlNameA, controlNameB, slot, true); }
	public static Vector2 GetVector(string controlNameA, string controlNameB, InputDeviceSlot slot, bool normalClip) { return Vector2Check(controlNameA, controlNameB, slot, normalClip); }

	static Vector2 Vector2Check(string controlNameA, string controlNameB, InputDeviceSlot slot, bool normalClip){

		SinputUpdate();

		Vector2 returnVec = Vector2.zero;
		returnVec.x = AxisCheck(controlNameA, slot);
		returnVec.y = AxisCheck(controlNameB, slot);

		if (normalClip && returnVec.magnitude > 1f) {
			returnVec.Normalize();
		}

		return returnVec;
	}

	public static Vector3 GetVector(string controlNameA, string controlNameB, string controlNameC) { return Vector3Check(controlNameA, controlNameB, controlNameC, InputDeviceSlot.any, true); }
	public static Vector3 GetVector(string controlNameA, string controlNameB, string controlNameC, bool normalClip) { return Vector3Check(controlNameA, controlNameB, controlNameC, InputDeviceSlot.any, normalClip); }
	public static Vector3 GetVector(string controlNameA, string controlNameB, string controlNameC, InputDeviceSlot slot) { return Vector3Check(controlNameA, controlNameB, controlNameC, slot, true); }
	public static Vector3 GetVector(string controlNameA, string controlNameB, string controlNameC, InputDeviceSlot slot, bool normalClip) { return Vector3Check(controlNameA, controlNameB, controlNameC, slot, normalClip); }

	static Vector3 Vector3Check(string controlNameA, string controlNameB, string controlNameC, InputDeviceSlot slot, bool normalClip){

		SinputUpdate();

		Vector3 returnVec = Vector3.zero;
		returnVec.x = AxisCheck(controlNameA, slot);
		returnVec.y = AxisCheck(controlNameB, slot);
		returnVec.z = AxisCheck(controlNameC, slot);

		if (normalClip && returnVec.magnitude > 1f) {
			returnVec.Normalize();
		}

		return returnVec;
	}

	//returns false if values returned by GetAxis(controlName) this frame should NOT be multiplied by deltaTime (eg, mouse movement)
	public static bool PrefersDeltaUse(string controlName) { return PrefersDeltaUse(controlName, InputDeviceSlot.any); }
	public static bool PrefersDeltaUse(string controlName, InputDeviceSlot slot) {

		SinputUpdate();

		bool preferDelta = true;

		if (controlName == "") return false;
		float axisVal = 0f;
		for (int i = 0; i < controls.Length; i++) {
			if (controls[i].name == controlName) {
				float v = controls[i].GetAxisState(slot);
				if (Mathf.Abs(v) > axisVal) {
					axisVal = v;
					preferDelta = controls[i].GetAxisStateDeltaPreference(slot);
				}
			}
		}

		//TODO NOW CHECK SMART CONTROLS FOR FRAMERATE INDEPENDENCE
		for (int i = 0; i < smartControls.Length; i++) {
			if (smartControls[i].name == controlName) {
				float v = smartControls[i].GetValue(slot, true);
				if (Mathf.Abs(v) > axisVal) {
					axisVal = v;

					if (!PrefersDeltaUse(smartControls[i].positiveControl, slot) || !PrefersDeltaUse(smartControls[i].negativeControl, slot)) preferDelta = false;
				}
			}
		}

		return preferDelta;
	}

	//sets whether a control treats GetButton() calls with press or with toggle behaviour
	public static void SetToggle(string controlName, bool toggle) {
		SinputUpdate();
		for (int i = 0; i < controls.Length; i++) {
			if (controls[i].name == controlName) {
				controls[i].isToggle = toggle;
			}
		}
	}
	//returns true if a control treats GetButton() calls with toggle behaviour
	public static bool GetToggle(string controlName) {
		SinputUpdate();
		for (int i = 0; i < controls.Length; i++) {
			if (controls[i].name == controlName) {
				return controls[i].isToggle;
			}
		}
		return false;
	}

	//set a smart control to be inverted or not
	public static void SetInverted(string smartControlName, bool invert, InputDeviceSlot slot=InputDeviceSlot.any) {
		SinputUpdate();
		for (int i = 0; i < smartControls.Length; i++) {
			if (smartControls[i].name == smartControlName) {
				if (slot == InputDeviceSlot.any) {
					for (int k=0; k<_totalPossibleDeviceSlots; k++) {
						smartControls[i].inversion[k] = invert;
					}
				} else {
					smartControls[i].inversion[(int)slot] = invert;
				}
			}
		}
	}
	//returns true if a smart control is inverted
	public static bool GetInverted(string smartControlName, InputDeviceSlot slot = InputDeviceSlot.any) {
		SinputUpdate();
		for (int i = 0; i < smartControls.Length; i++) {
			if (smartControls[i].name == smartControlName) {
				return smartControls[i].inversion[(int)slot];
			}
		}
		return false;
	}

	//sets scale ("sensitivity") of a smart control
	public static void SetScale(string smartControlName, float scale, InputDeviceSlot slot = InputDeviceSlot.any) {
		SinputUpdate();
		for (int i = 0; i < smartControls.Length; i++) {
			if (smartControls[i].name == smartControlName) {
				if (slot == InputDeviceSlot.any) {
					for (int k = 0; k < _totalPossibleDeviceSlots; k++) {
						smartControls[i].scales[k] = scale;
					}
				} else {
					smartControls[i].scales[(int)slot] = scale;
				}
			}
		}
	}

	//gets scale of a smart control
	public static float GetScale(string smartControlName, InputDeviceSlot slot = InputDeviceSlot.any) {
		for (int i = 0; i < smartControls.Length; i++) {
			if (smartControls[i].name == smartControlName) {
				return smartControls[i].scales[(int)slot];
			}
		}
		return 1f;
	}

}


