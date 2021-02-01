using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BlueprintManager : MonoBehaviour
{
    [Serializable]
    private class Blueprint
    {
        public string name;
        public List<string> dimensions;
    }

    private List<Blueprint> mBlueprints;
    private string mBlueprintsPath;

    public BlueprintManager()
    {
        mBlueprints = new List<Blueprint>();
        mBlueprintsPath = "C:\\tmp\\blueprints";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            updateBlueprintsList();
            listBlueprints();
        }
    }

    private void updateBlueprintsList()
    {
        if (!Directory.Exists(mBlueprintsPath))
        {
            Debug.LogError("Blueprints directory does not exist");
        }

        mBlueprints.Clear();

        Debug.Log("Discovered blueprints:");

        foreach (string filename in Directory.GetFiles(mBlueprintsPath))
        {
            if (!filename.EndsWith(".json")) continue;

            StreamReader reader = new StreamReader(filename);
            mBlueprints.Add(JsonUtility.FromJson<Blueprint>(reader.ReadToEnd()));
            reader.Close();
        }
    }

    private void listBlueprints()
    {
        foreach (Blueprint blueprint in mBlueprints)
        {
            Debug.Log(blueprint.name + ": [" + String.Join(",", blueprint.dimensions) + "]");
        }
    }
}
