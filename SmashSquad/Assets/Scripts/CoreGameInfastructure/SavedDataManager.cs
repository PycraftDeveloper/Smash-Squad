using System.Collections.Generic;
using System.IO;
using UnityEngine;

// This program is used to store the game's configuration when the game is closed.

[System.Serializable]
public class GameData
{
    // This class stores a copy of registry entries to be saved/loaded from disk.
    public float Master_AudioVolume = 1.0f;

    public float SFX_AudioVolume = 1.0f;
    public float Music_AudioVolume = 0.25f;

    public float P1_X_CamSense = 200.0f;
    public float P1_Y_CamSense = 200.0f;
    public float P2_X_CamSense = 200.0f;
    public float P2_Y_CamSense = 200.0f;

    public bool P1_X_InvertAxis = false;
    public bool P1_Y_InvertAxis = true;
    public bool P2_X_InvertAxis = false;
    public bool P2_Y_InvertAxis = true;
}

public class SavedDataManager
{
    // Used to save/load content from a save location - expand to save game progress.
    private string SaveFileLocation = Application.persistentDataPath;

    private string SaveFileName = "GameData.json";
    private string SavePath = "";

    public SavedDataManager()
    {
        SavePath = Path.Combine(SaveFileLocation, SaveFileName);
    }

    public void Load()
    {
        // Loads data from disk, or uses defaults if save not found.
        GameData LoadedData;
        if (File.Exists(SavePath))
        {
            string dataToLoad = "";
            using (FileStream stream = new FileStream(SavePath, FileMode.Open)) // Open file
            {
                using (StreamReader reader = new StreamReader(stream)) // Open stream
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }
            LoadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            Registry.MasterVolume = LoadedData.Master_AudioVolume;
            Registry.SFXVolume = LoadedData.SFX_AudioVolume;
            Registry.MusicVolume = LoadedData.Music_AudioVolume;

            Registry.P1_X_CamSense = LoadedData.P1_X_CamSense;
            Registry.P1_Y_CamSense = LoadedData.P1_Y_CamSense;
            Registry.P2_X_CamSense = LoadedData.P2_X_CamSense;
            Registry.P2_Y_CamSense = LoadedData.P2_Y_CamSense;

            Registry.P1_X_InvertAxis = LoadedData.P1_X_InvertAxis;
            Registry.P1_Y_InvertAxis = LoadedData.P1_Y_InvertAxis;
            Registry.P2_X_InvertAxis = LoadedData.P2_X_InvertAxis;
            Registry.P2_Y_InvertAxis = LoadedData.P2_Y_InvertAxis;
        }
    }

    public void Save()
    {
        // Saves data to disk.
        Directory.CreateDirectory(SaveFileLocation);

        GameData SavedData = new GameData();

        SavedData.Master_AudioVolume = Registry.MasterVolume;
        SavedData.SFX_AudioVolume = Registry.SFXVolume;
        SavedData.Music_AudioVolume = Registry.MusicVolume;

        SavedData.P1_X_CamSense = Registry.P1_X_CamSense;
        SavedData.P1_Y_CamSense = Registry.P1_Y_CamSense;
        SavedData.P2_X_CamSense = Registry.P2_X_CamSense;
        SavedData.P2_Y_CamSense = Registry.P2_Y_CamSense;

        SavedData.P1_X_InvertAxis = Registry.P1_X_InvertAxis;
        SavedData.P1_Y_InvertAxis = Registry.P1_Y_InvertAxis;
        SavedData.P2_X_InvertAxis = Registry.P2_X_InvertAxis;
        SavedData.P2_Y_InvertAxis = Registry.P2_Y_InvertAxis;

        string SerialisedGameData = JsonUtility.ToJson(SavedData, true);

        using (FileStream stream = new FileStream(SavePath, FileMode.Create)) // Open file
        {
            using (StreamWriter writer = new StreamWriter(stream)) // Open stream
            {
                writer.Write(SerialisedGameData);
            }
        }
    }
}