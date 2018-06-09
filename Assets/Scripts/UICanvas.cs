using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UICanvas: MonoBehaviour {

    private enum _playerInput
    {
      Any,
      Each
    };

	[SerializeField] private GameObject[] _eventSystem = new GameObject[3];
	[SerializeField] private UIEvents _uiHandler;
	[SerializeField] private _playerInput _kindOfPlayerInput;

	private GameObject _chuuse;
	
	void Start () {
		if (_uiHandler != null)
		{
			if (_uiHandler.RunFirstTime)
			{
				_eventSystem[0].SetActive(true);
				_eventSystem[1].SetActive(false);
				_eventSystem[2].SetActive(false);
				_chuuse = GameObject.FindGameObjectWithTag("ChooseFish");
				_chuuse.SetActive(false);
			}
			else
			{
				_uiHandler.OnSystemEnabled += DisableOtherEventSystems;
				Debug.Log("got me disabled");
			}
		}
      
	}
	
	// Update is called once per frame
	public void StartCalled ()
	{
		LoadFishChoosing();
	}

	void LoadFishChoosing()
	{
		_chuuse.SetActive(true);
		_kindOfPlayerInput = _playerInput.Each;
			gameObject.SetActive(false);
	}
	void DisableOtherEventSystems()
	{
       if(_kindOfPlayerInput == _playerInput.Any)
       {
        foreach (var obj in _eventSystem) 
        {
	        if (obj.CompareTag("Any"))
	        {
		        obj.SetActive(true);
	        }
        	if(!obj.CompareTag("Any"))
        	{ 
        		obj.SetActive(false);
        	}
        }
       }
       
       if(_kindOfPlayerInput == _playerInput.Each)
       {
       	foreach (var obj in _eventSystem) 
       	{
		    if (!obj.CompareTag("Any"))
		    {
			  obj.SetActive(true);
			    Debug.Log("why Not");
		    }
       		if(obj.CompareTag("Any"))
       		{
              obj.SetActive(false);
       		}
       	}
       }
		_uiHandler.OnSystemEnabled -= DisableOtherEventSystems;
   }
}
       
	

