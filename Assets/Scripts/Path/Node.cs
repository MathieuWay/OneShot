using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace oneShot
{
    public enum Direction
    {
        Top,
        Right,
        Bottom,
        Left
    }

    [Serializable]
    public class NeighbourNode
    {
        public Node node;
        public Direction direction;
    }

    public class Node : MonoBehaviour
    {
        public List<NeighbourNode> neighbourNodes = new List<NeighbourNode>();
        public Node GetNeighbourNodesFromDirection(Direction direction)
        {
            NeighbourNode neighbour = neighbourNodes.Find(node => node.direction == direction);
            if (neighbour != null)
                return neighbour.node;
            else
                return null;
        }
    }
}
