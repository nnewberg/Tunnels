using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelLayer : MonoBehaviour {

	//Tracked object whose Z coord
	//will interpolate content 
	public Transform trackedScroller;
	public TunnelLayerObject[] layerObjects;

	//Z boundaries for interpolation zones
	//will be converted to world-space
	public Vector2 revealZone = new Vector2 (0.5f, 1f);
	public Vector2 hideZone = new Vector2 (0f, 0.5f);

	//Spawn position of this Layer
	private Vector3 layerOrigin; 
	//current t value for interpolation
	private float currT;


	// Use this for initialization
	void Start () {

		setZoneBounds ();
		layerObjects = GetComponentsInChildren<TunnelLayerObject> ();
	}

	// Update is called once per frame
	void Update () {

		float revealT = 1f - getZoneT (revealZone);
		float hideT = getZoneT (hideZone);
		currT = hideT * revealT;

		foreach (TunnelLayerObject layerObj in layerObjects){

			if (!layerObj.isSelected) {
				
				layerObj.updateAnimation (currT);
				layerObj.fadeColor (currT);

			}

			//store the layer's T for
			//discrepancy thangs
			layerObj.layerT = currT;
		}

	}

	//Returns the t value for interpolation
	//given a distance zone bounds
	private float getZoneT(Vector2 zone){

		return Mathf.InverseLerp (zone[0],
			zone[1], 
			trackedScroller.position.z);
	}

	//Sets origin
	//Converts zone bounds to world space
	private void setZoneBounds(){
		//negative z is towards the camera
		layerOrigin = this.transform.position;
		float zoneOrigin = layerOrigin.z - (0.5f * this.transform.localScale.z);

		hideZone[0] = zoneOrigin - hideZone[0];
		hideZone [1] = zoneOrigin - hideZone[1];

		revealZone[0] = zoneOrigin - revealZone[0];
		revealZone [1] =  zoneOrigin - revealZone[1];


	}
}
