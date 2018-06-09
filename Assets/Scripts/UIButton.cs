using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIButton : MonoBehaviour
{
	public int ButtonIndex;
	[SerializeField]private Animator _anim;
	private Animator _imAnim;
	private CameraFollow _cm;
	[SerializeField] private bool _isFishButton;
	private GameObject _anyEventSystem, _eventSystem1, _eventSystem2;
	private bool _testBool;
	private bool _orangeBool, _greenBool;
	[SerializeField]private GameObject _herChoice;
	[SerializeField] private bool _testTwoControllers;
	private float _currentTimeOrange;
	private float _currentTimeGreen;
	[SerializeField] private float _oLength, _gLength;
	private Image _im;
	private Image _iman;
	private String _fishy;

	private void Start()
	{
		_cm = Camera.main.gameObject.GetComponent<CameraFollow>();
		if (_isFishButton)
		{
			_im = GetComponent<Image>();
			_iman = GetComponentInChildren<Image>();
			_imAnim = GetComponent<Animator>();
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
		_cm.Started = true;
		_anyEventSystem.SetActive(false);
		_eventSystem1.SetActive(true);
		_eventSystem2.SetActive(true);
		transform.parent.gameObject.SetActive(false);
		//UnbreakableManager.Feesh
	}

	public void Selectedd()
	{
		_anim.SetBool("highlighted",true);
		//_testBool = true;

	}

	public void Deselected()
	{
		_anim.SetBool("highlighted",false);
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
		if (ButtonIndex == 1 && _im.canvasRenderer.GetAlpha() == 0|| ButtonIndex == 2 && _im.canvasRenderer.GetAlpha() == 0)
		{
			_orangeBool = true;
		}
		if (ButtonIndex == 3 && _im.canvasRenderer.GetAlpha() == 0|| ButtonIndex == 4 && _im.canvasRenderer.GetAlpha() == 0)
		{
			_greenBool = true;
		}

		if (_orangeBool && ButtonIndex > 0)
		{
			Vector2 targetPosition = new Vector2(363, 126);
			Vector2 targetSize = new Vector2(1.4f,1.627907f);
			_currentTimeOrange += Time.deltaTime;
			if (_currentTimeOrange >= _oLength)
			{
				_currentTimeOrange = _oLength;
			}

			float p = _currentTimeOrange / _oLength;
			_iman.rectTransform.localScale= Vector2.Lerp(_iman.rectTransform.localScale, targetSize, p);
			_im.rectTransform.anchoredPosition = Vector2.Lerp(_im.rectTransform.anchoredPosition,targetPosition,p);
			if (_im.rectTransform.anchoredPosition == targetPosition)
			{
				GetComponent<Button>().interactable = false;
			}
		}
		if (_greenBool && ButtonIndex > 0)
		{
			Vector2 targetPosition = new Vector2(363, 126);
			Vector2 targetSize = new Vector2(1.4f,1.627907f);
			_currentTimeGreen += Time.deltaTime;
			if (_currentTimeOrange >= _gLength)
			{
				_currentTimeGreen= _gLength;
			}

			float d = _currentTimeGreen / _gLength;
			_iman.rectTransform.localScale = Vector2.Lerp(_iman.rectTransform.localScale, targetSize, d);
			_im.rectTransform.anchoredPosition = Vector2.Lerp(_im.rectTransform.anchoredPosition,targetPosition,d);
			
			if (_im.rectTransform.anchoredPosition == targetPosition)
			{
				GetComponent<Button>().interactable = false;
			}
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
				_anim.enabled = false;
				_imAnim.enabled = false;
				_im.CrossFadeAlpha(0f,0.7f,false);
			}

			if (ButtonIndex == 2)
			{
				UnbreakableManager.OrangeFeesh = UnbreakableManager.OrangeFishChosen.OrangeGroomChosen;
				_anim.enabled = false;
				_imAnim.enabled = false;
				_im.CrossFadeAlpha(0f,0.7f,false);
			}
		}

		if (UnbreakableManager.GreenFeesh == UnbreakableManager.GreenFishChosen.Undecided)
		{
			if (ButtonIndex == 3)
			{
				UnbreakableManager.GreenFeesh = UnbreakableManager.GreenFishChosen.GreenBrideChosen;
				_anim.enabled = false;
				_imAnim.enabled = false;
				_im.CrossFadeAlpha(0f,0.7f,false);	
			}

			if (ButtonIndex == 4)
			{
				UnbreakableManager.GreenFeesh = UnbreakableManager.GreenFishChosen.GreenGroomChosen;
				_anim.enabled = false;
				_imAnim.enabled = false;
				_im.CrossFadeAlpha(0f,0.7f,false);
			}
		}	
	}
}