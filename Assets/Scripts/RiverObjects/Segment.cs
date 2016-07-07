using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.RiverObjects
{
    class Segment
    {
        public Vector2 A { get; set; }
        public Vector2 B { get; set; }
        public Vector2 Coord { get; set; }

        public Segment(Vector2 A, Vector2 B)
        {
            this.A = A;
            this.B = B;
            this.Coord = new Vector2(Mathf.Sqrt(Mathf.Pow(B.x - A.x, 2)), Mathf.Sqrt(Mathf.Pow(B.y - A.y, 2)));
        }

        public static float operator *(Segment ab, Segment cd)
        {
            return Vector2.Dot(ab.Coord, cd.Coord);
        }
    }
}
