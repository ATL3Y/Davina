using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour 
{
	public float moveSpeed = 5.0f;
	public float jumpPower = 10.0f;
	public float maxVelocityChange = 10.0f;

	private float sensitivity = 200.0f;

	public void Start () 
	{
		
	}

	public void Update () 
	{

		//if using mouse input in input controller, cursorLocked must be commented out here. 
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		//restart if fall off terrain
		if (transform.position.y < -10.0f) 
		{
			SceneManager.LoadScene ("test1");
		}

		//set horizontal rotation based on mouse X input 
		float angleY = Input.GetAxisRaw ("Mouse X") * sensitivity * Time.deltaTime;
		Vector3 newForward = Quaternion.AngleAxis (angleY, Vector3.up) * transform.forward;
		newForward = newForward.normalized;

		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (newForward, Vector3.up), Time.deltaTime * 4.0f);
			
		Vector3 targetVelocity = new Vector3 (KeyToF (KeyCode.D) - KeyToF (KeyCode.A), 0.0f, KeyToF (KeyCode.W) - KeyToF (KeyCode.S));

		float moveSpeedMult = KeyToF (KeyCode.LeftShift) * 2.0f + 1.0f;
		targetVelocity = transform.TransformDirection (targetVelocity);

		targetVelocity *= moveSpeed * moveSpeedMult;

		//apply a force that attempts to reach our target velocity
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		Vector3 velocityChange = targetVelocity - velocity;
		velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0; 
		velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
		GetComponent<Rigidbody> ().AddForce (velocityChange, ForceMode.VelocityChange);

		float thrust = KeyToF(KeyCode.Space, true) * jumpPower;
		GetComponent<Rigidbody> ().AddForce (transform.up * thrust, ForceMode.Impulse );
	
	}

	float KeyToF ( KeyCode keyCode, bool onDown = false )
	{
		if (onDown) 
		{ 
			return Input.GetKeyDown (keyCode) ? 1.0f : 0.0f; 

		} else 
		{
			return Input.GetKey (keyCode) ? 1.0f : 0.0f;
		}
	}

}
