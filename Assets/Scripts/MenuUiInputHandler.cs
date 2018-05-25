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
	[SerializeField] private Image _start, _input, _exit;
	private bool _triggered;
	private float player1Sinput, player2Sinput;
	private InputDeviceSlot _player1 = InputDeviceSlot.gamepad1, _player2 = InputDeviceSlot.gamepad2;
	//public Button[] _menuButtons = new Button[3];
	void Start () {
		
	}

	void FixedUpdate ()
	{
		
		//Debug.Log(Sinput.GetAxis("Horizontal", _player1));
		player1Sinput = Sinput.GetAxis("Horizontal", _player1);

		if (CurrentMenuButtonIndex < 0)
		{
			CurrentMenuButtonIndex = 2;
		}

		if (CurrentMenuButtonIndex > 2)
		{
			
			CurrentMenuButtonIndex = 0;
		}
		player2Sinput = Sinput.GetAxis("Horizontal", _player2);
		if (player2Sinput == 0 && _triggered)
		{
			_triggered = false;
		}
		if (player2Sinput > 0 && !_triggered)
		{
			_triggered = true;
			CurrentMenuButtonIndex++;
		}

		if (player2Sinput < 0 && !_triggered)
		{
			_triggered = true;
			CurrentMenuButtonIndex--;
		}
var pointer = new PointerEventData(EventSystem.current);
		if (Sinput.GetButton("Submit", _player2) && CurrentMenuButtonIndex == 0)
		{
			ExecuteEvents.Execute(_start.gameObject, pointer, ExecuteEvents.submitHandler);
		}

			switch (CurrentMenuButtonIndex)
			{
				case 0:
					_uiEventSystem.SetSelectedGameObject(_start.gameObject);
					_start.rectTransform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
					_input.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					_exit.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					break;
				case 1:
					_uiEventSystem.SetSelectedGameObject(_input.gameObject);
					_input.rectTransform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
					_start.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					_exit.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					break;
				case 2:
					_uiEventSystem.SetSelectedGameObject(_exit.gameObject);
					_exit.rectTransform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
					_start.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					_input.rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					break;
			}
		
	}
}
