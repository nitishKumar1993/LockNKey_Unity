using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager {

    public static HeroData[] GetHeroesData()
    {
        TextAsset heroDataAsset = Resources.Load("GameData") as TextAsset;

        string[] heroesDataArrayString = heroDataAsset.text.Split('\n');

        HeroData[] tempHeroesDataArray = new HeroData[heroesDataArrayString.Length - 1];

        for (int i = 1; i < heroesDataArrayString.Length; i++)
        {
            string[] tempArray = heroesDataArrayString[i].Split(',');
            int heroTypeId = System.Int32.Parse(tempArray[0]);
            if (System.Int32.TryParse(tempArray[0], out heroTypeId))
            {
                HeroData tempData = new HeroData();
                tempData.m_heroType = heroTypeId == 0 ? HeroType.Chaser : HeroType.Runner;
                tempData.m_name = tempArray[1];
                tempData.m_skillID = System.Int32.Parse(tempArray[2]);
                tempData.m_movementSpeed = System.Int32.Parse(tempArray[3]);
                tempHeroesDataArray[i -1] = tempData;
            }
            else
                Debug.Log("Couldnt parse hero Type ID");
        }
        return tempHeroesDataArray;
    }

    public static SkillData[] GetSkillsData()
    {
        TextAsset skillDataAsset = Resources.Load("SkillsData") as TextAsset;

        string[] skillDataArraString = skillDataAsset.text.Split('\n');

        SkillData[] tempSkillsDataArray = new SkillData[skillDataArraString.Length - 1];

        for (int i = 1; i < skillDataArraString.Length; i++)
        {
            string[] tempArray = skillDataArraString[i].Split(',');

            SkillData tempData = new SkillData();
            tempData.m_name = tempArray[0];
            tempData.m_CD = float.Parse(tempArray[1]);
            tempData.m_duration = float.Parse(tempArray[2]);
            tempSkillsDataArray[i - 1] = tempData;
        }
        return tempSkillsDataArray;
    }
}

public enum HeroType { Chaser, Runner }

public struct HeroData
{
    public HeroType m_heroType;
    public string m_name;
    public int m_skillID;
    public int m_movementSpeed;
}

public struct SkillData
{
    public int m_skillID;
    public string m_name;
    public float m_CD;
    public float m_duration;
}
