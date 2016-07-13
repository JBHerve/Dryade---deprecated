using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.RiverObjects
{
    public class IncrementObject
    {
        public delegate Vector2 Increment(Vector2 vect);

        public static Increment GenerateIncrement(float x, float y)
        {
            return delegate (Vector2 vect) { return new Vector2(vect.x + x, vect.y + y); };
        }
    }
}
