using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace oneShot
{
    public class Pattern : MonoBehaviour
    {
        public List<PatternStep> patternSteps = new List<PatternStep>();
        PatternStep currentStep = null;

        private void Awake()
        {
            GetNextStep();
        }

        public void Update()
        {
            if (currentStep != null)
            {
                if (currentStep.EndCondition())
                {
                    GetNextStep();
                }
                currentStep.Update();
            }else if(patternSteps.Count > 0)
                GetNextStep();
        }

        private void GetNextStep()
        {
            if (patternSteps.Count > 0)
            {
                //currentStep = patternSteps.Dequeue();
                PatternStep step = patternSteps[0];
                patternSteps.RemoveAt(0);
                switch (currentStep.stepType)
                {
                    case StepType.Move:
                        PatternStepMove moveStep = (PatternStepMove)currentStep;
                        moveStep.InitMove(GetComponent<Path>());
                        break;
                }
            }
        }
    }
}
