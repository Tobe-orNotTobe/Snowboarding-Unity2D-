using UnityEngine;
using UnityEngine.UI;

public class RecordNotification : MonoBehaviour
{
	[SerializeField] private Text messageText;
	[SerializeField] private float displayDuration = 3f;
	[SerializeField] private float fadeSpeed = 2f;
	[SerializeField] private Vector3 floatDirection = new Vector3(0, 1, 0);
	[SerializeField] private float floatSpeed = 0.5f;

	private CanvasGroup canvasGroup;
	private Vector3 startPosition;
	private float displayTimer;
	private bool isShowing = false;
	private bool isFading = false;

	void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null)
		{
			canvasGroup = gameObject.AddComponent<CanvasGroup>();
		}
		startPosition = transform.localPosition;
		gameObject.SetActive(false);
	}

	void OnEnable()
	{
		ResetNotification();
		isShowing = true;
		displayTimer = displayDuration;

		if (messageText != null)
		{
			float distance = PlayerPrefs.GetFloat("FarthestDistance", 0f);
			messageText.text = $"NEW RECORD: {Mathf.Floor(distance)}m";
		}
	}

	void Update()
	{
		if (isShowing)
		{
			transform.localPosition += floatDirection * floatSpeed * Time.deltaTime;

			displayTimer -= Time.deltaTime;
			if (displayTimer <= 0)
			{
				isShowing = false;
				isFading = true;
			}
		}

		if (isFading)
		{
			canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
			if (canvasGroup.alpha <= 0)
			{
				gameObject.SetActive(false);
			}
		}
	}

	private void ResetNotification()
	{
		transform.localPosition = startPosition;
		canvasGroup.alpha = 1f;
		isFading = false;
	}
}