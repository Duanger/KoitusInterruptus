using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
	private bool _doOnceInUpdate;
	[SerializeField] private bool _isBag;
	private float _currentLerpTime;
	private float _bagCurrentTime;
	[SerializeField] private float _timeToLerp;
	[SerializeField] private GameObject _fish1,_fish2;
	[SerializeField] private GameObject _cvas;
	[SerializeField] private Vector3 _targetVector;
	
	public Image Bagginton;   
    public Vector3 _bagTargetVector;


	public bool Started;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (_isBag)
		{
			if (Started)
			{
				_currentLerpTime += Time.deltaTime;  
				if (_currentLerpTime >= _timeToLerp)   
				{                                      
					_currentLerpTime = _timeToLerp;    
				}

				float fractal = _currentLerpTime / _timeToLerp;
				transform.position = Vector3.Lerp(transform.position,_targetVector,fractal);
				if (transform.position == _targetVector)
				{
					_bagCurrentTime += Time.deltaTime;
					if (_bagCurrentTime >= _timeToLerp*5f)
					{
						_bagCurrentTime = _timeToLerp * 5f;
					}

					float myGoat = _bagCurrentTime / (_timeToLerp * 5f);
					Bagginton.rectTransform.anchoredPosition = Vector3.Lerp(Bagginton.rectTransform.anchoredPosition, _bagTargetVector, myGoat);
					if (Bagginton.rectTransform.anchoredPosition.y < -442)
					{
	                  _cvas.SetActive(true);
					}
				}
			}
		}
		
		if (!_isBag)
		{
			Vector3 targetPos = new Vector3((_fish1.transform.position.x + _fish2.transform.position.x)/2
				,(_fish1.transform.position.y + _fish2.transform.position.y)/2,-10);
			transform.position = Vector3.Lerp(transform.position, targetPos, 0.6f);
		}
	}
}
