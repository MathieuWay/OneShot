using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using oneShot;

public enum State
{
    Travelling,
    Focusing,
    Chasing
}
public class CameraController : MonoBehaviour
{
    [Header("Switch Phase Settings")]
    //state
    public State state;
    public float focusTime = 2f;
    public float focusFOVSize;
    //private float focusFovSizeTarget;

    //CURVE
    public AnimationCurve FocusPositionCurve;
    public AnimationCurve FocusCameraFOVSize;

    //Combo
    [Header("Combo Settings")]
    public float focusZoomTime = 0.33f;
    public float FocusResetDelay = 1f;
    public float TimeBeforeCameraReset;
    public AnimationCurve zoomStep;
    public AnimationCurve tiltStep;
    public AnimationCurve shakeDurStep;
    public AnimationCurve shakePuisStep;
    public int comboInputStep;
    private Quaternion initialRotation;
    public bool useZoom = true; 
    public bool useShake = true;
    public bool useTilt = true;

    //LERP
    private float focusStartTime;
    private Vector3 focusStartPosition;
    private float focusStartFOVSize;

    //REF
    private Camera mainCamera;
    private Transform AnchorCamera;

    //PATH
    [Header("Path Settings")]
    [Range(0.1f, 2f)]
    public float travellingSensitivity = 0.1f;
    public float SmoothTraveling = 0.3f;
    private Vector3 VelocitySmooth = Vector3.zero;
    public Node initNode;
    public Path path;
    public Vector3 offsetPath = Vector3.zero;
    //private float pathPourcentage = 0;


    private static CameraController instance;
    public static CameraController Instance
    {
        get
        {
            if (instance != null)
                return instance;
            GameObject inScene = GameObject.Find("Main Camera");
            if (inScene)
            {
                instance = inScene.GetComponent<CameraController>();
                return instance;
            }
            else
            {
                Debug.LogError("no CameraController instance in scene");
                return null;
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        mainCamera = GetComponent<Camera>();
        GameObject player = GameObject.FindWithTag("Player");
        if(player)
            AnchorCamera = player.transform.Find("CameraAnchor");
        state = State.Travelling;
        path = GetComponent<Path>();
        if (path)
        {
            //path.SetDirection(DirectionAxis.Horizontal);
            if (initNode)
            {
                path.SetNode(initNode);
                transform.position = path.GetPositionAlongPath() + offsetPath;
            }
        }
    }

    private void Start()
    {
        initialRotation = transform.rotation;
        if (!ComboController.Instance) return;
        ComboController.Instance.NextInputEvent += FocusOnCombatInput;
        ComboController.Instance.ComboCanceledEvent += FocusReset;

        ComboController.Instance.ComboCompletedEvent += () => {
            if(useShake)
                CameraShake.Instance.ShakeCamera(CameraShake.ShakeTemplate.defaultSetting);
            StartCoroutine(Delay(FocusResetDelay, FocusReset));
        };

        ComboController.Instance.ComboFailedEvent += () => {
            //CameraShake.Instance.ShakeCamera(CameraShake.ShakeTemplate.defaultSetting);
            StartCoroutine(Delay(FocusResetDelay, FocusReset));
        };
    }

    private void Update()
    {
        switch (state)
        {
            case State.Travelling:
                if (path.CurrentNode)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, path.GetPositionAlongPath() + offsetPath, ref VelocitySmooth, SmoothTraveling);
                    /*
                    if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                    {
                        path.AddProgression(Input.GetAxis("Mouse ScrollWheel") * ScrollSensitivity);
                    }
                    */
                    if(Gamepad.Instance.PadVertical != 0f)
                    {
                        path.AddProgression(Gamepad.Instance.PadVertical * travellingSensitivity);
                    }
                }
                break;
            case State.Focusing:
                float distCovered = Time.time - focusStartTime;
                float fraction = distCovered / focusTime;
                transform.position = Vector3.Lerp(focusStartPosition, new Vector3(AnchorCamera.position.x, AnchorCamera.position.y, offsetPath.z), FocusPositionCurve.Evaluate(fraction));
                mainCamera.fieldOfView = Mathf.Lerp(focusStartFOVSize, focusFOVSize, fraction) * FocusCameraFOVSize.Evaluate(fraction);
                if (fraction >= 1)
                {
                    state = State.Chasing;
                }
                break;
            case State.Chasing:
                transform.position = new Vector3(AnchorCamera.position.x, AnchorCamera.position.y, offsetPath.z);
                break;
        }
    }

    public void FocusOnPlayer()
    {
        focusStartTime = Time.time;
        focusStartPosition = transform.position;
        focusStartFOVSize = mainCamera.fieldOfView;
        state = State.Focusing;
    }

    public void FocusReset()
    {
        if(useZoom)
            StartCoroutine(Zoom(focusFOVSize));
        if(useTilt)
		    StartCoroutine(Tilt(0));
        //StartCoroutine(Delay(focusZoomTime, () => { transform.rotation = initialRotation; }));
		comboInputStep = 0;
    }

    private void FocusOnCombatInput()
    {
        comboInputStep++;
        if (useZoom)
            StartCoroutine(Zoom(zoomStep.Evaluate(comboInputStep) * focusFOVSize / 100));
        if(useTilt)
		    StartCoroutine(Tilt(tiltStep.Evaluate(comboInputStep)));
		if (comboInputStep < 3 && useShake)
            CameraShake.Instance.ShakeCamera(CameraShake.ShakeTemplate.inputValid);
            //CameraShake.Instance.ShakeCamera(shakePuisStep.Evaluate(comboInputStep),shakeDurStep.Evaluate(comboInputStep));
    }

    IEnumerator Delay(float time, System.Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

    IEnumerator Zoom(float target)
    {
        float startTime = Time.time;
        float initialFOV = mainCamera.fieldOfView;
        float norm = 0;
        while (norm < 1)
        {
            norm = (Time.time - startTime) / focusZoomTime;
            mainCamera.fieldOfView = Mathf.Lerp(initialFOV, target, norm);

            yield return null;
        }
    }

    IEnumerator Tilt(float target)
    {
        float startTime = Time.time;
        Quaternion initQuaternion = mainCamera.transform.rotation;
        Quaternion endResult = initialRotation * Quaternion.AngleAxis(target, Vector3.forward);
        float norm = 0;
        while (norm < 1)
        {
            norm = (Time.time - startTime) / focusZoomTime;
            transform.rotation = Quaternion.Lerp(initQuaternion, endResult, norm);
            //Debug.Log(target + "/" + mainCamera.transform.eulerAngles.z);
            //float tilt = Mathf.LerpAngle(initialTilt, target, norm);
            //mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, tilt);
            yield return null;
        }
    }
}
