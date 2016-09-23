using UnityEngine;
using System.Collections;

public class InputManager : MBehavior {

	public InputManager() { s_Instance = this; }
	public static InputManager Instance { get { return s_Instance; } }
	private static InputManager s_Instance;

	public LayerMask senseLayer;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (s_Instance == null)
			s_Instance = this;

		senseLayer = LayerMask.GetMask ("PasserBy","Focus","Collectable");
	}


	/// <summary>
	/// Save the focused object 
	/// </summary>
	private MObject m_focusObj;
	public MObject FocusedObject
	{
		get { return m_focusObj; }
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		/// check if the the camera look at a Mobject
		MObject lookObj = null;
		RaycastHit hitInfo;
		if (Physics.Raycast (GetCenterRayCast (), out hitInfo , 1000f , senseLayer)) {
			lookObj = hitInfo.collider.gameObject.GetComponent<MObject> ();
		}
		/// call the focus function of the focus object
		if (lookObj != m_focusObj) {
			if (m_focusObj != null)
				m_focusObj.OnOutofFocus ();
			m_focusObj = lookObj;
			if (m_focusObj != null) {
				m_focusObj.OnFocus ();
				FireFocusNewObject (m_focusObj.gameObject);
			}
		}
	}

	/// <summary>
	/// Gets the position of focus point on the screen .
	/// </summary>
	/// <returns><c>true</c>, if screen position was gotten, <c>false</c> otherwise.</returns>
	/// <param name="screenPos">the position of the screen point.</param>
	public virtual Ray GetCenterRayCast( )
	{
		return  Camera.main.ScreenPointToRay (new Vector2 (Screen.width / 2f, Screen.height / 2f));
	}

	protected void FireSelectObject()
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.SelectObject;
		M_Event.FireInput (arg);
	}

	protected void FireTransport()
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.Transport;
		M_Event.FireInput (arg);
	}

	protected void FireFocusNewObject( GameObject newObj )
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.FocusNewObject;
		arg.focusObject = newObj;
		M_Event.FireInput (arg);
	}
}
