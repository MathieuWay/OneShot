using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
#endif
namespace oneShot
{
    public class Layer : MonoBehaviour
    {
        public int index;
        private List<Transform> UpAccess = new List<Transform>();
        private List<Transform> DownAccess = new List<Transform>();

        public Vector3 GetClosestAccess(int direction, Vector3 pos)
        {
            List<Transform> access;
            if (direction > 0)
                access = UpAccess;
            else
                access = DownAccess;
            access = access.OrderBy(accessTransform => Vector3.Distance(pos, accessTransform.position)).ToList();
            if (access.Count > 0)
                return access[0].position;
            else
                return Vector3.zero;
        }

        public void LoadAccess()
        {
            Transform access = transform.Find("Access");
            //UP
            Transform up = access.Find("UP");
            foreach (Transform Upaccess in up)
            {
                UpAccess.Add(Upaccess);
            }

            //DOWN
            Transform down = access.Find("DOWN");
            foreach (Transform Downaccess in down)
            {
                DownAccess.Add(Downaccess);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Handles.yAxisColor;
            foreach (Transform access in UpAccess)
            {
                Handles.ArrowHandleCap(0, access.position, access.rotation * Quaternion.Euler(-90, 0, 0), 0.4f, EventType.Repaint);
            }


            Handles.color = Handles.xAxisColor;
            foreach (Transform access in DownAccess)
            {
                Handles.ArrowHandleCap(0, access.position, access.rotation * Quaternion.Euler(90, 0, 0), 0.4f, EventType.Repaint);
            }
        }
#endif
    }
}
