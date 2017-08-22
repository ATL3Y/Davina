using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HoleObject : MObject {

	Collider col;
	[SerializeField] List<string> matchTagList = new List<string>();
	[SerializeField] float fixInTime = 1f;
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
		if (storySound == null) {
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
        foreach ( MeshRenderer r in outlineRenders )
        {
            r.material.SetFloat( "_Outline", outlineWidth * 2f );
        }

		if (matchObject == null) {
			// play story
			if ( storySoundSource != null && !storySoundSource.isPlaying && gameObject.layer != 18 && GetStoryTimer() == 0f )
			{
				storySoundSource.Play( );
				SetStoryTimer( 3f );
			}
		}

    }

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();
        //SetOutline (false);
        foreach ( MeshRenderer r in outlineRenders )
        {
            r.material.SetFloat( "_Outline", outlineWidth / 2f );
        }
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
		Vector3 strength = new Vector3 (50f, 50f, 0f);
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
	protected virtual void OnTriggerEnter(Collider col) 
	{
		string tag = col.gameObject.tag;
		if (matchTagList.Contains (tag) && SelectObjectManager.Instance.IsSelectObject (col.gameObject)) {
			matchObject = col.gameObject;
			//print ("hole match obj name = " + matchObject.name);
			CollectableObj cobj = matchObject.GetComponent<CollectableObj> ();
			if (cobj != null) {
				cobj.matched = true;
			}

		} 
		/*
		else if (tag == "GameController" && matchObject == null) {
			// play story
			if ( storySoundSource != null && !storySoundSource.isPlaying && gameObject.layer != 18 && GetStoryTimer() == 0f )
            {
                storySoundSource.Play( );
                SetStoryTimer( 5f );
            }
		}
		*/

	}

	protected virtual void OnTriggerExit(Collider col)
	{
		if (matchObject == col.gameObject) {

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

		// if the match succeeds
		if (cobj != null && cobj.gameObject == matchObject)
        {
            gameObject.layer = LayerMask.NameToLayer( "Done" ); 
            cobj.gameObject.layer = LayerMask.NameToLayer( "Done" ); 

            //print ("in hole on match obj condition");
            // vibrate the controller holding the matchObject
            if (cobj.transform.gameObject.name == "Controller (right)") {
				InputManager.Instance.VibrateController (ViveInputController.Instance.rightControllerIndex);
			} else {
				InputManager.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);
			}

			// change the transform parent and position, scale, rotation of the object
			cobj.transform.parent = transform; 
			cobj.transform.DOLocalMove (Vector3.zero, fixInTime).SetEase (Ease.InCirc);
			cobj.transform.DOLocalRotate ( cobj.GSetOrinalRot(), fixInTime).SetEase (Ease.InCirc);
			cobj.transform.DOScale (1.02f, fixInTime).SetEase (Ease.InCirc);

            float delay = 3f;
            if ( storySoundSource != null && !storySoundSource.isPlaying)
            {
                storySoundSource.Play( );
                delay = storySoundSource.clip.length;
            }

            cobj.OnFill ( delay );
		}
	}
}






