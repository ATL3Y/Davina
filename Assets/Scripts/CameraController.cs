using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	private float sensitivity = 50.0f;

	public float smooth = 40.0f; // high number = low smooth = close to 0 (current position), low number = high smooth = close to 1 (next position)
	private PlayerController player;
	float angleX;

	public void Start () 
	{

		player = GameObject.FindObjectOfType< PlayerController > ();
		transform.position = player.transform.position + Vector3.up + player.transform.forward;
		transform.forward = player.transform.forward; // orient camera in direction of player
	}

	public void Update ( )
	{
		//transform.position = player.transform.position + Vector3.up; //attach camera to player's head - no rise

		//set vertical rotation based on mouse Y input 
		angleX += -Input.GetAxisRaw ("Mouse Y") * sensitivity * Time.deltaTime;
		Vector3 newForward = Quaternion.AngleAxis (angleX, player.transform.right) * Vector3.forward;
		//newForward = newForward.normalized;

		//AxKDebugLines.AddFancySphere (transform.position + newForward, Input.GetAxisRaw ("Mouse Y"), Color.black, 0);
		//AxKDebugLines.AddLine (transform.position, transform.position + newForward, Color.red, 0);

		//newForward = new Vector3 (player.transform.forward.x, newForward.y, player.transform.forward.z);
		//AxKDebugLines.AddLine (transform.position, transform.position + newForward, Color.green, 0);
		//AxKDebugLines.AddFancySphere (transform.position + newForward, newForward.y, Color.white, 0);
		Vector3 right = Vector3.Cross( newForward, Vector3.up ).normalized;
		Vector3 up = -Vector3.Cross (newForward, right).normalized;

		//transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (newForward, up), Time.deltaTime * 4.0f);
		//transform.rotation = player.transform.rotation;
		transform.localRotation = Quaternion.Euler( new Vector3(angleX, 0.0f, 0.0f) );

	}
				
}