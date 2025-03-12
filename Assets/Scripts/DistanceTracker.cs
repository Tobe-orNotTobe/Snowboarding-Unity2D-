using UnityEngine;
using UnityEngine.UI;

public class DistanceTracker : MonoBehaviour
{
	[Header("Distance Tracking")]
	[SerializeField] private Transform player;
	[SerializeField] private float metersPerUnit = 0.5f; 
	[SerializeField] private Text distanceText;

	[Header("Farthest Point Marker")]
	[SerializeField] private GameObject farthestPointMarker;
	[SerializeField] private float markerYOffset = 2f;

	[Header("Record Notification")]
	[SerializeField] private GameObject recordNotification;
	[SerializeField] private Text recordNotificationText;
	[SerializeField] private float notificationDuration = 3f;
	[SerializeField] private AudioClip recordBreakSound;

	private float startX;
	private float currentDistance;
	private float farthestDistance;
	private float farthestPointX;
	private bool notificationActive = false;
	private bool markerCreated = false;
	private bool hasShownRecordNotification = false; 
	private AudioSource audioSource;

	private const string FARTHEST_DISTANCE_KEY = "FarthestDistance";

	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null && recordBreakSound != null)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
		}
	}

	void Start()
	{
		if (player == null)
		{
			player = FindObjectOfType<PlayerController>().transform;
		}

		startX = player.position.x;
		LoadFarthestDistance();

		farthestPointX = startX + (farthestDistance / metersPerUnit);

		hasShownRecordNotification = false;

		if (farthestDistance > 0)
		{
			CreateFarthestPointMarker();
		}

		if (recordNotification != null)
		{
			recordNotification.SetActive(false);
		}
	}

	void Update()
	{
		if (player == null) return;

		currentDistance = (player.position.x - startX) * metersPerUnit;

		if (distanceText != null)
		{
			distanceText.text = $"{Mathf.Floor(currentDistance)}m";
		}

		if (currentDistance > farthestDistance && !notificationActive && !hasShownRecordNotification)
		{
			NewRecordReached();
		}
	}

	private void CreateFarthestPointMarker()
	{
		if (farthestPointMarker != null && !markerCreated)
		{
			Vector3 markerPosition = new Vector3(farthestPointX, player.position.y + markerYOffset, 0);
			GameObject marker = Instantiate(farthestPointMarker, markerPosition, Quaternion.identity);

			Text markerText = marker.GetComponentInChildren<Text>();
			if (markerText != null)
			{
				markerText.text = $"NEW RECORD: {Mathf.Floor(farthestDistance)}m";
			}

			markerCreated = true;
		}
	}

	private void LoadFarthestDistance()
	{
		farthestDistance = PlayerPrefs.GetFloat(FARTHEST_DISTANCE_KEY, 0f);
	}

	private void SaveFarthestDistance()
	{
		PlayerPrefs.SetFloat(FARTHEST_DISTANCE_KEY, farthestDistance);
		PlayerPrefs.Save();
	}

	private void NewRecordReached()
	{
		farthestDistance = currentDistance;
		farthestPointX = player.position.x;
		SaveFarthestDistance();

		if (recordNotification != null)
		{
			if (recordNotificationText != null)
			{
				recordNotificationText.text = $"NEW RECORD: {Mathf.Floor(farthestDistance)}m";
			}

			recordNotification.SetActive(true);
			notificationActive = true;
			Invoke("HideNotification", notificationDuration);
		}

		if (audioSource != null && recordBreakSound != null)
		{
			audioSource.PlayOneShot(recordBreakSound);
		}

		if (markerCreated)
		{
			GameObject oldMarker = GameObject.FindGameObjectWithTag("FarthestMarker");
			if (oldMarker != null)
			{
				Destroy(oldMarker);
			}
			markerCreated = false;
		}

		CreateFarthestPointMarker();

		hasShownRecordNotification = true;

		Debug.Log($"New record! Farthest distance: {Mathf.Floor(farthestDistance)}m");
	}

	private void HideNotification()
	{
		if (recordNotification != null)
		{
			recordNotification.SetActive(false);
		}
		notificationActive = false;
	}

	public float GetCurrentDistance()
	{
		return currentDistance;
	}

	public float GetFarthestDistance()
	{
		return farthestDistance;
	}

	// Call this when the game ends to ensure the farthest distance is saved
	public void SaveRecord()
	{
		if (currentDistance > farthestDistance)
		{
			farthestDistance = currentDistance;
			SaveFarthestDistance();
		}
	}
}