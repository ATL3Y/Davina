using UnityEngine;
using System.Collections;
using Valve.VR;
using System.Collections.Generic;

public class VRInputController : MonoBehaviour 
{

	[SerializeField] SteamVR_ControllerManager controllerManager;
	private static VRInputController m_Instance;
	public static VRInputController Instance { get { return m_Instance; } }

    public bool useViveInput = true;

    public int leftControllerIndex = -1, rightControllerIndex = -1;

    public GameObject leftController;
    public GameObject rightController;
    public Bounds boundsLeftController;
    public Bounds boundsRightController;
    public bool triggerRight; //true after down until down again 
	public bool triggerLeft;

    BoundsDetector bounds;

    public bool OVREnabled { get; set; }

    // Handle OVR vs Vive
    [SerializeField]
    GameObject ovrLeftHand;
    [SerializeField]
    GameObject ovrRightHand;

    void Awake()
    {
        m_Instance = this;
        if ( LogicManager.Instance.ovrEnable )
        {
            OVREnabled = true;
            leftController = ovrLeftHand;
            rightController = ovrRightHand;
        }
    }

    // Use this for initialization
    void Start () {

        triggerRight = false;
		triggerLeft = false;

        if ( OVREnabled )
        {

        }
        else
        {
            if ( controllerManager == null )
                controllerManager = GameObject.Find ( "[CameraRig]" ).GetComponent<SteamVR_ControllerManager> ( );
        }

        boundsLeftController = new Bounds(Vector3.zero, new Vector3(0.2f, 0.2f, 0.2f));
        boundsRightController = new Bounds(Vector3.zero, new Vector3(0.2f, 0.2f, 0.2f));

    }

    bool GetControllerIndices()
    {
        if (controllerManager != null)
        {
			leftControllerIndex = (int) controllerManager.leftIndex; // SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Leftmost);
			rightControllerIndex = (int) controllerManager.rightIndex; //SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Rightmost);
        }
        return leftControllerIndex != -1 || rightControllerIndex != -1; 
    }

	//triggerLeftPressed returns true while trigger Left pressed down
	public bool ReceivedLeftButtonPressSignal()
	{
        if ( OVREnabled )
        {
            return OVRInput.Get ( OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch ) >= 0.1f;
        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( leftControllerIndex > -1 && SteamVR_Controller.Input ( leftControllerIndex ).GetPress ( EVRButtonId.k_EButton_SteamVR_Trigger ) )
                    return true;
            }
        }


		return false;
	}
		
	//triggerRightPressed returns true while trigger Left pressed down
	public bool ReceivedRightButtonPressSignal()
	{
        if ( OVREnabled )
        {
            return OVRInput.Get ( OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch ) >= 0.1f;

        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( rightControllerIndex > -1 && SteamVR_Controller.Input ( rightControllerIndex ).GetPress ( EVRButtonId.k_EButton_SteamVR_Trigger ) )
                    return true;
            }
        }

		return false;
	}

	//gets one instance of pressing button down
    public bool ReceivedLeftButtonDownSignal()
    {
        if ( OVREnabled )
        {
            return OVRInput.Get ( OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch ) >= 0.9f;
        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( leftControllerIndex > -1 && SteamVR_Controller.Input ( leftControllerIndex ).GetPressDown ( EVRButtonId.k_EButton_SteamVR_Trigger ) )
                    //MetricManagerScript.instance.AddToMatchList (Time.timeSinceLevelLoad + "; Position = " + transform.position + "; ReceivedLeftButtonDownSignal() (Trigger)");
                    return true;
            }
        }

        return false;
    }

	//gets one instance of pressing button down
    public bool ReceivedRightButtonDownSignal()
    {
        if ( OVREnabled )
        {
            return OVRInput.Get ( OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch ) >= 0.9f;
        }
        else
        {
            //Debug.Log("in LBDS");
            //print(useViveInput + " " + GetControllerIndices());
            if ( useViveInput && GetControllerIndices ( ) )
            {
                //Debug.Log("in UVI and GCI");
                if ( rightControllerIndex > -1 && SteamVR_Controller.Input ( rightControllerIndex ).GetPressDown ( EVRButtonId.k_EButton_SteamVR_Trigger ) )
                    //MetricManagerScript.instance.AddToMatchList (Time.timeSinceLevelLoad + "; Position = " + transform.position + "; ReceivedRightButtonDownSignal() (Trigger)");
                    return true;
            }
        }

        return false;
    }

    public bool ReceivedLeftButtonUpSignal()
    {
        if ( OVREnabled )
        {
            return OVRInput.Get ( OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch ) < 0.1f;
        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( leftControllerIndex > -1 && SteamVR_Controller.Input ( leftControllerIndex ).GetPressUp ( EVRButtonId.k_EButton_SteamVR_Trigger ) )
                    return true;
            }
        }

        return false;
    }

    public bool ReceivedRightButtonUpSignal()
    {
        if ( OVREnabled )
        {
            return OVRInput.Get ( OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch ) < 0.1f;
        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( rightControllerIndex > -1 && SteamVR_Controller.Input ( rightControllerIndex ).GetPressUp ( EVRButtonId.k_EButton_SteamVR_Trigger ) )
                    return true;
            }
        }

        return false;
    }

	public bool ReceivedRightGripPressedSignal()
	{
        if ( OVREnabled )
        {

        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( rightControllerIndex > -1 && SteamVR_Controller.Input ( rightControllerIndex ).GetPress ( EVRButtonId.k_EButton_Grip ) )
                    return true;
            }
        }

		return false;
	}

	public bool ReceivedLeftGripPressedSignal()
	{
        if ( OVREnabled )
        {

        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( leftControllerIndex > -1 && SteamVR_Controller.Input ( leftControllerIndex ).GetPress ( EVRButtonId.k_EButton_Grip ) )
                    return true;
            }
        }

		return false;
	}

	public float DistanceBetweenControllers()
	{
        if ( OVREnabled )
        {
            float distance = Vector3.Distance (rightController.transform.position, leftController.transform.position);
            return distance;
        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                float distance = Vector3.Distance (rightController.transform.position, leftController.transform.position);
                return distance;
            }
        }

		return 1.0f; 
	}

	public Vector2 RightPadSwipeAxis()
	{
        Vector2 rightPadAxis = new Vector2 (2,2); //choose a value out of range for false
        if ( OVREnabled )
        {

        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( rightControllerIndex > -1 )
                {
                    rightPadAxis = SteamVR_Controller.Input ( rightControllerIndex ).GetAxis ( EVRButtonId.k_EButton_SteamVR_Touchpad ); //goes from -1(L) to +1(R)
                }
            }
        }

		return rightPadAxis;
	}

	public Vector2 LeftPadSwipeAxis()
	{
        Vector2 leftPadAxis = new Vector2 (2,2); //choose a value out of range for false

        if ( OVREnabled )
        {

        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( leftControllerIndex > -1 )
                {
                    leftPadAxis = SteamVR_Controller.Input ( leftControllerIndex ).GetAxis ( EVRButtonId.k_EButton_SteamVR_Touchpad ); //goes from -1(L) to +1(R)
                }
            }
        }

		return leftPadAxis;
	}

	public float LeftControllerPointsTo(Vector3 target)
	{
        Vector3 toTarget = target - leftController.transform.position;
		float angle = Vector3.Dot (leftController.transform.up, toTarget);
		return angle;
	}

	public float RightControllerPointsTo(Vector3 target)
	{
		Vector3 toTarget = target - rightController.transform.position;
		float angle = Vector3.Dot (rightController.transform.up, toTarget);
		return angle;
	}

	//gets one instance of pressing pad down
	public bool ReceivedRightPadDownSignal()
	{
        if ( OVREnabled )
        {

        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( rightControllerIndex > -1 && SteamVR_Controller.Input ( rightControllerIndex ).GetPressDown ( EVRButtonId.k_EButton_SteamVR_Touchpad ) )
                    //MetricManagerScript.instance.AddToMatchList (Time.timeSinceLevelLoad + "; Position = " + transform.position + "; ReceivedRightPadDownSignal()");
                    return true;
            }
        }

		return false;
	}

	//gets one instance of pressing pad down
	public bool ReceivedLeftPadDownSignal()
	{
        if ( OVREnabled )
        {

        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( leftControllerIndex > -1 && SteamVR_Controller.Input ( leftControllerIndex ).GetPressDown ( EVRButtonId.k_EButton_SteamVR_Touchpad ) )
                    //MetricManagerScript.instance.AddToMatchList (Time.timeSinceLevelLoad + "; Position = " + transform.position + "; ReceivedLeftPadDownSignal()");
                    return true;
            }
        }

		return false;
	}

	public bool ReceivedRightPadPressedSignal()
	{
        if ( OVREnabled )
        {

        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( rightControllerIndex > -1 && SteamVR_Controller.Input ( rightControllerIndex ).GetPress ( EVRButtonId.k_EButton_SteamVR_Touchpad ) )
                    return true;
            }
        }

		return false;
	}

	public bool ReceivedLeftPadPressedSignal()
	{
        if ( OVREnabled )
        {

        }
        else
        {
            if ( useViveInput && GetControllerIndices ( ) )
            {
                if ( leftControllerIndex > -1 && SteamVR_Controller.Input ( leftControllerIndex ).GetPress ( EVRButtonId.k_EButton_SteamVR_Touchpad ) )
                    return true;
            }
        }


		return false;
	}

	public void VibrateController( int index )
	{
        if ( OVREnabled )
        {

        }
        else
        {
            //Debug.Log ("Vibrate vive input controller called index = " + index);
            SteamVR_Controller.Input ( index ).TriggerHapticPulse ( ( ushort ) 3999 ); // microseconds, should omit 2nd param
        }

	}
		
    // Update is called once per frame
    void Update () {

        boundsRightController.center = rightController.transform.position;
        boundsLeftController.center = leftController.transform.position;

		//triggerLeft stays true untill next Left Button Down
        if (ReceivedLeftButtonDownSignal()) 
        {
			triggerLeft = true;
        }
		if (ReceivedLeftButtonUpSignal ()) 
		{
			triggerLeft = false;
		}

		//triggerRight stays true untill next Left Button Down
        if (ReceivedRightButtonDownSignal())
        {
            triggerRight = true;
        }

        if (ReceivedRightButtonUpSignal())
        {
            triggerRight = false;
        }

    }
}
