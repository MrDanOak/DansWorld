using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansWorld.Math
{
    public class Utils
    {
        public static Vector2 PointToVector(Point point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}
