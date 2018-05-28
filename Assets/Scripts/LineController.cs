using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
	[SerializeField] private Collider2D _fishColl, _fishColl2;
	public Rigidbody2D[] Points = new Rigidbody2D[10];
	private LineRenderer _line;
	private SpringJoint2D _spring;
	void Start ()
	{
		_line = GetComponent<LineRenderer>();
		for (int i = 1; i < Points.Length - 1; i++)
		{
			Physics2D.IgnoreCollision(_fishColl,Points[i].GetComponent<Collider2D>());
			Physics2D.IgnoreCollision(_fishColl2,Points[i].GetComponent<Collider2D>());
		}
		//_spring = GetComponent<SpringJoint2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{	
		
	
	}
}
