using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCompleteManager : MonoBehaviour
{
	[Header("UI Elements")]
	public Text finalScoreText;
	public Text highScoreText;
	public GameObject newHighScoreNotification;

	[Header("Distance Records")]
	public Text distanceTraveledText;
	public Text farthestDistanceText;
	public GameObject newDistanceRecordNotification;

	void Start()
	{
		Time.timeScale = 1;

		// Process score
		int finalScore = PlayerPrefs.GetInt("LastScore", 0);
		bool alreadyAdded = PlayerPrefs.GetInt("ScoreProcessed", 0) == 1;
		bool isNewHighScore = false;

		if (!alreadyAdded && ScoreManager.Instance != null)
		{
			int highestScoreBefore = ScoreManager.Instance.GetHighestScore();
			isNewHighScore = ScoreManager.Instance.AddScore(finalScore);

			if (finalScore <= highestScoreBefore)
			{
				isNewHighScore = false;
			}

			PlayerPrefs.SetInt("ScoreProcessed", 1);
			PlayerPrefs.Save();
		}

		// Update score UI
		if (finalScoreText != null)
		{
			finalScoreText.text = "Final Score: " + finalScore;
		}

		if (highScoreText != null)
		{
			highScoreText.text = "High Score: " + (ScoreManager.Instance != null ?
				ScoreManager.Instance.GetHighestScore() : 0);
		}

		if (newHighScoreNotification != null)
		{
			newHighScoreNotification.SetActive(isNewHighScore);
		}

		// Process distance records
		float distanceTraveled = PlayerPrefs.GetFloat("LastDistance", 0f);
		float farthestDistance = PlayerPrefs.GetFloat("FarthestDistance", 0f);
		bool isNewDistanceRecord = distanceTraveled >= farthestDistance && distanceTraveled > 0;

		// Update distance UI
		if (distanceTraveledText != null)
		{
			distanceTraveledText.text = "Distance: " + Mathf.Floor(distanceTraveled) + "m";
		}

		if (farthestDistanceText != null)
		{
			farthestDistanceText.text = "Farthest: " + Mathf.Floor(farthestDistance) + "m";
		}

		if (newDistanceRecordNotification != null)
		{
			newDistanceRecordNotification.SetActive(isNewDistanceRecord);
		}
	}

	public void PlayAgain()
	{
		SceneManager.LoadScene("GamePlay");
	}

	public void BackToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}