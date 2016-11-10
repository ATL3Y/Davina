using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HoleObject : MObject {

	Collider col;
	[SerializeField] List<string> matchTagList = new List<string>();
	[SerializeField] float fixInTime = 1f;

	// i want the holes to have an outline too
	[SerializeField] MeshRenderer[] outlineRenders;

	[SerializeField] protected AudioClip storySound;
	[SerializeField] protected AudioSource storySoundSource;

	[SerializeField] float outlineWidth;

	private Material material;
	private Color color;

	protected override void MAwake ()
	{
		base.MAwake ();
		material = new Material(Shader.Find("Outlined/Silhouette Only"));

		foreach (MeshRenderer r in outlineRenders) {
			r.material = material;
			ColorUtility.TryParseHtmlString ("#FFFFFFFF", out color);
			r.material.SetFloat ("_Outline", outlineWidth);
			r.material.SetVector ("_OutlineColor", color);
		}

		col = GetComponent<Collider> ();
		col.isTrigger = true;

		// set up the story sound
		if (storySound != null) {
			storySoundSource = gameObject.AddComponent<AudioSource> ();
			storySoundSource.playOnAwake = false;
			storySoundSource.loop = false;
			storySoundSource.volume = 1f;
			storySoundSource.spatialBlend = 1f;
			storySoundSource.clip = storySound;

		}
		GetComponent<Renderer> ().enabled = false;
		SetOutline (true);
	}

	public override void OnFocus ()
	{
		base.OnFocus ();
		//SetOutline (true);
	}

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();
		//SetOutline (false);
	}

	/// <summary>
	/// Set the outline render on or off(enable)
	/// </summary>
	/// <param name="isOn">If set to <c>true</c> is on.</param>
	protected void SetOutline( bool isOn )
	{
		foreach (MeshRenderer r in outlineRenders) {
			r.enabled = isOn;
		}
	}

	/// <summary>
	/// Shake this object 
	/// </summary>
	protected void Shake( )
	{
		Vector3 strength = new Vector3 (50f, 50f, 0f); // scale of 0-1
		transform.DOShakeRotation(1f, strength, 10, 40f, true);
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.MatchObject] += OnMatchObject; 
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.MatchObject] -= OnMatchObject; 
	}

	/// <summary>
	/// TODO: find a better way to handle the match object algorithm
	/// </summary>
	public GameObject matchObject;
	protected virtual void OnTriggerStay(Collider col) //changed from OnTriggerEnter
	{
		string tag = col.gameObject.tag;
		if (matchTagList.Contains (tag) && SelectObjectManager.Instance.IsSelectObject (col.gameObject)) {
			matchObject = col.gameObject;
			//print ("hole match obj name = " + matchObject.name);
			CollectableObj cobj = matchObject.GetComponent<CollectableObj> ();
			if (cobj != null) {
				// make it so the next click will not trigger Unselect's transform change in CollectableObject
				cobj.matched = true;
				//print ("in hole set matched = " + cobj.matched);
				//SetOutline(true);
			}
			// play story
			//if ( storySoundSource != null )
				//storySoundSource.Play ();
		} else if (tag == "GameController" && matchObject == null) {
			bool shake = true;
			foreach (Transform child in col.gameObject.transform) {
				if (child.gameObject.layer.ToString() == "17") { // 17 is "Hold"
					shake = false;
				}
			}
			if (shake) {
				//Shake ();
			}
			// play story
			//if ( storySoundSource != null && !storySoundSource.isPlaying && gameObject.layer != 18) // object is not Done
				//storySoundSource.Play ();
		}
	}

	protected virtual void OnTriggerExit(Collider col)
	{
		if (matchObject == col.gameObject) {
			//matchObject = null;
			//SetOutline(false);
		}
	}

	/// <summary>
	/// react to the match object event
	/// try to match the object
	/// </summary>
	/// <param name="arg">Argument.</param>
	protected virtual void OnMatchObject( LogicArg arg )
	{
		CollectableObj cobj = (CollectableObj) arg.GetMessage (Global.EVENT_LOGIC_MATCH_COBJECT);
		//print ("in hole on match obj");
		// if the match succeeds
		if (cobj != null && cobj.gameObject == matchObject) {
			//print ("in hole on match obj condition");
			// vibrate the controller holding the matchObject
			if (cobj.transform.gameObject.name == "Controller (left)") {
				InputManager.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);
			} else {
				InputManager.Instance.VibrateController (ViveInputController.Instance.rightControllerIndex);
			}

			// change the transform parent and position, scale, rotation of the object
			cobj.transform.parent = transform; //.parent;
			cobj.transform.DOLocalMove (Vector3.zero, fixInTime).SetEase (Ease.InCirc);
			cobj.transform.DOLocalRotate (Vector3.zero, fixInTime).SetEase (Ease.InCirc);
			cobj.transform.DOScale (1.02f, fixInTime).SetEase (Ease.InCirc);

			if ( storySoundSource != null && !storySoundSource.isPlaying && gameObject.layer != 18) // object is not Done
				storySoundSource.Play ();

			//would be nice to then repeat match sound here 

			gameObject.layer = 18; //change layer from Focus (16) to Done (18)

			// tell the object it is filled in the hole
			cobj.OnFill ();
			//this.gameObject.SetActive (false);
		}
	}
}






