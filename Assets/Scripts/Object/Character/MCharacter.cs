using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Script for the character
/// </summary>
public class MCharacter : MObject {

	[SerializeField] Transform outBody;
	[SerializeField] Transform innerWorld;

	// for the inner world
	[SerializeField] AudioClip innerWorldClip;
	[SerializeField] Renderer[] outerRender;
	public float enterInnerWorldScaleUp = 1.5f; //make get/set
	[SerializeField] BoxCollider innerWorldCollider;
	[SerializeField] Vector3 fallingSpeed;

	/// <summary>
	/// The characters with controllers in its range
	/// to make sure only one character is scaled up
	/// </summary>
	static private List<MCharacter> triggeredCharacter = new List<MCharacter> ();
	/// <summary>
	/// The focused character.
	/// </summary>
	static private MCharacter focusedCharacter;

	/// <summary>
	/// the pivot for the inner world
	/// the position match the center of the inner world collider
	/// </summary>
	GameObject pivot;

	/// <summary>
	/// Record the original scale of the charcter
	/// </summary>
	Vector3 originScale;
	protected override void MAwake ()
	{
		base.MAwake ();
		if (innerWorldCollider != null && innerWorld != null) {
			pivot = new GameObject ();
			pivot.transform.SetParent (transform);

			pivot.transform.localPosition = innerWorldCollider.center * innerWorld.localScale.x;
		}

		originScale = transform.localScale;
		innerWorld.gameObject.SetActive (true);
	}

	protected override void MOnEnable(){
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.End] += OnEnd;
	}

	protected override void MOnDisable(){
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
	}


	/// <summary>
	/// set this game object and all the body renders to a specific layer
	/// </summary>
	/// <param name="layer">Layer.</param>
	void SetToLayer( string layer )
	{
		gameObject.layer = LayerMask.NameToLayer (layer);
		foreach (Transform t in outBody.GetComponentsInChildren<Transform>()) {
			t.gameObject.layer = LayerMask.NameToLayer (layer);
		}
		foreach (Transform t in innerWorld.GetComponentsInChildren<Transform>()) {
			t.gameObject.layer = LayerMask.NameToLayer (layer);
		}
	}

	public override void OnFocus ()
	{
		base.OnFocus ();
	}

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();

	}

	private bool m_isInInnerWorld = false;
	public bool IsInInnerWorld
	{
		get { return m_isInInnerWorld; }
	}

	Coroutine changeScale;


	/// <summary>
	/// TODO: revise the scale design
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerEnter( Collider col )
	{
		if ( (col.gameObject.tag == "GameController" &&
			LogicManager.Instance.VREnable) || 
			(col.gameObject.tag == "Player" &&
			!LogicManager.Instance.VREnable) )
		{
			triggeredCharacter.Add (this);

			if (focusedCharacter == null) {
				focusedCharacter = this;
				LogicArg arg = new LogicArg (this);
				M_Event.FireLogicEvent (LogicEvents.EnterCharacterRange, arg);
				focusedCharacter = this;
				EnterCharacterRange ();
			}
		}
	}

	/// <summary>
	/// TODO: revise the scale design
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerExit( Collider col )
	{
		if ( (col.gameObject.tag == "GameController" &&
			LogicManager.Instance.VREnable) || 
			(col.gameObject.tag == "Player")) 
				//&& !LogicManager.Instance.VREnable) ) //so head triggers mesh for vr or no
		{
			if ( triggeredCharacter.Contains(this))
				triggeredCharacter.Remove (this);

			if (focusedCharacter == this) {
				focusedCharacter = null;
				LogicArg arg = new LogicArg (this);
				M_Event.FireLogicEvent (LogicEvents.ExitCharacterRange, arg);
			}

			if (focusedCharacter == null && triggeredCharacter.Count > 0) {
				focusedCharacter = triggeredCharacter [0];
				LogicArg arg = new LogicArg (focusedCharacter);
				M_Event.FireLogicEvent (LogicEvents.EnterCharacterRange, arg);
			}
		}
	}


	public void EnterCharacterRange()
	{
		innerWorld.gameObject.SetActive (true);
	}

	public void ExitCharacterRange()
	{
		innerWorld.gameObject.SetActive (false);
	}


	/// <summary>
	/// Fire the enter inner world event
	/// scale up the model
	/// and disable the exterior model
	/// </summary>
	/// <param name="col">Col.</param>
	public void EnterInnerWorld( Collider col )
	{
		if (!m_isInInnerWorld) {

			// fire the event
			LogicArg arg = new LogicArg (this);
			arg.AddMessage (Global.EVENT_LOGIC_ENTERINNERWORLD_CLIP, innerWorldClip);
			arg.AddMessage (Global.EVENT_LOGIC_ENTERINNERWORLD_MCHARACTER, this);
			M_Event.FireLogicEvent (LogicEvents.EnterInnerWorld, arg);

			m_isInInnerWorld = true;

			// scale up the model
			if (changeScale != null)
				StopCoroutine (changeScale);
			changeScale =  StartCoroutine (ChangeScale ( originScale * enterInnerWorldScaleUp, 2f));

			// disable the exterior model
			foreach (Renderer r in outerRender) {
				r.enabled = false;
			}

//			transform.DOScale (transform.localScale.x * enterInnerWorldScaleUp, 0.5f);
		}
	}

	/// <summary>
	/// Called when exit the inner world
	/// </summary>
	/// <param name="col">Col.</param>
	public void ExitInnerWorld( Collider col )
	{
		if (m_isInInnerWorld) {
			// fire the exit event
			LogicArg arg = new LogicArg (this);

			arg.AddMessage (Global.EVENT_LOGIC_EXITINNERWORLD_MCHARACTER, this);
			M_Event.FireLogicEvent (LogicEvents.ExitInnerWorld, arg);


			m_isInInnerWorld = false;

			// scale down the model
			if (changeScale != null)
				StopCoroutine (changeScale);
			changeScale = StartCoroutine (ChangeScale ( originScale , 2f));

			// enable the exterior model
			foreach (Renderer r in outerRender) {
				r.enabled = true;
			}
//			transform.DOScale (transform.localScale / enterInnerWorldScaleUp, 0.5f);
		}
	}

	/// <summary>
	/// Changes the scale of the model
	/// </summary>
	/// <returns>The scale.</returns>
	/// <param name="_toScale">To scale.</param>
	/// <param name="time">Time.</param>
	IEnumerator ChangeScale( Vector3 _toScale , float time )
	{
		if ( pivot == null )
			yield break;
		Vector3 originPosition = pivot.transform.position;
		//Debug.Log ("pivot pos " + pivot.transform.position);

		float timer = 0;
		Vector3 fromSclae = transform.localScale;
		Vector3 toScale = _toScale;
		while (timer < time ) {
			timer += Time.deltaTime;
			transform.localScale = Vector3.Lerp (fromSclae, toScale, timer / time);
			Vector3 offset = pivot.transform.position - originPosition;
			transform.position -= offset;
			yield return null;
		}
		changeScale = null;
	}


	protected override void MUpdate ()
	{
		base.MUpdate ();

		// update the position of the character
		if (!IsInInnerWorld) {
			transform.position += fallingSpeed * Time.deltaTime;
		}
	}

	void OnEnd( LogicArg arg ){
		//Debug.Log ("onend in mcharacter");
	}

}
