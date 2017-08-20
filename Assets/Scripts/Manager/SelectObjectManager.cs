using UnityEngine;
using System.Collections;

/// <summary>
/// Save the selected object and handle with the interaction of the selected object.
/// </summary>
public class SelectObjectManager : MBehavior {

	public SelectObjectManager() { s_Instance = this; }
	public static SelectObjectManager Instance { get { return s_Instance; } }
	private static SelectObjectManager s_Instance;

	private CollectableObj m_SelectObj;
	public CollectableObj SelectObj {
		get { return m_SelectObj; }
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.inputEvents [(int)MInputType.SelectObject] += OnSelectObject;
		M_Event.logicEvents [(int)LogicEvents.UnselectObject] += OnUnselectObject; 
	}
		
	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.inputEvents [(int)MInputType.SelectObject] -= OnSelectObject;
		M_Event.logicEvents [(int)LogicEvents.UnselectObject] -= OnUnselectObject; 
	}

	void OnUnselectObject( LogicArg arg )
	{
		CollectableObj cobj = (CollectableObj)arg.GetMessage (Global.EVENT_LOGIC_UNSELECT_COBJECT);
		if (cobj != null) {
			cobj.UnSelect ();
			if (cobj == m_SelectObj) {
				m_SelectObj = null;
			}
		}
	}
		
	/// <summary>
	/// React to the select object input event
	/// </summary>
	/// <param name="arg">Argument.</param>
	void OnSelectObject(InputArg arg)
	{
		// if player holds no object
		if (m_SelectObj == null) {
			Interactable[] focus = new Interactable[1]{  InputManager.Instance.FocusedObject};
			for ( int i = 0; i < focus.Length; i++ )
			{
				if ( focus[ i ] is Interactable) {
					Interactable cobj = (Interactable)focus[ i ];
					//Debug.Log ("Try Select");
					/*
					if (cobj.Select (arg.clickType)) {
						//Debug.Log ("Select success");
						m_SelectObj = cobj;
						MetricManagerScript.instance.AddToMatchList (Time.timeSinceLevelLoad + "; SelectObject " + "/n");
						LogicArg logicArg = new LogicArg (this);
						logicArg.AddMessage (Global.EVENT_LOGIC_SELECT_COBJECT, m_SelectObj);
						M_Event.FireLogicEvent (LogicEvents.SelectObject, logicArg);

					}
				*/
				}
			}
		} // unselect option available once object is held
		else if (m_SelectObj != null) {
			//print (" in on select obj and m_SelectObj = " + m_SelectObj.name);
			//MetricManagerScript.instance.AddToMatchList (Time.timeSinceLevelLoad + "; UnSelectObject " + "/n");
			LogicArg logicArg = new LogicArg (this);
			logicArg.AddMessage(Global.EVENT_LOGIC_UNSELECT_COBJECT, m_SelectObj);
			M_Event.FireLogicEvent (LogicEvents.UnselectObject, logicArg);
		}
			
	}

	static public void AttachToCamera( Transform trans, ClickType clickType )
	{
		if (LogicManager.Instance.GetHandTransform (clickType) != null) {
			trans.SetParent (LogicManager.Instance.GetHandTransform (clickType), true);
			if (LogicManager.Instance.VREnable) {
				trans.localPosition = Vector3.up * 0.1f + Vector3.forward * 0.1f;
			} else {
				trans.localPosition = Vector3.forward * 0.1f + Vector3.right * 0.1f;
			}
		}
	}

	static public void AttachToStayPasserBy( Transform trans )
	{
		if (LogicManager.Instance.StayTeleporter != null)
        {
			trans.SetParent (LogicManager.Instance.StayTeleporter.transform, true);
		}
	}

	static public void AttachToCharacter( Transform trans)
	{
		
	}

	/// <summary>
	/// Determines whether this instance is the selected object
	/// </summary>
	/// <returns><c>true</c> if this instance is the selected obj; otherwise, <c>false</c>.</returns>
	/// <param name="obj">Object.</param>
	public bool IsSelectObject( GameObject obj )
	{
		return m_SelectObj.gameObject == obj;
	}

}
