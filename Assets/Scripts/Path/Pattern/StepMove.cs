using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace oneShot
{
    public static class StepMove
    {
        public static float GetMoveFactor(MoveType type)
        {
            switch (type)
            {
                case MoveType.Walk:
                    return 1f;
                case MoveType.Run:
                    return 1.5f;
                case MoveType.Sprint:
                    return 2f;
                default:
                    return 0f;
            }
        }

        public static string GetClipName(MoveType type)
        {
            switch (type)
            {
                case MoveType.Walk:
                    return "walk";
                case MoveType.Run:
                    return "run";
                case MoveType.Sprint:
                    return "sprint";
                default:
                    return "walk";
            }
        }

        public static Vector3 GetPositionByTime(List<StepMovePath> stepMovePaths, float currentTime)
        {
            int i = 0;
            while (i < stepMovePaths.Count)
            {
                if (currentTime <= stepMovePaths[i].time) break;
                i++;
            }
            Vector3 pos = Vector3.zero;
            if(i == 0)
            {
                //Debug.Log("beginpath");
                float duration = stepMovePaths[i].time - stepMovePaths[i+1].time;
                float norm = (stepMovePaths[i].time - currentTime) / duration;
                //Debug.Log(norm);
                return Vector3.Lerp(stepMovePaths[i].waypoint, stepMovePaths[i+1].waypoint, norm);
            }
            else if (i == stepMovePaths.Count)
            {
                //Debug.Log("endpath");
                float duration = stepMovePaths[i - 1].time - UI_Timeline.Instance.GetTimerDuration();
                float norm = (stepMovePaths[i - 1].time - currentTime) / duration;
                //Debug.Log(norm);
                return stepMovePaths[i - 1].waypoint;//Vector3.Lerp(stepMovePaths[i - 1].waypoint, step.targetPos, norm);
            }
            else
            {
                float duration = stepMovePaths[i - 1].time - stepMovePaths[i].time;
                float norm = (stepMovePaths[i - 1].time - currentTime) / duration;
                //Debug.Log(norm);
                return Vector3.Lerp(stepMovePaths[i - 1].waypoint, stepMovePaths[i].waypoint, norm);
            }
        }

        public static float CalculateTime(Step step, Vector3 initialPos, Vector3 position, float speed, float starttime)
        {
            float duration = 0f;
            Vector3 cursor = initialPos;
            oneShot.Layer layer = LayersController.instance.GetLayer(LayersController.instance.GetLayerIndexByHeight(cursor.y));
            //path.Add(cursor);
            step.stepMovePaths.Add(new StepMovePath(new Vector2(cursor.x, cursor.y), starttime + duration));
            while (cursor.y != position.y)
            {
                int direction;
                if (cursor.y < position.y)
                    direction = 1;
                else
                    direction = -1;
                //Debug.Log("position:" + cursor + "  /direction:" + direction);
                Transform access = layer.GetClosestAccess(direction, cursor);
                if (access.position == Vector3.zero)
                {
                    //path.Clear();
                    step.stepMovePaths.Clear();
                    Debug.LogError("No access Found");
                    Debug.Break();
                    return -1;
                }
                //Path to access
                duration += Vector3.Distance(cursor, access.position) / speed;
                cursor = access.position;
                //path.Add(access);
                step.stepMovePaths.Add(new StepMovePath(new Vector2(access.position.x, access.position.y), starttime + duration));

                //ChangeLayer
                layer = LayersController.instance.GetLayer(layer.index + direction);
                /*
                access.GetComponent<InterLayer>().LoadPath();
                step.stepMovePaths
                */
                List<StepMovePath> accessPath = access.GetComponent<InterLayer>().LoadPath();
                for (int i = 0; i < accessPath.Count; i++)
                {
                    step.stepMovePaths.Add(new StepMovePath(accessPath[i].waypoint, starttime + duration + accessPath[i].time));
                }
                duration += accessPath[accessPath.Count -1].time;
                cursor = accessPath[accessPath.Count - 1].waypoint;
                //Vector3 nextPos = cursor;
                //nextPos.y = layer.transform.position.y;
                //path.Add(nextPos);
                //step.stepMovePaths.Add(new StepMovePath(new Vector2(nextPos.x, nextPos.y), starttime + duration));
            }
            duration += Vector3.Distance(cursor, position) / speed;
            //path.Add(position);
            step.stepMovePaths.Add(new StepMovePath(new Vector2(position.x, position.y), starttime + duration));
            return duration;
        }

    }
}
