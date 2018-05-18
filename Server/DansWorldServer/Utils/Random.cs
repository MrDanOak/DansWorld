using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Server.Utils
{
    public class RNG
    {
        /// <summary>
        /// Singleton Random Object
        /// </summary>
        private static Random _rnd = new Random();

        public static int Next(int min, int max)
        {
            return _rnd.Next(min, max);
        }
    }
}
