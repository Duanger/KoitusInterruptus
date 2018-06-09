using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class UIEvents : MonoBehaviour
{

	public bool RunFirstTime;
	public delegate void SelectHandler();
    public delegate void StartingHandler();
	public delegate void EventSystemHandler();
	public delegate void DisableButtonHandler();
	public event EventSystemHandler OnSystemEnabled;
	public event SelectHandler OnButtonSelect;
	public event DisableButtonHandler OnButtonDisabled;
	void Awake () {
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			RunFirstTime = true;
		}
	}

	void Start()
	{
		if (RunFirstTime)
		{
			RunFirstTime = false;
		}
	}

	void Update()
	{
		if (OnSystemEnabled != null)
		{
			OnSystemEnabled();
     		Debug.Log("is enabled");
		}

		if (OnButtonSelect != null)
		{
			OnButtonSelect();
		}

		if (OnButtonDisabled != null)
		{
			OnButtonDisabled();
		}
	}
	
}
