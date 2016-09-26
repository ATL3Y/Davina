using UnityEngine;
using System.Collections;

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
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.inputEvents [(int)MInputType.SelectObject] -= OnSelectObject;
	}

	void OnSelectObject(InputArg arg)
	{
		// throw away old object
		if (m_SelectObj != null) {
			m_SelectObj.UnSelect ();

			LogicArg logicArg = new LogicArg(this);
			logicArg.AddMessage(Global.EVENT_LOGIC_THROW_COBJECT,m_SelectObj);
			M_Event.FireLogicEvent (LogicEvents.ThrowAway, logicArg);

			m_SelectObj = null;
		}
		// select new object
		MObject focus = InputManager.Instance.FocusedObject;
		if (focus != null && focus is CollectableObj) {
			CollectableObj cobj = (CollectableObj)focus;

			if (m_SelectObj != null)
				m_SelectObj.UnSelect ();
			m_SelectObj = cobj;

			m_SelectObj.Select ();

			LogicArg logicArg = new LogicArg(this);
			logicArg.AddMessage(Global.EVENT_LOGIC_SELECT_COBJECT,m_SelectObj);
			M_Event.FireLogicEvent (LogicEvents.SelectObject, logicArg);

		}
	}

	static public void AttachToCamera( Transform trans)
	{
		if (LogicManager.Instance.GetHandTransform () != null) {
			trans.SetParent (LogicManager.Instance.GetHandTransform (), true);
			trans.localPosition = Vector3.forward * 0.2f + Vector3.right * 0.2f;
		}
	}

	static public void AttachToStayPasserBy( Transform trans )
	{
		if (LogicManager.Instance.StayPasserBy != null) {

			trans.SetParent (LogicManager.Instance.StayPasserBy.transform, true);
		}
	}

}
