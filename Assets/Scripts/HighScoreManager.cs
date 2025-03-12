using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class HighScoreManager : MonoBehaviour
{
	public static HighScoreManager Instance;

	[SerializeField] private GameObject highScorePanel;
	[SerializeField] private Text[] highScoreTexts; 

	private const int MAX_HIGH_SCORES = 5;
	private const string HIGH_SCORES_KEY = "HighScores";

	private List<int> highScores = new List<int>();

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			LoadHighScores();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Load saved high scores from PlayerPrefs
	private void LoadHighScores()
	{
		highScores.Clear();

		string scoresString = PlayerPrefs.GetString(HIGH_SCORES_KEY, "");

		if (!string.IsNullOrEmpty(scoresString))
		{
			string[] scoreArray = scoresString.Split(',');

			foreach (string scoreStr in scoreArray)
			{
				if (int.TryParse(scoreStr, out int score))
				{
					highScores.Add(score);
				}
			}
		}

		// Ensure we have exactly MAX_HIGH_SCORES entries
		while (highScores.Count < MAX_HIGH_SCORES)
		{
			highScores.Add(0);
		}

		// Sort in descending order
		highScores = highScores.OrderByDescending(score => score).ToList();

		// Trim to MAX_HIGH_SCORES
		if (highScores.Count > MAX_HIGH_SCORES)
		{
			highScores = highScores.Take(MAX_HIGH_SCORES).ToList();
		}
	}

	// Save high scores to PlayerPrefs
	private void SaveHighScores()
	{
		string scoresString = string.Join(",", highScores);
		PlayerPrefs.SetString(HIGH_SCORES_KEY, scoresString);
		PlayerPrefs.Save();
	}

	// Add a new score and check if it's a high score
	public bool AddScore(int score)
	{
		// Check if this score is higher than any existing high score
		bool isHighScore = false;

		if (score > highScores.Last() || highScores.Count < MAX_HIGH_SCORES)
		{
			highScores.Add(score);
			highScores = highScores.OrderByDescending(s => s).ToList();

			// Trim to MAX_HIGH_SCORES
			if (highScores.Count > MAX_HIGH_SCORES)
			{
				highScores = highScores.Take(MAX_HIGH_SCORES).ToList();
			}

			SaveHighScores();
			isHighScore = true;
		}

		return isHighScore;
	}

	// Get the highest score
	public int GetHighestScore()
	{
		if (highScores.Count > 0)
		{
			return highScores[0];
		}
		return 0;
	}

	// Display high scores in UI
	public void DisplayHighScores()
	{
		if (highScorePanel != null)
		{
			highScorePanel.SetActive(true);
		}

		// Update each high score text element
		for (int i = 0; i < highScoreTexts.Length && i < highScores.Count; i++)
		{
			if (highScoreTexts[i] != null)
			{
				highScoreTexts[i].text = $"{i + 1}. {highScores[i]}";
			}
		}
	}

	// Hide high score panel
	public void HideHighScores()
	{
		if (highScorePanel != null)
		{
			highScorePanel.SetActive(false);
		}
	}

	// Method to clear all high scores (for testing)
	public void ClearHighScores()
	{
		highScores.Clear();
		for (int i = 0; i < MAX_HIGH_SCORES; i++)
		{
			highScores.Add(0);
		}
		SaveHighScores();
	}
}