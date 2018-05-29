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
		float currentDistance = Vector2.Distance(_fishColl.transform.position, _fishColl2.transform.position);
		foreach (var b in Points)
		{
			if (currentDistance < _SnapDist)
			{
			
				_line.SetPosition(System.Array.IndexOf(Points,b),b.position);
			
			}
			else 
			{
			
				if (b == Points[0])
				{
					b.gameObject.GetComponent<SpringJoint2D>().enabled = false;
					b.gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
				}

				if (b == Points[13])
				{
					b.gameObject.GetComponent<SpringJoint2D>().enabled = true;
					b.gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
					b.gameObject.GetComponent<SpringJoint2D>().connectedBody = Points[0];
					b.gameObject.GetComponent<SpringJoint2D>().autoConfigureDistance = false;
					b.gameObject.GetComponent<SpringJoint2D>().distance = 5;
				}
				else if(b != Points[0] || b!= Points[13])
				{
					b.gameObject.GetComponent<SpringJoint2D>().enabled = false;
					b.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
				}
			
				_line.positionCount = 0;
			}
		}	
	}
}
