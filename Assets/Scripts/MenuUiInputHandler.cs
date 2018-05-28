using System;
using System.Collections;
using System.Collections.Generic;
using SinputSystems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityScript.Macros;

public  class MenuUiInputHandler : MonoBehaviour
{

	private static String[] _axeStrings = new string[4];
	
	private InputDeviceSlot AnySticks = InputDeviceSlot.any;
	private InputDeviceSlot _player1 = InputDeviceSlot.gamepad1;
	private InputDeviceSlot _player2 = InputDeviceSlot.gamepad2;

	void Start()
	{
		_axeStrings[0] = "Left";
		_axeStrings[1] = "Right";
		_axeStrings[2] = "Up";
		_axeStrings[3] = "Down";
	}

	public void InputPlayerHandler(int switchy, int switchy2, Image[] leftHandImage, Image[] rightHandImage)
	{
		foreach (var image in leftHandImage)
		{
			if (image == leftHandImage[switchy])
			{
				ExecuteEvents.Execute(leftHandImage[switchy].gameObject, new BaseEventData(EventSystem.current),
					ExecuteEvents.selectHandler);
			}
			if (image != leftHandImage[switchy])
			{
				ExecuteEvents.Execute(image.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.deselectHandler);
			}
			if (image == leftHandImage[switchy] && Sinput.GetButtonDown("Submit", _player1))
			{
				ExecuteEvents.Execute(image.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
			}
		}
		
		foreach (var image in rightHandImage)
		{
			if (image == rightHandImage[switchy2])
			{
				ExecuteEvents.Execute(rightHandImage[switchy2].gameObject, new BaseEventData(EventSystem.current),
					ExecuteEvents.selectHandler);
			}
			if (image != rightHandImage[switchy2])
			{
				ExecuteEvents.Execute(image.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.deselectHandler);
			}
			if (image == rightHandImage[switchy2] && Sinput.GetButtonDown("Submit", _player2))
			{
				ExecuteEvents.Execute(image.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
			}
		}
	}
	public void InputPlayerHandler(int switchy, Image[] buttonImage)
	{
			foreach (var b in buttonImage)
			{
				//EventSystem.current.SetSelectedGameObject(b.gameObject);
				ExecuteEvents.Execute(b.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.selectHandler);
				if (b != buttonImage[switchy])
				{
					ExecuteEvents.Execute(b.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.deselectHandler);
				}
				if (b == buttonImage[switchy] && Sinput.GetButtonDown("Submit", AnySticks))
				{
					ExecuteEvents.Execute(b.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
					Debug.Log(b.ToString());
				}
			}
	}
}
