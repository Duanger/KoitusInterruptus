using System.Collections;
using System.Collections.Generic;
using SinputSystems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityScript.Macros;

public class MenuUiInputHandler : MonoBehaviour
{

	public int CurrentMenuButtonIndex;
	[SerializeField] private EventSystem _uiEventSystem;
	[SerializeField] private Image[] _mainButts = new Image[3];
	private bool _triggered;
	private float _axes;
	private InputDeviceSlot AnySticks = InputDeviceSlot.any;
	void Start ()
	{
		Mathf.Clamp(CurrentMenuButtonIndex, 0, 2);
	}

	void Update ()
	{
		
		//Debug.Log(Sinput.GetAxis("Horizontal", _player1));
		_axes = Sinput.GetAxis("Horizontal", AnySticks);
	
		if (CurrentMenuButtonIndex < 0)
		{
			CurrentMenuButtonIndex = 2;
		}

		if (CurrentMenuButtonIndex > 2)
		{
			
			CurrentMenuButtonIndex = 0;
		}
		if(Sinput.GetButtonDownRepeating("Left", AnySticks))
		{
			CurrentMenuButtonIndex--;
		}
		if (Sinput.GetButtonDownRepeating("Right", AnySticks))
		{
			CurrentMenuButtonIndex++;
		}
		
		/*if (_axes == 0 && _triggered)
		{
			_triggered = false;
		}
		if (_axes > 0 && !_triggered)
		{
			_triggered = true;
			CurrentMenuButtonIndex++;
		}
		

		if (_axes < 0 && !_triggered)
		{
			_triggered = true;
			CurrentMenuButtonIndex--;
		}*/
        //var pointer = new PointerEventData(EventSystem.current);
		/*if (Sinput.GetButton("Submit", AnySticks) && CurrentMenuButtonIndex)
		{
			ExecuteEvents.Execute(_start.gameObject, pointer, ExecuteEvents.submitHandler);
		}*/

			switch (CurrentMenuButtonIndex)
			{
				case 0:
					_uiEventSystem.SetSelectedGameObject(_mainButts[0].gameObject);
					Debug.Log(_mainButts[0].ToString() + "is pressed");
					_mainButts[0].rectTransform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
					_mainButts[1].rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					_mainButts[2].rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					break;
				case 1:
					_uiEventSystem.SetSelectedGameObject(_mainButts[1].gameObject);
					Debug.Log(_mainButts[1].ToString() + "is pressed");
					_mainButts[1].rectTransform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
					_mainButts[0].rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					_mainButts[2].rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					break;
				case 2:
					_uiEventSystem.SetSelectedGameObject(_mainButts[2].gameObject);
					Debug.Log(_mainButts[2].ToString() + "is pressed");
					_mainButts[2].rectTransform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
					_mainButts[0].rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					_mainButts[1].rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					break;
			}
		
	}
}
