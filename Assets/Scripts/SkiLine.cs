using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(EdgeCollider2D))]
public class SkiLine : MonoBehaviour
{
	[Header("Cấu hình đường trượt")]
	[SerializeField] private Transform[] controlPoints;
	[SerializeField] private int segmentCount = 50;
	[SerializeField] private float lineWidth = 0.2f;
	[SerializeField] private Color lineColor = new Color(0.2f, 0.2f, 0.2f, 1f);

	[Header("Tính chất vật lý")]
	[SerializeField] private float frictionMultiplier = 1f;
	[SerializeField] private float bounciness = 0f;
	[SerializeField] private PhysicsMaterial2D surfaceMaterial;

	[Header("Điểm kết nối")]
	[SerializeField] private Transform startPoint;
	[SerializeField] private Transform endPoint;

	private LineRenderer lineRenderer;
	private EdgeCollider2D edgeCollider;
	private PlatformEffector2D effector;
	private Vector2[] linePoints;

	void Awake()
	{
		SetupComponents();
	}

	void Start()
	{
		GenerateLine();
		ConfigureEndPoints();
		UpdateCollider();
	}

	private void SetupComponents()
	{
		SetupLineRenderer();
		SetupEdgeCollider();
		SetupPlatformEffector();
	}

	private void SetupLineRenderer()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.startColor = lineColor;
		lineRenderer.endColor = lineColor;
		lineRenderer.startWidth = lineWidth;
		lineRenderer.endWidth = lineWidth;
		lineRenderer.useWorldSpace = true;
	}

	private void SetupEdgeCollider()
	{
		edgeCollider = GetComponent<EdgeCollider2D>();
		gameObject.tag = "SkiLine";
		edgeCollider.sharedMaterial = surfaceMaterial ?? CreateDefaultPhysicsMaterial();

		edgeCollider.usedByEffector = true;
	}

	private PhysicsMaterial2D CreateDefaultPhysicsMaterial()
	{
		var material = new PhysicsMaterial2D("SkiSurface")
		{
			friction = 0.1f,
			bounciness = bounciness
		};
		return material;
	}

	private void SetupPlatformEffector()
	{
		effector = GetComponent<PlatformEffector2D>();
		if (effector == null)
		{
			effector = gameObject.AddComponent<PlatformEffector2D>();
		}

		effector.useOneWay = true;          
		effector.useSideFriction = true;    
		effector.surfaceArc = 160f;        
		effector.rotationalOffset = 0f;    

		effector.sideArc = 20f;           
		effector.useColliderMask = true;    
	}

	private void GenerateLine()
	{
		if (controlPoints == null || controlPoints.Length < 2)
		{
			GenerateStraightLine();
		}
		else
		{
			GenerateBezierCurve();
		}
	}

	private void GenerateStraightLine()
	{
		if (startPoint == null || endPoint == null)
		{
			Debug.LogWarning("SkiLine missing start or end points!");
			return;
		}

		linePoints = new Vector2[] { (Vector2)startPoint.position, (Vector2)endPoint.position };
		UpdateLineRenderer(linePoints);
	}

	private void GenerateBezierCurve()
	{
		linePoints = new Vector2[segmentCount];

		for (int i = 0; i < segmentCount; i++)
		{
			float t = i / (float)(segmentCount - 1);
			linePoints[i] = CalculateBezierPoint(t, controlPoints);
		}

		UpdateLineRenderer(linePoints);
	}

	private void UpdateLineRenderer(Vector2[] points)
	{
		lineRenderer.positionCount = points.Length;
		for (int i = 0; i < points.Length; i++)
		{
			lineRenderer.SetPosition(i, points[i]);
		}
	}

	private Vector2 CalculateBezierPoint(float t, Transform[] points)
	{
		if (points.Length == 2)
		{
			return Vector2.Lerp(points[0].position, points[1].position, t);
		}
		else if (points.Length == 3)
		{
			return QuadraticBezier(points, t);
		}
		else if (points.Length >= 4)
		{
			return CubicBezier(points, t);
		}

		return Vector2.zero;
	}

	private Vector2 QuadraticBezier(Transform[] points, float t)
	{
		Vector2 p0 = Vector2.Lerp(points[0].position, points[1].position, t);
		Vector2 p1 = Vector2.Lerp(points[1].position, points[2].position, t);
		return Vector2.Lerp(p0, p1, t);
	}

	private Vector2 CubicBezier(Transform[] points, float t)
	{
		Vector2 p0 = Vector2.Lerp(points[0].position, points[1].position, t);
		Vector2 p1 = Vector2.Lerp(points[1].position, points[2].position, t);
		Vector2 p2 = Vector2.Lerp(points[2].position, points[3].position, t);

		Vector2 p3 = Vector2.Lerp(p0, p1, t);
		Vector2 p4 = Vector2.Lerp(p1, p2, t);

		return Vector2.Lerp(p3, p4, t);
	}

	private void ConfigureEndPoints()
	{
		if (startPoint == null)
		{
			startPoint = CreateEndPoint("StartPoint", linePoints[0]);
		}

		if (endPoint == null)
		{
			endPoint = CreateEndPoint("EndPoint", linePoints[linePoints.Length - 1]);
		}
	}

	private Transform CreateEndPoint(string name, Vector2 position)
	{
		var point = new GameObject(name).transform;
		point.parent = transform;
		point.position = position;
		return point;
	}

	private void UpdateCollider()
	{
		if (linePoints == null || linePoints.Length < 2) return;

		Vector2[] colliderPoints = new Vector2[linePoints.Length];
		for (int i = 0; i < linePoints.Length; i++)
		{
			colliderPoints[i] = transform.InverseTransformPoint(linePoints[i]);
		}
		edgeCollider.points = colliderPoints;
	}

	public Vector2 GetNormalAtPosition(Vector2 position)
	{
		if (linePoints == null || linePoints.Length < 2)
			return Vector2.up;

		int closestSegmentIndex = FindClosestSegment(position);
		Vector2 direction = linePoints[closestSegmentIndex + 1] - linePoints[closestSegmentIndex];
		Vector2 normal = new Vector2(-direction.y, direction.x).normalized;

		return normal.y < 0 ? -normal : normal;
	}

	private int FindClosestSegment(Vector2 position)
	{
		int closestIndex = 0;
		float closestDistance = float.MaxValue;

		for (int i = 0; i < linePoints.Length - 1; i++)
		{
			Vector2 closestPoint = GetClosestPointOnLineSegment(position, linePoints[i], linePoints[i + 1]);
			float distance = Vector2.Distance(position, closestPoint);

			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestIndex = i;
			}
		}

		return closestIndex;
	}

	private Vector2 GetClosestPointOnLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
	{
		Vector2 line = lineEnd - lineStart;
		float lineLength = line.magnitude;
		line.Normalize();

		float projection = Vector2.Dot(point - lineStart, line);
		projection = Mathf.Clamp(projection, 0, lineLength);

		return lineStart + line * projection;
	}

	public float GetFrictionAt(Vector2 position) => frictionMultiplier;

	public float GetSlopeMultiplier(Vector2 position)
	{
		Vector2 normal = GetNormalAtPosition(position);
		float slopeAngle = Vector2.Angle(normal, Vector2.up);
		return Mathf.Sin(slopeAngle * Mathf.Deg2Rad);
	}

	public Transform StartPoint => startPoint;
	public Transform EndPoint => endPoint;

	private void OnDrawGizmos()
	{
		DrawControlPoints();
		DrawEndPoints();
	}

	private void DrawControlPoints()
	{
		if (controlPoints == null) return;

		Gizmos.color = Color.yellow;
		foreach (var point in controlPoints)
		{
			if (point != null) Gizmos.DrawSphere(point.position, 0.3f);
		}
	}

	private void DrawEndPoints()
	{
		if (startPoint != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(startPoint.position, 0.5f);
		}

		if (endPoint != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(endPoint.position, 0.5f);
		}
	}
}