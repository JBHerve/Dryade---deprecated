using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.RiverObjects
{
    public class Node
    {
        public Vector3 Position { get; set; }
        public int Priority { get; set; }
        public int Flow { get; set; }
        public List<Node> Sons { get; set; }
        public Node Father { get; set; }

        public Node(Vector3 position, int priority, int flow, Node father = null)
        {
            this.Position = position;
            this.Priority = priority;
            this.Flow = flow;
            this.Father = father;
            Sons = new List<Node>();
        }

        public void AddSon(Node node)
        {
            Sons.Add(node);
            node.Father = this;
        }

        public bool isLeaf()
        {
            return Sons.Count == 0;
        }

        public bool isRoot()
        {
            return Father == null;
        }

        public static bool operator >(Node a, Node b)
        {
            return a.Priority > b.Priority;
        }

        public static bool operator <(Node a, Node b) 
        {
            return a.Priority < b.Priority;
        }
    }
}
