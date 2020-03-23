using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using oneShot;

public enum State
{
    Idle,
    Focusing,
    Chasing
}
public class CameraController : MonoBehaviour
{
    //state
    public State state;
    public float focusTime = 2f;
    public float focusorthographicSize;

    //CURVE
    public AnimationCurve FocusPositionCurve;
    public AnimationCurve FocusCameraSizeCurve;

    //LERP
    private float focusStartTime;
    private Vector3 focusStartPosition;
    private float focusStartorthographicSize;

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
    private float pathPourcentage = 0;


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
        state = State.Idle;
        path = GetComponent<Path>();
        if (path)
        {
            path.SetDirection(DirectionAxis.Horizontal);
            path.SetNode(initNode);
            transform.position = path.GetPositionAlongPath() + offsetPath;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                if (path)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, path.GetPositionAlongPath() + offsetPath, ref VelocitySmooth, SmoothTraveling);
                    if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                    {
                        //Debug.Log("Scroll Value:" + Input.GetAxis("Mouse ScrollWheel") + "    /    Scroll Value with sensitivity:" + Input.GetAxis("Mouse ScrollWheel") * ScrollSensitivity);
                        path.AddProgression(Input.GetAxis("Mouse ScrollWheel") * ScrollSensitivity);
                    }
                }
                break;
            case State.Focusing:
                float distCovered = Time.time - focusStartTime;
                float fraction = distCovered / focusTime;
                transform.position = Vector3.Lerp(focusStartPosition, new Vector3(AnchorCamera.position.x, AnchorCamera.position.y, offsetPath.z), FocusPositionCurve.Evaluate(fraction));
                mainCamera.orthographicSize = Mathf.Lerp(focusStartorthographicSize, focusorthographicSize, fraction) * FocusCameraSizeCurve.Evaluate(fraction);
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
        focusStartorthographicSize = mainCamera.orthographicSize;
        state = State.Focusing;
    }
}
