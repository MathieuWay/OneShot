using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        AnchorCamera = GameObject.FindWithTag("Player").transform.Find("CameraAnchor");
        state = State.Idle;

    }

    private void Update()
    {
        switch (state)
        {
            case State.Focusing:
                float distCovered = Time.time - focusStartTime;
                float fraction = distCovered / focusTime;
                transform.position = Vector3.Lerp(focusStartPosition, new Vector3(AnchorCamera.position.x, AnchorCamera.position.y, -10), FocusPositionCurve.Evaluate(fraction));
                mainCamera.orthographicSize = Mathf.Lerp(focusStartorthographicSize, focusorthographicSize, FocusCameraSizeCurve.Evaluate(fraction));
                if (fraction >= 1)
                {
                    state = State.Chasing;
                    LevelController.Instance.phase = Phase.Combat;
                }
                break;
            case State.Chasing:
                transform.position = new Vector3(AnchorCamera.position.x, AnchorCamera.position.y, -10);
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
