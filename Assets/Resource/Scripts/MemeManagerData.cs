using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MemeManagerData : MonoBehaviour
{
   // public List<MemeElement> memeElements= new List<MemeElement> ();
    private string jsonPath;
    private void Awake()
    {
        jsonPath = Path.Combine(Application.persistentDataPath, "memeData.json");
        Debug.Log("File Path: " + jsonPath);
        LoadData();
    }
    public List<MemeElement> LoadData()
    {
        if(File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            MemeWrapper wrapper = JsonUtility.FromJson<MemeWrapper>(json);
            
            Debug.Log("Loaded memes from: " + jsonPath);
            foreach (var meme in wrapper.elements)
            {
                meme.LoadAssets();  
            }
            return wrapper.elements;
        }
        else
        {
            Debug.LogWarning("No data found. Using default meme list.");
            return new List<MemeElement>();
        }
    }
    public void SaveData(List<MemeElement> memes)
    {
        string json = JsonUtility.ToJson(new MemeWrapper(memes), true);
        File.WriteAllText(jsonPath, json);
        Debug.Log("Save to " + jsonPath);
    }
    [System.Serializable]
    private class MemeWrapper
    {
        public List<MemeElement> elements;
        public MemeWrapper(List<MemeElement> memes)
        {
            this.elements = memes;
        }
    }
}
