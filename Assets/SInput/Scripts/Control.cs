using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinputSystems{
	public class Control{
		//name of control
		public string name;


		//list of inputs we will check when the control is polled
		public List<DeviceInput> inputs;

		public List<CommonGamepadInputs> commonBindings = new List<CommonGamepadInputs>();

		//control constructor
		public Control(string controlName){
			name = controlName;
			inputs = new List<DeviceInput>();
		}


		public bool ButtonCheck(ButtonAction bAction, InputDeviceSlot slot){
			for (int i=0; i<inputs.Count; i++){
				if (inputs[i].ButtonCheck(bAction, slot)) return true;
			}
			return false;
		}

		public float AxisCheck(InputDeviceSlot slot){
			float returnV = 0f;
			for (int i=0; i<inputs.Count; i++){
				float v = inputs[i].AxisCheck(slot);
				if (Mathf.Abs(v) > returnV) returnV = v;
			}
			return returnV;
		}


		public void AddKeyboardInput(KeyCode keyCode){
			DeviceInput input = new DeviceInput(InputDeviceType.Keyboard);
			input.keyboardKeyCode = keyCode;
			input.commonBindingType = CommonGamepadInputs.NOBUTTON;//don't remove this input when gamepads are unplugged/replugged
			inputs.Add(input);
		}

		public void AddGamepadInput(CommonGamepadInputs gamepadButtonOrAxis){ AddGamepadInput(gamepadButtonOrAxis, true); }
		private void AddGamepadInput(CommonGamepadInputs gamepadButtonOrAxis, bool isNewBinding){
			Sinput.UpdateGamepads();

			if (isNewBinding) commonBindings.Add(gamepadButtonOrAxis);
			List<DeviceInput> applicableMapInputs = CommonGamepadBindings.GetApplicableMaps( gamepadButtonOrAxis, Sinput.GetGamepads() );

			string[] gamepads = Sinput.GetGamepads();

			//find which common bound inputs apply here, but already have custom binding loaded, and disregard those common bindings
			for (int ai=0; ai<applicableMapInputs.Count; ai++){
				bool samePad = false;
				for (int i=0; i<inputs.Count; i++){
					if (inputs[i].inputType == InputDeviceType.GamepadAxis || inputs[i].inputType == InputDeviceType.GamepadButton){
						if (inputs[i].isCustom){
							for (int ais=0; ais<applicableMapInputs[ai].allowedSlots.Length; ais++){
								for (int toomanyints=0; toomanyints<inputs[i].allowedSlots.Length; toomanyints++){
									if (applicableMapInputs[ai].allowedSlots[ais] == inputs[i].allowedSlots[toomanyints]) samePad = true;
								}
								if (gamepads[applicableMapInputs[ai].allowedSlots[ais]].ToUpper() == inputs[i].deviceName.ToUpper()) samePad = true;
							}
							if (samePad){
								//if I wanna be copying input display names, here's the place to do it
								//TODO: decide if I wanna do this
								//pro: it's good if the common binding is accurate but the user wants to rebind
								//con: it's bad if the common binding is bad or has a generic gamepad name and so it mislables different inputs
								//maybe I should do this, but with an additional check so it's not gonna happen with say, a device labelled "wireless controller"?
							}
						}
					}
				}
				if (samePad){
					//we already have a custom bound control for this input, we don't need more
					applicableMapInputs.RemoveAt(ai);
					ai--;
				}
			}

			//add whichever common bindings still apply
			for (int i=0; i<applicableMapInputs.Count; i++){
				inputs.Add(applicableMapInputs[i]);
			}
		}

		public void AddMouseInput(MouseInputType mouseInputType){
			DeviceInput input = new DeviceInput(InputDeviceType.Mouse);
			input.mouseInputType = mouseInputType;
			input.commonBindingType = CommonGamepadInputs.NOBUTTON;
			inputs.Add(input);
		}

		public void AddVirtualInput(string virtualInputID){
			DeviceInput input = new DeviceInput(InputDeviceType.Virtual);
			input.virtualInputID = virtualInputID;
			input.commonBindingType = CommonGamepadInputs.NOBUTTON;
			inputs.Add(input);
		}

		public void ReapplyCommonBindings(){
			//connected gamepads have changed, so we want to remove all old common bindings, and replace them now new binding information has been loaded
			for (int i=0; i<inputs.Count; i++){
				if (inputs[i].commonBindingType != CommonGamepadInputs.NOBUTTON){
					inputs.RemoveAt(i);
					i--;
				}
			}



			for (int i=0; i<commonBindings.Count; i++){
				AddGamepadInput(commonBindings[i], false);
			}

			//also recheck allowed slots for custom bound pads (their inputs have a device name, common bound stuff don't)
			//need to do this anyway so we can check if common & custom bindings are about to match on the same slot
			string[] gamepads= Sinput.GetGamepads();
			for (int i=0; i<inputs.Count; i++){
				if (inputs[i].deviceName!=""){
					List<int> allowedSlots = new List<int>();
					for (int g=0; g<gamepads.Length; g++){
						if (gamepads[g].ToUpper() == inputs[i].deviceName.ToUpper()) allowedSlots.Add(i);
					}
					inputs[i].allowedSlots = allowedSlots.ToArray();
				}
			}
		}

		public void UpdateAxisButtonStates(){
			for (int i=0; i<inputs.Count; i++){
				inputs[i].UpdateAxisButtonStates();
			}
		}
		public void ResetAxisButtonStates(){
			for (int i=0; i<inputs.Count; i++){
				inputs[i].ResetAxisButtonStates();
			}
		}
		//functions needed
		//functions to add/set/remove? deviceinputs
	}
}
