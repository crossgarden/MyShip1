using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public enum SFXClip { NONE=-1, FAIL, EATTING, SLIDE, BUY, CLICK, MAX }
    public enum BGMClip { NONE=-1, MAX }

    public enum RoomNum { WAITTING, PRIVATE, RESTAURANT, MAX }

    [Serializable]
    public class UserData
    {
        public int ship_level;
        public int coin;
        public List<Food> foods;
        public List<Character> characters;
        public int[] favor_max;

        override
        public string ToString()
        {
            string foodStr = "";
            for(int i = 0; i < foods.Count ; i++){
                foodStr += foods[i].ToString();
            }
            return string.Format("foods: {0}", foodStr);
        }
    }

    [Serializable]
    public class Food
    {
        public string name;
        public string kr_name;
        public int cost;
        public int fullness;
        public int favor;
        public string descript;
        public int count;

        override
        public string ToString()
        {
            return string.Format("{0}: {1}\n", name, count);
        }
    }

    [Serializable]
    public class Character
    {
        public string name;
        public string kr_name;
        public int cost;
        public int level;
        public int favor;
        public int fullness;
        public int energy;
        public int locked;
        public string unlockDate;
        public string introduction;
        public string[] script;

        override
        public string ToString()
        {
            return string.Format("{0}: {1}\n", name, level);
        }
    }

}
