using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TunnelLayerObject : MonoBehaviour {

	public bool is3D;
	//true when the user taps on this object
	public bool isSelected;

	[System.NonSerialized]
	public float layerT;

	//position and rotation of object when hidden
	private Vector3 hiddenPos;
	private Quaternion hiddenRot;

	//position and rotation of object when shown
	private Vector3 revealedPos;
	private Quaternion revealedRot;

	private Vector3 initScale;

	//used to attach to its layer
	private Transform initialParent;

	private Material material;
	private VideoPlayer videoPlayer;

	// Use this for initialization
	void Start () {

		material = GetComponent<Renderer> ().material;
		videoPlayer = GetComponent<VideoPlayer> ();

		generateAnimCurves ();
		initialParent = this.transform.parent;
		initScale = this.transform.localScale;

		//start in the hidden state
		this.transform.position = hiddenPos;
		this.transform.rotation = hiddenRot;

		isSelected = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (is3D && isSelected) {
			this.transform.Rotate (Vector3.up * 30f*Time.deltaTime);
		}
		
	}

	public void updateAnimation(float t){

		this.transform.position = 
			Vector3.Lerp (hiddenPos, revealedPos, t);
		this.transform.rotation =
			Quaternion.Lerp (hiddenRot, revealedRot, t);
	}

	public void fadeColor(float t){
		
		Color interpolatedColor = new Color (material.color.r, material.color.g,
			material.color.b, t);
		material.SetColor ("_Color", interpolatedColor);
	}

	public void handleTap(){

		if (!isSelected) { //in its layer
			//Start Co-Routine to Lerp to the camera
			StartCoroutine ("moveToCamera", 0.5f);
			if (videoPlayer) {
				videoPlayer.Play ();
			}
	
		} else { //already selected
			//Start Co-Routine to Lerp to back to layer
			StartCoroutine ("returnToLayer", 0.5f);
			if (videoPlayer) {
				videoPlayer.Stop ();
			}
		}

	}

	IEnumerator moveToCamera(float time)
	{
		isSelected = true;

		float elapsedTime = 0f;
		Transform camTransform = Camera.main.transform;
		Vector3 targetScale = (0.3f / Mathf.Abs(this.transform.localScale.x)) * this.transform.localScale;

		while (elapsedTime < time)
		{
			Vector3 currPos = Vector3.Lerp(revealedPos, camTransform.position + 
				(camTransform.forward * 0.3f) , elapsedTime / time);
			this.transform.position = currPos;

			this.transform.LookAt (camTransform);

			if (!is3D) {
				this.transform.localScale = Vector3.Lerp (initScale, targetScale, elapsedTime/time);

			}

			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		this.transform.parent = Camera.main.transform;

	}

	IEnumerator returnToLayer(float time)
	{
		float elapsedTime = 0f;
		Vector3 selectedPos = this.transform.position;
		Quaternion selectedRot = this.transform.rotation;

		this.transform.parent = initialParent;
		Vector3 camScale = this.transform.localScale;
			

		while (elapsedTime < time)
		{

			Vector3 currPos = Vector3.Lerp(selectedPos, revealedPos, elapsedTime / time);
			this.transform.position = currPos;

			Quaternion currRot = Quaternion.Slerp (selectedRot, revealedRot, elapsedTime / time);
			this.transform.rotation = currRot;

			this.transform.localScale = Vector3.Lerp (camScale, initScale, elapsedTime / time);

			//fade color back if the rest of the layer is not completely visible
			fadeColor(Mathf.Lerp(1f, layerT, elapsedTime));

			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		isSelected = false;


	}

	//Procedurally generate an animation path
	private void generateAnimCurves(){

		//initialized Position and Rotation
		revealedPos = this.transform.position;
		revealedRot = this.transform.rotation;

		//if local x coord is negative falls on right side (so flip it)
		float dir = - Mathf.Sign (this.transform.localPosition.x);
		//let hidden positions remain in the direction of the defualt pos
		float randX = revealedPos.x + dir * Random.Range (0.2f, 0.5f);
		float randY = revealedPos.y + Random.Range (0f, 0.5f);
		hiddenPos = new Vector3 (randX, randY, revealedPos.z);
		hiddenRot = Quaternion.Euler(this.transform.rotation.eulerAngles + Vector3.forward * Random.Range (0f, 180f));

	}
}
