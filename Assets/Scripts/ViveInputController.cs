using UnityEngine;
using System.Collections;
using Valve.VR;
using System.Collections.Generic;

public class ViveInputController : MonoBehaviour 
{

    SteamVR_ControllerManager controllerManager;
	public static ViveInputController instance;

    public bool useViveInput = true;

    int leftControllerIndex = -1, rightControllerIndex = -1;

    public GameObject leftController;
    public GameObject rightController;
    public Bounds boundsLeftController;
    public Bounds boundsRightController;
    public bool triggerRight; //true after down until down again 
	public bool triggerLeft;

    BoundsDetector bounds;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {

        triggerRight = false;
		triggerLeft = false;

        if (controllerManager == null)
            controllerManager = GameObject.Find("[CameraRig]").GetComponent<SteamVR_ControllerManager>();

        boundsLeftController = new Bounds(Vector3.zero, new Vector3(0.2f, 0.2f, 0.2f));
        boundsRightController = new Bounds(Vector3.zero, new Vector3(0.2f, 0.2f, 0.2f));

    }

    bool GetControllerIndices()
    {
        if (controllerManager != null)
        {
			leftControllerIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost); //controllerManager.leftIndex; 
			rightControllerIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost); //controllerManager.rightIndex; 
        }
        return leftControllerIndex != -1 || rightControllerIndex != -1;
    }

	//triggerLeftPressed returns true while trigger Left pressed down
	public bool ReceivedLeftButtonPressSignal()
	{
		if (useViveInput && GetControllerIndices())
		{
			if (leftControllerIndex > -1 && SteamVR_Controller.Input(leftControllerIndex).GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
				return true;
		}

		return false;
	}
		
	//triggerRightPressed returns true while trigger Left pressed down
	public bool ReceivedRightButtonPressSignal()
	{
		if (useViveInput && GetControllerIndices())
		{
			if (rightControllerIndex > -1 && SteamVR_Controller.Input(rightControllerIndex).GetPress(EVRButtonId.k_EButton_SteamVR_Trigger))
				return true;
		}

		return false;
	}

	//gets one instance of pressing button down
    public bool ReceivedLeftButtonDownSignal()
    {
        if (useViveInput && GetControllerIndices())
        {
            if (leftControllerIndex > -1 && SteamVR_Controller.Input(leftControllerIndex).GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger))
                return true;
        }

        return false;
    }

	//gets one instance of pressing button down
    public bool ReceivedRightButtonDownSignal()
    {
        //Debug.Log("in LBDS");
        //print(useViveInput + " " + GetControllerIndices());
        if (useViveInput && GetControllerIndices())
        {
            //Debug.Log("in UVI and GCI");
            if (rightControllerIndex > -1 && SteamVR_Controller.Input(rightControllerIndex).GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger))
                return true;
        }

        return false;
    }

    bool ReceivedLeftButtonUpSignal()
    {
        if (useViveInput && GetControllerIndices())
        {
            if (leftControllerIndex > -1 && SteamVR_Controller.Input(leftControllerIndex).GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
                return true;
        }

        return false;
    }

    bool ReceivedRightButtonUpSignal()
    {
        if (useViveInput && GetControllerIndices())
        {
            if (rightControllerIndex > -1 && SteamVR_Controller.Input(rightControllerIndex).GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
                return true;
        }

        return false;
    }

	public bool ReceivedRightGripPressedSignal()
	{
		if (useViveInput && GetControllerIndices ()) 
		{
			if (rightControllerIndex > -1 && SteamVR_Controller.Input (rightControllerIndex).GetPress(EVRButtonId.k_EButton_Grip)) 
				return true;
		}

		return false;
	}

	public bool ReceivedLeftGripPressedSignal()
	{
		if (useViveInput && GetControllerIndices ()) 
		{
			if (leftControllerIndex > -1 && SteamVR_Controller.Input (leftControllerIndex).GetPress (EVRButtonId.k_EButton_Grip))
				return true;
		}

		return false;
	}

	public float DistanceBetweenControllers()
	{
		if (useViveInput && GetControllerIndices ()) 
		{
			float distance = Vector3.Distance (rightController.transform.position, leftController.transform.position);
			return distance;
		}

		return 1.0f; 
	}

	public Vector2 RightPadSwipeAxis()
	{
		Vector2 rightPadAxis = new Vector2 (2,2); //choose a value out of range for false

		if (useViveInput && GetControllerIndices ()) 
		{
			if (rightControllerIndex > -1)
			{
				rightPadAxis = SteamVR_Controller.Input (rightControllerIndex).GetAxis (EVRButtonId.k_EButton_SteamVR_Touchpad); //goes from -1(L) to +1(R)
			}
		}
		return rightPadAxis;
	}

	public Vector2 LeftPadSwipeAxis()
	{
		Vector2 leftPadAxis = new Vector2 (2,2); //choose a value out of range for false

		if (useViveInput && GetControllerIndices ()) 
		{
			if (leftControllerIndex > -1)
			{
				leftPadAxis = SteamVR_Controller.Input (leftControllerIndex).GetAxis (EVRButtonId.k_EButton_SteamVR_Touchpad); //goes from -1(L) to +1(R)
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
		if (useViveInput && GetControllerIndices())
		{
			if (rightControllerIndex > -1 && SteamVR_Controller.Input(rightControllerIndex).GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad))
				return true;
		}

		return false;
	}

	//gets one instance of pressing pad down
	public bool ReceivedLeftPadDownSignal()
	{
		if (useViveInput && GetControllerIndices())
		{
			if (leftControllerIndex > -1 && SteamVR_Controller.Input(leftControllerIndex).GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad))
				return true;
		}

		return false;
	}

	public bool ReceivedRightPadPressedSignal()
	{
		if (useViveInput && GetControllerIndices ()) 
		{
			if (rightControllerIndex > -1 && SteamVR_Controller.Input (rightControllerIndex).GetPress(EVRButtonId.k_EButton_SteamVR_Touchpad)) 
				return true;
		}

		return false;
	}

	public bool ReceivedLeftPadPressedSignal()
	{
		if (useViveInput && GetControllerIndices ()) 
		{
			if (leftControllerIndex > -1 && SteamVR_Controller.Input (leftControllerIndex).GetPress (EVRButtonId.k_EButton_SteamVR_Touchpad))
				return true;
		}

		return false;
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
