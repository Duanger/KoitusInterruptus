using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using SinputSystems;

[CustomEditor(typeof(CommonBinding))]
public class CommonBindingEditor : Editor {

	int currentPanel = 0;

	List<bool> axisEditFoldouts = new List<bool>();

	public override void OnInspectorGUI(){
		
		CommonBinding padBinding = (CommonBinding)target;
		EditorGUI.BeginChangeCheck();

		string[] strs = new string[]{"Gamepad","Buttons","Axis"};
		currentPanel = GUILayout.Toolbar(currentPanel, strs);
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

		/*if (GUILayout.Button("clearmatchingslots ~ " +padBinding.matchingGamepadSlots.Count.ToString())){
			padBinding.matchingGamepadSlots = new List<int>();
		}*/

		if (currentPanel==0){
			//Gamepad general menu
			padBinding.os = (OSFamily)EditorGUILayout.EnumPopup("Operating System:", padBinding.os);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			EditorGUILayout.PrefixLabel("Gamepad Names:");
			EditorGUILayout.LabelField("Which gamepads will this mapping apply to?");
			EditorGUILayout.LabelField("(Case insensitive, but needs to match what unity detects)");


			for (int i=0; i<padBinding.names.Count; i++){
				EditorGUILayout.BeginHorizontal();
				padBinding.names[i] = EditorGUILayout.TextField(padBinding.names[i]);
				if (GUILayout.Button("x")){
					//remove gamepad name
					padBinding.names.RemoveAt(i);
				}
				EditorGUILayout.EndHorizontal();
			}
			if (GUILayout.Button("+")){
				//add gamepad name here
				padBinding.names.Add("GAMEPAD_NAME_HERE");
			}
		}

		if (currentPanel==1){
			//button binding menu
			if (padBinding.buttons.Count>0){
				//EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Button Type / Button ID / Display Name");
				//EditorGUILayout.EndHorizontal();
			}

			CommonBinding.GamepadButtonInput activeButton = new CommonBinding.GamepadButtonInput();
			for (int i=0; i<padBinding.buttons.Count; i++){
				EditorGUILayout.BeginHorizontal();
				activeButton = padBinding.buttons[i];
				activeButton.buttonType = (CommonGamepadInputs)EditorGUILayout.EnumPopup(padBinding.buttons[i].buttonType);
				activeButton.buttonNumber = EditorGUILayout.IntField( padBinding.buttons[i].buttonNumber);
				activeButton.displayName = EditorGUILayout.TextField( activeButton.displayName);
				padBinding.buttons[i] = activeButton;
				if (GUILayout.Button("x")){
					//remove button
					padBinding.buttons.RemoveAt(i);
				}
				EditorGUILayout.EndHorizontal();
			}
			if (GUILayout.Button("+")){
				//add button binding name here
				CommonBinding.GamepadButtonInput newButtonInput = new CommonBinding.GamepadButtonInput();
				newButtonInput.buttonType = CommonGamepadInputs.NOBUTTON;
				newButtonInput.displayName = "[?]";
				newButtonInput.buttonNumber = 0;
				padBinding.buttons.Add(newButtonInput);
			}

		}

		if (currentPanel==2){
			//axis binding menu
			if (axisEditFoldouts.Count != padBinding.axis.Count){
				axisEditFoldouts = new List<bool>();
				for (int i=0; i<padBinding.axis.Count; i++) axisEditFoldouts.Add(false);
			}

			CommonBinding.GamepadAxisInput activeAxis = new CommonBinding.GamepadAxisInput();
			bool delete = false;
			for (int i=0; i<padBinding.axis.Count; i++){
				axisEditFoldouts[i] = EditorGUILayout.Foldout(axisEditFoldouts[i],padBinding.axis[i].buttonType.ToString());
				if (axisEditFoldouts[i]){
					delete = false;
					activeAxis = padBinding.axis[i];
					EditorGUILayout.BeginHorizontal();
					activeAxis.buttonType = (CommonGamepadInputs)EditorGUILayout.EnumPopup(activeAxis.buttonType);
					if (GUILayout.Button("x")) delete = true;
					EditorGUILayout.EndHorizontal();

					activeAxis.axisNumber = EditorGUILayout.IntField("Axis ID", activeAxis.axisNumber);

					activeAxis.displayName = EditorGUILayout.TextField("Display name", activeAxis.displayName);
					EditorGUILayout.Space();
					activeAxis.defaultVal=EditorGUILayout.FloatField("Default Value", activeAxis.defaultVal);
					activeAxis.invert = EditorGUILayout.Toggle("Invert", activeAxis.invert);
					activeAxis.clamp = EditorGUILayout.Toggle("Clamp to [0-1]", activeAxis.clamp);

					if (!activeAxis.rescaleAxis){
						activeAxis.rescaleAxis=EditorGUILayout.Toggle("Rescale to [0-1]", activeAxis.rescaleAxis);
					}else{
						EditorGUILayout.BeginHorizontal();
						activeAxis.rescaleAxis=EditorGUILayout.Toggle("Rescale to [0-1]", activeAxis.rescaleAxis);
						activeAxis.rescaleAxisMin=EditorGUILayout.FloatField(activeAxis.rescaleAxisMin);
						activeAxis.rescaleAxisMax=EditorGUILayout.FloatField(activeAxis.rescaleAxisMax);

						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.Space();

					//EditorGUILayout.LabelField("Counts as pressed button when:");
					EditorGUILayout.BeginHorizontal();
					string compareStr = "Pressed if >";
					if (!activeAxis.compareGreater) compareStr = "Pressed if <";
					if (GUILayout.Button(compareStr)) activeAxis.compareGreater = !activeAxis.compareGreater;
					activeAxis.compareVal=EditorGUILayout.FloatField(activeAxis.compareVal);

					EditorGUILayout.EndHorizontal();

					padBinding.axis[i] = activeAxis;

					if (delete){
						//remove axis
						padBinding.axis.RemoveAt(i);
						axisEditFoldouts.RemoveAt(i);
						i--;
					}
					EditorGUILayout.Space();
				}

				//EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			}
			EditorGUILayout.Space();
			if (GUILayout.Button("+")){
				//add button binding name here
				CommonBinding.GamepadAxisInput newAxisInput = new CommonBinding.GamepadAxisInput();
				newAxisInput.buttonType = CommonGamepadInputs.NOBUTTON;

				newAxisInput.axisNumber = 1;
				newAxisInput.invert = false;
				newAxisInput.clamp = false; //applied AFTER invert, to keep input result between 0 and 1

				//for using the axis as a button
				newAxisInput.compareGreater=true;//true is ([axisVal]>compareVal), false is ([axisVal]<compareVal)
				newAxisInput.compareVal=0.4f;//how var does have to go to count as "pressed" as a button

				newAxisInput.rescaleAxis=false;
				newAxisInput.rescaleAxisMin=0f;
				newAxisInput.rescaleAxisMax=1f;

				newAxisInput.defaultVal = 0f; //all GetAxis() checks will return default value until a measured change occurs, since readings before then can be wrong


				newAxisInput.displayName = "[?]";

				padBinding.axis.Add(newAxisInput);
				axisEditFoldouts.Add(true);
			}

		}


		if (EditorGUI.EndChangeCheck()){
			//something was changed
			EditorUtility.SetDirty(padBinding);
		}

	}
}
