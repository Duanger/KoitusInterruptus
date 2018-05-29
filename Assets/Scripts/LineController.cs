using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
	[SerializeField] private float _SnapDist;
	[SerializeField] private Collider2D _fishColl, _fishColl2;
	public Rigidbody2D[] Points = new Rigidbody2D[14];
	private LineRenderer _line;
	private SpringJoint2D _spring;
	void Start ()
	{
		_line = GetComponent<LineRenderer>();
		Physics2D.IgnoreLayerCollision(8,9);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (Vector2.Distance(_fishColl.transform.position, _fishColl2.transform.position) < _SnapDist)
		{
			for (int i = 0; i < Points.Length; i++)
			{
				_line.SetPosition(i,Points[i].position);
			}

		}
		
		if (Vector2.Distance(_fishColl.transform.position, _fishColl2.transform.position) > _SnapDist)
		{
			_line.positionCount = 6;
			for (int i = 0; i < 11; i++)
			{
				_line.SetPosition(i,Points[i].position);
			}
			
		}
	}
}
