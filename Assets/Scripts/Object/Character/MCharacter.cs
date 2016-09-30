using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MCharacter : MObject {

	[SerializeField] Transform outBody;
	[SerializeField] Transform innerWorld ;

	// for the inner world
	[SerializeField] AudioClip innerWorldClip;
	[SerializeField] Renderer[] outerRender;
	[SerializeField] float enterInnerWorldScaleUp = 1.5f;
	[SerializeField] BoxCollider innerWorldCollider;
	[SerializeField] Vector3 fallingSpeed;


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

	public void EnterInnerWorld( Collider col )
	{
		Debug.Log ("Enter Inner World");
		if (!m_isInInnerWorld) {
			LogicArg arg = new LogicArg (this);
			arg.AddMessage (Global.EVENT_LOGIC_ENTERINNERWORLD_CLIP, innerWorldClip);
			arg.AddMessage (Global.EVENT_LOGIC_ENTERINNERWORLD_MCHARACTER, this);
			M_Event.FireLogicEvent (LogicEvents.EnterInnerWorld, arg);

			m_isInInnerWorld = true;

			if (changeScale != null)
				StopCoroutine (changeScale);
			changeScale =  StartCoroutine (ChangeScale ( originScale * enterInnerWorldScaleUp, 0.33f));

			foreach (Renderer r in outerRender) {
				r.enabled = false;
			}

//			transform.DOScale (transform.localScale.x * enterInnerWorldScaleUp, 0.5f);
		}
	}

	public void ExitInnerWorld( Collider col )
	{
		if (m_isInInnerWorld) {
			LogicArg arg = new LogicArg (this);

			arg.AddMessage (Global.EVENT_LOGIC_EXITINNERWORLD_MCHARACTER, this);
			M_Event.FireLogicEvent (LogicEvents.ExitInnerWorld, arg);

			m_isInInnerWorld = false;
			if (changeScale != null)
				StopCoroutine (changeScale);
			changeScale = StartCoroutine (ChangeScale ( originScale , 0.33f));

			foreach (Renderer r in outerRender) {
				r.enabled = true;
			}
//			transform.DOScale (transform.localScale / enterInnerWorldScaleUp, 0.5f);
		}
	}

	IEnumerator ChangeScale( Vector3 _toScale , float time )
	{
		if ( pivot == null )
			yield break;
		Vector3 originPosition = pivot.transform.position;
		Debug.Log ("pivot pos " + pivot.transform.position);

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
		if (!IsInInnerWorld) {
			transform.position += fallingSpeed * Time.deltaTime;
		}
	}
}
