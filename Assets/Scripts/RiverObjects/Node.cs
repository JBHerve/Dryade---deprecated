using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.RiverObjects
{
    class Node
    {
        public Vector3 position { get; set; }
        public int priority { get; set; }
        public int flow { get; set; }
        public Node son { get; set; }

        public Node(Vector3 position, int priority, int flow, Node son)
        {
            this.position = position;
            this.priority = priority;
            this.flow = flow;
            this.son = son;
        }

        public bool isALeaf()
        {
            return son == null;
        }
    }
}
