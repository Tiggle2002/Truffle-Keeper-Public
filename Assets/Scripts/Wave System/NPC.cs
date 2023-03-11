using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class NPC : IProbability
{
    public GameObject obj;

    [Range(0, 100)]
    public int minimumWaveReached;

    [field: SerializeField, PropertyRange(0.0001f, 1f)]
    public float chance { get; set; }

    [MinMaxSlider(0.25f, 20f, true)]
    public Vector2 spawnDelay;

    [Range(1, 100)]
    public int poolSize;

    [HideInInspector]
    public string ID { get { return obj.name; } }

    public float GetDelay()
    {
        return Random.Range(spawnDelay.x, spawnDelay.y);
    }
}