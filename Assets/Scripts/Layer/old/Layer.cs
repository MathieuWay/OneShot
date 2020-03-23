using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace oneShot.old
{
    public class Layer : MonoBehaviour
    {
        public List<Node> nodes;
        public Node StartingNode;
        public Collider floorHitBox;

        private void Start()
        {
            LoadLayerNodes();
        }

        private void LoadLayerNodes()
        {
            while (StartingNode)
            {
                nodes.Add(StartingNode);
                StartingNode = StartingNode.GetNeighbourNodesFromDirection(Direction.Right);
            }
        }
    }
}
