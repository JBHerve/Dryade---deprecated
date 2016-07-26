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


        //On veut savoir si[AB] coupe[A'B']
        //ceci est vrai ssi : 
        //->produit vectoriel (AB A'B') != 0 cad les droites ne sont pas parallèles(et aussi A'!=B', A != B). (cf ligne 99)
        //->ET produit vectoriel (AB, AB').produit vectoriel (AB,AA')<= 0 cad le point d'intersection est entre B' et A' (donc sur le segment [A'B']) (cf ligne 109 -> 111)
        //->ET produit vectoriel (A'B', A'B).produit vectoriel (A'B',A'A)<= 0 cad le point d'intersection est entre B et A (donc sur le segment [AB]) (cf ligne 113 -> 115)
        public static bool Cross(Segment ab, Segment cd)
        {
            if (ab * cd != 0)
            {
                //TODO: Add a minimal distance
                Segment aux1 = new Segment(ab.A, cd.B);
                Segment aux2 = new Segment(ab.A, cd.A);
                if ((ab * aux1) * (ab * aux2) <= 0)
                {
                    aux1 = new Segment(cd.A, ab.B);
                    aux2 = new Segment(cd.A, ab.A);
                    if ((cd * aux1) * (cd * aux2) <= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
