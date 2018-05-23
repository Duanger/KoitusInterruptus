using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinputPlayerController : MonoBehaviour
{
	public int PlayerNumber;
	
	private Animator _animController;
	[SerializeField] private bool _forceAdded;
	private float _zRot;
	private Rigidbody2D _fishRigidbody;
	private Vector2 _playerAxis;
    private SinputSystems.InputDeviceSlot _playerSlot1 = SinputSystems.InputDeviceSlot.gamepad1;
	private SinputSystems.InputDeviceSlot _playerSlot2 = SinputSystems.InputDeviceSlot.gamepad2;

	void Start ()
	{
		_fishRigidbody = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		if (PlayerNumber == 1)
		{
			_playerAxis = Sinput.GetVector("Horizontal", "Vertical", _playerSlot1) * Time.deltaTime * 50f;
		}

		if (PlayerNumber == 2)
		{
			_playerAxis = Sinput.GetVector("Horizontal", "Vertical", _playerSlot2) * Time.deltaTime * 50f;
		}
		if (!_forceAdded)
		{
			_forceAdded = true;
			_fishRigidbody.AddForce(_playerAxis,ForceMode2D.Impulse);
			_forceAdded = false;
		}
		ClampingVelocity(_zRot);
	}

	void ClampingVelocity(float thotty)
	{
		if (_fishRigidbody.velocity.x < 0)
		{
			if ( _fishRigidbody.velocity.y < 0.5f &&  _fishRigidbody.velocity.y > 0)
			{
				thotty = 0f;
				Mathf.Clamp(thotty, 0, 0);
				transform.rotation = Quaternion.Euler(0,0,thotty);
			}
			if ( _fishRigidbody.velocity.y > 0.5f)
			{
				thotty = -45f;
				Mathf.Clamp(thotty, -90, 0);
				transform.rotation = Quaternion.Euler(0,0,thotty);
			}
			if ( _fishRigidbody.velocity.y > -0.5f &&  _fishRigidbody.velocity.y < 0)
			{
				thotty = 0f;
				Mathf.Clamp(thotty, 0, 0);
				transform.rotation = Quaternion.Euler(0,0,thotty);
			}

			if ( _fishRigidbody.velocity.y < -0.5f)
			{
				thotty = 45f;
				Mathf.Clamp(thotty, 0, 90);
				transform.rotation = Quaternion.Euler(0,0,thotty);
			}
		}

		if (_fishRigidbody.velocity.x > 0)
		{
			if ( _fishRigidbody.velocity.y < 0.5f &&  _fishRigidbody.velocity.y > 0)
			{
				thotty = 0f;
				Mathf.Clamp(thotty, 0, 0);
				transform.rotation = Quaternion.Euler(0,0,thotty);
			}
			if ( _fishRigidbody.velocity.y > 0.5f)
			{
				thotty = 45f;
				Mathf.Clamp(thotty, -90, 0);
				transform.rotation = Quaternion.Euler(0,0,thotty);
			}
			if ( _fishRigidbody.velocity.y > -0.5f &&  _fishRigidbody.velocity.y < 0)
			{
				thotty = 0f;
				Mathf.Clamp(thotty, 0, 0);
				transform.rotation = Quaternion.Euler(0,0,thotty);
			}

			if ( _fishRigidbody.velocity.y < -0.5f)
			{
				thotty = -45f;
				Mathf.Clamp(thotty, 0, 90);
				transform.rotation = Quaternion.Euler(0,0,thotty);
			}
		}
	}
}
