using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableData", menuName = "Custom/SpawnableData")]
public class SpawnableData : ScriptableObject
{
    public string objectName;
    public Sprite sprite;
    public GameObject prefab;
    public Vector2 size = Vector2.one;
    public float timeLimit = 10f;
}
