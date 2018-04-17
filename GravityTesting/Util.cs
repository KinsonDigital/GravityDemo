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
    }
}
