using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 1. set up the input manager according to the VREnable
/// 2. set up the state machine for different scenes
/// </summary>
public class LogicManager : MBehavior {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

	public bool VREnable = false;

	[SerializeField] GameObject PC;
	[SerializeField] GameObject VR;
	[SerializeField] Transform PCHand;
	//[SerializeField] Transform VRHandLeft; // need to get this from ViveInputController
	//[SerializeField] Transform VRHandRight; // need to get this from ViveInputController
	[SerializeField] GameObject Rain;
	[SerializeField] TextStatic text;

	private PasserBy m_stayPasserBy;
	public PasserBy StayPasserBy{
		get { return m_stayPasserBy; }
	}

	private MCharacter m_stayCharacter;
	public MCharacter StayCharacter{
		get { return m_stayCharacter; }
	}

	public enum State{
		Init,
		Tutorial,
		CharacterScene,
	}

	private AStateMachine<State,LogicEvents> m_stateMachine = new AStateMachine<State, LogicEvents>();

	protected override void MAwake (){
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
		//Rain.transform.SetParent ( VREnable ? VR.transform : PC.transform);
		//Rain.transform.localPosition = Vector3.up * 5f;
		DontDestroyOnLoad (gameObject);
		Cursor.visible = false;
		InitStateMachine ();
	}

	void InitStateMachine(){
		m_stateMachine.AddUpdate (State.Init, delegate() {
			m_stateMachine.State = State.Tutorial;
		});
		m_stateMachine.AddEnter (State.Tutorial, delegate() {
			M_Event.FireLogicEvent(LogicEvents.TutorialSceneEnter, new LogicArg(this));	
		});
		m_stateMachine.AddUpdate (State.Init, delegate() {
			m_stateMachine.State = State.CharacterScene;
		});
		m_stateMachine.AddEnter (State.CharacterScene, delegate() {
			M_Event.FireLogicEvent(LogicEvents.CharacterSceneEnter, new LogicArg(this));	
		});
		//m_stateMachine.BlindTimeStateChange (State.OpenShotOne, State.MotherScene, 6f);
		m_stateMachine.State = State.Init;
	}
		
	public Transform GetPlayerTransform(){
		return VREnable ? VR.transform : PC.transform;
	}

	public Transform GetHandTransform( ClickType clickType ){
		switch (clickType) 
		{
		case ClickType.LeftController:
			return ViveInputController.Instance.leftController.transform;
		case ClickType.RightController:
			return ViveInputController.Instance.rightController.transform;
		default:
			return PCHand;
		}
	}
		
	protected override void MOnEnable (){
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] += OnTransportToNewObject;
		M_Event.logicEvents [(int)LogicEvents.EnterInnerWorld] += OnEnterInnerWorld;
		M_Event.logicEvents [(int)LogicEvents.ExitInnerWorld] += OnExitInnerWorld;
		M_Event.logicEvents [(int)LogicEvents.CameraAttachPointChange] += OnNewAttachPoint;
		/*
		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] += OnEnd;
		M_Event.logicEvents [(int)LogicEvents.Credits] += OnCredits;
		*/

		for (int i = 0; i < M_Event.logicEvents.Length; ++i) {
			M_Event.logicEvents [i] += OnLogicEvent;
		}
	}

	protected override void MOnDisable (){
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] -= OnTransportToNewObject;
		M_Event.logicEvents [(int)LogicEvents.EnterInnerWorld] -= OnEnterInnerWorld;
		M_Event.logicEvents [(int)LogicEvents.ExitInnerWorld] -= OnExitInnerWorld;
		M_Event.logicEvents [(int)LogicEvents.CameraAttachPointChange] -= OnNewAttachPoint;
		/*
		M_Event.logicEvents [(int)LogicEvents.Characters] -= OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
		M_Event.logicEvents [(int)LogicEvents.Credits] -= OnCredits;
		*/
		for (int i = 0; i < M_Event.logicEvents.Length; ++i) {
			M_Event.logicEvents [i] -= OnLogicEvent;
		}
	}

	void OnNewAttachPoint( LogicArg arg ){
		CameraAttachPoint point = (CameraAttachPoint)arg.sender;
		if (point != null) {
			transform.position = point.transform.position;
			Quaternion cameraTurn = Quaternion.FromToRotation (Camera.main.transform.forward, point.transform.forward);
			transform.rotation = point.transform.rotation;
		}
	}

	void OnLogicEvent( LogicArg arg ){
		m_stateMachine.OnEvent (arg.type);
	}

	void OnTransportToNewObject( LogicArg arg ){
		var obj = arg.GetMessage (Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT);
		if (obj is PasserBy) {
			m_stayPasserBy = (PasserBy)obj;
		}
	}

	void OnEnterInnerWorld(LogicArg arg ){
		MCharacter character = (MCharacter) arg.GetMessage (Global.EVENT_LOGIC_ENTERINNERWORLD_MCHARACTER);
		if (character != null)
			m_stayCharacter = character;
	}

	void OnExitInnerWorld(LogicArg arg ){
		MCharacter character = (MCharacter) arg.GetMessage (Global.EVENT_LOGIC_EXITINNERWORLD_MCHARACTER);
		if (m_stayCharacter == character)
			m_stayCharacter = null;
	}

	protected override void MUpdate (){
		base.MUpdate ();
		m_stateMachine.Update ();
	}

	void RaiseTheBody(){
		LogicArg arg = new LogicArg (this);
		arg.AddMessage ("isUp", true);
		M_Event.FireLogicEvent (LogicEvents.RaiseFallingCharacter, arg);
	}

	void LowerTheBody(){
		LogicArg arg = new LogicArg (this);
		arg.AddMessage ("down", true);
		M_Event.FireLogicEvent (LogicEvents.LowerFallingCharacter, arg);
	}

	public void OnCharacters (LogicArg arg ){

	}

	void OnEnd( LogicArg arg ){

	}

	void OnCredits(LogicArg arg){
		
	}

}
