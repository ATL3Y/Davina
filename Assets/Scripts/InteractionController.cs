using UnityEngine;
using System.Collections;

public class InteractionController : MonoBehaviour 
{
	public GameObject myGameObj;

	void Start () 
	{
	
	}
	

	void Update () 
	{
		int count = 0;

		if (GetComponent<InputController> ().Hover (myGameObj)) 
		{
			myGameObj.GetComponent<SelectionLightController> ().isHovered = true;

			//Do whatever you want here. This is an example:
			if (GetComponent<InputController> ().LeftPadOrQDown ()) 
			{
				myGameObj.transform.position -= new Vector3 (.1f, 0f, 0f);
			}

		} 
		else 
		{
			myGameObj.GetComponent<SelectionLightController> ().isHovered = false;
		}

		//selection: True if hover + leftPress
		if (myGameObj.GetComponent<SelectionLightController> ().isHovered && GetComponent<InputController> ().LeftPress()) 
		{
			myGameObj.GetComponent<SelectionLightController> ().isHovered = false;
			myGameObj.GetComponent<SelectionLightController> ().isSelected = true;
			if (count == 0) 
			{
				GetComponent<InputController> ().SetLeftPositionDown ();
				count++;
			}
		}
		if (myGameObj.GetComponent<SelectionLightController> ().isSelected && !GetComponent<InputController> ().LeftPress())
		{
			myGameObj.GetComponent<SelectionLightController> ().isSelected = false;
			myGameObj.GetComponent<SelectionLightController> ().isHovered = true;
			count = 0;
		}

		if (myGameObj.GetComponent<SelectionLightController> ().isSelected) 
		{
			//this only works with the vive right now
			//movement: adds change in left controller position to the sphere position 
			//myGameObj.transform.position += GetComponent<InputController> ().LeftControllerMovement(); 

			//Do whatever you want here. This is an example:
			myGameObj.transform.position += new Vector3 (.1f, 0f, 0f);



			//scale: enables right trigger to change the scale of the sphere
//			if (GetComponent<InputController> ().RightPress()) 
//			{
//				if (count == 1) 
//				{
//					GetComponent<InputController> ().SetRightPositionDown ();
//					count++;
//				}

				//this only works with the vive right now
				//float distance = GetComponent<InputController> ().RightControllerDistance (); 
				//myGameObj.transform.localScale = new Vector3(distance, distance, distance);
//			}
		}
	
	}
}
