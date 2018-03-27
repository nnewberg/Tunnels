using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

	private TunnelLayerObject selectedObject;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {

				if (selectedObject) { //if there's already a selection
				
					//any tap on the screen returns the object
					selectedObject.handleTap ();
					selectedObject = null;
				
				} else { //if new selection

					// Construct a ray from the current touch coordinates
					Ray ray = Camera.main.ScreenPointToRay (touch.position);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit, 5f)) {
						selectedObject = hit.collider.GetComponent<TunnelLayerObject> (); 
						selectedObject.handleTap();
					}
				}
			}
		}

	}
}
