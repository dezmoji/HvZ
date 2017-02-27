using UnityEngine;
using System.Collections;

/*
 *Author: Dezmon Gilbert
 *Purpose: To handle the basic movement of agents and hold different steering functions
*/

abstract public class VehicleMovement : MonoBehaviour 
{
	// vectors needed for movement
	protected Vector3 acceleration;
	protected Vector3 direction;
	protected Vector3 velocity;
	protected Vector3 position;

	// float necessary for forces
	public float mass; // 1
	public float maxSpeed; // 5 for Human, 3 for Zombie

	// needed for the lists of objects and weight changes
	protected SceneManager manager;

	// holds the radius of the area
	protected float boundsRadius;

	// Use this for initialization
	public virtual void Start () 
	{
		manager = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager> ();

		// the radius of the controller (.5f) is multiplied by the scale(39) to get the radius of the floor
		boundsRadius = GameObject.FindGameObjectWithTag ("Floor").GetComponent<SphereCollider> ().radius * 39f;
	}
	
	// Update is called once per frame
	protected void Update () 
	{
		CalcSteeringForces ();
		UpdatePosition ();
		SetTransform ();	
	}
	
	// update the position of the vehicle
	void UpdatePosition()
	{
		// grab the world position from the transform component
		position = gameObject.transform.position;
		
		// add accel to vel * time
		velocity += acceleration * Time.deltaTime;
		
		// add velocity to position
		position += velocity * Time.deltaTime;
		
		// derive a normalized direction
		direction = velocity.normalized;
		
		// start fresh with new forces every frame
		acceleration = Vector3.zero;
	}
	
	// set the transform compoent to reflect the local position vector
	void SetTransform()
	{
		// setting the y position to 0 ensures that the creature always stays on doesn't move up or donw
		position.y = 0;
		transform.forward = direction;
		gameObject.GetComponent<CharacterController> ().Move (velocity * Time.deltaTime);
	}
	
	// adds to the acceleration
	protected void ApplyForce(Vector3 force)
	{
		acceleration += force / mass;
	}
	
	// will determine the steering force for an object to reach a desired location
	private Vector3 Seek(Vector3 targetPos)
	{
		// find the vector pointing from myself to the target
		Vector3 desired = targetPos - position;
		
		// scale magnitude to maximum speed to move as quickly as possible
		desired = desired.normalized * maxSpeed;
		
		// find the steering force
		Vector3 steeringForce = desired - velocity;
		
		// return the steering force
		return steeringForce;
	}
	
	// will determine the steering force for an object to reach a desired location
	private Vector3 Flee(Vector3 targetPos)
	{
		// find the vector pointing from myself to the target
		Vector3 desired = targetPos - position;
		
		// scale magnitude to maximum speed to move as quickly as possible
		// make the desired vector point away by multiplying by -1
		desired = desired.normalized * maxSpeed * -1;
		
		// find the steering force
		Vector3 steeringForce = desired - velocity;
		
		// return the steering force
		return steeringForce;
	}

	// predicts where target will go and then seeks that position
	protected Vector3 Pursue(GameObject target)
	{
		// find the distance between the target and gameobject
		Vector3 distance = target.GetComponent<VehicleMovement>().position - position;	

		// value to scale by to look ahead
		float updatesAhead = distance.magnitude / maxSpeed;

		// get the future position
		Vector3 futurePos = target.GetComponent<VehicleMovement>().position + 
			target.GetComponent<VehicleMovement>().velocity * updatesAhead;

		// seek that position
		return Seek (futurePos);
	}

	protected Vector3 Evade(GameObject target)
	{
		// find the distance between the target and gameobject
		Vector3 distance = target.GetComponent<VehicleMovement>().position - position;	

		// value to scale by to look ahead
		float updatesAhead = distance.magnitude / maxSpeed;

		// get the future position
		Vector3 futurePos = target.GetComponent<VehicleMovement>().position + 
			target.GetComponent<VehicleMovement>().velocity * updatesAhead;

		// flee that position
		return Flee(futurePos);
	}


	// will keep the agent in bounds
	protected Vector3 InBounds()
	{
		// find the distance between the object and center of floor(which is at 0,0,0)
		float distance = Vector3.Distance (gameObject.transform.position, Vector3.zero);

		// stay in bounds
		if (distance > boundsRadius) 
		{
			// seek the center
			return Seek (Vector3.zero) * distance/boundsRadius;
		}

		// return zero if agent is in bounds
		return Vector3.zero;
	}

	// handles avoiding obstacles
	protected Vector3 AvoidObstacle(GameObject obst, float safeDis)
	{

		Vector3 steer = Vector3.zero;

		// create a vector from the character to the center of the obstace 
		Vector3 vecToCenter = gameObject.transform.position - obst.transform.position;

		// find the distance to the obstacle
		float dist = vecToCenter.magnitude;

		// return a zero vector is obstace is too far to concern us
		if (dist > safeDis) 
		{
			return steer;
		}

		//retrn a zero vector if the obstacle is behind us
		float dotProd = Vector3.Dot(vecToCenter,gameObject.transform.forward);
		if (dotProd < 0) 
		{
			return steer;
		}

		// check if the obstacle can be passed without collisions
		dotProd = Vector3.Dot(vecToCenter,gameObject.transform.right);
		float objRadius = 0;

		if(gameObject.tag == "Human")
		{
			objRadius = .75f;
		}

		if(gameObject.tag == "Zombie")
		{
			objRadius = .55f;
		}
		float radiiSum = objRadius + .65f;

		if (radiiSum < Mathf.Abs(dotProd)) 
		{
			return steer;
		}

		Vector3 desired = Vector3.zero;

		// if the obstacle is on the right, go left
		if (dotProd > 0) 
		{
			desired = gameObject.transform.right;
		}

		// if the obstacle is on the left, go right
		if (dotProd < 0) 
		{
			desired = -gameObject.transform.right;
		}

		// find the steering force
		steer = (desired.normalized * maxSpeed) - velocity;

		// weight the steering force based on the closeness of the objects
		steer = steer * (safeDis / dist);

		return steer;
	}

	// handles the wandering around of the area
	protected Vector3 Wander()
	{
		// make the center of the circle a normalized velocity vector
		Vector3 circleCenter = velocity.normalized;

		// find a random spot within the unit circle and multiply it by a radius of 3f
		Vector2 rndSpot = Random.insideUnitCircle * 3f;

		// make the random spot on the circle into a Vector3 and and the the center circle to find the desried spot
		Vector3 desired = new Vector3(rndSpot.x, 0 ,rndSpot.y) + circleCenter;

		// find the steering vector
		Vector3 steer = (desired.normalized * maxSpeed) - velocity;

		//return steer
		return steer;
	}
	
	// all child classes need this
	public abstract void CalcSteeringForces();
}