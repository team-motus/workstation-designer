using System;
using UnityEngine;

namespace WorkstationDesigner.Util
{
    public static class MathUtil
    {
        /// <summary>
        /// Return true if both numbers have the same sign
        /// </summary>
        public static bool SameSign(double a, double b)
        {
            return (a < 0) == (b < 0);
        }

        /// <summary>
        /// Check if two vectors are approximately equal to each other (i.e. each angle is within a given tolerance)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool ApproxEquals(Vector3 a, Vector3 b, float tolerance = 0.01f)
        {
            return (Math.Abs(a.x - b.x) < tolerance) && (Math.Abs(a.y - b.y) < tolerance) && (Math.Abs(a.z - b.z) < tolerance);
        }
    }
}
