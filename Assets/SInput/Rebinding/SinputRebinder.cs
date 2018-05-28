using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using SinputSystems;
namespace SinputSystems.Rebinding{
	public class SinputRebinder : MonoBehaviour {

		private Control[] controls;
		private Control[] controlsDefaults;
		
		public Transform rebindMenuContainer;

		public GameObject devicePanelPrefab;
		public GameObject ControlPanelPrefab;
		public GameObject inputPanelPrefab;

		private List<devicePanel> devicePanels = new List<devicePanel>();

		public SinputMonitor inputMonitor;

		

		// Use this for initialization
		void Start() {
			Init();
		}

		void Init () {
			//Debug.Log("Loading Default controls");
			Sinput.LoadControlScheme("MainControlScheme", false);
			controlsDefaults = Sinput.controls;
			
			//Debug.Log("loading controls with saved data");
			Sinput.LoadControlScheme("MainControlScheme", true);
			controls = Sinput.controls;

			recordedPads = Sinput.gamepads;

			//Debug.Log("It should be over?");
			


			BuildRebindingPanels();
		}

		
		private bool rebinding = false;
		private int rebindingFrames=0;
		private int rebindingControlIndex=-1;
		private int rebindingInputIndex=-1;
		private string rebindingDevice;
		private Text rebindInputText;

		string[] recordedPads = new string[0];
		void Update(){
			

			bool gamepadsChanged = false;
			string[] gamepads = Sinput.gamepads;
			if (recordedPads.Length!=gamepads.Length) gamepadsChanged = true;
			if (!gamepadsChanged){
				for (int i=0; i<recordedPads.Length; i++){
					if (recordedPads[i].ToUpper() != gamepads[i].ToUpper()) gamepadsChanged = true;
				}
			}
			if (gamepadsChanged){
				//connected gamepads have changed
				rebinding = false;

				//Debug.Log("Gamepads changed!");

				recordedPads = Sinput.gamepads;
				Init();
				return;
			}


			if (!rebinding) return;
			rebindingFrames++;
			if (rebindingFrames<5) return;

			//we're ready to swap out an input now
			if (inputMonitor.changeFound){
				rebinding = false;
				rebindInputText.text = "?";
				//Debug.Log("CHANGE INPUT SETTING NOW!");

				InputDeviceType changedInputType = InputDeviceType.Keyboard;

				if (inputMonitor.changedKey != KeyCode.None){
					//change was keyboard input
					//Debug.Log(inputMonitor.changedKey.ToString());
					changedInputType = InputDeviceType.Keyboard;
				}
				if (inputMonitor.changedPadAxis.padIndex >=0){
					//change was a gamepad axis
					//Debug.Log("Found change in gamepad " + (inputMonitor.changedPadAxis.padIndex+1).ToString() + " axis " + (inputMonitor.changedPadAxis.inputIndex).ToString());
					changedInputType = InputDeviceType.GamepadAxis;
				}
				if (inputMonitor.changedPadButton.padIndex >=0){
					//change was a gamepad button
					//Debug.Log("Found change in gamepad " + (inputMonitor.changedPadButton.padIndex+1).ToString() + " button " + inputMonitor.changedPadButton.inputIndex.ToString());
					changedInputType = InputDeviceType.GamepadButton;
				}
				if (inputMonitor.changedMouse != MouseInputType.None){
					//change was mouse click
					//Debug.Log(inputMonitor.changedMouse.ToString());
					changedInputType = InputDeviceType.Mouse;
				}

				DeviceInput newInput = new DeviceInput(changedInputType);
				newInput.isCustom = true;
				newInput.deviceName = rebindingDevice;
				newInput.commonMappingType = CommonGamepadInputs.NOBUTTON;//don't remove this input when gamepads are unplugged/replugged

				if (changedInputType == InputDeviceType.Keyboard){
					newInput.keyboardKeyCode = inputMonitor.changedKey;
				}

				int padIndex = -1;
				if (changedInputType == InputDeviceType.GamepadButton){
					newInput.gamepadButtonNumber = inputMonitor.changedPadButton.inputIndex;
					newInput.displayName = "B" + inputMonitor.changedPadButton.inputIndex.ToString();

					padIndex = inputMonitor.changedPadButton.padIndex;
					List<int> slots = new List<int>();
					//string[] gamepads = Sinput.GetGamepads();
					for (int g=0;g<gamepads.Length; g++){
						if (gamepads[g].ToUpper() == rebindingDevice.ToUpper()) slots.Add(g);
					}
					newInput.allowedSlots = slots.ToArray();
				}
					
				if (changedInputType == InputDeviceType.GamepadAxis){
					SinputMonitor.gamepadStateChange axisChange = inputMonitor.changedPadAxis;

					newInput.gamepadAxisNumber = axisChange.inputIndex;
					newInput.displayName = "A" + axisChange.inputIndex.ToString();
					if (axisChange.axisMotionIsPositive) newInput.displayName += "+";
					if (!axisChange.axisMotionIsPositive) newInput.displayName += "-";

					newInput.invertAxis = false;
					if (!axisChange.axisMotionIsPositive) newInput.invertAxis = true;

					newInput.clampAxis = true;
					newInput.axisButtoncompareVal = 0.4f;
					newInput.defaultAxisValue = 0f;

					newInput.rescaleAxis = false;
					if (axisChange.restingValue != 0f){
						newInput.rescaleAxis = true;
						if (axisChange.restingValue<0f){
							newInput.rescaleAxisMin = -1f;
							newInput.rescaleAxisMax = 1f;
						}else{
							newInput.rescaleAxisMin = 1f;
							newInput.rescaleAxisMax = -1f;
						}
					}

					padIndex = axisChange.padIndex;
					List<int> slots = new List<int>();
					//string[] gamepads = Sinput.GetGamepads();
					for (int g=0;g<gamepads.Length; g++){
						if (gamepads[g].ToUpper() == rebindingDevice.ToUpper()) slots.Add(g);
					}
					newInput.allowedSlots = slots.ToArray();

				}

				if (changedInputType == InputDeviceType.GamepadAxis || changedInputType == InputDeviceType.GamepadButton){
					//lets also set all other inputs on this control with matching allowed slots to be custom and remove common binding
					//this should preserve stuff with the same common binding from being ignored when re reload common bindings
					for (int i=0; i<controls[rebindingControlIndex].inputs.Count; i++){
						bool sameDevice = false;
						if (controls[rebindingControlIndex].inputs[i].commonMappingType != CommonGamepadInputs.NOBUTTON){
							for (int k=0; k<controls[rebindingControlIndex].inputs[i].allowedSlots.Length; k++){
								if (controls[rebindingControlIndex].inputs[i].allowedSlots[k] == padIndex) sameDevice = true;
							}
						}
						if (sameDevice){
							controls[rebindingControlIndex].inputs[i].isCustom = true;
							controls[rebindingControlIndex].inputs[i].commonMappingType = CommonGamepadInputs.NOBUTTON;
							controls[rebindingControlIndex].inputs[i].deviceName = rebindingDevice;

							/*List<int> slots = new List<int>();
							string[] gamepads = Sinput.GetGamepads();
							for (int g=0;g<gamepads.Length; g++){
								if (gamepads[g].ToUpper() == rebindingDevice.ToUpper()) slots.Add(g);
							}
							newInput.allowedSlots = slots.ToArray();*/
						}
					}
				}

				if (changedInputType == InputDeviceType.Mouse){
					
					newInput.mouseInputType = inputMonitor.changedMouse;
				}

				controls[rebindingControlIndex].inputs[rebindingInputIndex] = newInput;

				BuildRebindingPanels();
			}
		}

		//functions called by UI
		public void SetDefaults(){
			//PlayerPrefs.DeleteAll();
			SinputFileIO.DeleteSavedControls();
			Init();
		}
		public void Apply(){

			SinputFileIO.SaveControls(controls);
			Init();
		}

		public void CollapseControlPanel(GameObject panel){
			if (rebinding) return;
			panel.SetActive(!panel.activeSelf);
		}

		public void BeginRebindInput(int controlIndex, int inputIndex, string deviceName, Text text){
			if (rebinding) return;
			//Debug.Log("Begind rebind for : " + controls[controlIndex].name + " input " + controls[controlIndex].inputs[inputIndex].GetDisplayName() + " of " + deviceName);
			rebinding = true;
			rebindingFrames = 0;
			rebindingControlIndex = controlIndex;
			rebindingInputIndex = inputIndex;
			rebindInputText = text;
			rebindInputText.text = "...";
			rebindingDevice = deviceName;

			inputMonitor.SetListeningDevice(rebindingDevice);

		}

		public void DeleteInput(int controlIndex, int inputIndex, string deviceName){
			if (rebinding) return;
			//Debug.Log("Delete input : " + controls[controlIndex].name + " input " + controls[controlIndex].inputs[inputIndex].GetDisplayName() + " of " + deviceName);
			controls[controlIndex].inputs.RemoveAt(inputIndex);
			BuildRebindingPanels();
		}
		public void ResetControlInputs(int controlIndex, string deviceName){
			if (rebinding) return;
			//Debug.Log("reset inputs for : " + controls[controlIndex].name + " of " + deviceName);

			int padIndex = -1;
			string[] padNames = Input.GetJoystickNames();
			for (int i=0;i<padNames.Length; i++){
				if (padNames[i].ToUpper()==deviceName.ToUpper()) padIndex = i;
			}
				

			//remove current inputs for this control from this device
			for (int i=0; i<controls[controlIndex].inputs.Count; i++){
				bool removeControl = false;
				if (deviceName == "KeyboardMouse" && controls[controlIndex].inputs[i].inputType == InputDeviceType.Keyboard) removeControl = true;
				if (deviceName == "KeyboardMouse" && controls[controlIndex].inputs[i].inputType == InputDeviceType.Mouse) removeControl = true;
				if (deviceName != "KeyboardMouse" && controls[controlIndex].inputs[i].inputType == InputDeviceType.GamepadAxis) removeControl = true;
				if (deviceName != "KeyboardMouse" && controls[controlIndex].inputs[i].inputType == InputDeviceType.GamepadButton) removeControl = true;
				if (controls[controlIndex].inputs[i].inputType == InputDeviceType.Virtual) removeControl = false; //don't remove virtual inputs

				//make sure we only remove inputs for this gamepad
				if (removeControl && deviceName != "KeyboardMouse"){
					bool matchingPad = false;
					for (int k=0; k<controls[controlIndex].inputs[i].allowedSlots.Length; k++){
						if (controls[controlIndex].inputs[i].allowedSlots[k] == padIndex) matchingPad = true;
					}
					if (!matchingPad) removeControl = false;
				}

				if (removeControl){
					controls[controlIndex].inputs.RemoveAt(i);
					i--;
				}
			}

			//add default inputs
			for (int i=0; i<controlsDefaults[controlIndex].inputs.Count; i++){
				bool addControl = false;
				if (deviceName == "KeyboardMouse" && controlsDefaults[controlIndex].inputs[i].inputType == InputDeviceType.Keyboard) addControl = true;
				if (deviceName == "KeyboardMouse" && controlsDefaults[controlIndex].inputs[i].inputType == InputDeviceType.Mouse) addControl = true;
				if (deviceName != "KeyboardMouse" && controlsDefaults[controlIndex].inputs[i].inputType == InputDeviceType.GamepadAxis) addControl = true;
				if (deviceName != "KeyboardMouse" && controlsDefaults[controlIndex].inputs[i].inputType == InputDeviceType.GamepadButton) addControl = true;
				if (controlsDefaults[controlIndex].inputs[i].inputType == InputDeviceType.Virtual) addControl = false; //don't add virtual inputs

				//make sure we only add inputs for this gamepad
				if (addControl && deviceName != "KeyboardMouse"){
					bool matchingPad = false;
					for (int k=0; k<controlsDefaults[controlIndex].inputs[i].allowedSlots.Length; k++){
						if (controlsDefaults[controlIndex].inputs[i].allowedSlots[k] == padIndex) matchingPad = true;
					}
					if (!matchingPad) addControl = false;
				}

				if (addControl){
					controls[controlIndex].inputs.Add(controlsDefaults[controlIndex].inputs[i]);
				}
			}
			BuildRebindingPanels();



		}
		public void AddControlInput(int controlIndex, string deviceName){
			if (rebinding) return;
			//Debug.Log("Add new input for : " + controls[controlIndex].name + " of " + deviceName);

			InputDeviceType t = InputDeviceType.Keyboard;
			if (deviceName != "KeyboardMouse") t = InputDeviceType.GamepadButton;
			DeviceInput newInput = new DeviceInput(t);

			newInput.isCustom = true;
			newInput.deviceName = deviceName;
			newInput.commonMappingType = CommonGamepadInputs.NOBUTTON;//don't remove this input when gamepads are unplugged/replugged

			if (t == InputDeviceType.Keyboard){
				newInput.keyboardKeyCode = KeyCode.None;
			}

			if (t == InputDeviceType.GamepadButton){
				newInput.gamepadButtonNumber = 18;
				newInput.displayName = "B?";

				string[] padNames = Input.GetJoystickNames();
				List<int> allowedSlots = new List<int>();
				for (int i=0;i<padNames.Length; i++){
					if (padNames[i].ToUpper()==deviceName.ToUpper()) allowedSlots.Add(i);
				}
				newInput.allowedSlots = allowedSlots.ToArray();
			}
				

			controls[controlIndex].inputs.Add(newInput);

			BuildRebindingPanels();

		}


		//stuff for building UI
		void BuildRebindingPanels(){

			//Debug.Log("Building rebind UI");

			//clear any existing panels
			for (int i=0; i<devicePanels.Count; i++){
				GameObject.Destroy(devicePanels[i].devicePanelObj);
			}
			devicePanels = new List<devicePanel>();

			//add keyboard/mouse cdevice panel
			AddDevicePanel(InputDeviceType.Keyboard, "KeyboardMouse", -1);

			//add gamepad device panels
			string[] pads = Sinput.gamepads;
			for (int p=0; p<pads.Length; p++){
				bool deviceAlreadyListed = false;
				for (int d=0; d<devicePanels.Count; d++){
					if (devicePanels[d].deviceName == pads[p]) deviceAlreadyListed = true;
				}

				if (!deviceAlreadyListed) AddDevicePanel(InputDeviceType.GamepadAxis, pads[p], p);
			}
			
		}
	

		void AddDevicePanel(InputDeviceType deviceType, string deviceName, int deviceSlotIndex){
			GameObject newDevicePanelObj = (GameObject)GameObject.Instantiate(devicePanelPrefab);
			newDevicePanelObj.name = deviceName;
			newDevicePanelObj.transform.SetParent(rebindMenuContainer);
			newDevicePanelObj.transform.localScale = Vector3.one;
			devicePanel newDevicePanel = new devicePanel();
			newDevicePanel.devicePanelObj = newDevicePanelObj;

			newDevicePanel.deviceName = deviceName;

			newDevicePanel.foldoutButton = newDevicePanelObj.transform.Find("DeviceNameFoldout").Find("FoldoutButton").GetComponent<Button>();
			newDevicePanel.foldoutButton.enabled = false;//lets not have foldouts for now, they get reset all the time anyway
			newDevicePanel.deviceNameText = newDevicePanel.foldoutButton.transform.Find("DeviceNameText").GetComponent<Text>();

			newDevicePanel.foldoutButton.onClick.AddListener(delegate {CollapseControlPanel(newDevicePanelObj.transform.Find("ControlsPanel").gameObject); });

			newDevicePanel.deviceNameText.text = "Keyboard/Mouse:";
			if (deviceType == InputDeviceType.GamepadAxis || deviceType == InputDeviceType.GamepadButton){
				newDevicePanel.deviceNameText.text = "Gamepad: \"" + deviceName + "\"";
			}

			newDevicePanel.controlPanels = new List<controlPanel>();

			for (int c=0; c<controls.Length; c++){
				GameObject newControlPanelObj = (GameObject)GameObject.Instantiate(ControlPanelPrefab);
				newControlPanelObj.name = controls[c].name;
				newControlPanelObj.transform.SetParent(newDevicePanelObj.transform.Find("ControlsPanel").transform);
				newControlPanelObj.transform.localScale = Vector3.one;
				controlPanel newControlPanel = new controlPanel();
				newControlPanel.controlPanelObj = newControlPanelObj;

				newControlPanel.controlNameText = newControlPanelObj.transform.Find("ControlName").Find("ControlNameText").GetComponent<Text>();
				newControlPanel.addInputButton = newControlPanelObj.transform.Find("AddInput").GetComponent<Button>();
				newControlPanel.resetInputsButton = newControlPanelObj.transform.Find("ResetControl").GetComponent<Button>();

				newControlPanel.controlNameText.text = controls[c].name;
				//Debug.Log(c);
				int controlIndex = c;
				newControlPanel.addInputButton.onClick.AddListener(delegate {AddControlInput(controlIndex,deviceName); });
				newControlPanel.resetInputsButton.onClick.AddListener(delegate {ResetControlInputs(controlIndex,deviceName); });


				newControlPanel.inputPanels = new List<inputPanel>();

				for (int i=0; i<controls[c].inputs.Count; i++){
					bool applicableInput = false;
					if (deviceType == InputDeviceType.GamepadAxis || deviceType == InputDeviceType.GamepadButton){
						if (null != controls[c].inputs[i].allowedSlots){
							for (int s=0; s<controls[c].inputs[i].allowedSlots.Length; s++){
								if (controls[c].inputs[i].allowedSlots[s] == deviceSlotIndex) applicableInput = true;
							}
						}
						if (controls[c].inputs[i].deviceName == deviceName) applicableInput = true;
					}
					if (deviceType == InputDeviceType.Keyboard || deviceType == InputDeviceType.Mouse){
						if (controls[c].inputs[i].inputType == InputDeviceType.Keyboard) applicableInput = true;
						if (controls[c].inputs[i].inputType == InputDeviceType.Mouse) applicableInput = true;
					}
					if (applicableInput){
						//this input is referring to this device
						GameObject newInputPanelObj = (GameObject)GameObject.Instantiate(inputPanelPrefab);
						newInputPanelObj.transform.SetParent(newControlPanelObj.transform);
						newInputPanelObj.transform.localScale = Vector3.one;
						inputPanel newInputPanel = new inputPanel();

						newInputPanel.inputButton = newInputPanelObj.transform.Find("Input").GetComponent<Button>();
						newInputPanel.deleteButton = newInputPanelObj.transform.Find("InputDelete").GetComponent<Button>();
						newInputPanel.inputButtonText = newInputPanel.inputButton.transform.Find("Text").GetComponent<Text>();

						int inputIndex = i;
						newInputPanel.inputButton.onClick.AddListener(delegate {BeginRebindInput(controlIndex,inputIndex,deviceName, newInputPanel.inputButtonText); });
						newInputPanel.deleteButton.onClick.AddListener(delegate {DeleteInput(controlIndex,inputIndex,deviceName); });

						newInputPanel.inputButtonText.text = controls[c].inputs[i].GetDisplayName();

						newControlPanel.inputPanels.Add(newInputPanel);
					}
				}

				newControlPanel.addInputButton.transform.SetSiblingIndex(98);
				newControlPanel.resetInputsButton.transform.SetSiblingIndex(99);

				newDevicePanel.controlPanels.Add(newControlPanel);
			}


			devicePanels.Add(newDevicePanel);
		}

		struct devicePanel{
			public GameObject devicePanelObj;
			public Button foldoutButton;
			public Text deviceNameText;
			public string deviceName;

			public List<controlPanel> controlPanels;
		}
		struct controlPanel{
			public GameObject controlPanelObj;

			public Text controlNameText;
			public Button addInputButton;
			public Button resetInputsButton;
			public List<inputPanel> inputPanels;
		}
		struct inputPanel{
			public GameObject inputPanelObj;
			public Button inputButton;
			public Text inputButtonText;
			public Button deleteButton;
		}


	}
}