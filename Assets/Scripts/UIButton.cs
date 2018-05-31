using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class UIButton : MonoBehaviour, ISelectHandler,IDeselectHandler
{
	public int ButtonIndex;
	private Animator _anim;
	[SerializeField] private bool _isFishButton;
	private bool _testBool;
	private GameObject _herChoice;
	[SerializeField] private bool _testTwoControllers;
	private String _fishy;

	private void Start()
	{
		
		
		if (_isFishButton)
		{
			_anim = GetComponentInChildren<Animator>();
			_anim.enabled = false;
		}
		if (gameObject.name == "Start")
		{
			_herChoice = GameObject.FindGameObjectWithTag("ChooseFish");
			_herChoice.SetActive(false);
		}

		if (ButtonIndex == 1)
		{
			_fishy = "orangeBride";
			//ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
		}
		if (ButtonIndex == 2)
		{
			_fishy = "orangeGroom";
		}
		if (ButtonIndex == 3)
		{
			_fishy = "greenBride";
			//ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
		}

		if (ButtonIndex == 4)
		{
			_fishy = "greenGroom";
		}
	}

	public void PressedStart()
	{
		_herChoice.SetActive(true);
		transform.parent.gameObject.SetActive(false);
		//UnbreakableManager.Feesh
	}

	public void OnSelect(BaseEventData eventData)
	{
		if(_isFishButton)
		_anim.enabled = true;
		//_testBool = true;

	}

	public void OnDeselect(BaseEventData eventData)
	{
		if(_isFishButton)
		_anim.enabled = false;
		/*if (_isFishButton)
		{
			_anim.enabled = false;
		}*/
	}
	
	private void Update()
	{
		if (_testTwoControllers)
		{
			SceneManager.LoadScene(1);
		}

		
	}

	// Update is called once per frame
	public void ChooseAFish()
	{
		if (UnbreakableManager.OrangeFeesh == UnbreakableManager.OrangeFishChosen.Undecided)
		{
			if (ButtonIndex == 1)
			{
				
				
				UnbreakableManager.OrangeFeesh = UnbreakableManager.OrangeFishChosen.OrangeBrideChosen;
			}

			if (ButtonIndex == 2)
			{
				UnbreakableManager.OrangeFeesh = UnbreakableManager.OrangeFishChosen.OrangeGroomChosen;
			}
		}

		if (UnbreakableManager.GreenFeesh == UnbreakableManager.GreenFishChosen.Undecided)
		{
			if (ButtonIndex == 3)
			{
				UnbreakableManager.GreenFeesh = UnbreakableManager.GreenFishChosen.GreenBrideChosen;
			}

			if (ButtonIndex == 4)
			{
				UnbreakableManager.GreenFeesh = UnbreakableManager.GreenFishChosen.GreenGroomChosen;
			}
		}	
	}
}