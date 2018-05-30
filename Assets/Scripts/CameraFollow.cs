using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

	[SerializeField] private GameObject _fish1,_fish2;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 targetPos = new Vector3((_fish1.transform.position.x + _fish2.transform.position.x)/2
			,(_fish1.transform.position.y + _fish2.transform.position.y)/2,-10);
		transform.position = Vector3.Lerp(transform.position, targetPos, 0.6f);

	}
}
