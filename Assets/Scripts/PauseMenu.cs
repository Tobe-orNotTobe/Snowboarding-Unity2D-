using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public GameObject pauseMenuUI;
	private bool isPaused = false;

	void Start()
	{
		pauseMenuUI.SetActive(true);
		HideMenu();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log("ESC Pressed! isPaused = " + isPaused);
			if (isPaused)
				ResumeGame();
			else
				PauseGame();
		}
	}

	public void ResumeGame()
	{
		HideMenu();
		Time.timeScale = 1f;
		isPaused = false;

	
	}

	public void RestartGame()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void PauseGame()
	{
		ShowMenu();
		Time.timeScale = 0f;
		isPaused = true;

		
	}

	public void QuitGame()
	{
		PlayerPrefs.SetInt("TotalScore", 0);
		PlayerPrefs.Save();
		Time.timeScale = 1f;
		SceneManager.LoadScene("MainMenu");
	}

	private void ShowMenu()
	{
		pauseMenuUI.SetActive(true);
		CanvasGroup canvasGroup = pauseMenuUI.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 1;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}

	private void HideMenu()
	{
		CanvasGroup canvasGroup = pauseMenuUI.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}
}