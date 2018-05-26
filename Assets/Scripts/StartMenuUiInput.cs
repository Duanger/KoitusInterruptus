using System.Collections;
using System.Collections.Generic;
using SinputSystems;
using UnityEngine;

public class StartMenuUiInput : MonoBehaviour
{

	private MenuUiInputHandler _menuHandler;
	private InputDeviceSlot _playerOneSlot = InputDeviceSlot.gamepad1;
	private InputDeviceSlot _playerTwoSlot = InputDeviceSlot.gamepad2;
	void Start ()
	{
		_menuHandler = GetComponent<MenuUiInputHandler>();
		_menuHandler.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
