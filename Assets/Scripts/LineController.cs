using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
	[SerializeField] private Transform _fishPos1, _fishPos2;
	public Rigidbody2D[] Joints = new Rigidbody2D[16];
	private LineRenderer _line;
	private SpringJoint2D _spring;
	void Start ()
	{
		_line = GetComponent<LineRenderer>();
		_spring = GetComponent<SpringJoint2D>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		_spring.anchor = _fishPos1.position;
		_spring.connectedAnchor = _fishPos2.position;
		_line.SetPosition(0,_fishPos1.position);
		for (int i = 1; i < Joints.Length-1; i++)
		{
			_line.SetPosition(i,Joints[i].position);
		}
		_line.SetPosition(15,_fishPos2.position);
	}
}
