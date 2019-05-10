using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour 
{
	private Vector3 rightPositionOnDown = Vector3.zero;
	private Vector3 leftPositionOnDown = Vector3.zero;

	private Vector3 rightPositionNow = Vector3.zero;
	private Vector3 leftPositionNow = Vector3.zero;

	private bool pc = false;
	public GameObject player;

	Color color = new Color();

	// Use this for initialization
	void Start () 
	{
		if (player.activeInHierarchy) { pc = true; }
		ColorUtility.TryParseHtmlString ("#4347CC", out color);
		color *= 1.3f;
	}

	// Update is called once per frame
	//something is disabling mouse coordinates when i'm in the play window 
	void Update () 
	{
		if (pc) 
		{
			//if using mouse input here, cursorLocked must be commented out at the top of player controller
			rightPositionNow = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
			//print ("rightPositionNow update " + rightPositionNow);
			leftPositionNow = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);

			//alternate way based on player's gaze rather than mouse position
			rightPositionNow = Camera.main.transform.forward; // player.transform.forward;
			leftPositionNow = Camera.main.transform.forward; //player.transform.forward;
		} 
		else if (!pc) 
		{
			rightPositionNow = new Vector3(VRInputController.Instance.rightController.transform.position.x, VRInputController.Instance.rightController.transform.position.y, VRInputController.Instance.rightController.transform.position.z);
			leftPositionNow = new Vector3(VRInputController.Instance.leftController.transform.position.x, VRInputController.Instance.leftController.transform.position.y, VRInputController.Instance.leftController.transform.position.z);

		}
	}

	public void SetRightPositionDown()
	{
		if (pc) 
		{
			rightPositionOnDown = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
			//print ("rightPositionOnDown set " + rightPositionOnDown);

			//alternate way based on player's gaze rather than mouse position
			rightPositionOnDown = Camera.main.transform.forward; //player.transform.forward;

		} 
		else if (!pc) 
		{
			rightPositionOnDown = new Vector3(VRInputController.Instance.rightController.transform.position.x, VRInputController.Instance.rightController.transform.position.y, VRInputController.Instance.rightController.transform.position.z);
		}
	}

	public void SetLeftPositionDown()
	{
		if (pc) 
		{
			leftPositionOnDown = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);

			//alternate way based on player's gaze rather than mouse position
			leftPositionOnDown = Camera.main.transform.forward; //player.transform.forward;
		} 
		else if (!pc) 
		{
			leftPositionOnDown = new Vector3(VRInputController.Instance.leftController.transform.position.x, VRInputController.Instance.leftController.transform.position.y, VRInputController.Instance.leftController.transform.position.z);
		}
	}

	//rightDown: True if right trigger down or right mouse button down
	public bool RightDown()
	{
		if (pc) 
		{
			if (Input.GetMouseButtonDown (1)) { return true; } 
		} 
		else if (!pc) 
		{
			if (VRInputController.Instance.ReceivedRightButtonDownSignal ()) { return true; }
		}

		return false;
	}

	//leftDown: True if left trigger down or left mouse button down 
	public bool LeftDown()
	{
		if (pc) 
		{
			if (Input.GetMouseButtonDown (0)) { return true; } 
		} 
		else if (!pc) 
		{
			if (VRInputController.Instance.ReceivedLeftButtonDownSignal ()) { return true; }
		}

		return false;
	}

	//leftPress: True if right trigger pressed or right mouse button pressed 
	public bool RightPress()
	{
		if (pc) 
		{
			if (Input.GetMouseButton (1)) { return true; }
		} 
		else if (!pc) 
		{
			if (VRInputController.Instance.ReceivedRightButtonPressSignal ()) { return true; }
		}
		return false;
	}

	//rightPress: True if left trigger pressed or right mouse button pressed 
	public bool LeftPress()
	{
		if (pc) 
		{
			if (Input.GetMouseButton (0)) { return true; }
		} 
		else if (!pc) 
		{
			if ( VRInputController.Instance.ReceivedLeftButtonPressSignal() ) { return true; } 
		}
		return false;
	}

	//rightPadOrEDown: True if right pad down or E down 
	public bool RightPadOrEDown()
	{
		if (pc) 
		{
			if (Input.GetKeyDown (KeyCode.E)) { return true; }
		} 
		else if (!pc) 
		{
			if (VRInputController.Instance.ReceivedRightPadPressedSignal ()) { return true; }
		}
		return false;
		
	}

	//leftPadOrQDown: True if left pad down or Q down 
	public bool LeftPadOrQDown()
	{
		if (pc) 
		{
			if (Input.GetKeyDown (KeyCode.Q)) { return true; }

		} 
		else if (!pc) 
		{
			if (VRInputController.Instance.ReceivedLeftPadPressedSignal ()) { return true; }
		}
		return false;
	}

	//hover: True if left controller pointing to obj parameter or mouse hovering over obj parameter
	public bool Hover( GameObject obj )
	{
		Ray ray;
		RaycastHit hit;

		if (pc) 
		{
			//ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);//(player.transform.position, player.transform.forward);

			if (Physics.Raycast (ray, out hit, 50.0f)) //should modify the distance of "50" do your context
			{
				if (hit.collider == obj.GetComponent<Collider> ()) { return true; }
			}

		} 
		else if (!pc) 
		{
			ray = new Ray (VRInputController.Instance.leftController.transform.position, VRInputController.Instance.leftController.transform.forward);
			if (Physics.Raycast (ray, out hit)) 
			{
				if (hit.collider == obj.GetComponent<Collider> ()) { return true; }
			}
		}
		return false;
	}

	//LeftControllerMovement: returns Vector3 movement of left controller from last frame to this frame. 
	//increases scale of near/far based on the direction of the controller?
	public Vector3 LeftControllerMovement()
	{

		if (pc) 
		{
			//assuming leftPositionPress, set in LeftDown, is a frame or so ahead of when this is called
			Vector3 move = (leftPositionNow - leftPositionOnDown)/10.0f;
//			print ("L: rightPositionOnDown " + rightPositionOnDown);
//			print ("L: inputmousenow" + leftPositionNow);
//			print ("L: move " + move);
			leftPositionOnDown = leftPositionNow;
			return move;
		} 
		else 
		{
			Vector3 move = leftPositionNow - leftPositionOnDown;
			leftPositionOnDown = leftPositionNow;
			return move;
		}
			
	}

	//scaleSelection: when called, stores start position of right controller. returns float distance between start position and current position 
	public float RightControllerDistance()
	{

		if (pc) 
		{
			float distance = Vector3.Distance (rightPositionOnDown, rightPositionNow);
//			print ("R: rightPositionOnDown " + rightPositionOnDown);
//			print ("R: inputmousenow" + rightPositionNow);
//			print ("R: distance " + distance);
			distance /= 100.0f; //set max distance or armspan to generate values between 0 and 1;
			distance *= distance; //for a parabolic motion
			distance = Mathf.Lerp(0.0f, 500.0f, distance);
			return distance;
		} 
		else
		{
			float distance = Vector3.Distance (rightPositionOnDown, rightPositionNow);
			distance /= 100.0f; //set max distance or armspan to generate values between 0 and 1;
			//distance *= distance; //for a parabolic motion
			distance = Mathf.Lerp(0.0f, 500.0f, distance);
			return distance;
		}

	}

}