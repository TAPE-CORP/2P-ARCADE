using UnityEngine;

[System.Serializable]
public class BoxPrefabSet
{
    public int areaSize;
    public GameObject unboxedPrefab; // 포장 전
    public GameObject packedPrefab;  // 포장 후
}
