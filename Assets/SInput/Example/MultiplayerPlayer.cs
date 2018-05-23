using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinputSystems.Examples{
	public class MultiplayerPlayer : MonoBehaviour {

		//which input device controls this player
		public SinputSystems.InputDeviceSlot playerSlot = SinputSystems.InputDeviceSlot.any;

		//lets display which input slot we are using, just for kicks
		public TextMesh playerSlotDisplay;

		//stuff we need for our platforming code
		private CharacterController characterController;
		private float yMotion = 0f;

		// Use this for initialization
		void Start () {
			characterController = transform.GetComponent<CharacterController>();
			//set the player a random colour
			transform.GetComponent<Renderer>().material.color = new Color(Random.Range(0.5f,1f),Random.Range(0.5f,1f),Random.Range(0.5f,1f),1f);
			playerSlotDisplay.text = "Input:\n" + playerSlot.ToString();
		}
		
		// Update is called once per frame
		void Update () {

			//get player input for motion
			Vector3 motionInput = Sinput.GetVector("Horizontal", "", "Vertical", playerSlot);

			//we want to move like, three times as much as this
			motionInput *= 3f;

			//gravity
			yMotion -= Time.deltaTime * 10f;
			motionInput.y = yMotion;

			//move our character controller now
			characterController.Move(motionInput * Time.deltaTime);

			if (characterController.isGrounded){
				yMotion = -0.05f;

				if (Sinput.GetButtonDown("Jump", playerSlot)) yMotion = 5f;
			}
		}
	}

}