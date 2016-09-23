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

	public void OnTransport ( InputArg arg )
	{
		
		Debug.Log ("On Transport");

		if (InputManager.Instance.FocusedObject != null) {
			Sequence seq = DOTween.Sequence ();
			if (toColorEffect != null && bloomAndFlares != null) {
				seq.Append (DOTween.To (() => toColorEffect.rate, (x) => toColorEffect.rate = x, 0.7f, fadeTime));
				seq.Join (DOTween.To (() => bloomAndFlares.bloomIntensity, (x) => bloomAndFlares.bloomIntensity = x, 0f, fadeTime));
			}
			

			Transform myTrans = LogicManager.Instance.GetPlayerTransform ();
			Vector3 target = InputManager.Instance.FocusedObject.transform.position;
			Vector3 offset = (myTrans.position - target);
			offset.y = 0;
			offset.Normalize (); 
			offset *= transportOffset;
			seq.Append (LogicManager.Instance.GetPlayerTransform ().DOMove (target + offset, transportTime));

			if (toColorEffect != null && bloomAndFlares != null) {
				seq.Append (DOTween.To (() => toColorEffect.rate, (x) => toColorEffect.rate = x, 0f, fadeTime));
				seq.Join (DOTween.To (() => bloomAndFlares.bloomIntensity, (x) => bloomAndFlares.bloomIntensity = x, 8f, fadeTime));
			}
		}
	}
}
