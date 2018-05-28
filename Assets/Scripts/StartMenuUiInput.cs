using System.Collections;
using System.Collections.Generic;
using SinputSystems;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuUiInput : MonoBehaviour
{
	[SerializeField] private Color _highlightedColor;
	private MenuUiInputHandler _menu;
	private GameObject _herChoice;
     public  Image[] MainStartButtons = new Image[3];
	[SerializeField] private Vector3 _smallSize, _highlightedSize;

	public int StartMenuIndex;
	// Update is called once per frame
	void Start()
	{
		_menu = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<MenuUiInputHandler>();
	}
	void Update () {
		if (StartMenuIndex > 2)
		{
			StartMenuIndex = 0;
		}
		else if (StartMenuIndex < 0)
		{
			StartMenuIndex = 2;
		}
		if (Sinput.GetButtonDownRepeating("Up", InputDeviceSlot.any))
		{
			StartMenuIndex--;
		}

		if (Sinput.GetButtonDownRepeating("Down", InputDeviceSlot.any))
		{
			StartMenuIndex++;
		}
		switch (StartMenuIndex)
		{
			case 0:
				/*MainStartButtons[0].rectTransform.localScale = _highlightedSize;
				MainStartButtons[1].rectTransform.localScale = _smallSize;
				MainStartButtons[2].rectTransform.localScale = _smallSize;*/
				MainStartButtons[0].color = _highlightedColor;
				MainStartButtons[1].color = Color.white;
				MainStartButtons[2].color = Color.white;
				break;
			case 1:
				MainStartButtons[1].rectTransform.localScale = _highlightedSize;
				MainStartButtons[0].rectTransform.localScale = _smallSize;
				MainStartButtons[2].rectTransform.localScale = _smallSize;
				MainStartButtons[1].color = _highlightedColor;
				MainStartButtons[0].color = Color.white;
				MainStartButtons[2].color = Color.white;
				break;
			case 2:
				MainStartButtons[2].rectTransform.localScale = _highlightedSize;
				MainStartButtons[0].rectTransform.localScale = _smallSize;
				MainStartButtons[1].rectTransform.localScale = _smallSize;
				MainStartButtons[2].color = _highlightedColor;
				MainStartButtons[0].color = Color.white;
				MainStartButtons[1].color = Color.white;
				break;
		}
		_menu.InputPlayerHandler(StartMenuIndex,MainStartButtons);
		
	}
}
