using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LogicManager : MBehavior {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

	public bool VREnable = false;

	[SerializeField] GameObject PC;
	[SerializeField] GameObject VR;

	[SerializeField] GameObject Rain;

	private GameObject text;
	public string introText = "IT IS NOT YOUR TIME YET";

	public enum State
	{
		Init,
		Intro,
		IntroText,
		Rain,
		InRain,
	}

	public State state;

	protected override void MAwake ()
	{
		base.MAwake ();

		if (VREnable) {
			gameObject.AddComponent<ViveInputManager> ();
		} else {
			gameObject.AddComponent<PCInputManager> ();
		}

		if (PC != null) {
			PC.SetActive (!VREnable);
		}
		if (VR != null) {
			VR.SetActive (VREnable);
		}

		Rain.transform.SetParent ( VREnable ? VR.transform : PC.transform);
		Rain.transform.localPosition = Vector3.up * 5f;

		DontDestroyOnLoad (gameObject);

	}

	void Update(){
		switch (state) {
		case State.Init:
			SceneManager.LoadScene ("Intro");
			state = State.IntroText;
			break;
		case State.IntroText:
			text = GameObject.Find ("Text");
			text.GetComponent<TextStatic> ().MakeTextGO (introText);
			if (Time.timeSinceLevelLoad >= 12.0f) {
				state = State.Rain;
			} 
			break;
		case State.Rain:
			SceneManager.LoadScene ("RainSceneTestAL");
			state = State.InRain;
			text.SetActive (false);
			break;
		case State.InRain:
			
			break;
		default:
			break;
		}
	}

	public Transform GetPlayerTransform()
	{
		return VREnable ? VR.transform : PC.transform;
	}

}
