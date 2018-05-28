using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectOurBodies : MonoBehaviour
{
	private SpringJoint2D _s;
	[SerializeField] private Rigidbody2D _parentRigid, _childRigid;
	void Start ()
	{
		_s = GetComponent<SpringJoint2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		_s.connectedAnchor = _childRigid.transform.position;
		
	}
}
