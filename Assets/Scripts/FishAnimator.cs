using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FishAnimator : MonoBehaviour
{

	[SerializeField] private bool _facingLeft, _facingRight;

	public enum FishType
	{
		greenBride,
		greenGroom,
		orangeBride,
		orangeGroom 
	};

	public FishType fishType;
	[SerializeField]private RuntimeAnimatorController[] _fishAnimController = new RuntimeAnimatorController[4];
	[SerializeField] private Sprite[] _defaultSprite = new Sprite[4];
	private Animator _fishController;
	[SerializeField] private int _enumIndex;
	private float _zRot;
	private Rigidbody2D _rigidbody;
	private String _fishTypeString;

	void Start ()
	{
		
		if (fishType == FishType.greenBride)
		{
			_enumIndex = 0;
		}

		if (fishType == FishType.greenGroom)
		{
			_enumIndex = 1;
		}

		if (fishType == FishType.orangeBride)
		{
			_enumIndex = 2;
		}

		if (UnbreakableManager.OrangeFeesh == UnbreakableManager.OrangeFishChosen.OrangeGroomChosen && GetComponent<SinputPlayerController>().PlayerNumber == 1)
		{
			_enumIndex = 3;
			Debug.Log(UnbreakableManager.OrangeFeesh);
		}
		_fishTypeString = fishType.ToString();
		_facingLeft = true;
		GetComponent<SpriteRenderer>().sprite = _defaultSprite[_enumIndex];
		_fishController = GetComponent<Animator>();
		_rigidbody = GetComponent<Rigidbody2D>();
		
	}

	void Update ()
	{
		AnimController(_enumIndex,_fishTypeString);
	}

	void AnimController(int i,String fish)
	{
		_fishController.runtimeAnimatorController = _fishAnimController[i];
			if (_rigidbody.velocity.x < 0)
			{
				if (!_facingLeft && !_facingRight)
				{
					_fishController.Play(_fishTypeString);
				}
				_facingLeft = true;
				if (_facingLeft && _facingRight)
				{
					_fishController.Play(fish+"_turnaround-again");
					_facingRight = false;
					_fishController.SetBool("turnRight",false);
					_fishController.SetBool("turnLeft",true);
				}
			}
			if (_rigidbody.velocity.x > 0)
			{
				_facingRight = true;
				if (_facingRight && _facingLeft)
				{
					_fishController.Play(fish+"_turnaround");
					_facingLeft = false;
					_fishController.SetBool("turnLeft",false);
					_fishController.SetBool("turnRight",true);
				}
			}
			if (_rigidbody.velocity.x == 0)
			{
				_fishController.speed = 0;
			}
			if (_rigidbody.velocity.x != 0)
			{
				_fishController.speed = Mathf.Abs(_rigidbody.velocity.x /3f);
			}

		if (_facingLeft)
		{
			_rigidbody.AddTorque(-_rigidbody.velocity.y,ForceMode2D.Impulse);
		}

		if (_facingRight)
		{
			_rigidbody.AddTorque(_rigidbody.velocity.y,ForceMode2D.Impulse);
		}
	}
}
