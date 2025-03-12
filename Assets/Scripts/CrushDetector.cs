using UnityEngine;
using UnityEngine.SceneManagement;

public class CrushDetector : MonoBehaviour
{
	[SerializeField] float loadDelay = 0.5f;
	[SerializeField] AudioClip crushSFX;

	private bool hasCrushed = false;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (gameObject.CompareTag("Player") &&
		    (other.CompareTag("Ground") || other.CompareTag("DeathZone")) &&
		    !hasCrushed)
		{
			HandleCrush();
		}
		else if ((gameObject.CompareTag("Player") || gameObject.CompareTag("Skateboard")) &&
		         other.CompareTag("Spike") && !hasCrushed)
		{
			HandleCrush();
		}
	}

	void HandleCrush()
	{
		hasCrushed = true;

		var playerController = FindAnyObjectByType<PlayerController>();
		if (playerController != null)
		{
			Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
			if (rb != null)
			{
				rb.simulated = false;
			}
		}

		AudioSource audioSource = GetComponent<AudioSource>();
		if (audioSource != null && crushSFX != null)
		{
			audioSource.PlayOneShot(crushSFX);
		}

		// Save distance traveled
		var distanceTracker = FindObjectOfType<DistanceTracker>();
		if (distanceTracker != null)
		{
			float currentDistance = distanceTracker.GetCurrentDistance();
			PlayerPrefs.SetFloat("LastDistance", currentDistance);
			distanceTracker.SaveRecord();
		}

		Invoke("LoadGameCompleteScene", loadDelay);
	}

	void LoadGameCompleteScene()
	{
		// Save current score
		int currentScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetCurrentScore() : 0;
		PlayerPrefs.SetInt("LastScore", currentScore);
		PlayerPrefs.SetInt("ScoreProcessed", 0); 
		PlayerPrefs.Save();

		SceneManager.LoadScene("GameCompleted");
	}
}