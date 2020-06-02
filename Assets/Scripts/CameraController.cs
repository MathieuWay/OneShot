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
    //state
    public State state;
    public float focusTime = 2f;
    public float focusFOVSize;

    //CURVE
    public AnimationCurve FocusPositionCurve;
    public AnimationCurve FocusCameraFOVSize;

    //LERP
    private float focusStartTime;
    private Vector3 focusStartPosition;
    private float focusStartFOVSize;

    //REF
    private Camera mainCamera;
    private Transform AnchorCamera;

    //PATH
    [Range(0.1f, 2f)]
    public float ScrollSensitivity = 0.1f;
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
            path.SetDirection(DirectionAxis.Horizontal);
            if (initNode)
            {
                path.SetNode(initNode);
                transform.position = path.GetPositionAlongPath() + offsetPath;
            }
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Travelling:
                if (path.CurrentNode)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, path.GetPositionAlongPath() + offsetPath, ref VelocitySmooth, SmoothTraveling);
                    if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                    {
                        path.AddProgression(Input.GetAxis("Mouse ScrollWheel") * ScrollSensitivity);
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
}
