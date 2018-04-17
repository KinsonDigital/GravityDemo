using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravityTesting
{
    public static class Util
    {
        public static float CalculateForce(float mass, float acceleration)
        {
            return mass * acceleration;
        }

        public static float Squared(float value)
        {
            return value * value;
        }

        public static float Average(float[] values)
        {
            var sum = 0f;

            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }

            return sum / values.Length;
        }
    }
}
