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

        /// <summary>
        /// This performs a verlet velocity integration on a single axis.
        /// </summary>
        /// <param name="velOnSingleAxis">The velocity on a single axis.  Must be the same axis as the <paramref name="accelerationOnSingleAxis"/> param.</param>
        /// <param name="dt">The delta time in seconds of the current frame.</param>
        /// <param name="accelerationOnSingleAxis">The current accerlation on a single axis.  Must be the same axis as the <paramref name="velOnSingleAxis"/> param.</param>
        /// <returns></returns>
        public static float IntegrateVelocityVerlet(float velOnSingleAxis, float dt, float accelerationOnSingleAxis)
        {
            return velOnSingleAxis * dt + (0.5f * accelerationOnSingleAxis * Squared(dt));
        }
    }
}
