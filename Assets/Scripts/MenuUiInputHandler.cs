using System;
using System.Collections;
using System.Collections.Generic;
using SinputSystems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityScript.Macros;

public class MenuUiInputHandler : MonoBehaviour
{

	public int CurrentMenuButtonIndex;
	[SerializeField] private Color _highlightedColor;
	[SerializeField] private EventSystem _uiEventSystem;
	[SerializeField] private Image[] _mainButts = new Image[3];
	private bool _triggered;
	private float _axes;
	[SerializeField] private String[] _axeStrings = new string[4];
	[SerializeField] private Vector3 _smallSize, _highlightedSize;
	private InputDeviceSlot AnySticks = InputDeviceSlot.any;
	private InputDeviceSlot _player1 = InputDeviceSlot.gamepad1;
	private InputDeviceSlot _player2 = InputDeviceSlot.gamepad2;

	public void InputPlayerHandler(int switchy, int switchy2, Image[] leftHandImage, Image[] rightHandImage)
	{
		Dictionary<int,int> buttonGridPos = new Dictionary<int, int>();
		if (Sinput.GetButtonDownRepeating(_axeStrings[0], _player1))
		{
			switchy--;
		}

		if (Sinput.GetButtonDownRepeating(_axeStrings[1], _player1))
		{
			switchy++;
		}
		if (Sinput.GetButtonDownRepeating(_axeStrings[2], _player2))
		{
			switchy2--;
		}

		if (Sinput.GetButtonDownRepeating(_axeStrings[3], _player2))
		{
			switchy2++;
		}

		ExecuteEvents.Execute(leftHandImage[switchy].gameObject, new BaseEventData(EventSystem.current),
			ExecuteEvents.selectHandler);
		foreach (var image in leftHandImage)
		{
			if (image != leftHandImage[switchy])
			{
				ExecuteEvents.Execute(image.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.deselectHandler);
			}
		}
		ExecuteEvents.Execute(rightHandImage[switchy2].gameObject, new BaseEventData(EventSystem.current),
			ExecuteEvents.selectHandler);
		foreach (var image in rightHandImage)
		{
			if (image != rightHandImage[switchy2])
			{
				ExecuteEvents.Execute(image.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.deselectHandler);
			}
		}
		foreach (var b in leftHandImage)
		{
			if (switchy == System.Array.IndexOf(leftHandImage, b) && Sinput.GetButtonDown("Submit", _player1))
			{
				ExecuteEvents.Execute(b.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
			}
		}

		foreach (var c in rightHandImage)
		{
			if (switchy == System.Array.IndexOf(leftHandImage, c) && Sinput.GetButtonDown("Submit", _player2))
			{
				ExecuteEvents.Execute(c.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
			}
		}

	}
	public void InputPlayerHandler(bool anyPlayer,int switchy, Image[] buttonImage)
	{
		
		
	        if (Sinput.GetButtonDownRepeating(_axeStrings[0], AnySticks))
			{
			    switchy--;
			}

			if (Sinput.GetButtonDownRepeating(_axeStrings[1], AnySticks))
			{
					switchy++;
			}
				EventSystem.current.SetSelectedGameObject(buttonImage[switchy].gameObject);
			
			foreach (var b in buttonImage)
			{
				if (switchy == System.Array.IndexOf(buttonImage, b) && Sinput.GetButtonDown("Submit", AnySticks))
				{
					ExecuteEvents.Execute(b.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
				}
			}
	}
		
			

		

			/*switch (CurrentMenuButtonIndex)
			{
				case 0:
					_uiEventSystem.SetSelectedGameObject(_mainButts[0].gameObject);
					_mainButts[0].rectTransform.localScale = _highlightedSize;
					_mainButts[1].rectTransform.localScale = _smallSize;
					_mainButts[2].rectTransform.localScale = _smallSize;
					_mainButts[0].color = _highlightedColor;
					_mainButts[1].color = Color.white;
					_mainButts[2].color = Color.white;
					break;
				case 1:
					_uiEventSystem.SetSelectedGameObject(_mainButts[1].gameObject);
					_mainButts[1].rectTransform.localScale = _highlightedSize;
					_mainButts[0].rectTransform.localScale = _smallSize;
					_mainButts[2].rectTransform.localScale = _smallSize;
					_mainButts[1].color = _highlightedColor;
					_mainButts[0].color = Color.white;
					_mainButts[2].color = Color.white;
					break;
				case 2:
					_uiEventSystem.SetSelectedGameObject(_mainButts[2].gameObject);
					_mainButts[2].rectTransform.localScale = _highlightedSize;
					_mainButts[0].rectTransform.localScale = _smallSize;
					_mainButts[1].rectTransform.localScale = _smallSize;
					_mainButts[2].color = _highlightedColor;
					_mainButts[0].color = Color.white;
					_mainButts[1].color = Color.white;
					break;
			}	*/	
	
}
