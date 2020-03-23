using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace oneShot
{
	public class CursorLayerController : MonoBehaviour
	{
		public string horizontalAxis;
		public string verticalAxis;
		public float speed;
		private Layer currentLayer;

		public float LayerMoveCooldown;
		private float LayerMoveTimer;
		public GameObject prefab;
		// Start is called before the first frame update
		void Start()
		{
			currentLayer = LayersController.instance.GetLayer(LayersController.instance.GetLayerIndexByHeight(transform.position.y));
		}

		// Update is called once per frame
		void Update()
		{
			//HORIZONTAL MOVEMENT
			transform.Translate(transform.right * Input.GetAxis(horizontalAxis) * (speed * Time.deltaTime));

			//VERTICAL MOVEMENT
			if (Input.GetAxisRaw(verticalAxis) != 0 && LayerMoveTimer <= 0)
			{
				Vector3 direction = Vector3.up;
				if (Input.GetAxis(verticalAxis) < 0)
					direction = -direction;
				int layerMask = 1 << 8;
				RaycastHit hit;
				if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
				{
					//Layer1 nextLayer = LayersController.instance.GetLayer(currentLayer.index + direction);
					Layer nextLayer = hit.collider.GetComponentInParent<Layer>();
					if (nextLayer)
					{
						currentLayer = nextLayer;
						ChangeLayer();
						LayerMoveTimer = LayerMoveCooldown;
					}
				}
			}

			if (LayerMoveTimer > 0)
				LayerMoveTimer -= Time.deltaTime;

			//PLACEHOLDER TP POINT
			if (Input.GetKeyDown(KeyCode.Space))
				Instantiate(prefab, transform.position + prefab.transform.position, Quaternion.identity);
		}

		private void ChangeLayer()
		{
			Vector3 newPos = transform.position;
			newPos.y = currentLayer.transform.position.y;
			transform.position = newPos;
		}

		private void OnDrawGizmos()
		{
			/*
			Vector3 direction = Vector3.up;
			if (Input.GetAxis(verticalAxis) < 0)
				direction = -direction;
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, direction);
			*/

			Vector3 gizmoPos = transform.position + new Vector3(0, 0.4f, 0);
			Handles.color = Handles.zAxisColor;
			Handles.ArrowHandleCap(0, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity * Quaternion.Euler(90, 0, 0), 0.4f, EventType.Repaint);
		}
	}

}
