using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class FileDataHandler
{
    private string dataFilePath;
    private string dataFileName;
    public FileDataHandler(string dataFilePath, string dataFileName)
    {
        this.dataFilePath = dataFilePath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataFilePath, dataFileName);
        
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = string.Empty;

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                loadedData = JsonConvert.DeserializeObject<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error occurs while loading data to file" + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save(GameData gameData)
    {
        string fullPath = Path.Combine(dataFilePath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //string dataToStore = JsonUtility.ToJson(gameData);
            string dataToStore = JsonConvert.SerializeObject(gameData);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurs while saving data to file" + fullPath + "\n" + e);
        }
    }
}
