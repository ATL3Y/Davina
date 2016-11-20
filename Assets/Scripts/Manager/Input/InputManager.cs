using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MBehavior {

	public InputManager() { s_Instance = this; }
	public static InputManager Instance { get { return s_Instance; } }
	private static InputManager s_Instance;

	public LayerMask senseLayer;
	public static float DETECT_DISTANCE = 9999f;
    private MObject[ ] collectableObjects = new MObject[ 200 ];
    private int collectableObjectsCount = 0;

	public class Hand
	{
		public MObject lastFocusedObject;
		public MObject focusedObject;
		public MObject heldObject;
		public Ray ray;
		public Bounds bounds;
	}
	public Hand rightHand;
	public Hand leftHand;
	private List< Hand > m_hands;

	/// <summary>
	/// Save the focused object 
	/// only one focused object at a time
	/// </summary>
	//private List<MObject> m_focusObj;

	protected override void MAwake ()
	{
		base.MAwake ();

		//m_focusObj = new List<MObject> ();
		leftHand = new Hand();
		rightHand = new Hand ();

		m_hands = new List< Hand > ();
		m_hands.Add (leftHand);
		m_hands.Add (rightHand);

		if (s_Instance == null)
			s_Instance = this;

		senseLayer = LayerMask.GetMask ("PasserBy","Focus","Collectable");
	}

    // recheched each level from the logic manager
    protected void loadCollectableObjects( )
    {
        //clear
        for( int i=0; i< collectableObjects.Length; i++ )
        {
            collectableObjects[ i ] = null;
        }

        int j = 0;
        foreach ( MatchObj g in GameObject.FindObjectsOfType<MatchObj>( ) )
        {
            collectableObjects[ j ] = g;
            j++;
        }
        foreach ( HoleObject g in GameObject.FindObjectsOfType<HoleObject>( ) )
        {
            collectableObjects[ j ] = g;
            j++;
        }

        collectableObjectsCount = j;
    }

	/// <summary>
	/// Save the focused object 
	/// only one focused object at a time
	/// </summary>
	public MObject FocusedObject;
	//{
		//get { 
			//List< MObject > returnValues = new List< MObject >();
		//	if ( rightHand.focusedObject != null ) returnValues.Add( rightHand.focusedObject );
			//if ( leftHand.focusedObject != null ) returnValues.Add( leftHand.focusedObject );
			//return returnValues;
		//}

	//}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		return;
		/// check if the the camera look at a Mobject
		//List<MObject> lookObj = new List<MObject>();

		/// make array of 2 for the 2 controllers. PC mouse input is same for [0] and [1]
		Ray[] centers = new Ray[2];
		centers = GetCenterRayCast ();
		leftHand.ray = centers [0];
		rightHand.ray = centers [1];

		leftHand.bounds = ViveInputController.Instance.boundsLeftController;
		rightHand.bounds = ViveInputController.Instance.boundsRightController;

		for (int i = 0; i < m_hands.Count; i++)
		{
			//bounds check
			for ( int j=0; j<collectableObjectsCount; j++ )
			{
				if( collectableObjects[ j ].GetComponent<Collider>( ).bounds.Intersects( m_hands[ i ].bounds ) )
				{
					//lookObj.Add(collectableObjects[ j ]);
					m_hands[ i ].focusedObject = collectableObjects[ j ];
					break;
				}
			}

			if (m_hands[ i ].focusedObject == null) 
			{
				RaycastHit hitInfo = new RaycastHit();
				if (Physics.Raycast (m_hands[ i ].ray, out hitInfo, DETECT_DISTANCE , senseLayer)) 
				{
					//lookObj.Add(hitInfo.collider.gameObject.GetComponent<MObject> ());
					m_hands[ i ].focusedObject = hitInfo.collider.gameObject.GetComponent<MObject> ();
					break;
				} 
			}

			if (m_hands[ i ].focusedObject != m_hands[ i ].lastFocusedObject) {
				if (m_hands[ i ].lastFocusedObject != null) {
					m_hands[ i ].lastFocusedObject.OnOutofFocus ();
					FireOutofFocusObject (m_hands[ i ].lastFocusedObject.gameObject);
				}
				if (m_hands [i].focusedObject != null) {
					m_hands [i].focusedObject.OnFocus ();
					MetricManagerScript.instance.AddToMatchList (Time.timeSinceLevelLoad + "; name of new focus obj = " + m_hands [i].focusedObject.gameObject.name + "/n");
					FireFocusNewObject (m_hands [i].focusedObject.gameObject);
				}
				m_hands [i].lastFocusedObject = m_hands [i].focusedObject;
			}
			//ray
		}
		/*
        // left controller: check bounds
        for ( int i=0; i<collectableObjectsCount; i++ )
        {
			if( collectableObjects[ i ].GetComponent<Collider>( ).bounds.Intersects( ViveInputController.Instance.boundsLeftController ) )
            {
				lookObj.Add(collectableObjects[ i ]);
            }
        } 

		// left controller didn't intersect bounds, so check raycast
		if (lookObj [0] = null) 
		{
			RaycastHit hitInfo = new RaycastHit();
			if (Physics.Raycast (centers[0], out hitInfo, DETECT_DISTANCE , senseLayer)) 
			{
				lookObj.Add(hitInfo.collider.gameObject.GetComponent<MObject> ());
			} 
		}

		/// call the focus function of the focus object



		/// call the focus function of the focus object
		if (lookObj != m_focusObj) {
			if (m_focusObj != null) {
				m_focusObj.OnOutofFocus ();
				FireOutofFocusObject (m_focusObj.gameObject);
			}
			m_focusObj = lookObj;
			if (m_focusObj != null) {
				m_focusObj.OnFocus ();
				MetricManagerScript.instance.AddToMatchList (Time.timeSinceLevelLoad + "; name of new focus obj = " + m_focusObj.gameObject.name + "/n");
				FireFocusNewObject (m_focusObj.gameObject);
			}
		}
			*/


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
	protected void FireSelectObject( ClickType clickType ) 
	{
		
		InputArg arg = new InputArg (this);
		arg.clickType = clickType;
		M_Event.FireInput (MInputType.SelectObject, arg);
	}

	/// <summary>
	/// Fires the transport action (Input), call the M_Event.fireInput
	/// </summary>
	public void FireTransport()
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
