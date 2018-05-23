using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinputSystems.Examples{
	public class MultiplayerSpawner : MonoBehaviour {

		public GameObject playerPrefab;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			SinputSystems.InputDeviceSlot slot = Sinput.ListenForSlotPress("Join");

			if (slot != SinputSystems.InputDeviceSlot.any){
				
				GameObject newPlayer = (GameObject)GameObject.Instantiate(playerPrefab);
				newPlayer.transform.position = new Vector3(Random.Range(-4f,4f), 3f, Random.Range(-4f,4f));

				newPlayer.GetComponent<MultiplayerPlayer>().playerSlot = slot;

			}
		}

	}
}
