﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityStandardAssets.ImageEffects;

public class TransportManager : MBehavior {

	public TransportManager() { s_Instance = this; }
	public static TransportManager Instance { get { return s_Instance; } }
	private static TransportManager s_Instance;

	[SerializeField] ToColorEffect toColorEffect;
	[SerializeField] BloomAndFlares bloomAndFlares;
//	[SerializeField] float transportOffset = 1f;
	[SerializeField] float fadeTime = 1f;
	[SerializeField] float transportTime = 2f;
	[SerializeField] LineRenderer transportLine;
	[SerializeField] ParticleSystem transportCircle;

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
		M_Event.inputEvents [(int)MInputType.FocusNewObject] += OnFocusNew;
		M_Event.inputEvents [(int)MInputType.OutOfFocusObject] += OnOutofFocus;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.inputEvents [(int)MInputType.Transport] -= OnTransport;
		M_Event.inputEvents [(int)MInputType.FocusNewObject] -= OnFocusNew;
		M_Event.inputEvents [(int)MInputType.OutOfFocusObject] -= OnOutofFocus;
	}

	PasserBy focusPasserby;
	public void OnFocusNew( InputArg arg )
	{
		PasserBy p = arg.focusObject.GetComponent<PasserBy> ();
		if (p != null) {
			focusPasserby = p;
			if (transportLine != null) {
				Vector3 transportStart = Camera.main.transform.position;
				Vector3 transportToward = focusPasserby.GetObservePosition ();
				float length = (transportStart - transportToward).magnitude;
				transportStart.y = transportToward.y = 0.2f;

				transportLine.enabled = true;
				transportLine.SetPosition (0, transportStart);
				transportLine.SetPosition (1, transportToward);
				transportLine.material.SetVector ("_Scale", new Vector4 (length * 2f, 1f, 1f, 1f));
			}

			if (transportCircle != null) {
				Vector3 transportToward = focusPasserby.GetObservePosition ();
				transportToward.y = 0.25f;
				transportCircle.transform.position = transportToward;
				transportCircle.gameObject.SetActive (true);
			}
		}
	}

	public void OnOutofFocus( InputArg arg )
	{
		PasserBy p = arg.focusObject.GetComponent<PasserBy> ();
		if ( focusPasserby == p) {
			if (transportLine != null) {
				transportLine.enabled = false;
			}
			if (transportCircle != null) {
				transportCircle.gameObject.SetActive (false);
			}
			focusPasserby = null;
		}
	}

	private MObject transportToObject;

	public void OnTransport ( InputArg arg )
	{
		if (InputManager.Instance.FocusedObject != null && InputManager.Instance.FocusedObject is PasserBy ) {

			// do not make a mutiple transport
			if (IsTransporting)
				return;

			transportToObject = InputManager.Instance.FocusedObject;

			// do not transport to myself
			if ( transportToObject == LogicManager.Instance.StayPasserBy)
				return;

			PasserBy p = transportToObject.GetComponent<PasserBy> ();
			if (p == null)
				return;
			

			// fire the transport start event
			LogicArg logicArg = new LogicArg (this);
			logicArg.AddMessage (Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT, transportToObject);
			M_Event.FireLogicEvent ( LogicEvents.TransportStart, logicArg);


			// set up the animation sequence
			transportSequence = DOTween.Sequence ();
			// add the vfx if there is the image effect in the camera
			if (toColorEffect != null && bloomAndFlares != null) {
				transportSequence.Append (DOTween.To (() => toColorEffect.rate, (x) => toColorEffect.rate = x, 1f, fadeTime));
				transportSequence.Join (DOTween.To (() => bloomAndFlares.bloomIntensity, (x) => bloomAndFlares.bloomIntensity = x, 0f, fadeTime));
			}


			// calculate the transport varible
			Transform myTrans = LogicManager.Instance.GetPlayerTransform ();
//			Vector3 target = InputManager.Instance.FocusedObject.transform.position;
//			Vector3 offset = (myTrans.position - target);
//			offset.y = 0;
//			offset.Normalize (); 
//			offset *= transportOffset;
			Vector3 target = p.GetObservePosition();
			target.y = transform.position.y;

			transportSequence.Append (LogicManager.Instance.GetPlayerTransform ().DOMove (target , transportTime));
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
		// fire the transport end event
		if (transportToObject != null) {
			LogicArg arg = new LogicArg (this);
			arg.AddMessage (Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT, transportToObject);

			M_Event.FireLogicEvent ( LogicEvents.TransportEnd, arg);
		}

		transportSequence = null;
		transportToObject = null;
	}
}