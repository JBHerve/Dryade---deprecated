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
        public List<Node> son { get; set; }

        public Node(Vector3 position, int priority, int flow)
        {
            this.position = position;
            this.priority = priority;
            this.flow = flow;
            son = null;
        }

        public bool isALeaf()
        {
            return son == null;
        }

        public static bool operator >(Node a, Node b)
        {
            return a.priority > b.priority;
        }

        public static bool operator <(Node a, Node b) 
        {
            return a.priority < b.priority;
        }
    }
}
