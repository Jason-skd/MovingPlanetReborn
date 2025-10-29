using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class DataBaseManger :ScriptableObject
{
    //name is your gameobject's tag,not class name or script name;
    //请求各位在自己的Script里写一个函数把相应的数据填进里面方便管理（拜托了）
    //怪物的Tag请包含“Enermy"
   private static Dictionary<string,Vector3> enermyposition=new Dictionary<string,Vector3>();
   private static Dictionary<string, Vector3> buildingposition = new Dictionary<string, Vector3>();
   private static Dictionary<string,int> buildingenergy= new Dictionary<string,int>();
  public static void RegisterBuildingData(string name,Vector3 position,int energy)
    {
        
        buildingenergy.Add(name,energy);
        buildingposition.Add(name, position);

    }
  public static void RegisterEnermyData(string name,Vector3 position)
    {
        
        enermyposition.Add(name ,position);
    }
  public static void RemoveBuildingData(string key)
    {
        buildingposition.Remove(key);
        buildingenergy.Remove(key);
    }
  public static void RemoveEnermyData(string key)
    {
        buildingenergy.Remove(key);
    }
  public static Vector3 GetEnermyPosition(string key)
     {
          Vector3 value;
          if (enermyposition.TryGetValue(key,out value))
          {
              
                return value;
          }
          else
          {
                Debug.LogError("No such key in enermy position database");
                return Vector3.zero;
          }

    }
  public static int GetBuildingEnergy(string key)
    {
        int value;
        if (buildingenergy.TryGetValue(key,out value))
        {
           
            return value;
        }
        else
        {
            Debug.LogError("No such key in building energy database");
            return -1;
        }
        
    }
  public static Vector3 GetBuildingPosition(string key)
    {
        Vector3 value;
        if (buildingposition.TryGetValue(key,out value))
        {
           
            return value;
        }
        else
        {
            Debug.LogError("No such key in building position database");
            return Vector3.zero;
        }

    }
    public static string GetKeyByBuildingPosition(Vector3 position)
    {
        foreach (KeyValuePair<string, Vector3> pair in buildingposition)
        {
            if (pair.Value == position)
            {
                return pair.Key;
            }
        }
        Debug.LogError("No such position in building position database");
        return null;
    }
    public static string GetKeyByEnermyPosition(Vector3 position)
    {
        foreach (KeyValuePair<string, Vector3> pair in enermyposition)
        {
            if (pair.Value == position)
            {
                return pair.Key;
            }
        }
        Debug.LogError("No such position in enermy position database");
        return null;
    }
    public static string GetKeyByBuildingEnergy(int energy)
    {
        foreach (KeyValuePair<string, int> pair in buildingenergy)
        {
            if (pair.Value == energy)
            {
                return pair.Key;
            }
        }
        Debug.LogError("No such energy in building energy database");
        return null;
    }
    private static bool Find(string key,string str)
    {
        int count2 = 0;
        for (int count=0;count<key.Length-str.Length;count++) { 
            for (int i = count; i < count+str.Length; i++)
            {
                if (key[i] == str[i-count]&&count2==str.Length)
                {
                    return true;
                }
                count2++;
                
            }
        }
        Debug.LogError("No such key in database");
        return false;
    }
public static List<int> GetBuildingEnergys(string str)
    {
        List<int> energys = new List<int>();
        
        foreach(string keys in buildingenergy.Keys)
        {
            if (Find(keys,str))
            {
                energys.Add(buildingenergy[keys]);
            }
        }
        return energys;
    }
public static List<Vector3> GetEnermyPositions(string str)
    {
        List<Vector3> positions = new List<Vector3>();
        
        foreach(string keys in enermyposition.Keys)
        {
            if (Find(keys,str))
            {
                positions.Add(enermyposition[keys]);
            }
        }
        return positions;
    }
    public static List<Vector3> GetBuildingPositions(string str)
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (string keys in buildingposition.Keys)
        {
            if (Find(keys, str))
            {
                positions.Add(buildingposition[keys]);
            }
        }
        return positions;
    }

    public static void ClearAllData()
    {
        enermyposition.Clear();
        buildingposition.Clear();
        buildingenergy.Clear();
        
    }
    public static bool ModifyBuildingEnergy(string key,int newenergy)
    {
        if (buildingenergy.ContainsKey(key))
        {
            buildingenergy[key] = newenergy;
            return true;
        }
        else
        {
            Debug.LogError("No such key in building energy database");
            return false;
        }
    }
    public static bool ModifyBuildingPosition(string key,Vector3 newposition)
    {
        if (buildingposition.ContainsKey(key))
        {
            buildingposition[key] = newposition;
            return true;
        }
        else
        {
            Debug.LogError("No such key in building position database");
            return false;
        }
    }
    public static bool ModifyEnermyPosition(string key,Vector3 newposition)
    {
        if (enermyposition.ContainsKey(key))
        {
            enermyposition[key] = newposition;
            return true;
        }
        else
        {
            Debug.LogError("No such key in enermy position database");
            return false;
        }
    }
    public static void SaveDataToJSONFile()
    {
        try
        {
            string savePath = Application.persistentDataPath + "GameData.json";
            string jsonData = JsonUtility.ToJson(new DataBaseManger());
            File.WriteAllText(savePath, jsonData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save Data Failed:" + e.Message);
        }
    }
    public  static DataBaseManger LoadDataFromJSONFile()
    {
        try
        {
            string loadPath = Application.persistentDataPath + "GameData.json";
            if (File.Exists(loadPath))
            {
                string jsonData = File.ReadAllText(loadPath);
                DataBaseManger data=new DataBaseManger();
                JsonUtility.FromJsonOverwrite(jsonData, data);
                return data;
            }
            else
            {
                Debug.LogWarning("Load Data Failed: File does not exist.");
                return null;
            }
        }
        catch (System.Exception e)
        { 
            Debug.LogError("Load Data Failed:" + e.Message);
            return null;
        }
    }
}


