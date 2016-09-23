using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityStandardAssets.ImageEffects;

public class TransportManager : MBehavior {

	public TransportManager() { s_Instance = this; }
	public static TransportManager Instance { get { return s_Instance; } }
	private static TransportManager s_Instance;

	[SerializeField] ToColorEffect toColorEffect;
	[SerializeField] BloomAndFlares bloomAndFlares;
	[SerializeField] float transportOffset = 1f;
	[SerializeField] float fadeTime = 1f;
	[SerializeField] float transportTime = 2f;

	/// <summary>
	/// For the transport animation
	/// </summary>
	private Sequence transportSequence;
	public bool IsTransporting{
		get { 
			if ( transportSequence != null ) 
				return !transportSequence.IsComplete();
			return false;
		}
	}

	protected override void MAwake ()
	{
		base.MAwake ();

	}

	protected override void MStart ()
	{
		base.MStart ();
		toColorEffect = LogicManager.Instance.GetPlayerTransform ().GetComponentInChildren<ToColorEffect> ();
		bloomAndFlares = LogicManager.Instance.GetPlayerTransform ().GetComponentInChildren<BloomAndFlares> ();
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.inputEvents [(int)MInputType.Transport] += OnTransport;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.inputEvents [(int)MInputType.Transport] -= OnTransport;
	}

	private MObject transportToObject;

	public void OnTransport ( InputArg arg )
	{
		if (InputManager.Instance.FocusedObject != null && !IsTransporting ) {

			transportToObject = InputManager.Instance.FocusedObject;

			if ( transportToObject == LogicManager.Instance.StayPasserBy)
				return;

			// fire the transport start event
			LogicArg logicArg = new LogicArg (this);
			logicArg.AddMessage (Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT, transportToObject);
			M_Event.FireLogicEvent ( LogicEvents.TransportStart, logicArg);


			// set up the animation sequence
			transportSequence = DOTween.Sequence ();
			// add the vfx if there is the image effect in the camera
			if (toColorEffect != null && bloomAndFlares != null) {
				transportSequence.Append (DOTween.To (() => toColorEffect.rate, (x) => toColorEffect.rate = x, 0.7f, fadeTime));
				transportSequence.Join (DOTween.To (() => bloomAndFlares.bloomIntensity, (x) => bloomAndFlares.bloomIntensity = x, 0f, fadeTime));
			}


			// calculate the transport varible
			Transform myTrans = LogicManager.Instance.GetPlayerTransform ();
			Vector3 target = InputManager.Instance.FocusedObject.transform.position;
			Vector3 offset = (myTrans.position - target);
			offset.y = 0;
			offset.Normalize (); 
			offset *= transportOffset;

			transportSequence.Append (LogicManager.Instance.GetPlayerTransform ().DOMove (target + offset, transportTime));
			// add the vfx if there is the image effect in the camera
			if (toColorEffect != null && bloomAndFlares != null) {
				transportSequence.Append (DOTween.To (() => toColorEffect.rate, (x) => toColorEffect.rate = x, 0f, fadeTime));
				transportSequence.Join (DOTween.To (() => bloomAndFlares.bloomIntensity, (x) => bloomAndFlares.bloomIntensity = x, 8f, fadeTime));
			}

			transportSequence.OnComplete (OnTransportCOmplete);
		}
	}

	void OnTransportCOmplete()
	{
		transportSequence = null;

		if (transportToObject != null) {
			LogicArg arg = new LogicArg (this);
			arg.AddMessage (Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT, transportToObject);

			M_Event.FireLogicEvent ( LogicEvents.TransportEnd, arg);
		}
		transportToObject = null;
	}
}
