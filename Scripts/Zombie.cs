using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *Author: Dezmon Gilbert
 *Purpose: To handle the movement of zombies
*/
public class Zombie : VehicleMovement
{
	// Gameobject the zombie is currently seeking
	public GameObject target;

	// needed for weighting and limiting 
	public float seekWeight;
	public float boundsWeight; // 150

	// max force allowed
	public float maxForce; // 10

	// materials needed for gl
	public Material forward;
	public Material right;
	public Material targeting;
	public Material futurePos;

	// Use this for initialization
	public override void Start () 
	{
		base.Start ();

		seekWeight = 15;

		// set target to null at start to avoid any compiling errors
		target = null;
	}

	// calculates the steering force
	public override void CalcSteeringForces()
	{
		// find the nearest human while there are still humans
		if (manager.humanList.Count > 0) 
		{
			FindNearestHuman ();
		}

		// if there is a target
		if (manager.humanList.Count > 0 && target != null) 
		{	
			// create a new vector
			Vector3 ultimateForce = Vector3.zero;

			// pursue the human
			ultimateForce += Pursue (target) * seekWeight;	

			// avoid obstacles
			foreach (GameObject ob in manager.obstacleList) 
			{
				ultimateForce += AvoidObstacle (ob, 3f);
			}

			// keep zombie in bounds
			ultimateForce += InBounds () * boundsWeight;
			
			// limit the new force
			ultimateForce = Vector3.ClampMagnitude (ultimateForce, maxForce);
			
			// apply the ultimate force
			ApplyForce (ultimateForce);

			return;
		}

		// if there are no humans
		else 
		{
			// create a new vector
			Vector3 ultimateForce = Vector3.zero;

			// wander around
			ultimateForce += Wander () * seekWeight;

			// avoid obstacles
			foreach (GameObject ob in manager.obstacleList) 
			{
				ultimateForce += AvoidObstacle (ob, 3f);
			}

			// keep agent in bounds
			ultimateForce += InBounds () * boundsWeight;
			
			// limit the new force
			ultimateForce = Vector3.ClampMagnitude (ultimateForce, maxForce);
			
			// apply the ultimate force
			ApplyForce (ultimateForce);
		}
	}

	// method to find the closest human 
	void FindNearestHuman()
	{
		// create a float to hold the closests distance; set it to infinity
		float closestDistance = Mathf.Infinity;

		// if there are no humans available
		if (manager.humanList.Count == 0) 
		{
			target = null;
			return;
		}

		// iterate through each object in array
		for (int i = 0; i < manager.humanList.Count; i++) 
		{
			// find the distance between current human and zombie
			float distance = Vector3.Distance(gameObject.transform.position, manager.humanList[i].transform.position);

			// if the distance is less than the current closest distance
			if(distance < closestDistance)
			{
				// set the target to the closest human
				target = manager.humanList[i];

				// set the current distance as the closest distance 
				closestDistance = distance;
			}
		}
	}

	// renders debug lines
	void OnRenderObject()
	{
		if (manager.linesOn == true) 
		{
			// Set the material to be used for the first line
			forward.SetPass(0);
			// Draws one line
			GL.Begin(GL.LINES); 				 										// Begin to draw lines
			GL.Vertex(gameObject.transform.position); 									// First endpoint of this line
			GL.Vertex(gameObject.transform.forward + gameObject.transform.position );	// Second endpoint of this line
			GL.End();																	// Finish drawing the line
			
			// Second line
			// Set another material to draw this second line in a different color
			right.SetPass(0);
			GL.Begin(GL.LINES);
			GL.Vertex(gameObject.transform.position);
			GL.Vertex(gameObject.transform.right + gameObject.transform.position);
			GL.End ();
			
			if (target != null) 
			{
				targeting.SetPass(0);
				GL.Begin(GL.LINES);
				GL.Vertex(gameObject.transform.position);
				GL.Vertex(target.transform.position);
				GL.End ();
			}

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
