using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
	public static ScoreManager Instance;
	private int currentScore = 0;

	[Header("UI Elements")]
	public Text scoreText;

	[Header("High Score System")]
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
			return;
		}

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "GamePlay")
		{
			currentScore = 0;
			PlayerPrefs.SetInt("ScoreProcessed", 0);
			PlayerPrefs.Save();

			UpdateScoreUI();
			if (scoreText != null) scoreText.gameObject.SetActive(true);

		}
		else if (scene.name == "GameCompleted")
		{

			PlayerPrefs.SetInt("ScoreProcessed", 0);
			PlayerPrefs.Save();
		}
		
	}


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

		while (highScores.Count < MAX_HIGH_SCORES)
		{
			highScores.Add(0);
		}

		highScores = highScores.OrderByDescending(score => score).ToList();

		if (highScores.Count > MAX_HIGH_SCORES)
		{
			highScores = highScores.Take(MAX_HIGH_SCORES).ToList();
		}
	}

	private void SaveHighScores()
	{
		string scoresString = string.Join(",", highScores);
		PlayerPrefs.SetString(HIGH_SCORES_KEY, scoresString);
		PlayerPrefs.Save();
	}

	public bool AddScore(int score)
	{
		bool isHighScore = false;

		if (score > highScores.Last() || highScores.Count < MAX_HIGH_SCORES)
		{
			highScores.Add(score);
			highScores = highScores.OrderByDescending(s => s).ToList();

			if (highScores.Count > MAX_HIGH_SCORES)
			{
				highScores = highScores.Take(MAX_HIGH_SCORES).ToList();
			}

			SaveHighScores();
			isHighScore = true;
		}

		return isHighScore;
	}

	public void AddPoints(int amount)
	{
		currentScore += amount;
		UpdateScoreUI();
	}

	void UpdateScoreUI()
	{
		if (scoreText != null)
		{
			scoreText.text = "Score: " + currentScore;
		}
	}

	public int GetCurrentScore()
	{
		return currentScore;
	}

	public int GetHighestScore()
	{
		if (highScores.Count > 0)
		{
			return highScores[0];
		}
		return 0;
	}
	public List<int> GetHighScores()
	{
		return new List<int>(highScores);
	}

	public void DisplayHighScores()
	{
		if (highScorePanel != null)
		{
			highScorePanel.SetActive(true);
		}

		for (int i = 0; i < highScoreTexts.Length && i < highScores.Count; i++)
		{
			if (highScoreTexts[i] != null)
			{
				highScoreTexts[i].text = (i + 1) + ". " + highScores[i];
			}
		}
	}

	public void HideHighScores()
	{
		if (highScorePanel != null)
		{
			highScorePanel.SetActive(false);
		}
	}

	public void ResetScore()
	{
		currentScore = 0;
		UpdateScoreUI();
	}

	public void ClearHighScores()
	{
		highScores.Clear();
		for (int i = 0; i < MAX_HIGH_SCORES; i++)
		{
			highScores.Add(0);
		}
		SaveHighScores();
	}

	public void BackToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void StartGame()
	{
		currentScore = 0;
		SceneManager.LoadScene("GamePlay");
	}
}