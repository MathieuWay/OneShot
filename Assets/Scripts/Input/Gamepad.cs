using System.Collections;
using UnityEngine;
using XInputDotNetPure;

public enum ControllerType
{
    Gamepad,
    KeyboardMouse
}
public class Gamepad : MonoBehaviour
{
	public static Gamepad Instance { get; private set; }

    public ControllerType type = ControllerType.KeyboardMouse;
    public float HorizontalJL
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetAxis("Horizontal_JL");
            }
            else
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    return 1;
                }
                else if(Input.GetKey(KeyCode.LeftArrow))
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
    public float VerticalJL
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetAxis("Vertical_JL");
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    return 1;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
    public float HorizontalJR
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetAxis("Horizontal_JR");
            }
            else
            {
                return Input.GetAxis("Horizontal");
            }
        }
    }
    //public float VerticalJR { get; private set; }
    public float PadHorizontal
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetAxis("Pad_Horizontal");
            }
            else
            {
                return 0;
            }
        }
    }
    public float PadVertical
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetAxis("Pad_Vertical");
            }
            else
            {
                if (oneShot.LevelController.Instance.phase == oneShot.Phase.Combat)
                {
                    return 0;
                }
                else
                {
                    return Input.GetAxis("Mouse ScrollWheel");
                }
            }
        }
    }
    public bool TriggerL { get; private set; }
	public bool TriggerR { get; private set; }
    /*
	public bool ButtonA { get; private set; }
    public bool ButtonB { get; private set; }
    public bool ButtonX { get; private set; }
	public bool ButtonY { get; private set; }
	public bool ButtonR { get; private set; }
	public bool ButtonL { get; private set; }
	public bool ButtonTriggerL { get; private set; }
	public bool ButtonTriggerR { get; private set; }
	public bool ButtonStart { get; private set; }
    */
    public bool ButtonDownA
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetKeyDown("joystick button 0");
            }
            else
            {
                if (oneShot.LevelController.Instance.phase == oneShot.Phase.Combat)
                {
                    return Input.GetKeyDown(KeyCode.S);
                }
                else
                {
                    return Input.GetMouseButtonDown(0);
                }
            }
        }
    }
    public bool ButtonDownB
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetKeyDown("joystick button 1");
            }
            else
            {
                if (oneShot.LevelController.Instance.phase == oneShot.Phase.Combat)
                {
                    return Input.GetKeyDown(KeyCode.D);
                }
                else
                {
                    return Input.GetMouseButtonDown(1);
                }
            }
        }
    }
    public bool ButtonDownX
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetKeyDown("joystick button 2");
            }
            else
            {
                if (oneShot.LevelController.Instance.phase == oneShot.Phase.Combat)
                {
                    return Input.GetKeyDown(KeyCode.A);
                }
                else
                {
                    return Input.GetKeyDown(KeyCode.Space);
                }
            }
        }
    }
    public bool ButtonDownY
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetKeyDown("joystick button 3");
            }
            else
            {
                if (oneShot.LevelController.Instance.phase == oneShot.Phase.Combat)
                {
                    return Input.GetKeyDown(KeyCode.W);
                }
                else
                {
                    return Input.GetKeyDown(KeyCode.Return);
                }
            }
        }
    }
    //public bool ButtonDownL { get; private set; }
	//public bool ButtonDownR { get; private set; }
	public bool ButtonDownTriggerL
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetKeyDown("joystick button 4");
            }
            else
            {
                return Input.GetKeyDown(KeyCode.Q);
            }
        }
    }
    public bool ButtonDownTriggerR
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetKeyDown("joystick button 5");
            }
            else
            {
                return Input.GetKeyDown(KeyCode.E);
            }
        }
    }
    public bool ButtonDownStart
    {
        get
        {
            if (type == ControllerType.Gamepad)
            {
                return Input.GetKeyDown("joystick button 7");
            }
            else
            {
                return Input.GetKeyDown(KeyCode.Return);
            }
        }
    }
    //Gamepad Specific
    private bool isVibrating;

    //Keyboard/Mouse Specific
    private Vector2 lastMousePosition = Vector2.zero;
    private Vector2 currentMousePosition = Vector2.zero;
    private Vector2 deltaMousePosition = Vector2.zero;


    public void Vibrate(float leftMotor, float rightMotor, float duration)
	{
		if (isVibrating) return;

		GamePad.SetVibration(0, leftMotor, rightMotor);
		StartCoroutine(VibrateDuration(duration));
	}
	private IEnumerator VibrateDuration(float duration)
	{
		isVibrating = true;

		yield return new WaitForSeconds(duration);

		GamePad.SetVibration(0, 0, 0);

		isVibrating = false;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
        if (!Instance)
            Instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
        currentMousePosition = Input.mousePosition;
        deltaMousePosition = currentMousePosition - lastMousePosition;
        lastMousePosition = currentMousePosition;
    }

	private void Update()
	{
        if (type == ControllerType.Gamepad)
            ListenKeyboardInput();
        else
            ListenGamepadInput();
		//Gâchettes
		TriggerL = Input.GetAxis("TriggerL") > 0 ? true : false;
		TriggerR = Input.GetAxis("TriggerR") > 0 ? true : false;

        //Joysticks
        //HorizontalJL = Input.GetAxis("Horizontal_JL");
        //VerticalJL = Input.GetAxis("Vertical_JL");

        //HorizontalJR = Input.GetAxis("Horizontal_JR");
		//VerticalJR = Input.GetAxis("Vertical_JR");

		//Pad
		//PadHorizontal = Input.GetAxis("Pad_Horizontal");
        //PadVertical = Input.GetAxis("Pad_Vertical");
        //if (PadVertical == 0)
        //    PadVertical = Input.GetAxis("Mouse ScrollWheel");

        //Boutons enfoncé une fois
        //ButtonDownA = Input.GetKeyDown("joystick button 0");
		//ButtonDownB = Input.GetKeyDown("joystick button 1");
		//ButtonDownX = Input.GetKeyDown("joystick button 2");
		//ButtonDownY = Input.GetKeyDown("joystick button 3");
		//ButtonDownR = Input.GetKeyDown("joystick button 9");
		//ButtonDownL = Input.GetKeyDown("joystick button 8");
		//ButtonStart = Input.GetKeyDown("joystick button 7");
		//ButtonDownTriggerL = Input.GetKeyDown("joystick button 4");
		//ButtonDownTriggerR = Input.GetKeyDown("joystick button 5");

		//Boutons enfoncé en continue
		//ButtonA = Input.GetKey("joystick button 0");
		//ButtonB = Input.GetKey("joystick button 1");
		//ButtonX = Input.GetKey("joystick button 2");
		//ButtonY = Input.GetKey("joystick button 3");
		//ButtonR = Input.GetKey("joystick button 9");
		//ButtonL = Input.GetKey("joystick button 8");
		//ButtonTriggerL = Input.GetKey("joystick button 4");
		//ButtonTriggerR = Input.GetKey("joystick button 5");
		//ButtonDownStart = Input.GetKeyDown("joystick button 7");

		//!DEBUG : Input Clavier/Souris
		//ButtonDownStart = Input.GetKeyDown(KeyCode.Return);
		//ButtonDownA = Input.GetKeyDown(KeyCode.S);
		//ButtonDownB = Input.GetKeyDown(KeyCode.D);
		//ButtonDownX = Input.GetKeyDown(KeyCode.Q);
		//ButtonDownY = Input.GetKeyDown(KeyCode.Z);
	}

	private void OnApplicationQuit()
	{
		GamePad.SetVibration(0, 0, 0);
	}

    private void ListenKeyboardInput()
    {
        KeyCode[] inputs = new KeyCode[] { KeyCode.A, KeyCode.E, KeyCode.Z, KeyCode.Q, KeyCode.S, KeyCode.D, KeyCode.Space, KeyCode.Return};
        int i = 0;
        while(i < inputs.Length)
        {
            if (Input.GetKeyDown(inputs[i]))
            {
                type = ControllerType.KeyboardMouse;
                return;
            }
            i++;
        }
        /*
        if (deltaMousePosition.magnitude > 0.1f)
        {
            type = ControllerType.KeyboardMouse;
            return;
        }
        currentMousePosition = Input.mousePosition;
        deltaMousePosition = currentMousePosition - lastMousePosition;
        lastMousePosition = currentMousePosition;
        */
    }


    private void ListenGamepadInput()
    {
        string[] inputs = new string[] { "joystick button 0", "joystick button 1", "joystick button 2", "joystick button 3", "joystick button 4", "joystick button 5", "joystick button 7"};
        string[] axis = new string[] { "Pad_Vertical", "Horizontal_JR", "Vertical_JR", "Horizontal_JL", "Vertical_JL" };
        int i = 0;
        while (i < inputs.Length)
        {
            if (Input.GetKeyDown(inputs[i]))
            {
                type = ControllerType.KeyboardMouse;
                return;
            }
            i++;
        }

        i = 0;
        while (i < axis.Length)
        {
            if (Mathf.Abs(Input.GetAxis(axis[i])) >= 0.1f)
            {
                type = ControllerType.Gamepad;
                return;
            }
            i++;
        }
    }
}