using UnityEngine;

[System.Serializable]
public class OrderData
{
    public string itemName;
    public Sprite boxImage;
    public Vector2 boxSize;  // 가로, 세로
    public Vector2 itemSize; // 가로, 세로
    public float timeLimit;  // 초 단위 (예: 90초)
}
