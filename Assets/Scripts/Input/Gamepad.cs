using UnityEngine;


public class Gamepad : MonoBehaviour
{
	public static Gamepad Instance { get; private set; }

	public float HorizontalJL { get; private set; }
	public float VerticalJL { get; private set; }
	public float HorizontalJR { get; private set; }
	public float VerticalJR { get; private set; }
	public float PadHorizontal { get; private set; }
	public float PadVertical { get; private set; }
	public bool TriggerL { get; private set; }
	public bool TriggerR { get; private set; }
	public bool ButtonA { get; private set; }
	public bool ButtonB { get; private set; }
	public bool ButtonX { get; private set; }
	public bool ButtonY { get; private set; }
	public bool ButtonR { get; private set; }
	public bool ButtonL { get; private set; }
	public bool ButtonTriggerL { get; private set; }
	public bool ButtonTriggerR { get; private set; }
	public bool ButtonStart { get; private set; }
	public bool ButtonDownA { get; private set; }
	public bool ButtonDownB { get; private set; }
	public bool ButtonDownX { get; private set; }
	public bool ButtonDownY { get; private set; }
	public bool ButtonDownL { get; private set; }
	public bool ButtonDownR { get; private set; }
	public bool ButtonDownTriggerL { get; private set; }
	public bool ButtonDownTriggerR { get; private set; }
	public bool ButtonDownStart { get; private set; }


	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private void Update()
	{
		//Gâchettes
		TriggerL = Input.GetAxis("TriggerL") > 0 ? true : false;
		TriggerR = Input.GetAxis("TriggerR") > 0 ? true : false;

		//Joysticks
		HorizontalJL = Input.GetAxis("Horizontal_JL");
		VerticalJL = Input.GetAxis("Vertical_JL");
		HorizontalJR = Input.GetAxis("Horizontal_JR");
		VerticalJR = Input.GetAxis("Vertical_JR");

		//Pad
		PadHorizontal = Input.GetAxis("Pad_Horizontal");
		PadVertical = Input.GetAxis("Pad_Vertical");

		//Boutons enfoncé une fois
		ButtonDownA = Input.GetKeyDown("joystick button 0");
		ButtonDownB = Input.GetKeyDown("joystick button 1");
		ButtonDownX = Input.GetKeyDown("joystick button 2");
		ButtonDownY = Input.GetKeyDown("joystick button 3");
		ButtonDownR = Input.GetKeyDown("joystick button 9");
		ButtonDownL = Input.GetKeyDown("joystick button 8");
		ButtonStart = Input.GetKeyDown("joystick button 7");
		ButtonDownTriggerL = Input.GetKeyDown("joystick button 4");
		ButtonDownTriggerR = Input.GetKeyDown("joystick button 5");

		//Boutons enfoncé en continue
		ButtonA = Input.GetKey("joystick button 0");
		ButtonB = Input.GetKey("joystick button 1");
		ButtonX = Input.GetKey("joystick button 2");
		ButtonY = Input.GetKey("joystick button 3");
		ButtonR = Input.GetKey("joystick button 9");
		ButtonL = Input.GetKey("joystick button 8");
		ButtonTriggerL = Input.GetKey("joystick button 4");
		ButtonTriggerR = Input.GetKey("joystick button 5");
		ButtonDownStart = Input.GetKeyDown("joystick button 7"); 
	}
}