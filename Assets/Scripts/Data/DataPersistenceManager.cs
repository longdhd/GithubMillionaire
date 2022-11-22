using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] string fileName;
    private FileDataHandler fileHandler;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("More than one instance of DataPersistenceManger exist");

        Instance = this;
    }

    private void Start()
    {
        this.fileHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        //Load any data from a file
        gameData = fileHandler.Load();

        //If no data can be loaded, create new game data
        if (this.gameData == null)
        {
            Debug.Log("No data found. Initialize new game data.");
            NewGame();
        }

        //Push the loaded data to all scripts that need it
        foreach(IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.LoadGame(gameData);
        }
    }

    public void SaveGame()
    {
        //Pass data to all scripts so they can update
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveGame(ref gameData);
        }

        //Save data
        fileHandler.Save(gameData);
    }

    List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
                                                                    .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
