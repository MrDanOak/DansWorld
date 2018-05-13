using Microsoft.Xna.Framework;
namespace DansWorld.GameClient.Math
{
    public class Utils
    {
        public static Vector2 PointToVector(Point point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}
