using UnityEngine;
using System.Collections;

public class InputManager : MBehavior {

	public InputManager() { s_Instance = this; }
	public static InputManager Instance { get { return s_Instance; } }
	private static InputManager s_Instance;

	public LayerMask senseLayer;
	public static float DETECT_DISTANCE = 9999f;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (s_Instance == null)
			s_Instance = this;

		senseLayer = LayerMask.GetMask ("PasserBy","Focus","Collectable");
	}

	/// <summary>
	/// Save the focused object 
	/// only one focused object at a time
	/// </summary>
	private MObject m_focusObj;
	/// <summary>
	/// Save the focused object 
	/// only one focused object at a time
	/// </summary>
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

		/// make array of 2 for the 2 controllers. PC mouse input is same for [0] and [1]
		/// this will prefer the left controller if users are trying to select two things at once
		Ray[] centers = new Ray[2];
		centers = GetCenterRayCast ();

		/// if centers[1] is null, it should not return? 
		if (Physics.Raycast (centers[0], out hitInfo , DETECT_DISTANCE , senseLayer)) {
			lookObj = hitInfo.collider.gameObject.GetComponent<MObject> ();
		} else if (Physics.Raycast (centers[1], out hitInfo , DETECT_DISTANCE , senseLayer)) {
			lookObj = hitInfo.collider.gameObject.GetComponent<MObject> ();
		}
			
		/// call the focus function of the focus object
		if (lookObj != m_focusObj) {
			if (m_focusObj != null) {
				m_focusObj.OnOutofFocus ();
				FireOutofFocusObject (m_focusObj.gameObject);
			}
			m_focusObj = lookObj;
			if (m_focusObj != null) {
				m_focusObj.OnFocus ();
				FireFocusNewObject (m_focusObj.gameObject);
			}
		}

	}

	void OnGUI()
	{
	}

	/// <summary>
	/// Gets the position of focus point on the screen .
	/// </summary>
	/// <returns><c>true</c>, if screen position was gotten, <c>false</c> otherwise.</returns>
	/// <param name="screenPos">the position of the screen point.</param>
	public virtual Ray[] GetCenterRayCast( )
	{
		Ray[] centers = new Ray[2];
		centers [0] = Camera.main.ScreenPointToRay (new Vector2 (Screen.width / 2f, Screen.height / 2f));
		centers [1] = Camera.main.ScreenPointToRay (new Vector2 (Screen.width / 2f, Screen.height / 2f));;

		return  centers;
	}

	public virtual void VibrateController( int index )
	{

	}

	/// <summary>
	/// Fires the select object action (Input), call the M_Event.fireInput
	/// </summary>
	protected void FireSelectObject( ClickType clickType = ClickType.Mouse ) //bool left 
	{
		
		InputArg arg = new InputArg (this);
		arg.clickType = clickType;
		M_Event.FireInput (MInputType.SelectObject, arg);
	}

	/// <summary>
	/// Fires the transport action (Input), call the M_Event.fireInput
	/// </summary>
	protected void FireTransport()
	{
		InputArg arg = new InputArg (this);
		M_Event.FireInput (MInputType.Transport , arg);
	}

	/// <summary>
	/// Fires the focus on new object action (Input), call the M_Event.fireInput
	/// </summary>
	/// <param name="newObj">New object.</param>
	protected void FireFocusNewObject( GameObject newObj )
	{
		InputArg arg = new InputArg (this);
		arg.focusObject = newObj;
		M_Event.FireInput (MInputType.FocusNewObject , arg);
	}

	/// <summary>
	/// Fires the focus on new object action (Input), call the M_Event.fireInput
	/// </summary>
	/// <param name="newObj">New object.</param>
	protected void FireOutofFocusObject( GameObject obj )
	{
		InputArg arg = new InputArg (this);
		arg.focusObject = obj;
		M_Event.FireInput (MInputType.OutOfFocusObject , arg);
	}
		
}
