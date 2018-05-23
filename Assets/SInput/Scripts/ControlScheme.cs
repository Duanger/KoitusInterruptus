using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinputSystems{
	[CreateAssetMenu]
	public class ControlScheme : ScriptableObject {

		public List<ControlSetup> controls = new List<ControlSetup>();
		public List<SmartControlSetup> smartControls = new List<SmartControlSetup>();


		[System.Serializable]
		public struct ControlSetup{
			public string name;// = "New Control";
			public List<KeyboardInputType> keyboardInputs;// = new List<InputSetup>();
			public List<CommonGamepadInputs> gamepadInputs;
			public List<MouseInputType> mouseInputs;
			public List<string> virtualInputs;
		}
		[System.Serializable]
		public struct SmartControlSetup{
			public string name;// = "New Mix Control";
			public string positiveControl;
			public string negativeControl;

			public float deadzone;//=0.001f;

			public float gravity;//=3;
			public float speed;//=3;
			public bool snap;//=true;

			public float scale;// =1f;
		}

	
	}

}