using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour{
    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private float scoreMultiplier = 5;

    private float _score;

    public const string HighScoreKey = "HighScore";

    private void Update(){
        _score += Time.deltaTime * scoreMultiplier;
        scoreText.text = Mathf.FloorToInt(_score).ToString();
    }

    private void OnDestroy(){
        var currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (_score > currentHighScore){
            PlayerPrefs.SetInt("HighScore", Mathf.FloorToInt(_score));
        }
    }
}