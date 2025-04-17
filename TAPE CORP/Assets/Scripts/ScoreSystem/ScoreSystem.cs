using UnityEngine;
using TMPro;
public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance;

    public int totalScore = 0;
    public TextMeshProUGUI scoreText;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int score)
    {
        totalScore += score;
        scoreText.text = totalScore.ToString();
        Debug.Log("Á¡¼ö Ãß°¡µÊ: " + score + ", ÃÑ Á¡¼ö: " + totalScore);
    }
}
