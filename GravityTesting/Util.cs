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
        /// <remarks>Refer to links for more information.
        /// Videos:
        ///     Part 1: https://www.youtube.com/watch?v=3HjO_RGIjCU
        ///     Part 2: https://www.youtube.com/watch?v=pBMivz4rIJY
        /// Other: https://leios.gitbooks.io/algorithm-archive/content/chapters/physics_solvers/verlet/verlet.html
        /// </remarks>
        /// <returns></returns>
        public static float IntegrateVelocityVerlet(float velOnSingleAxis, float dt, float accelerationOnSingleAxis)
        {
            return velOnSingleAxis * dt + (0.5f * accelerationOnSingleAxis * Squared(dt));
        }


        /// <summary>
        /// Calculates the drag force of air/fluid on the surface of an object.
        /// </summary>
        /// <param name="fluidDensity"></param>
        /// <param name="dragCoefficient"></param>
        /// <param name="surfaceAreaInContact"></param>
        /// <param name="velocityOnSingleAxis"></param>
        /// <remarks>Refer to http://www.softschools.com/formulas/physics/air_resistance_formula/85/ for information.</remarks>
        /// <returns></returns>
        public static float CalculateDragForceOnObject(float fluidDensity, float dragCoefficient, float surfaceAreaInContact, float velocityOnSingleAxis)
        {
            return -1 * ((fluidDensity * dragCoefficient * surfaceAreaInContact) / 2.0f) * Squared(velocityOnSingleAxis);
        }
    }
}
