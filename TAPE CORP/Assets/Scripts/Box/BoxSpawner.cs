<<<<<<< HEAD
using UnityEngine;
using UnityEngine.Tilemaps;
=======
Ôªøusing UnityEngine;
>>>>>>> main
using TMPro;

public class BoxSpawner : MonoBehaviour
{
    [Header("Î∞ïÏä§ ÌîÑÎ¶¨Ìåπ")]
    public GameObject boxPrefab;

<<<<<<< HEAD
    [Header("¡§¥‰π⁄Ω∫ ≈©±‚ «•Ω√ TMP ≈ÿΩ∫∆Æ")]
    public TMP_Text correctBoxText;

    [Header("Ω∫∆˘ π¸¿ß")]
=======
    [Header("Ï†ïÎãµÎ∞ïÏä§ ÌÅ¨Í∏∞ ÌëúÏãú TMP ÌÖçÏä§Ìä∏")]
    public TMP_Text correctBoxText;

    [Header("Ïä§Ìè∞ Î≤îÏúÑ")]
>>>>>>> main
    public float xStart = 0f;
    public float xEnd = 10f;
    public float ySpawn = 20f;

    [Header("ÏÉùÏÑ± ÏÑ§Ï†ï")]
    public int boxCount = 5;
    public Vector2 minBoxScale = new Vector2(0.8f, 1.0f);
    public Vector2 maxBoxScale = new Vector2(2.0f, 3.0f);

    [Header("ÎîîÎ≤ÑÍ∑∏")]
    [SerializeField] private Vector2 correctBoxActualSize;

    private void Start()
    {
        SpawnAllBoxes();
    }

    private void SpawnAllBoxes()
    {
        int correctIndex = Random.Range(0, boxCount);

        for (int i = 0; i < boxCount; i++)
        {
            float xPos = Random.Range(xStart, xEnd);
            Vector3 spawnPos = new Vector3(xPos, ySpawn, 0f);

            GameObject box = Instantiate(boxPrefab, spawnPos, Quaternion.identity);

<<<<<<< HEAD
            Vector2 size;
=======
            // ‚úÖ ÎûúÎç§ Ïä§ÏºÄÏùº ÏÑ§Ï†ï
            float scaleX = Random.Range(minBoxScale.x, maxBoxScale.x);
            float scaleY = Random.Range(minBoxScale.y, maxBoxScale.y);
            Vector3 scale = new Vector3(scaleX, scaleY, 1f);
            box.transform.localScale = scale;

>>>>>>> main
            if (i == correctIndex)
            {
                box.name = "boxÏ†ïÎãµÎ∞ïÏä§";

                SpriteRenderer sr = box.GetComponent<SpriteRenderer>();
                if (sr != null && correctBoxText != null)
                {
                    Vector2 worldSize = sr.bounds.size;
                    correctBoxActualSize = worldSize;
                    correctBoxText.text = $"{worldSize.x:F1} * {worldSize.y:F1}";
                }
            }

<<<<<<< HEAD
            //  π⁄Ω∫ ≈©±‚ º≥¡§ (localScale¿∫ 1∑Œ ∞Ì¡§«œ∞Ì, size∏∏ ¡∂¡§)
            box.transform.localScale = Vector3.one;

            BoxCollider2D col = box.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.size = size;
            }

            Box boxScript = box.GetComponent<Box>();
            if (boxScript != null)
                boxScript.groundTilemap = groundTilemap;

            //  ¡§¥‰π⁄Ω∫ ≈ÿΩ∫∆Æ «•Ω√
            if (i == correctIndex)
            {
                BoxSizeDisplay sizeDisplay = box.GetComponent<BoxSizeDisplay>();
                if (sizeDisplay != null && correctBoxText != null)
                {
                    sizeDisplay.SetSizeTextTarget(correctBoxText);
                }
            }
=======
>>>>>>> main
        }
    }
}
