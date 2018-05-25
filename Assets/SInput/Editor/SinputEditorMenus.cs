using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SinputEditorMenus {
	
	[MenuItem("Sinput/Generate Unity Inputs")]
	static void GenerateInputSettings(){
		//Debug.Log("Doing Something...");
		//Debug.Log("Writing new Input Manager asset to \"ProjectSettings/InputManager.asset\"...");
	
		int joystickNumber = Sinput.MAXCONNECTEDGAMEPADS;
		int axisNumber = Sinput.MAXAXISPERGAMEPAD;

		string inputManagerAssetLocation = "ProjectSettings/InputManager.asset";

		System.IO.File.Delete(inputManagerAssetLocation);

		System.IO.StreamWriter sr = System.IO.File.CreateText(inputManagerAssetLocation);

		sr.WriteLine("%YAML 1.1");
		sr.WriteLine("%TAG !u! tag:unity3d.com,2011:");
		sr.WriteLine("--- !u!13 &1");
		sr.WriteLine("InputManager:");
		sr.WriteLine("  m_Axes:");

		//need input axis for all possible gamepad axis
		for (int j=1; j<=joystickNumber; j++){
			for (int a=1; a<=axisNumber; a++){

				sr.WriteLine("  - serializedVersion: 3");
				sr.WriteLine("    m_Name: J_" + j.ToString() + "_" + a.ToString());
				sr.WriteLine("    descriptiveName: ");
				sr.WriteLine("    descriptiveNegativeName: ");
				sr.WriteLine("    negativeButton: ");
				sr.WriteLine("    positiveButton: ");
				sr.WriteLine("    altNegativeButton: ");
				sr.WriteLine("    altPositiveButton: ");
				sr.WriteLine("    gravity: 0");
				sr.WriteLine("    dead: 0.19");
				sr.WriteLine("    sensitivity: 1");
				sr.WriteLine("    snap: 0");
				sr.WriteLine("    invert: 0");
				sr.WriteLine("    type: 2");
				sr.WriteLine("    axis: " + (a-1).ToString());
				sr.WriteLine("    joyNum: " + j.ToString());
			}
		}

		//need these for mouse inputs
		sr.WriteLine("  - serializedVersion: 3");
		sr.WriteLine("    m_Name: Mouse Horizontal");
		sr.WriteLine("    descriptiveName: ");
		sr.WriteLine("    descriptiveNegativeName: ");
		sr.WriteLine("    negativeButton: ");
		sr.WriteLine("    positiveButton: ");
		sr.WriteLine("    altNegativeButton: ");
		sr.WriteLine("    altPositiveButton: ");
		sr.WriteLine("    gravity: 1000");
		sr.WriteLine("    dead: 0");
		sr.WriteLine("    sensitivity: 1");
		sr.WriteLine("    snap: 0");
		sr.WriteLine("    invert: 0");
		sr.WriteLine("    type: 1");
		sr.WriteLine("    axis: 0");
		sr.WriteLine("    joyNum: 0");

		sr.WriteLine("  - serializedVersion: 3");
		sr.WriteLine("    m_Name: Mouse Vertical");
		sr.WriteLine("    descriptiveName: ");
		sr.WriteLine("    descriptiveNegativeName: ");
		sr.WriteLine("    negativeButton: ");
		sr.WriteLine("    positiveButton: ");
		sr.WriteLine("    altNegativeButton: ");
		sr.WriteLine("    altPositiveButton: ");
		sr.WriteLine("    gravity: 1000");
		sr.WriteLine("    dead: 0");
		sr.WriteLine("    sensitivity: 1");
		sr.WriteLine("    snap: 0");
		sr.WriteLine("    invert: 0");
		sr.WriteLine("    type: 1");
		sr.WriteLine("    axis: 1");
		sr.WriteLine("    joyNum: 0");

		sr.WriteLine("  - serializedVersion: 3");
		sr.WriteLine("    m_Name: Mouse Scroll");
		sr.WriteLine("    descriptiveName: ");
		sr.WriteLine("    descriptiveNegativeName: ");
		sr.WriteLine("    negativeButton: ");
		sr.WriteLine("    positiveButton: ");
		sr.WriteLine("    altNegativeButton: ");
		sr.WriteLine("    altPositiveButton: ");
		sr.WriteLine("    gravity: 1000");
		sr.WriteLine("    dead: 0");
		sr.WriteLine("    sensitivity: 1");
		sr.WriteLine("    snap: 0");
		sr.WriteLine("    invert: 0");
		sr.WriteLine("    type: 1");
		sr.WriteLine("    axis: 2");
		sr.WriteLine("    joyNum: 0");


		//and annoyingly, we need these so any StandaloneInputModule doesn't blow it's mind when it tries to use unity's crummy input system for UI events
		sr.WriteLine("  - serializedVersion: 3");
		sr.WriteLine("    m_Name: Horizontal");
		sr.WriteLine("    descriptiveName: ");
		sr.WriteLine("    descriptiveNegativeName: ");
		sr.WriteLine("    negativeButton: left");
		sr.WriteLine("    positiveButton: d");
		sr.WriteLine("    altNegativeButton: a");
		sr.WriteLine("    altPositiveButton: right");
		sr.WriteLine("    gravity: 1000");
		sr.WriteLine("    dead: 0.001");
		sr.WriteLine("    sensitivity: 1000");
		sr.WriteLine("    snap: 0");
		sr.WriteLine("    invert: 0");
		sr.WriteLine("    type: 0");
		sr.WriteLine("    axis: 0");
		sr.WriteLine("    joyNum: 0");

		sr.WriteLine("  - serializedVersion: 3");
		sr.WriteLine("    m_Name: Vertical");
		sr.WriteLine("    descriptiveName: ");
		sr.WriteLine("    descriptiveNegativeName: ");
		sr.WriteLine("    negativeButton: down");
		sr.WriteLine("    positiveButton: up");
		sr.WriteLine("    altNegativeButton: s");
		sr.WriteLine("    altPositiveButton: w");
		sr.WriteLine("    gravity: 1000");
		sr.WriteLine("    dead: 0.001");
		sr.WriteLine("    sensitivity: 1000");
		sr.WriteLine("    snap: 0");
		sr.WriteLine("    invert: 0");
		sr.WriteLine("    type: 0");
		sr.WriteLine("    axis: 0");
		sr.WriteLine("    joyNum: 0");

		sr.Close();

		AssetDatabase.Refresh();

		//System.IO.File.SetLastWriteTimeUtc(inputManagerAssetLocation, System.DateTime.UtcNow);


		EditorUtility.DisplayDialog("Sinput", "Input Manager settings have been generated.", "OK");



		//Debug.Log("...Done!");
		//Debug.Log("If you need to see the change in the input manager immediately, close & re-open unity now.");
	}

	[MenuItem("Sinput/Select Control Scheme")]
	static void SelectControlScheme(){
		//select main control scheme
		Object mainControlScheme = Resources.Load("MainControlScheme");
		if (mainControlScheme != null){
			Selection.activeObject = mainControlScheme;
			return;
		}

		//couldn't find control scheme, creating a new one
		Debug.LogError("Control scheme: \"MainControlScheme\" not found, attempting to create one.");
		SinputSystems.ControlScheme controlScheme = (SinputSystems.ControlScheme)ScriptableObject.CreateInstance(typeof(SinputSystems.ControlScheme));
		AssetDatabase.CreateAsset(controlScheme, "Assets/SInput/Resources/MainControlScheme.asset");

		//now select the new control scheme
		mainControlScheme = Resources.Load("MainControlScheme");
		if (mainControlScheme != null){
			Selection.activeObject = mainControlScheme;
			return;
		}
	}
}
