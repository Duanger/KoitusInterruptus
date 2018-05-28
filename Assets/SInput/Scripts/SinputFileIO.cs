using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SinputSystems{
	public static  class SinputFileIO {

		public static bool SaveDataExists(){
			if (PlayerPrefs.GetString("Sinput_savedInputsList","nope") != "nope") return true;
			return false;
		}

		//loading stuff
		static string activeControlName = "";
		static string[] joysticks;
		public static Control[] LoadControls(Control[] schemeToReplace){
			//we pass the existing control scheme so that info on needed common bindings can be kept

			//create our new controls list, and add any common bindings we might need to load later when pads are swapped in/out
			List<Control> controls = new List<Control>();
			for (int c=0; c<schemeToReplace.Length; c++){
				controls.Add( new Control(schemeToReplace[c].name) );
				controls[c].commonMappings = new List<CommonGamepadInputs>();
				for (int b=0; b<schemeToReplace[c].commonMappings.Count; b++){
					controls[c].commonMappings.Add(schemeToReplace[c].commonMappings[b]);
				}
			}

			//load all saved inputs
			joysticks = Input.GetJoystickNames();
			string[] savedInputsList = PlayerPrefs.GetString("Sinput_savedInputsList","").Split('\n');
			for (int i=0; i<savedInputsList.Length; i++){
				DeviceInput loadedInput = LoadInput(savedInputsList[i]);

				bool foundRelevantControl = false;
				for (int c=0; c<controls.Count; c++){
					if (controls[c].name == activeControlName){
						foundRelevantControl = true;

						controls[c].inputs.Add(loadedInput);
					}
				}

				if (!foundRelevantControl){
					//this shouldn't happen, except in cases where you are loading from a control scheme saved before the dev removed or changed a named control
					//and being real, if the dev didn't keep the name it's pointless to load it anyway
					//tbh I honestly don't know why I even coded this check for finding a named control.
					//real talk I'm probably like two weeks into non-stop coding this system and
					//I
					//am
					//tired
					//
					//I really hope all of this code works when I'm done. OTL
					Debug.Log("Hi. This is probably nothing to worry about, but who knows!");
				}
			}




			return controls.ToArray();
		}

		static DeviceInput LoadInput(string saveName){
			InputDeviceType t = InputDeviceType.Keyboard;
			foreach(InputDeviceType inputType in Enum.GetValues(typeof(InputDeviceType))){
				if (inputType.ToString() == PlayerPrefs.GetString(saveName + "deviceType", "")) t = inputType;
			}

			DeviceInput newInput = new DeviceInput(t);


			activeControlName = PlayerPrefs.GetString(saveName + "controlName", "");

			newInput.displayName = PlayerPrefs.GetString(saveName + "displayName", "?");
			newInput.isCustom = 1==PlayerPrefs.GetInt(saveName + "isCustom", 0);
			newInput.deviceName = PlayerPrefs.GetString(saveName + "deviceName", "");

			//load keyboard specific stuff
			if (t == InputDeviceType.Keyboard){
				newInput.keyboardKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(saveName + "keyCode", KeyCode.None.ToString()) );
			}

			//load mouse specific stuff
			if (t == InputDeviceType.Mouse){
				newInput.mouseInputType = (MouseInputType)Enum.Parse(typeof(MouseInputType), PlayerPrefs.GetString(saveName + "mouseInput", MouseInputType.None.ToString()) );
			}

			//load gamepad button specific stuff
			if (t == InputDeviceType.GamepadButton){
				newInput.gamepadButtonNumber = PlayerPrefs.GetInt(saveName + "padButton", 0);
			}

			//load gamepad axis specific stuff
			if (t == InputDeviceType.GamepadAxis){
				newInput.gamepadAxisNumber = PlayerPrefs.GetInt(saveName + "padAxis", 0);
				newInput.invertAxis = 1==PlayerPrefs.GetInt(saveName + "padAxisInvert", 0);
				newInput.clampAxis = 1==PlayerPrefs.GetInt(saveName + "padAxisClamp", 0);
				newInput.rescaleAxis = 1==PlayerPrefs.GetInt(saveName + "padAxisRescale", 0);
				newInput.rescaleAxisMin = PlayerPrefs.GetFloat(saveName + "padAxisRescaleMin", 0f);
				newInput.rescaleAxisMax = PlayerPrefs.GetFloat(saveName + "padAxisRescaleMax", 1f);
				newInput.defaultAxisValue = 0f;
				newInput.axisButtoncompareVal = 0.4f;
			}

			//load virtual specific stuff
			if (t == InputDeviceType.Virtual){
				newInput.virtualInputID = PlayerPrefs.GetString(saveName + "virtualInputID", "");
			}

			//lets not forget stuff that isn't saved, but needed anyway
			newInput.commonMappingType = CommonGamepadInputs.NOBUTTON;
			newInput.defaultAxisValue = 0f;
			if (t==InputDeviceType.GamepadAxis || t==InputDeviceType.GamepadButton){
				List<int> allowedSlots = new List<int>();
				for (int i=0; i<joysticks.Length; i++){
					if (joysticks[i].ToUpper() == newInput.deviceName.ToUpper()) allowedSlots.Add(i);
				}
				newInput.allowedSlots = allowedSlots.ToArray();
			}


			return newInput;
		}

		//saving stuff
		public static void SaveControls(Control[] controls){

			DeleteSavedControls();//delete existing controls so we dont have stray inputs saved that have since been removed

			string savedInputNames = "";
			int savedInputIndex = 0;
			for (int c=0; c<controls.Length; c++){
				for (int i=0; i<controls[c].inputs.Count; i++){


					//save input
					string saveName = "snpt" + savedInputIndex.ToString() + "_";
					SaveInput( controls[c].inputs[i] , controls[c].name, saveName);

					//add to our list of saved inputs
					if (savedInputIndex>0) savedInputNames+= "\n";
					savedInputNames += saveName;

					savedInputIndex++;


				}
			}

			PlayerPrefs.SetString("Sinput_savedInputsList", savedInputNames);
		}

		static void SaveInput(DeviceInput input, string controlName, string saveName){
			
			PlayerPrefs.SetString(saveName + "controlName", controlName);

			PlayerPrefs.SetString(saveName + "deviceType", input.inputType.ToString());
			PlayerPrefs.SetString(saveName + "displayName", input.displayName);
			//PlayerPrefs.SetInt(saveName + "isCustom", 0);
			//if (input.isCustom) PlayerPrefs.SetInt(saveName + "isCustom", 1);
			PlayerPrefs.SetInt(saveName + "isCustom", 1);
			PlayerPrefs.SetString(saveName + "deviceName", input.deviceName);

			//save keyboard specific stuff
			if (input.inputType == InputDeviceType.Keyboard){
				PlayerPrefs.SetString(saveName + "keyCode", input.keyboardKeyCode.ToString());
			}

			//save mouse specific stuff
			if (input.inputType == InputDeviceType.Mouse){
				PlayerPrefs.SetString(saveName + "mouseInput", input.mouseInputType.ToString());
			}

			//save gamepad button specific stuff
			if (input.inputType == InputDeviceType.GamepadButton){
				PlayerPrefs.SetInt(saveName + "padButton", input.gamepadButtonNumber);
			}

			//save gamepad axis specific stuff
			if (input.inputType == InputDeviceType.GamepadAxis){
				PlayerPrefs.SetInt(saveName + "padAxis", input.gamepadAxisNumber);
				PlayerPrefs.SetInt(saveName + "padAxisInvert", 0);
				if (input.invertAxis) PlayerPrefs.SetInt(saveName + "padAxisInvert", 1);
				PlayerPrefs.SetInt(saveName + "padAxisClamp", 0);
				if (input.clampAxis) PlayerPrefs.SetInt(saveName + "padAxisClamp", 1);
				PlayerPrefs.SetInt(saveName + "padAxisRescale", 0);
				if (input.rescaleAxis) PlayerPrefs.SetInt(saveName + "padAxisRescale", 1);
				PlayerPrefs.SetFloat(saveName + "padAxisRescaleMin", input.rescaleAxisMin);
				PlayerPrefs.SetFloat(saveName + "padAxisRescaleMax", input.rescaleAxisMax);
			}

			//save virtual specific stuff
			if (input.inputType == InputDeviceType.Virtual){
				PlayerPrefs.SetString(saveName + "virtualInputID", input.virtualInputID);
			}
		}

		//deleting stuff
		public static void DeleteSavedControls(){
			string[] savedInputsList = PlayerPrefs.GetString("Sinput_savedInputsList","").Split('\n');
			for (int i=0; i<savedInputsList.Length; i++){
				DeleteSavedInput(savedInputsList[i]);
			}

			PlayerPrefs.DeleteKey("Sinput_savedInputsList");
		}

		static void DeleteSavedInput(string saveName){
			if (PlayerPrefs.GetString(saveName + "deviceType", "") == InputDeviceType.Keyboard.ToString()){
				PlayerPrefs.DeleteKey(saveName + "keyCode");
			}
			if (PlayerPrefs.GetString(saveName + "deviceType", "") == InputDeviceType.Mouse.ToString()){
				PlayerPrefs.DeleteKey(saveName + "mouseInput");
			}
			if (PlayerPrefs.GetString(saveName + "deviceType", "") == InputDeviceType.GamepadButton.ToString()){
				PlayerPrefs.DeleteKey(saveName + "padButton");
			}
			if (PlayerPrefs.GetString(saveName + "deviceType", "") == InputDeviceType.GamepadAxis.ToString()){
				PlayerPrefs.DeleteKey(saveName + "padAxis");
				PlayerPrefs.DeleteKey(saveName + "padAxisInvert");
				PlayerPrefs.DeleteKey(saveName + "padAxisClamp");
				PlayerPrefs.DeleteKey(saveName + "padAxisRescale");
				PlayerPrefs.DeleteKey(saveName + "padAxisRescaleMin");
				PlayerPrefs.DeleteKey(saveName + "padAxisRescaleMax");
			}
			if (PlayerPrefs.GetString(saveName + "deviceType", "") == InputDeviceType.Virtual.ToString()){
				PlayerPrefs.DeleteKey(saveName + "virtualInputID");
			}

			PlayerPrefs.DeleteKey(saveName + "controlName");
			PlayerPrefs.DeleteKey(saveName + "deviceType");
			PlayerPrefs.DeleteKey(saveName + "displayName");
			PlayerPrefs.DeleteKey(saveName + "isCustom");
			PlayerPrefs.DeleteKey(saveName + "deviceName");
		}

	}
}