using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsoleteSaveManager : MonoBehaviour
{
    public static SaveDataStruct GetOldSaveFile()
    {
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class SaveFileStructV1
{
    public string name;

    public string firstSaveDate = System.DateTime.Now.ToString("dd-MM-yyyy");
    public string latestSaveDate = System.DateTime.Now.ToString("dd-MM-yyyy");

    public int seed = UnityEngine.Random.Range(0, 999999);
    public float funds = 0f;

    public List<string> inventoryItems = new List<string>();
    public List<StructureData> structures = new List<StructureData>();
    public List<float> vitals = new List<float>();
    public List<string> clickedAlerts = new List<string>();

    public Dictionary<Vector2, ChunkPropData> chunkData = new Dictionary<Vector2, ChunkPropData>();
    public Dictionary<Vector3, StorageData> storage = new Dictionary<Vector3, StorageData>();

    public Vector3 playerPos;

    public bool finishedTutorial = false;

    public float MainAudioLevel = 0f;
    public float SFAudioLevel = 0f;
    public float MusicAudioLevel = 0f;

    public int FPSLimit = 30;

    public bool aa = false;
    public bool hdr = false;
    public bool dr = false;

    public int vd = 1;
    public int pp = 2;
}

[System.Serializable]
public class SaveDataStructV1
{
    public int version = 1;

    public bool milk_statue = false;
    public bool fragment_statue = false;
    public bool crystal_statue = false;

    public List<string> SaveFileNames = new List<string>();
    public List<SaveFileStruct> SaveFiles = new List<SaveFileStruct>();
}
