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
	[SerializeField] Transform PCHand;
	[SerializeField] Transform VRHand;

	[SerializeField] GameObject Rain;

	[SerializeField] TextStatic text;

	private PasserBy m_stayPasserBy;
	public PasserBy StayPasserBy
	{
		get { return m_stayPasserBy; }
	}

	private MCharacter m_stayCharacter;
	public MCharacter StayCharacter
	{
		get { return m_stayCharacter; }
	}

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

		Cursor.visible = false;

	}

	void Update(){


		switch (state) {
		case State.Init:
			if ( SceneManager.GetActiveScene().name != "Intro" )
				SceneManager.LoadScene ("Intro");
			state = State.IntroText;
			break;
		case State.IntroText:
			text.MakeTextGO (introText);
			if (Time.timeSinceLevelLoad >= 12.0f) {
				state = State.Rain;
			} 
			break;
		case State.Rain:
			if ( SceneManager.GetActiveScene().name != "RainSceneTestAL" )
				SceneManager.LoadScene ("RainSceneTestAL");
			state = State.InRain;
			text.gameObject.SetActive (false);
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

	public Transform GetHandTransform()
	{
		return VREnable ? VRHand : PCHand;
	}


	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] += OnTransportToNewObject;
		M_Event.logicEvents [(int)LogicEvents.EnterInnerWorld] += OnEnterInnerWorld;
		M_Event.logicEvents [(int)LogicEvents.ExitInnerWorld] += OnExitInnerWorld;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] -= OnTransportToNewObject;
		M_Event.logicEvents [(int)LogicEvents.EnterInnerWorld] -= OnEnterInnerWorld;
		M_Event.logicEvents [(int)LogicEvents.ExitInnerWorld] -= OnExitInnerWorld;
	}

	void OnTransportToNewObject( LogicArg arg )
	{
		var obj = arg.GetMessage (Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT);
		if (obj is PasserBy) {
			m_stayPasserBy = (PasserBy)obj;
		}
	}

	void OnEnterInnerWorld(LogicArg arg )
	{
		MCharacter character = (MCharacter) arg.GetMessage (Global.EVENT_LOGIC_ENTERINNERWORLD_CHARACTER);
		if (character != null)
			m_stayCharacter = character;
	}

	void OnExitInnerWorld(LogicArg arg )
	{
		MCharacter character = (MCharacter) arg.GetMessage (Global.EVENT_LOGIC_EXITINNERWORLD_CHARACTER);
		if (m_stayCharacter == character)
			m_stayCharacter = null;
	}


}
