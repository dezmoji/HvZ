using UnityEngine;
using System.Collections;

/*
 *Author: Dezmon Gilbert
 *Purpose: To handle the movement of humans.
*/
public class Human : VehicleMovement
{
	// objects for seeking and fleeing
	public GameObject zombie;

	// needed for weighting and limiting 
	public float seekWeight;
	public float fleeWeight; 
	public float boundsWeight; // 150
	public float maxForce; // 15

	// needed for debug lines
	public Material forward;
	public Material right;
	public Material futurePos;

	// Use this for initialization
	public override void Start () 
	{
		base.Start ();
		seekWeight = 15;
		fleeWeight = 40;
		zombie = null;
	}

	// calculates the steering force
	public override void CalcSteeringForces()
	{
		// find the nearest zombie
		FindNearestZombie ();

		// create a new vector
		Vector3 ultimateForce = Vector3.zero;

		// if the human is out of bound, don't wander
		if (InBounds ().magnitude != 0) {
			// keep agent in bounds
			ultimateForce = InBounds () * boundsWeight;

			// avoid obstacles
			foreach (GameObject ob in manager.obstacleList) {
				ultimateForce += AvoidObstacle (ob, 3f);
			}

			// add fleeing if zombie is too close
			if (Vector3.Distance (gameObject.transform.position, zombie.transform.position) < 10f) {
				ultimateForce += Evade (zombie) * fleeWeight;
			} 
			
			// limit the new force
			ultimateForce = Vector3.ClampMagnitude (ultimateForce, maxForce);
			
			// apply the ultimate force
			ApplyForce (ultimateForce);

			return;
		}

		//if the human is in bounds
		else 
		{
			// avoid obstacles
			foreach (GameObject ob in manager.obstacleList) 
			{
				ultimateForce += AvoidObstacle (ob, 3f);
			}
			
			// add fleeing if zombie is too close
			if (Vector3.Distance (gameObject.transform.position, zombie.transform.position) < 5f) {
				ultimateForce += Evade (zombie) * fleeWeight;
			} 

			// wander around if theere is no threat nearby
			else 
			{
				// add the wandering force to the new force
				ultimateForce += Wander () * seekWeight;
			}
			
			// limit the new force
			ultimateForce = Vector3.ClampMagnitude (ultimateForce, maxForce);
			
			// apply the ultimate force
			ApplyForce (ultimateForce);
		}
	}

	// method to find the closest human 
	void FindNearestZombie()
	{	
		// create a float to hold the closests distance; set it to infinity
		float closestDistance = Mathf.Infinity;
		
		// iterate through each object in array
		foreach (GameObject z in manager.zombieList) 
		{
			// find the distance between current human and zombie
			float distance = Vector3.Distance(gameObject.transform.position, z.transform.position);
			
			// if the distance is less than the current closest distance
			if(distance < closestDistance)
			{
				// set the target to the closest human
				zombie = z;
				
				// set the current distance as the closest distance 
				closestDistance = distance;
			}
		}
	}

	// renders the debug lines
	void OnRenderObject()
	{
		if (manager.linesOn == true) 
		{
			// Set the material to be used for the first line
			forward.SetPass(0);
			// Draws one line
			GL.Begin(GL.LINES); 				 										// Begin to draw lines
			GL.Vertex(gameObject.transform.position); 									// First endpoint of this line
			GL.Vertex(gameObject.transform.forward + gameObject.transform.position);	// Second endpoint of this line
			GL.End();																	// Finish drawing the line
			
			// Second line
			// Set another material to draw this second line in a different color
			right.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex(gameObject.transform.position);
			GL.Vertex(gameObject.transform.right + gameObject.transform.position);
			GL.End ();

			futurePos.SetPass(0);
			GL.Begin(GL.LINES);
			float degRad = Mathf.PI / 180;
			for(float theta = 0.0f; theta < (2*Mathf.PI); theta += 0.01f)
			{
				Vector3 circle = (new Vector3(Mathf.Cos(theta) * .5f + gameObject.transform.position.x,
				                          gameObject.transform.position.y, Mathf.Sin(theta) * .5f + gameObject.transform.position.z));
				circle += velocity;
				circle.y = 1f;
				GL.Vertex3(circle.x, circle.y, circle.z);
			}
			GL.End();
		}
	}
}
