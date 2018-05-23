﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinputSystems{
	public class DeviceInput{

		public InputDeviceType inputType;
		public string displayName;

		//custom bound stuff
		public bool isCustom = false;
		public string deviceName = "";

		public string GetDisplayName(){
			if (inputType == InputDeviceType.Keyboard){
				return keyboardKeyCode.ToString();
			}
			if (inputType == InputDeviceType.Mouse){
				return mouseInputType.ToString();
			}
			return displayName;
		}


		public DeviceInput(InputDeviceType type){
			inputType = type;

			if (inputType==InputDeviceType.Virtual){
				virtualAxisValue = 0f;
				virtualInputState = ButtonAction.NOTHING;
			}
		}

		//////////// ~ keyboard specific stuff ~ ////////////
		public KeyCode keyboardKeyCode; //keycode for if this input is controlled by a keyboard key

		//////////// ~ mouse specific stuff ~ ////////////
		public MouseInputType mouseInputType;

		//////////// ~ gamepad specific stuff ~ ////////////
		public int[] allowedSlots; //list of gamepad slots that this input is allowed to check (they will be ones with a matching name to the known binding
		public CommonGamepadInputs commonBindingType; //if this is set, this input is a preset/default
		public int gamepadButtonNumber; //button number for if this input is controlled by a gamepad button

		public int gamepadAxisNumber; //axis number for if this input is controlled by a gamepad axis
		public bool invertAxis;
		public bool clampAxis;
		public bool rescaleAxis;//for rescaling input axis from something else to 0-1
		public float rescaleAxisMin;
		public float rescaleAxisMax;

		//stuff for treating axis like a button
		ButtonAction[] axisButtonState; //state of the axis for when used as a button, updated on the first button checks of a frame. list contains state of this axis for each gamepad slot
		public float axisButtoncompareVal; //axis button is 'pressed' if (axisValue [compareType] compareVal)

		//all GetAxis() checks will return default value until a measured change occurs, since readings before then can be wrong
		private bool useDefaultAxisValue = true;
		public float defaultAxisValue;
		private float measuredAxisValue=-54.321f;

		//////////// ~ virtual specific stuff ~ ////////////
		public string virtualInputID;
		public ButtonAction virtualInputState;
		public float virtualAxisValue;


		public bool ButtonCheck(ButtonAction bAction, InputDeviceSlot slot){

			//keyboard key checks
			if (inputType == InputDeviceType.Keyboard){
				if (slot == InputDeviceSlot.any || slot == InputDeviceSlot.keyboard || slot == InputDeviceSlot.keyboardAndMouse){
					if (bAction == ButtonAction.HELD) return Input.GetKey    ( keyboardKeyCode );
					if (bAction == ButtonAction.DOWN) return Input.GetKeyDown( keyboardKeyCode );
					if (bAction == ButtonAction.UP)   return Input.GetKeyUp  ( keyboardKeyCode );
				}

				return false;
			}

			//gamepad button checks
			if (inputType == InputDeviceType.GamepadButton || inputType == InputDeviceType.GamepadAxis){
				if (slot == InputDeviceSlot.keyboard || slot == InputDeviceSlot.mouse || slot == InputDeviceSlot.keyboardAndMouse) return false;

				//if checking any slot, call this function for each possible slot
				if (slot == InputDeviceSlot.any){
					return ButtonCheck(bAction, InputDeviceSlot.gamepad1) || ButtonCheck(bAction, InputDeviceSlot.gamepad2) || ButtonCheck(bAction, InputDeviceSlot.gamepad3) || ButtonCheck(bAction, InputDeviceSlot.gamepad4) || ButtonCheck(bAction, InputDeviceSlot.gamepad5) || ButtonCheck(bAction, InputDeviceSlot.gamepad6) || ButtonCheck(bAction, InputDeviceSlot.gamepad7) || ButtonCheck(bAction, InputDeviceSlot.gamepad7) || ButtonCheck(bAction, InputDeviceSlot.gamepad9) || ButtonCheck(bAction, InputDeviceSlot.gamepad10) || ButtonCheck(bAction, InputDeviceSlot.gamepad11) || ButtonCheck(bAction, InputDeviceSlot.gamepad12) || ButtonCheck(bAction, InputDeviceSlot.gamepad13) || ButtonCheck(bAction, InputDeviceSlot.gamepad14) || ButtonCheck(bAction, InputDeviceSlot.gamepad15) || ButtonCheck(bAction, InputDeviceSlot.gamepad16);
				}

				int slotIndex = ((int)slot)-1;

				//don't check slots without a connected gamepad
				if (Sinput.GetGamepads().Length<=slotIndex) return false;

				//make sure the gamepad in this slot is one this input is allowed to check (eg don't check PS4 pad bindings for an XBOX pad)
				bool allowInputFromThisPad=false;
				for (int i=0; i<allowedSlots.Length; i++){
					if (slotIndex == allowedSlots[i]) allowInputFromThisPad = true;
				}
				if (!allowInputFromThisPad) return false;

				//gamepad button check
				if (inputType == InputDeviceType.GamepadButton){
					//get the keycode for the gamepad's slot/button
					string buttonString = string.Format("Joystick{0}Button{1}", (slotIndex+1), gamepadButtonNumber);
					if (string.IsNullOrEmpty(buttonString)) return false;
					UnityGamepadKeyCode keyCode = (UnityGamepadKeyCode)System.Enum.Parse(typeof(UnityGamepadKeyCode), buttonString);

					//button check now
					if (bAction == ButtonAction.HELD) return Input.GetKey    ( (KeyCode)(int)keyCode );
					if (bAction == ButtonAction.DOWN) return Input.GetKeyDown( (KeyCode)(int)keyCode );
					if (bAction == ButtonAction.UP)   return Input.GetKeyUp  ( (KeyCode)(int)keyCode );
				}

				//gamepad axis as a button check
				if (inputType == InputDeviceType.GamepadAxis){
					if (bAction == axisButtonState[ slotIndex+1 ]) return true;
					if (bAction == ButtonAction.HELD && axisButtonState[ slotIndex+1 ] == ButtonAction.DOWN) return true;

					return false;
				}

				return false;
			}


			//virtual device input checks
			if (inputType == InputDeviceType.Virtual){
				if (bAction == virtualInputState) return true;
				if (bAction == ButtonAction.HELD && virtualInputState == ButtonAction.DOWN) return true;
			}

			//mouseaxis button checks (these don't happen)
			if (inputType == InputDeviceType.Mouse){
				if (slot != InputDeviceSlot.any && slot !=InputDeviceSlot.mouse && slot != InputDeviceSlot.keyboardAndMouse) return false;

				KeyCode mouseKeyCode = KeyCode.None;
				if (mouseInputType==MouseInputType.Mouse0) mouseKeyCode = KeyCode.Mouse0;
				if (mouseInputType==MouseInputType.Mouse1) mouseKeyCode = KeyCode.Mouse1;
				if (mouseInputType==MouseInputType.Mouse2) mouseKeyCode = KeyCode.Mouse2;
				if (mouseInputType==MouseInputType.Mouse3) mouseKeyCode = KeyCode.Mouse3;
				if (mouseInputType==MouseInputType.Mouse4) mouseKeyCode = KeyCode.Mouse4;
				if (mouseInputType==MouseInputType.Mouse5) mouseKeyCode = KeyCode.Mouse5;
				if (mouseInputType==MouseInputType.Mouse6) mouseKeyCode = KeyCode.Mouse6;

				if (mouseKeyCode != KeyCode.None){
					//clicky mouse input
					if (bAction == ButtonAction.HELD) return Input.GetKey    ( mouseKeyCode );
					if (bAction == ButtonAction.DOWN) return Input.GetKeyDown( mouseKeyCode );
					if (bAction == ButtonAction.UP)   return Input.GetKeyUp  ( mouseKeyCode );
				}else{
					//mouse axis as button input
					if (bAction == axisButtonState[0]) return true;
					if (bAction == ButtonAction.HELD && axisButtonState[0] == ButtonAction.DOWN) return true;
				}

				return false;
			}

			return false;
		}

		public float AxisCheck(InputDeviceSlot slot){

			//keyboard checks
			if (inputType == InputDeviceType.Keyboard){
				if (slot == InputDeviceSlot.any || slot == InputDeviceSlot.keyboard || slot == InputDeviceSlot.keyboardAndMouse){
					if (Input.GetKey( keyboardKeyCode )) return 1f;
				}

				return 0f;
			}

			//gamepad button and axis checks
			if (inputType == InputDeviceType.GamepadButton || inputType == InputDeviceType.GamepadAxis){
				if (slot == InputDeviceSlot.keyboard || slot == InputDeviceSlot.mouse || slot == InputDeviceSlot.keyboardAndMouse) return 0f;

				//if checking any slot, call this function for each possible slot
				if (slot == InputDeviceSlot.any){
					float greatestV = 0f;
					for (int i=1; i<=Sinput.GetGamepads().Length; i++){
						greatestV = Mathf.Max(greatestV, Mathf.Abs( AxisCheck((InputDeviceSlot)i) ));
					}
					return greatestV;
				}

				int slotIndex = ((int)slot)-1;



				//don't check slots without a connected gamepad
				if (Sinput.GetGamepads().Length<=slotIndex) return 0f;

				//make sure the gamepad in this slot is one this input is allowed to check (eg don't check PS4 pad bindings for an XBOX pad)
				bool allowInputFromThisPad=false;
				for (int i=0; i<allowedSlots.Length; i++){
					if (slotIndex == allowedSlots[i]) allowInputFromThisPad = true;
				}

				if (!allowInputFromThisPad) return 0f;

				//button as axis checks
				if (inputType == InputDeviceType.GamepadButton){
					string buttonString = string.Format("Joystick{0}Button{1}", (slotIndex+1), gamepadButtonNumber);
					if (string.IsNullOrEmpty(buttonString)) return 0f;
					UnityGamepadKeyCode keyCode = (UnityGamepadKeyCode)System.Enum.Parse(typeof(UnityGamepadKeyCode), buttonString);

					//button check now
					if (Input.GetKey( (KeyCode)(int)keyCode )) return 1f;
				}

				//gamepad axis check
				if (inputType == InputDeviceType.GamepadAxis){
					string axisString = string.Format("J_{0}_{1}", (slotIndex+1), gamepadAxisNumber);
					float axisValue = Input.GetAxisRaw(axisString);
					if (invertAxis) axisValue*=-1f;
					if (rescaleAxis){
						//some gamepad axis are -1 to 1 or something when you want them as 0 to 1, EG; triggers on XBONE pad on OSX
						axisValue = Mathf.InverseLerp(rescaleAxisMin, rescaleAxisMax, axisValue);
					}

					if (clampAxis) axisValue = Mathf.Clamp01(axisValue);

					//we return every axis' default value unless we measure a change first
					//this prevents weird snapping and false button presses if the pad is reporting a weird value to start with
					if (useDefaultAxisValue){
						if (measuredAxisValue!=-54.321f){
							if (axisValue!=measuredAxisValue) useDefaultAxisValue = false;
						}else{
							measuredAxisValue=axisValue;
						}
						if (useDefaultAxisValue) axisValue = defaultAxisValue;
					}

					return axisValue;
				}

				return 0f;
			}


			//virtual device axis input checks
			if (inputType == InputDeviceType.Virtual){
				return virtualAxisValue;
			}

			//mouseaxis button checks (these don't happen)
			if (inputType == InputDeviceType.Mouse){
				if (slot != InputDeviceSlot.any && slot != InputDeviceSlot.mouse && slot != InputDeviceSlot.keyboardAndMouse) return 0f;

				switch (mouseInputType){
				case MouseInputType.MouseHorizontal:
					return Input.GetAxisRaw("Mouse Horizontal");
				case MouseInputType.MouseVertical:
					return Input.GetAxisRaw("Mouse Vertical");
				case MouseInputType.MouseScroll:
					return Input.GetAxisRaw("Mouse Scroll");
				case MouseInputType.MousePositionX:
					return Input.mousePosition.x;
				case MouseInputType.MousePositionY:
					return Input.mousePosition.y;
				default:
					//it's a click type mouse input
					if (Input.GetKey( (KeyCode)(System.Enum.Parse(typeof(KeyCode),mouseInputType.ToString())) )) return 1f;
					break;
				}
				//return Input.GetAxisRaw(mouseAxis);
			}

			return 0f;

		}

		//called max once per frame
		public void UpdateAxisButtonStates(){

			if (inputType == InputDeviceType.GamepadAxis){
				float axisValue;
				bool held;
				for (int i=1; i<=Sinput.GetGamepads().Length; i++){
					axisValue = AxisCheck( (InputDeviceSlot)i );
					held = false;
					if (axisValue>axisButtoncompareVal) held = true;
					axisButtonState[i]= AxisButtonChange(axisButtonState[i], held);
				}
				return;
			}
			/*if (inputType == InputDeviceType.Keyboard){
					float axisValue = AxisCheck( InputDeviceSlot.keyboard );
					bool held = false;
					if (Mathf.Abs(axisValue)>0.5f) held = true;
					axisButtonState[0]= AxisButtonChange(axisButtonState[0], held);
					return;
				}*/
			if (inputType == InputDeviceType.Mouse){
				//ignore clicky inputs
				if (mouseInputType == MouseInputType.Mouse0 || mouseInputType == MouseInputType.Mouse1 || mouseInputType == MouseInputType.Mouse2 || mouseInputType == MouseInputType.Mouse3 || mouseInputType == MouseInputType.Mouse4 || mouseInputType == MouseInputType.Mouse5 || mouseInputType == MouseInputType.Mouse6) return;
				float axisValue = AxisCheck( InputDeviceSlot.mouse );
				bool held = false;
				if (Mathf.Abs(axisValue)>0.5f) held = true;
				axisButtonState[0]= AxisButtonChange(axisButtonState[0], held);

			}
		}
		public void ResetAxisButtonStates(){

			if (inputType == InputDeviceType.GamepadAxis){
				if (null==axisButtonState || axisButtonState.Length != Sinput.GetGamepads().Length+1){
					axisButtonState = new ButtonAction[Sinput.GetGamepads().Length+1];
				}

				for (int i=1; i<=Sinput.GetGamepads().Length; i++){
					axisButtonState[i]=ButtonAction.NOTHING;
				}
			}

			/*if (inputType == InputDeviceType.Keyboard){
					if (null==axisButtonState){
						axisButtonState = new ButtonAction[1];
					}
					axisButtonState[0]=ButtonAction.NOTHING;
				}*/

			if (inputType == InputDeviceType.Mouse){
				//ignore clicky inputs
				if (mouseInputType == MouseInputType.Mouse0 || mouseInputType == MouseInputType.Mouse1 || mouseInputType == MouseInputType.Mouse2 || mouseInputType == MouseInputType.Mouse3 || mouseInputType == MouseInputType.Mouse4 || mouseInputType == MouseInputType.Mouse5 || mouseInputType == MouseInputType.Mouse6) return;

				if (null==axisButtonState){
					axisButtonState = new ButtonAction[1];
				}
				axisButtonState[0]=ButtonAction.NOTHING;
			}
		}
		ButtonAction AxisButtonChange(ButtonAction fromState, bool buttonHeld){
			if (buttonHeld){
				switch (fromState){
				case ButtonAction.NOTHING: return ButtonAction.DOWN;
				case ButtonAction.UP: return ButtonAction.DOWN;
				case ButtonAction.DOWN: return ButtonAction.HELD;
				default: return ButtonAction.HELD;
				}
			}else{
				switch (fromState){
				case ButtonAction.NOTHING: return ButtonAction.NOTHING;
				case ButtonAction.UP: return ButtonAction.NOTHING;
				case ButtonAction.DOWN: return ButtonAction.UP;
				case ButtonAction.HELD: return ButtonAction.UP;
				default: return ButtonAction.NOTHING;
				}
			}
		}



		//should be able to allow 'virtual' inputs that can be set by script, allowing people to have say, touchscreen buttons or custom hardware still hook into SInput controls
	}
}
