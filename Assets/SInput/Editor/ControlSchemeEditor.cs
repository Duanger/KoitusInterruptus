using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using SinputSystems;

[CustomEditor(typeof(ControlScheme))]
public class ControlSchemeEditor : Editor {

	int currentPanel = 0;

	List<bool> controlFoldouts = new List<bool>();
	List<bool> smartControlFoldouts = new List<bool>();

	public override void OnInspectorGUI(){

		ControlScheme controlScheme = (ControlScheme)target;
		EditorGUI.BeginChangeCheck();



		string[] strs = new string[]{"Controls","Smart Controls"};
		currentPanel = GUILayout.Toolbar(currentPanel, strs);
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

		if (currentPanel==0){
			//show controls list

			if (controlFoldouts.Count != controlScheme.controls.Count){
				controlFoldouts = new List<bool>();
				for (int i=0; i<controlScheme.controls.Count; i++) controlFoldouts.Add(false);
			}

			ControlScheme.ControlSetup activeControl = new ControlScheme.ControlSetup();
			for (int i=0; i<controlScheme.controls.Count; i++){
				
				controlFoldouts[i] = EditorGUILayout.Foldout(controlFoldouts[i],controlScheme.controls[i].name);

				//EditorGUILayout.LabelField("");
				bool deleteControl = false;

				if (controlFoldouts[i]){
					activeControl = controlScheme.controls[i];

					EditorGUILayout.BeginHorizontal();
					activeControl.name = EditorGUILayout.TextField("Control Name",activeControl.name);

					if (GUILayout.Button("X")) deleteControl = true;
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Space();

					//keyboard inputs
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.BeginVertical();
					//column a
					EditorGUILayout.LabelField("Keyboard Inputs:");

					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
					//column b
					for (int k=0; k<activeControl.keyboardInputs.Count; k++){
						EditorGUILayout.BeginHorizontal();

						activeControl.keyboardInputs[k] = (KeyboardInputType)EditorGUILayout.EnumPopup(activeControl.keyboardInputs[k]);
						if (GUILayout.Button("x")){
							activeControl.keyboardInputs.RemoveAt(k);
							k--;
						}
						EditorGUILayout.EndHorizontal();
					}
					if (GUILayout.Button("+")){
						KeyboardInputType newKeycode = KeyboardInputType.None;
						activeControl.keyboardInputs.Add(newKeycode);
					}

					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Space();


					//gamepad inputs
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.BeginVertical();
					//column a
					EditorGUILayout.LabelField("Gamepad Inputs:");

					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
					//column b
					for (int k=0; k<activeControl.gamepadInputs.Count; k++){
						EditorGUILayout.BeginHorizontal();

						activeControl.gamepadInputs[k] = (CommonGamepadInputs)EditorGUILayout.EnumPopup(activeControl.gamepadInputs[k]);
						if (GUILayout.Button("x")){
							activeControl.gamepadInputs.RemoveAt(k);
							k--;
						}
						EditorGUILayout.EndHorizontal();

					}
					if (GUILayout.Button("+")){
						CommonGamepadInputs newGamepadInput = CommonGamepadInputs.NOBUTTON;
						activeControl.gamepadInputs.Add(newGamepadInput);
					}

					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Space();


					//mouse inputs
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.BeginVertical();
					//column a
					EditorGUILayout.LabelField("Mouse Inputs:");

					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
					//column b
					for (int k=0; k<activeControl.mouseInputs.Count; k++){
						EditorGUILayout.BeginHorizontal();

						activeControl.mouseInputs[k] = (MouseInputType)EditorGUILayout.EnumPopup(activeControl.mouseInputs[k]);
						if (GUILayout.Button("x")){
							activeControl.mouseInputs.RemoveAt(k);
							k--;
						}
						EditorGUILayout.EndHorizontal();

					}
					if (GUILayout.Button("+")){
						activeControl.mouseInputs.Add(MouseInputType.Mouse0);
					}

					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Space();

					//virtual inputs
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.BeginVertical();
					//column a
					EditorGUILayout.LabelField("Virtual Inputs:");

					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
					//column b
					for (int k=0; k<activeControl.virtualInputs.Count; k++){
						EditorGUILayout.BeginHorizontal();
						activeControl.virtualInputs[k] = EditorGUILayout.TextField(activeControl.virtualInputs[k]);
						if (GUILayout.Button("x")){
							activeControl.virtualInputs.RemoveAt(k);
							k--;
						}
						EditorGUILayout.EndHorizontal();
					}

					if (GUILayout.Button("+")){
						activeControl.virtualInputs.Add("");
					}

					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();


					controlScheme.controls[i] = activeControl;

					EditorGUILayout.Space();
					EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

				}
				if (deleteControl){
					controlScheme.controls.RemoveAt(i);
					controlFoldouts.RemoveAt(i);
					//controlKeyboardInputNames.RemoveAt(i);
					i--;
				}


			}
			if (GUILayout.Button("+")){
				ControlScheme.ControlSetup newControl = new ControlScheme.ControlSetup();
				newControl.name = "New Control";
				newControl.keyboardInputs = new List<KeyboardInputType>();
				newControl.gamepadInputs = new List<CommonGamepadInputs>();
				newControl.mouseInputs = new List<MouseInputType>();
				newControl.virtualInputs = new List<string>();
				controlScheme.controls.Add(newControl);
				controlFoldouts.Add(true);
				//controlKeyboardInputNames.Add("");
			}

		}

		if (currentPanel==1){
			//show smart controls list
			if (smartControlFoldouts.Count != controlScheme.smartControls.Count){
				smartControlFoldouts = new List<bool>();
				for (int i=0; i<controlScheme.smartControls.Count; i++) smartControlFoldouts.Add(false);
			}

			ControlScheme.SmartControlSetup activeSmartControl = new ControlScheme.SmartControlSetup();
			for (int i=0; i<controlScheme.smartControls.Count; i++){
				EditorGUILayout.BeginHorizontal();
				smartControlFoldouts[i] = EditorGUILayout.Foldout(smartControlFoldouts[i],controlScheme.smartControls[i].name);

				EditorGUILayout.LabelField("");
				bool deleteControl = false;
				if (GUILayout.Button("X")) deleteControl = true;
				EditorGUILayout.EndHorizontal();

				activeSmartControl = controlScheme.smartControls[i];

				if (smartControlFoldouts[i]){
					activeSmartControl.name = EditorGUILayout.TextField("Name", activeSmartControl.name);

					activeSmartControl.positiveControl = EditorGUILayout.TextField("Positive Control", activeSmartControl.positiveControl);
					activeSmartControl.negativeControl = EditorGUILayout.TextField("Negative Control", activeSmartControl.negativeControl);

					activeSmartControl.deadzone = EditorGUILayout.FloatField("Deadzone", activeSmartControl.deadzone);
					activeSmartControl.deadzone = Mathf.Clamp(activeSmartControl.deadzone, 0f, 1f);

					activeSmartControl.gravity = EditorGUILayout.FloatField("Gravity", activeSmartControl.gravity);
					activeSmartControl.gravity = Mathf.Clamp(activeSmartControl.gravity, 0f, float.MaxValue);

					activeSmartControl.speed = EditorGUILayout.FloatField("Speed", activeSmartControl.speed);
					activeSmartControl.speed = Mathf.Clamp(activeSmartControl.speed, 0f, float.MaxValue);

					activeSmartControl.snap = EditorGUILayout.Toggle("Snap", activeSmartControl.snap);
					activeSmartControl.scale = EditorGUILayout.FloatField("Scale", activeSmartControl.scale);

					EditorGUILayout.Space();

				}


				controlScheme.smartControls[i] = activeSmartControl;
				if (deleteControl){
					controlScheme.smartControls.RemoveAt(i);
					smartControlFoldouts.RemoveAt(i);
					i--;
				}


			}


			if (GUILayout.Button("+")){
				ControlScheme.SmartControlSetup newSmartControl = new ControlScheme.SmartControlSetup();
				newSmartControl.name = "New Control";
				newSmartControl.positiveControl = "";
				newSmartControl.negativeControl = "";

				newSmartControl.deadzone = 0.001f;
				newSmartControl.gravity = 3f;
				newSmartControl.speed = 3f;
				newSmartControl.snap = false;
				newSmartControl.scale = 1f;

				controlScheme.smartControls.Add(newSmartControl);
				smartControlFoldouts.Add(true);
			}
		}

		if (EditorGUI.EndChangeCheck()){
			//something was changed
			EditorUtility.SetDirty(controlScheme);
		}

	}
}
