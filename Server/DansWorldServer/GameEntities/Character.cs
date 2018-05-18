using DansWorld.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DansWorld.Server.GameEntities
{
    public class Character
    {
        /// <summary>
        /// X Co-ordinate
        /// </summary>
        public int X;
        /// <summary>
        /// Y Co-ordinate
        /// </summary>
        public int Y;
        /// <summary>
        /// Experience value of the character
        /// </summary>
        public int EXP;
        /// <summary>
        /// Name of the character
        /// </summary>
        public string Name;
        /// <summary>
        /// Level of the character
        /// </summary>
        public int Level;
        /// <summary>
        /// ID of the character
        /// </summary>
        public int ID;
        /// <summary>
        /// Strength stat value
        /// </summary>
        public int Strength;
        /// <summary>
        /// Intelligence stat value
        /// </summary>
        public int Intelligence;
        /// <summary>
        /// Dexterity stat value
        /// </summary>
        public int Dexterity;
        /// <summary>
        /// Vitality stat value
        /// </summary>
        public int Vitality;
        /// <summary>
        /// Health value
        /// </summary>
        public int Health;
        /// <summary>
        /// Max health value
        /// </summary>
        public int MaxHealth;
        /// <summary>
        /// Direction that the character is facing
        /// </summary>
        public Direction Facing;
    }
}
