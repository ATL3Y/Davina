using UnityEngine;
using System.Collections;

public class AudioManager : MBehavior {

	public AudioManager() { s_Instance = this; }
	public static AudioManager Instance { get { return s_Instance; } }
	private static AudioManager s_Instance;

	/// <summary>
	/// input pair for recording the input sound effect
	/// </summary>
	[System.Serializable]
	public struct InputClipPair
	{
		public MInputType input;
		public AudioClip clip;
	};
	[SerializeField] InputClipPair[] InputClipPairs;

	[System.Serializable]
	public struct LogicClipPair
	{
		public LogicEvents type;
		public AudioClip clip;
	};
	[SerializeField] LogicClipPair[] LogicClipPairs;

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		for (int i = 0; i < System.Enum.GetNames (typeof(MInputType)).Length; ++i) {
			M_Event.inputEvents [i] += OnInputEvent;
		}
		for (int i = 0; i < System.Enum.GetNames (typeof(LogicEvents)).Length; ++i) {
			M_Event.logicEvents [i] += OnLogicEvent;
		}
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		for (int i = 0; i < System.Enum.GetNames (typeof(MInputType)).Length; ++i) {
			M_Event.inputEvents [i] -= OnInputEvent;
		}
		for (int i = 0; i < System.Enum.GetNames (typeof(LogicEvents)).Length; ++i) {
			M_Event.logicEvents [i] -= OnLogicEvent;
		}
	}

	void OnInputEvent( InputArg input )
	{
		foreach (InputClipPair pair in InputClipPairs) {
			if (pair.input == input.type) {
				StartCoroutine(PlayerClip(pair.clip));
			}
		}
	}

	void OnLogicEvent( LogicArg logicEvent )
	{
		foreach (LogicClipPair pair in LogicClipPairs) {
			if (pair.type == logicEvent.type) {
				StartCoroutine(PlayerClip(pair.clip));
			}
		}
	}

	IEnumerator PlayerClip( AudioClip clip )
	{
		if (clip == null)
			yield break;
		AudioSource source = gameObject.AddComponent<AudioSource> ();
		source.clip = clip;
		source.playOnAwake = source.loop = false;

		source.Play ();
		while (source.isPlaying) {
			yield return null;
		}

		Destroy (source);
		
	}
}
