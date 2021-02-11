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
    }
}
