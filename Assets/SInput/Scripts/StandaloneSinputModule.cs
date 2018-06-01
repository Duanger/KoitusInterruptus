using System;
using System.Collections;
using System.Collections.Generic;
using SinputSystems;
using UnityEngine;
//using UnityEngine.EventSystems;

namespace UnityEngine.EventSystems {
	public class StandaloneSinputModule : StandaloneInputModule {

		

		//[SerializeField]
		public string m_SinputUpButton = "Up";

		//[SerializeField]
		public string m_SinputDownButton = "Down";

		//[SerializeField]
		public string m_SinputLeftButton = "Left";

		//[SerializeField]
		public string m_SinputRightButton = "Right";

		//[SerializeField]
		public string m_SinputSubmitButton = "Submit";

		//[SerializeField]
		public string m_SinputCancelButton = "Cancel";

		public string m_InputSlotter = "any";

		private InputDeviceSlot m_InputDeviceSlot;

		public override void Process() {
			bool usedEvent = SendUpdateEventToSelectedObject();

			m_InputDeviceSlot = (InputDeviceSlot) Enum.Parse(typeof(InputDeviceSlot), m_InputSlotter);

			if (eventSystem.sendNavigationEvents) {
				if (!usedEvent)
					usedEvent |= SendMoveEventToSelectedObject();

				if (!usedEvent) {
					//SendSubmitEventToSelectedObject();
					if (SendSubmitEventToSelectedObject()) Sinput.ResetInputs();
				}
			}

			ProcessMouseEvent();
		}

		public override bool ShouldActivateModule() {
			//Debug.Log("happens");
			if (!base.ShouldActivateModule())
				return false;

			var shouldActivate = Sinput.GetButtonDown(m_SinputSubmitButton,m_InputDeviceSlot);
			shouldActivate |= Sinput.GetButtonDown(m_SinputCancelButton,m_InputDeviceSlot);
			shouldActivate |= Sinput.GetButtonDownRepeating(m_SinputUpButton,m_InputDeviceSlot);
			shouldActivate |= Sinput.GetButtonDownRepeating(m_SinputDownButton,m_InputDeviceSlot);
			shouldActivate |= Sinput.GetButtonDownRepeating(m_SinputLeftButton,m_InputDeviceSlot);
			shouldActivate |= Sinput.GetButtonDownRepeating(m_SinputRightButton,m_InputDeviceSlot);

			shouldActivate |= (m_MousePos - m_LastMousePos).sqrMagnitude > 0.0f;
			shouldActivate |= Input.GetMouseButtonDown(0);
			return shouldActivate;
		}


		private new bool SendSubmitEventToSelectedObject() {
			if (eventSystem.currentSelectedGameObject == null)
				return false;

			var data = GetBaseEventData();
			if (Sinput.GetButtonDown(m_SinputSubmitButton))
				ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

			if (Sinput.GetButtonDown(m_SinputCancelButton))
				ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
			return data.used;
		}

		private new bool SendMoveEventToSelectedObject() {

			Vector2 movement = GetRawMoveVector();

			var axisEventData = GetAxisEventData(movement.x, movement.y, 0.4f);
			if (!Mathf.Approximately(axisEventData.moveVector.x, 0f) || !Mathf.Approximately(axisEventData.moveVector.y, 0f)) {

				//Debug.Log("Move " + movement.ToString());
				ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
			}

			return axisEventData.used;
		}




		private Vector2 GetRawMoveVector() {
			Vector2 move = Vector2.zero;
			if (Sinput.GetButtonDownRepeating(m_SinputUpButton,m_InputDeviceSlot)) move.y += 1f;
			if (Sinput.GetButtonDownRepeating(m_SinputDownButton,m_InputDeviceSlot)) move.y -= 1f;
			if (Sinput.GetButtonDownRepeating(m_SinputLeftButton,m_InputDeviceSlot)) move.x -= 1f;
			if (Sinput.GetButtonDownRepeating(m_SinputRightButton,m_InputDeviceSlot)) move.x += 1f;

			return move;
		}


		private Vector2 m_LastMousePos;
		private Vector2 m_MousePos;
		public override void UpdateModule() {
			m_LastMousePos = m_MousePos;
			m_MousePos = Input.mousePosition;
		}
	}

}