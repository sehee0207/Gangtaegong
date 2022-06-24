using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
	public TextMesh[] ScoreFinal;
    public TextMesh Best;
    public TextMesh YourScore;
    private int score;

    void Start()
    {
        score = PlayerPrefs.GetInt("score");
        if (score > PlayerPrefs.GetInt("HighScore")) {
			Best.text = "New Best: " + score.ToString();
		} else {
			Best.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		}
		
		ScoreFinal[0].text = "* " + (score / 10000).ToString();
        ScoreFinal[1].text = "* " + (score % 10000 / 1000).ToString();
        ScoreFinal[2].text = "* " + (score % 1000 / 100).ToString();
		
        YourScore.text = "Score: " + score.ToString();
    }
}
