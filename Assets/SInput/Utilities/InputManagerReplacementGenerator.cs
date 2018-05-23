using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinputSystems{
public class InputManagerReplacementGenerator : MonoBehaviour {


	//generates a new InputManager asset which will REPLACE the existing one
	//use this in the editor only
	//might take a little bit for unity to notice the change, close & reopen the editor if you must


	void Start () {

		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.LinuxEditor) return;

		Debug.Log("Writing new Input Manager asset to \"ProjectSettings/InputManager.asset\"...");
		GenerateInputManagerData(Sinput.MAXCONNECTEDGAMEPADS, Sinput.MAXAXISPERGAMEPAD);
		Debug.Log("...Done!");
	}
	
	public void GenerateInputManagerData(int joystickNumber, int axisNumber){

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
	}
}
}