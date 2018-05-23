using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinputSystems{

	public class SmartControl{
		//InputControls combine various inputs, and can behave as buttons or 1-dimensional axis
		//SmartControls combine various InputControls or other SmartControls, and can have a bunch of extra behaviour like normal InputManager smoothing
		//These won't be exposed to players when rebinding because they are built on other controls (and it'd be a headache to present anyway)

		public string name;
		public string displayName;

		//control constructor
		public SmartControl(string controlName){
			name = controlName;
		}

		//values for each slot's input
		private float[] rawValues;
		private float[] controlValues;

		public string positiveControl;
		public string negativeControl;


		public float deadzone=0.001f; //clip values less than this

		public float gravity=3; //how quickly the value shifts to zero
		public float speed=3; //how quickly does the value shift towards it's target
		public bool snap=false;//if value is negative and input is positive, snaps to zero

		public float scale =1f;

		public void Init(){
			//prepare to record values for all gamepads AND keyboard & mouse inputs
			int possibleInputDeviceCount = System.Enum.GetValues(typeof(InputDeviceSlot)).Length;
			rawValues = new float[possibleInputDeviceCount];
			controlValues = new float[possibleInputDeviceCount];
			for (int i=0; i<controlValues.Length; i++){
				rawValues[i] = 0f;
				controlValues[i] = 0f;
			}
		}

		private int lastUpdateFrame = -10;
		public void Update(){
			if (Time.frameCount == lastUpdateFrame) return;
			lastUpdateFrame = Time.frameCount;

			float posVal = 0f;
			float negVal = 0f;
			for (int slot=0; slot<rawValues.Length; slot++){
				//get values of positive/negative controls
				posVal = Sinput.GetAxis(positiveControl, (InputDeviceSlot)slot);
				//}
				negVal = Sinput.GetAxis(negativeControl, (InputDeviceSlot)slot);

				//float targetVal = posVal-negVal;
				rawValues[slot] = posVal-negVal;
			}

			for (int slot=0; slot<controlValues.Length; slot++){
				//shift to zero
				if (gravity>0f){
					if (rawValues[slot]==0f || (rawValues[slot]<controlValues[slot] && controlValues[slot]>0f) || (rawValues[slot]>controlValues[slot] && controlValues[slot]<0f)){
						if (controlValues[slot]>0f){
							controlValues[slot] -= gravity * Time.deltaTime;
							if (controlValues[slot]<0f) controlValues[slot] = 0f;
							if (controlValues[slot]<rawValues[slot]) controlValues[slot] = rawValues[slot];
						}else if (controlValues[slot]<0f){
							controlValues[slot] += gravity * Time.deltaTime;
							if (controlValues[slot]>0f) controlValues[slot] = 0f;
							if (controlValues[slot]>rawValues[slot]) controlValues[slot] = rawValues[slot];
						}
					}
				}

				//snapping
				if (snap){
					if (rawValues[slot]>0f && controlValues[slot]<0f) controlValues[slot] = 0f;
					if (rawValues[slot]<0f && controlValues[slot]>0f) controlValues[slot] = 0f;
				}

				//move value towards target value
				if (rawValues[slot]<0f){
					if (controlValues[slot]>rawValues[slot]){
						controlValues[slot] -= speed * Time.deltaTime;
						if (controlValues[slot]<rawValues[slot]) controlValues[slot] = rawValues[slot];
					}
				}
				if (rawValues[slot]>0f){
					if (controlValues[slot]<rawValues[slot]){
						controlValues[slot] += speed * Time.deltaTime;
						if (controlValues[slot]>rawValues[slot]) controlValues[slot] = rawValues[slot];
					}
				}
			}

		}

		//return current value
		public float GetValue(){ return GetValue(InputDeviceSlot.any); }
		public float GetValue(InputDeviceSlot slot){
			Update();
			if ((int)slot>=controlValues.Length) return 0f;

			if (Mathf.Abs(controlValues[(int)slot]) < deadzone) return 0f;
			return controlValues[(int)slot]*scale;
		}

		//button check
		public bool ButtonCheck(ButtonAction bAction){ return ButtonCheck(bAction, InputDeviceSlot.any); }
		public bool ButtonCheck(ButtonAction bAction, InputDeviceSlot slot){
			if (bAction == ButtonAction.DOWN && Sinput.GetButtonDown(positiveControl, slot)) return true;
			if (bAction == ButtonAction.DOWN && Sinput.GetButtonDown(negativeControl, slot)) return true;
			if (bAction == ButtonAction.HELD && Sinput.GetButton(positiveControl, slot)) return true;
			if (bAction == ButtonAction.HELD && Sinput.GetButton(negativeControl, slot)) return true;
			if (bAction == ButtonAction.UP && Sinput.GetButtonUp(positiveControl, slot)) return true;
			if (bAction == ButtonAction.UP && Sinput.GetButtonUp(negativeControl, slot)) return true;
			return false;
		}

	}

}
