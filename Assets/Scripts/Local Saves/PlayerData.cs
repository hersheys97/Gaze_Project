using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public Vector3 playerPosition;
    public int playerHealth;
    public List<CheckpointData> unlockedCheckpoints = new();
}

[System.Serializable]
public class CheckpointData
{
    public string name;
    public Vector3 position;

    public CheckpointData(string name, Vector3 position)
    {
        this.name = name;
        this.position = position;
    }
}