using TMPro;
using UnityEngine;

public class ScoreKeeper : Singleton<ScoreKeeper>
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int score;
    public int Score => score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}
