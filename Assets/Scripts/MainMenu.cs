using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public Button playButton;
	public Button howToPlayButton;
	public Button quitButton;

	[SerializeField] private  GameObject helpPanel;
	[SerializeField] private GameObject highScorePanel;
	[SerializeField] private Text[] highScoreTexts;

	void Start()
	{
		ShowHighScores();

		if (playButton != null)
		{
			playButton.onClick.RemoveAllListeners();
			playButton.onClick.AddListener(() => StartGame());
		}

		if (howToPlayButton != null)
		{
			howToPlayButton.onClick.RemoveAllListeners();
			howToPlayButton.onClick.AddListener(() => ToggleHelp());
		}

		if (quitButton != null)
		{
			quitButton.onClick.RemoveAllListeners();
			quitButton.onClick.AddListener(() => QuitGame());
		}

		if (helpPanel != null) helpPanel.SetActive(false);
	}

	public void StartGame()
	{
		SceneManager.LoadScene("GamePlay");
	}

	private void OnEnable()
	{
		if (highScorePanel == null)
		{
			highScorePanel = transform.Find("HighScorePanel")?.gameObject;
		}

		if (highScoreTexts == null || highScoreTexts.Length == 0)
		{
			// Tìm lại các high score text
			List<Text> texts = new List<Text>();
			if (highScorePanel != null)
			{
				texts.AddRange(highScorePanel.GetComponentsInChildren<Text>(true));
			}
			highScoreTexts = texts.ToArray();
		}
	}

	public void ShowHighScores()
	{
		if (ScoreManager.Instance == null)
		{
			Debug.LogError("ScoreManager.Instance is null!");
			return;
		}

		if (highScorePanel == null)
		{
			highScorePanel = GameObject.Find("HighScorePanel");
			if (highScorePanel == null)
			{
				Debug.LogError("Could not find HighScorePanel in scene!");
				return;
			}
		}

		highScorePanel.SetActive(true);

		List<int> highScores = ScoreManager.Instance.GetHighScores();

		for (int i = 0; i < highScoreTexts.Length && i < highScores.Count; i++)
		{
			if (highScoreTexts[i] != null)
			{
				highScoreTexts[i].text = (i + 1) + ". " + highScores[i];
			}
		}
	}


	public void ToggleHelp()
	{
		if (helpPanel != null)
		{
			helpPanel.SetActive(!helpPanel.activeSelf);
		}
	}

	public void CloseHelp()
	{
		if (helpPanel != null)
		{
			helpPanel.SetActive(false);
		}
	}

	public void CloseHighScores()
	{
		ScoreManager.Instance.HideHighScores();
	}

	public void QuitGame()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}