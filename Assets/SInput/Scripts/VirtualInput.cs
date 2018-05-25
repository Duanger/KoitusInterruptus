using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinputSystems {

	public static class VirtualInputs {
		private static List<VirtualInput> inputs = new List<VirtualInput>();

		public static void AddInput(string virtualInputName) {
			for (int i=0; i<inputs.Count; i++) {
				if (inputs[i].name == virtualInputName) return;
			}
			inputs.Add(new VirtualInput(virtualInputName));
		}

		public static float GetVirtualAxis(string virtualInputName) {
			for (int i = 0; i < inputs.Count; i++) {
				if (inputs[i].name == virtualInputName) return inputs[i].axisValue;
			}
			Debug.Log("Virtual input \"" + virtualInputName + "\" not found.");
			return 0f;
		}

		public static ButtonAction GetVirtualButton(string virtualInputName) {
			for (int i = 0; i < inputs.Count; i++) {
				if (inputs[i].name == virtualInputName) return inputs[i].buttonState;
			}
			Debug.Log("Virtual input \"" + virtualInputName + "\" not found.");
			return ButtonAction.NOTHING;
		}

		public static bool GetDeltaPreference(string virtualInputName) {
			for (int i = 0; i < inputs.Count; i++) {
				if (inputs[i].name == virtualInputName) return inputs[i].preferDeltaUse;
			}
			Debug.Log("Virtual input \"" + virtualInputName + "\" not found.");
			return false;
		}

		public static void SetVirtualAxis(string virtualInputName, float newAxisValue) {
			Sinput.SinputUpdate(); //make sure sinput is set up, so any bound virtual inputs have been instantiated
			for (int i = 0; i < inputs.Count; i++) {
				if (inputs[i].name == virtualInputName) {
					inputs[i].SetAxisValue(newAxisValue);
					return;
				}
			}
			Debug.Log("Virtual input \"" + virtualInputName + "\" not found.");
		}

		public static void SetVirtualButton(string virtualInputName, ButtonAction newButtonState) {
			Sinput.SinputUpdate(); //make sure sinput is set up, so any bound virtual inputs have been instantiated
			for (int i = 0; i < inputs.Count; i++) {
				if (inputs[i].name == virtualInputName) {
					inputs[i].SetButtonState(newButtonState);
					return;
				}
			}
			Debug.Log("Virtual input \"" + virtualInputName + "\" not found.");
		}

		public static void SetVirtualButtonHeld(string virtualInputName, bool held) {
			Sinput.SinputUpdate(); //make sure sinput is set up, so any bound virtual inputs have been instantiated
			for (int i = 0; i < inputs.Count; i++) {
				if (inputs[i].name == virtualInputName) {
					inputs[i].UpdateButtonState(held);
					return;
				}
			}
			Debug.Log("Virtual input \"" + virtualInputName + "\" not found.");
		}

		public static void SetDeltaPreference(string virtualInputName, bool preferFrameDelta) {
			Sinput.SinputUpdate(); //make sure sinput is set up, so any bound virtual inputs have been instantiated
			for (int i = 0; i < inputs.Count; i++) {
				if (inputs[i].name == virtualInputName) {
					inputs[i].preferDeltaUse = preferFrameDelta;
					return;
				}
			}
			Debug.Log("Virtual input \"" + virtualInputName + "\" not found.");
		}
	}

	class VirtualInput {
		public string name;

		public bool preferDeltaUse = true;
		public float axisValue = 0f;
		public ButtonAction buttonState = ButtonAction.NOTHING;

		public VirtualInput(string virtualInputName) {
			name = virtualInputName;
		}

		public void UpdateButtonState(bool held) {

			if (!held) {
				switch (buttonState) {
					case ButtonAction.HELD:
						buttonState = ButtonAction.UP;
						break;
					case ButtonAction.DOWN:
						buttonState = ButtonAction.UP;
						break;
					default:
						buttonState = ButtonAction.NOTHING;
						break;
				}
			} else {
				switch (buttonState) {
					case ButtonAction.NOTHING:
						buttonState = ButtonAction.DOWN;
						break;
					case ButtonAction.UP:
						buttonState = ButtonAction.DOWN;
						break;
					default:
						buttonState = ButtonAction.HELD;
						break;
				}
			}

		}

		public void SetButtonState(ButtonAction newState) {
			buttonState = newState;
		}

		public void SetAxisValue(float newValue) {
			axisValue = newValue;
		}

	}
}