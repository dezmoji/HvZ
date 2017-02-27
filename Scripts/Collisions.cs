using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 *Author: Dezmon Gilbert
 *Purpose: To handle collisions between Humans and Zombies.
 *Notes: The radii are close approximations I found using the radius of the character controller.
*/

public class Collisions : MonoBehaviour 
{
	// needed to get access to the list of Gameobjects
	private SceneManager manager;

	// keeps track of positions
	public Vector3 zombiePos;
	public Vector3 humanPos;

	// distance between objects
	public float distance;
	
	// Use this for initialization
	void Start ()
	{
		manager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager> ();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	// checks for collisions between humans and zombies
	public bool HumanZombieCollision()
	{
		// prevents complier errors 
		if( gameObject.GetComponent<Human>().zombie == null)
		{
			return false;
		}

		// loop through the list of zombies
		for(int i = 0; i < manager.zombieList.Count; i++)
		{	
			// find the current positions
			humanPos = gameObject.transform.position;
			zombiePos = manager.zombieList[i].transform.position;

			// find the distance between
			distance = Vector3.Distance (humanPos, zombiePos);

			// add the radii together (Human, then zombie)
			float radiiSum = .65f + .5f;

			// return true if there is a collision
			if ( radiiSum > distance)
			{
				return true;
			}
		}

		// no collision
		return false;
	}
}