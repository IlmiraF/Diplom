using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockUnitController : MonoBehaviour
{
	[SerializeField] private float FOVAngle;
	[SerializeField] private float smoothDamp;
	[SerializeField] private LayerMask obstacleMask;
	[SerializeField] private LayerMask enemyMask;
	[SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles;

	private List<FlockUnitController> cohesionNeighbours = new List<FlockUnitController>();
	private List<FlockUnitController> avoidanceNeighbours = new List<FlockUnitController>();
	private List<FlockUnitController> aligementNeighbours = new List<FlockUnitController>();
	private Flock assignedFlock;
	private Vector3 currentVelocity;
	private Vector3 currentObstacleAvoidanceVector;
	private float speed;
	public bool girl;

	private Animator animator;
	private float stateTimer = 0;

	[SerializeField]
	private FlockUnitStates states;

	private Vector3 enemyVec = Vector3.zero;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
	}

	public void AssignFlock(Flock flock)
	{
		assignedFlock = flock;
	}

	public void InitializeSpeed(float speed)
	{
		this.speed = speed;
	}

	private void Update()
	{
		stateTimer += Time.deltaTime;
		SetState(states);
	}

	public void SetState(FlockUnitStates state)
	{
		states = state;
		states.DoState(this);
	}

	public void MoveUnit()
	{
		FindNeighbours();
		CalculateSpeed();

		var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
		var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
		var aligementVector = CalculateAligementVector() * assignedFlock.aligementWeight;
		var boundsVector = CalculateBoundsVector() * assignedFlock.boundsWeight;
		var obstacleVector = CalculateObstacleVector() * assignedFlock.obstacleWeight;

		var moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector + obstacleVector;
		moveVector = Vector3.SmoothDamp(transform.forward, moveVector, ref currentVelocity, smoothDamp);
		moveVector = moveVector.normalized * speed;
		if (moveVector == Vector3.zero)
			moveVector = transform.forward;

		transform.forward = moveVector;

		transform.position += moveVector * Time.deltaTime;
	}

	public void RunAway()
    {
		var moveVector = -enemyVec;
		moveVector = Vector3.SmoothDamp(transform.forward, moveVector, ref currentVelocity, smoothDamp);
		moveVector = moveVector.normalized * speed;
		if (moveVector == Vector3.zero)
			moveVector = transform.forward;

		transform.forward = -moveVector;

		transform.position += moveVector * Time.deltaTime;
	}

	private void FindNeighbours()
	{
		cohesionNeighbours.Clear();
		avoidanceNeighbours.Clear();
		aligementNeighbours.Clear();
		var allUnits = assignedFlock.allUnits;
		for (int i = 0; i < allUnits.Length; i++)
		{
			var currentUnit = allUnits[i];
			if (currentUnit != this)
			{
				float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentUnit.transform.position - transform.position);
				if (currentNeighbourDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
				{
					cohesionNeighbours.Add(currentUnit);
				}
				if (currentNeighbourDistanceSqr <= assignedFlock.avoidanceDistance * assignedFlock.avoidanceDistance)
				{
					avoidanceNeighbours.Add(currentUnit);
				}
				if (currentNeighbourDistanceSqr <= assignedFlock.aligementDistance * assignedFlock.aligementDistance)
				{
					aligementNeighbours.Add(currentUnit);
				}
			}
		}
	}

	private void CalculateSpeed()
	{
		if (cohesionNeighbours.Count == 0)
			return;
		speed = 0;
		for (int i = 0; i < cohesionNeighbours.Count; i++)
		{
			speed += cohesionNeighbours[i].speed;
		}

		speed /= cohesionNeighbours.Count;
		speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
	}

	private Vector3 CalculateCohesionVector()
	{
		var cohesionVector = Vector3.zero;
		if (cohesionNeighbours.Count == 0)
			return Vector3.zero;
		int neighboursInFOV = 0;
		for (int i = 0; i < cohesionNeighbours.Count; i++)
		{
			if (IsInFOV(cohesionNeighbours[i].transform.position))
			{
				neighboursInFOV++;
				cohesionVector += cohesionNeighbours[i].transform.position;
			}
		}

		cohesionVector /= neighboursInFOV;
		cohesionVector -= transform.position;
		cohesionVector = cohesionVector.normalized;
		return cohesionVector;
	}

	private Vector3 CalculateAligementVector()
	{
		var aligementVector = transform.forward;
		if (aligementNeighbours.Count == 0)
			return transform.forward;
		int neighboursInFOV = 0;
		for (int i = 0; i < aligementNeighbours.Count; i++)
		{
			if (IsInFOV(aligementNeighbours[i].transform.position))
			{
				neighboursInFOV++;
				aligementVector += aligementNeighbours[i].transform.forward;
			}
		}

		aligementVector /= neighboursInFOV;
		aligementVector = aligementVector.normalized;
		return aligementVector;
	}

	private Vector3 CalculateAvoidanceVector()
	{
		var avoidanceVector = Vector3.zero;
		if (aligementNeighbours.Count == 0)
			return Vector3.zero;
		int neighboursInFOV = 0;
		for (int i = 0; i < avoidanceNeighbours.Count; i++)
		{
			if (IsInFOV(avoidanceNeighbours[i].transform.position))
			{
				neighboursInFOV++;
				avoidanceVector += (transform.position - avoidanceNeighbours[i].transform.position);
			}
		}

		avoidanceVector /= neighboursInFOV;
		avoidanceVector = avoidanceVector.normalized;
		return avoidanceVector;
	}

	private Vector3 CalculateBoundsVector()
	{
		var offsetToCenter = assignedFlock.allUnits[0].transform.position - transform.position;
		bool isNearCenter = (offsetToCenter.magnitude >= assignedFlock.boundsDistance * 0.9f);
		return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
	}

	public Vector3 CalculateObstacleVector()
	{
		var obstacleVector = Vector3.zero;
		if (ObstacleFind())
		{
			obstacleVector = FindBestDirectionToAvoidObstacle();
		}
		else
		{
			currentObstacleAvoidanceVector = Vector3.zero;
		}
		return obstacleVector;
	}

    public Vector3 CalculateEnemyObstacleVector(Vector3 target)
    {
		Vector3 obstacleVector = Vector3.zero;
		if (IsInFOV(target))
		{
			obstacleVector = (transform.position - target).normalized;
		}
        return obstacleVector;
    }

    public bool ObstacleFind()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, assignedFlock.obstacleDistance, obstacleMask))
		{
			return true;
		}
		return false;
	}

	public bool EnemyFind()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, assignedFlock.obstacleDistance, enemyMask))
		{
			enemyVec = CalculateEnemyObstacleVector(hit.transform.position);
			return true;
		}
		return false;
	}

	private Vector3 FindBestDirectionToAvoidObstacle()
	{
		if (currentObstacleAvoidanceVector != Vector3.zero)
		{
			if (!ObstacleFind())
			{
				return currentObstacleAvoidanceVector;
			}
		}
		float maxDistance = int.MinValue;
		var selectedDirection = Vector3.zero;
		for (int i = 0; i < directionsToCheckWhenAvoidingObstacles.Length; i++)
		{

			RaycastHit hit;
			var currentDirection = transform.TransformDirection(directionsToCheckWhenAvoidingObstacles[i].normalized);
			if (Physics.Raycast(transform.position, currentDirection, out hit, assignedFlock.obstacleDistance, obstacleMask))
			{

				float currentDistance = (hit.point - transform.position).sqrMagnitude;
				if (currentDistance > maxDistance)
				{
					maxDistance = currentDistance;
					selectedDirection = currentDirection;
				}
			}
			else
			{
				selectedDirection = currentDirection;
				currentObstacleAvoidanceVector = currentDirection.normalized;
				return selectedDirection.normalized;
			}
		}
		return selectedDirection.normalized;
	}

	private bool IsInFOV(Vector3 position)
	{
		return Vector3.Angle(transform.forward, position - transform.position) <= FOVAngle;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, transform.forward * assignedFlock.obstacleDistance);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(assignedFlock.allUnits[0].transform.position, assignedFlock.allUnits[0].transform.forward * assignedFlock.obstacleDistance);
	}

	public bool TimeOut(float timeToWait) => stateTimer >= timeToWait;

	public float Timer { get { return stateTimer; } set { stateTimer = value; } }
	public Animator Animator { get { return animator; } }
	public float Speed { get { return speed; } set { speed = value; } }
}