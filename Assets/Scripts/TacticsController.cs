using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Step
{
    public Transform transform;
    public float time;

}
public class TacticsController : MonoBehaviour
{
    private static TacticsController instance;
    public static TacticsController Instance
    {
        get
        {
            if (instance != null)
                return instance;
            GameObject inScene = GameObject.Find("TacticsController");
            if (inScene)
            {
                instance = inScene.GetComponent<TacticsController>();
                return instance;
            }
            else
            {
                Debug.LogError("no TacticsController instance in scene");
                return null;
            }
        }
    }

    //TIMER
    public float time;
    //STEPS
    public Step[] tpArray;
    private Queue<Step> tpQueue;
    private Step nextStep;
    private GameObject player;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        tpQueue = new Queue<Step>();
        foreach (Step tp in tpArray)
        {
            tpQueue.Enqueue(tp);
        }
        nextStep = tpQueue.Dequeue();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(LevelController.Instance.phase == Phase.Combat)
        {
            if (time >= nextStep.time)
                ExecuteNextStep();
            time += Time.fixedDeltaTime;
        }
    }

    private void ExecuteNextStep()
    {
        player.transform.position = nextStep.transform.position;
        if(tpQueue.Count > 0)
            nextStep = tpQueue.Dequeue();
    }
}
