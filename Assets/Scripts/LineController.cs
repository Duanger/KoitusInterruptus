using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
	[SerializeField] private Transform _fishPos1, _fishPos2;
	public Rigidbody2D[] Points = new Rigidbody2D[10];
	private LineRenderer _line;
	private SpringJoint2D _spring;
	void Start ()
	{
		_line = GetComponent<LineRenderer>();
		//_spring = GetComponent<SpringJoint2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{	
		/*for (int i = 0; i < Points.Length; i++)
		{
			_line.SetPosition(i,Points[i].position);
		}*/
	}
}
