using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
	[SerializeField]private bool _snapped;
	private float _timers;
	[SerializeField] private float _snapDist;
	[SerializeField] private float _snapBackTime;
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
		bool runOncePlease = false;
		float currentDistance = 0;
		for (int i = 2; i < Points.Length - 1; i++)
		{
			currentDistance = Points[1].position.magnitude - Points[i].position.magnitude;
		}
		currentDistance = Mathf.Abs(currentDistance);
		Debug.Log(currentDistance);
		//Vector2.Distance(_fishColl.transform.position, _fishColl2.transform.position);
		if (!_snapped)
		{
			runOncePlease = false;
			for (int i = 0; i < Points.Length; i++)
			{
				if (currentDistance < _snapDist && !_snapped)
				{
					_line.SetPosition(i, Points[i].position);

				}
			}
		}
	
		/*if(currentDistance > _snapDist)
			{
				if (!_snapped)
				{
					for(int i = 1; i < Points.Length -1; i++)
					{
						if (!runOncePlease)
						{
							_line.widthMultiplier -= 0.05f;
							_timers = 0;
							Points[i].gameObject.SetActive(false);
							runOncePlease = true;
							_snapped = true;
							//Score--;
							//_line.width -= 0.05;
						}
					}
				}
				Killme();
			}*/
	}

	void Killme()
	{
		if (_snapped)
		{
			Points[2].transform.parent.position = (Points[0].position - Points[13].position) / 2;
			_line.positionCount = 2;
			_line.SetPosition(0,Points[0].position);
			_line.SetPosition(1,Points[13].position);
			Points[0].GetComponent<SpringJoint2D>().enabled = false;
			Points[13].gameObject.GetComponent<SpringJoint2D>().connectedBody = Points[0];
			Points[13].gameObject.GetComponent<SpringJoint2D>().autoConfigureDistance = false;
			Points[13].gameObject.GetComponent<SpringJoint2D>().distance = 5;

		}
		_timers += Time.deltaTime;
		if (_timers > _snapBackTime)
		{
			foreach (var lambda in Points)
			{
				lambda.gameObject.SetActive(true);
			}
			_line.positionCount = 14;
			Points[13].gameObject.GetComponent<SpringJoint2D>().connectedBody = Points[12];
			Points[13].gameObject.GetComponent<SpringJoint2D>().autoConfigureDistance = true;
			_snapped = false;
		}
	}

}
