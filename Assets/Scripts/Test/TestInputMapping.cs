using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TestInputMapping : MonoBehaviour {

	[SerializeField] Text text;
	void OnEnable()
	{
		for (int i = 0; i < System.Enum.GetNames (typeof(MInputType)).Length; ++i) {
			M_Event.inputEvents [i] += OnInput;
		}
	}

	void OnInput( InputArg arg )
	{
		switch (arg.type) {
		case MInputType.SelectObject:
			text.text = "Select Object";
			break;
		case MInputType.Transport:
			text.text = "Transport";
			break;
		default:
			break;
		}
	}

	void Update()
	{
		/// for VR, this only takes the left controller
		Ray ray = InputManager.Instance.GetCenterRayCast ()[0];
		Debug.DrawRay (ray.origin , ray.direction , Color.red);
	}
}
