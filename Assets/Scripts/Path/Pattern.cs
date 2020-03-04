using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace oneShot
{
    public class Pattern : MonoBehaviour
    {
        public List<PatternStepMove> patternSteps = new List<PatternStepMove>();
        private List<PatternStepMove> patternStepsLoaded = new List<PatternStepMove>();

        //private PatternStepMove currentStep = null;
        private Animator anim;
        private Agent agent;
        private Vector3 initialPosition;

        public bool RecordPattern;

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            agent = GetComponent<Agent>();

            patternStepsLoaded = new List<PatternStepMove>(patternSteps);
            UI_Timeline.OnTimelineReset += ResetPattern;
            initialPosition = transform.position;

            if (patternStepsLoaded.Count > 0)
                GetNextStep();
        }

        public void Update()
        {
            if (patternStepsLoaded.Count > 0)
            {
                if (patternStepsLoaded[0].startTime <= UI_Timeline.Instance.GetCurrentTime())
                    GetNextStep();
            }

            if (RecordPattern && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layerMask = 1 << 8;
                if (Physics.Raycast(ray, out hit, layerMask))
                {
                    Vector3 normalizePos = new Vector3(hit.point.x, (int)hit.point.y, 0);
                    //Debug.Log(normalizePos);
                    patternSteps.Add(new PatternStepMove(UI_Timeline.Instance.GetCurrentTime(), normalizePos));
                    //Transform objectHit = hit.transform;
                    // Do something with the object that was hit by the raycast.
                }
            }
        }

        private void GetNextStep()
        {
            if (patternStepsLoaded.Count > 0)
            {
                //currentStep = patternSteps.Dequeue();
                PatternStepMove step = patternStepsLoaded[0];
                patternStepsLoaded.RemoveAt(0);
                switch (step.stepType)
                {
                    case StepType.Move:
                        agent.SetTarget(step.target);
                        agent.CalculateTime(agent.target);
                        //moveStep.InitMove(GetComponent<Path>());
                        break;
                }
            }
        }

        private void ResetPattern()
        {
            agent.resetPosition();
            patternStepsLoaded = new List<PatternStepMove>(patternSteps);
            Debug.Log("resetPattern");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (PatternStepMove moveStep in patternSteps)
            {
                Gizmos.DrawSphere(moveStep.target, 0.1f);
            }
        }
    }
}
