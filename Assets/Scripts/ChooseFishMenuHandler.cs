using System.Collections;
using System.Collections.Generic;
using SinputSystems;
using UnityEngine;
using UnityEngine.UI;

public class ChooseFishMenuHandler : MonoBehaviour
{
	private GameObject _start;
	private MenuUiInputHandler _menu;
	[SerializeField] private Vector3 _regular, _large;

	public Image[] OrangeFishImages, GreenFishImages = new Image[2];
	public int OrangeFishIndex;
	public int GreenFishIndex;
	void Start ()
	{
		_start = GameObject.FindGameObjectWithTag("StartMenu");
		_menu = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<MenuUiInputHandler>();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.activeSelf && !_start.activeSelf)
		{
			if (OrangeFishIndex > 1)
			{
				OrangeFishIndex = 0;
			}

			if (OrangeFishIndex < 0)
			{
				OrangeFishIndex = 1;
			}

			if (GreenFishIndex > 1)
			{
				GreenFishIndex = 0;
			}

			if (GreenFishIndex < 0)
			{
				GreenFishIndex = 1;
			}

			if (Sinput.GetButtonDownRepeating("Up", InputDeviceSlot.gamepad1))
			{
				OrangeFishIndex--;
			}

			if (Sinput.GetButtonDownRepeating("Down", InputDeviceSlot.gamepad1))
			{
				OrangeFishIndex++;
			}
			if (Sinput.GetButtonDownRepeating("Up", InputDeviceSlot.gamepad2))
			{
				GreenFishIndex--;
			}

			if (Sinput.GetButtonDownRepeating("Down", InputDeviceSlot.gamepad2))
			{
				GreenFishIndex++;
			}

			switch (OrangeFishIndex)
			{
					case 0:
						OrangeFishImages[0].rectTransform.localScale = _large;
						OrangeFishImages[1].rectTransform.localScale = _regular;
						break;
					case 1:
						OrangeFishImages[1].rectTransform.localScale = _large;
						OrangeFishImages[0].rectTransform.localScale = _regular;
						break;
			}
			switch (GreenFishIndex)
			{
				case 0:
					GreenFishImages[0].rectTransform.localScale = _large;
					GreenFishImages[1].rectTransform.localScale = _regular;
					break;
				case 1:
					GreenFishImages[1].rectTransform.localScale = _large;
					GreenFishImages[0].rectTransform.localScale = _regular;
					break;
			}
			_menu.InputPlayerHandler(OrangeFishIndex,GreenFishIndex,OrangeFishImages,GreenFishImages);
		}
	}
}
