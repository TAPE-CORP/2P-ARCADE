using UnityEngine;

[System.Serializable]
public class OrderData
{
    public string itemName;
    public Sprite boxImage;
    public Vector2 boxSize;  // ����, ����
    public Vector2 itemSize; // ����, ����
    public float timeLimit;  // �� ���� (��: 90��)
}
