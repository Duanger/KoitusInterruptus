using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinputSystems {
	public static class CommonGamepadBindings {


		static List<CommonBinding> commonBindings;
		static BindingSlots[] bindingSlots;

		public static void ReloadCommonMaps() {
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
			if (Application.platform == RuntimePlatform.Switch) thisOS = OSFamily.Switch;

			System.Object[] commonBindingAssets = Resources.LoadAll("", typeof(CommonBinding));
			commonBindings = new List<CommonBinding>();
			//List<CommonBinding> partialMatchBindings = new List<CommonBinding>();
			string[] gamepads = Sinput.GetGamepads();
			int defaultBindingIndex = -1;
			for (int i = 0; i < commonBindingAssets.Length; i++) {
				if (((CommonBinding)commonBindingAssets[i]).os == thisOS) {
					bool gamepadConnected = false;
					bool partialMatch = false;
					for (int k = 0; k < ((CommonBinding)commonBindingAssets[i]).names.Count; k++) {
						for (int g = 0; g < gamepads.Length; g++) {
							if (((CommonBinding)commonBindingAssets[i]).names[k].ToUpper() == gamepads[g]) gamepadConnected = true;
						}
					}

					for (int k = 0; k < ((CommonBinding)commonBindingAssets[i]).partialNames.Count; k++) {
						for (int g = 0; g < gamepads.Length; g++) {
							if (gamepads[g].Contains(((CommonBinding)commonBindingAssets[i]).partialNames[k].ToUpper())) partialMatch = true;
						}
					}

					if (gamepadConnected) commonBindings.Add((CommonBinding)commonBindingAssets[i]);
					if (partialMatch && !gamepadConnected) commonBindings.Add((CommonBinding)commonBindingAssets[i]);
					if (!partialMatch && !gamepadConnected && ((CommonBinding)commonBindingAssets[i]).isDefault) commonBindings.Add((CommonBinding)commonBindingAssets[i]);

					if (((CommonBinding)commonBindingAssets[i]).isDefault) defaultBindingIndex = commonBindings.Count - 1;
				}
			}



			//for each common binding, find which gamepad slots it applies to
			//inputs built from common bindings will only check slots which match
			bindingSlots = new BindingSlots[commonBindings.Count];
			for (int i = 0; i < bindingSlots.Length; i++) {
				bindingSlots[i].slots = new List<int>();
			}
			//string[] gamepads = Sinput.GetGamepads();
			for (int i = 0; i < commonBindings.Count; i++) {
				for (int k = 0; k < commonBindings[i].names.Count; k++) {
					for (int g = 0; g < gamepads.Length; g++) {
						if (gamepads[g] == commonBindings[i].names[k].ToUpper()) {
							bindingSlots[i].slots.Add(g);
						}
					}
				}
			}

			//find out if there are any connected gamepads that dont match anything in bindingSlots
			//then find 
			for (int g = 0; g < gamepads.Length; g++) {
				bool bindingMatch = false;
				for (int b = 0; b < bindingSlots.Length; b++) {
					for (int s = 0; s < bindingSlots[b].slots.Count; s++) {
						if (bindingSlots[b].slots[s] == g) bindingMatch = true;
					}
				}
				if (!bindingMatch) {
					//check for partial name matches with this gamepad slot
					for (int i = 0; i < commonBindings.Count; i++) {
						for (int k = 0; k < commonBindings[i].partialNames.Count; k++) {
							if (!bindingMatch && gamepads[g].Contains(commonBindings[i].partialNames[k])) {
								bindingMatch = true;
								bindingSlots[i].slots.Add(g);
							}
						}
					}
					if (!bindingMatch && defaultBindingIndex != -1) {
						//apply default common binding to this slot
						bindingSlots[defaultBindingIndex].slots.Add(g);
					}
				}
			}

		}
		struct BindingSlots {
			public List<int> slots;
		}


		public static List<DeviceInput> GetApplicableMaps(CommonGamepadInputs t, string[] connectedGamepads) {
			//builds input mapping of type t for all known connected gamepads


			List<DeviceInput> applicableInputs = new List<DeviceInput>();


			for (int i = 0; i < commonBindings.Count; i++) {

				//add any applicable button mappings
				for (int k = 0; k < commonBindings[i].buttons.Count; k++) {
					if (commonBindings[i].buttons[k].buttonType == t) {
						//add this button input
						DeviceInput newInput = new DeviceInput(InputDeviceType.GamepadButton);
						//newInput.gamepadNames = commonBindings[i].names.ToArray();
						newInput.gamepadButtonNumber = commonBindings[i].buttons[k].buttonNumber;
						newInput.commonBindingType = t;
						newInput.displayName = commonBindings[i].buttons[k].displayName;

						newInput.allowedSlots = bindingSlots[i].slots.ToArray();

						applicableInputs.Add(newInput);
					}
				}
				//add any applicable axis bingings
				for (int k = 0; k < commonBindings[i].axis.Count; k++) {
					if (commonBindings[i].axis[k].buttonType == t) {
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

						if (commonBindings[i].axis[k].rescaleAxis) {
							newInput.rescaleAxis = true;
							newInput.rescaleAxisMin = commonBindings[i].axis[k].rescaleAxisMin;
							newInput.rescaleAxisMax = commonBindings[i].axis[k].rescaleAxisMax;
						}

						applicableInputs.Add(newInput);
					}
				}

			}



			return applicableInputs;
		}

	}
}