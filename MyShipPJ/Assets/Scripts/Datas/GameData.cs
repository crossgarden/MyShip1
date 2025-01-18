using System;
using UnityEngine;
namespace GameData
{
    [Serializable]
    public struct UserData
    {
        public UserFood[] food;

        override
        public string ToString()
        {
            string foodStr = "";
            for(int i = 0; i < food.Length ; i++){
                foodStr += food[i].ToString();
            }
            return string.Format("food: {0}", foodStr);
        }
    }

    [Serializable]
    public struct UserFood
    {
        public string name;
        public int count;

        override
        public string ToString()
        {
            return string.Format("{0}: {1}\n", name, count);
        }
    }

    // 시스템 데이터
    public struct SystemData
    {
        public FoodInfo[] foodInfos;
    }

    public struct FoodInfo
    {
        public int id;
        public string name;
        public string descript;
        public string imgPath;
    }

    // public struct Characters{
    //     public int YouJay;
    // }

    // public struct Character{

    // }
}
