using UnityEngine;
using System.Collections;

public class InputManager : MBehavior {

	public InputManager() { s_Instance = this; }
	public static InputManager Instance { get { return s_Instance; } }
	private static InputManager s_Instance;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (s_Instance == null)
			s_Instance = this;
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
