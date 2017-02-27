using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *Author: Dezmon Gilbert
 *Purpose: To handle the scene.
*/

public class SceneManager : MonoBehaviour 
{
	// obtain the correct prefabs
	public GameObject zombie;
	public GameObject human;
	public GameObject obstacle;

	// determines if Debug lines are on
	public bool linesOn;

	// list to keep track of all gameobjects
	public List<GameObject> zombieList;
	public List<GameObject> humanList;
	public List<GameObject> obstacleList;

	// Use this for initialization
	void Start () 
	{
		humanList = new List<GameObject> ();
		zombieList = new List<GameObject> ();
		obstacleList = new List<GameObject> ();

		linesOn = false;

		// start with one zombie at a random positon
		Vector3 zombiePos = new Vector3 (Random.Range (-13f, 13f), 0, Random.Range (-13f, 13f));
		
		// create the zombie
		zombieList.Add((GameObject)Instantiate(zombie,zombiePos,Quaternion.identity));

		// create the humans
		for (int i = 0; i < 5; i++)
		{
			// place them at a random position
			Vector3 humanPos = new Vector3(Random.Range(-13f,13f), 0, Random.Range(-13f,13f));
			Vector3 randomRoation = new Vector3(0, Random.Range(-180f,180f), 0);

			// add the human to the array
			humanList.Add((GameObject)Instantiate(human,humanPos,Quaternion.Euler(randomRoation)));
		}

		// create obstacles
		for (int i = 0; i < 10; i++)
		{
			// place them at a random position
			Vector3 obstaclePos = new Vector3(Random.Range(-13f,13f), 0, Random.Range(-13f,13f));
			
			// add the human to the array
			obstacleList.Add((GameObject)Instantiate(obstacle,obstaclePos,Quaternion.identity));
		}
	}
	
	// Update is called once per frame
	void Update () 
	{	
		// press D to toggle the debug lines
		if (Input.GetKeyDown (KeyCode.D) == true) 
		{
			linesOn = !linesOn;
		}

		// press H to add a human to the scene
		if (Input.GetKeyDown (KeyCode.H) == true) 
		{
			// place them at a random position
			Vector3 humanPos = new Vector3(Random.Range(-13f,13f), 0, Random.Range(-13f,13f));
			
			// add the human to the array
			humanList.Add((GameObject)Instantiate(human,humanPos,Quaternion.identity));
		}

		/// only check for collsions if there are humans present
		if (humanList.Count > 0) 
		{
			CollisionResponse ();
		}

	}

	// respond to collision
	void CollisionResponse()
	{
		// obtain the count at the start
		int count = humanList.Count;

		for(int i = 0; i < count; i++)
		{
			// check to see if there is a collision, the current human looked at is still in the scene, 
			// and that the amount of humans didn't change
			if(humanList[i].GetComponent<Collisions>().HumanZombieCollision() == true && humanList[i] != null && count == humanList.Count)
			{
				// put the human in a temporary object
				GameObject destroy = humanList[i];

				// create a new zombie and add it to the list
				GameObject newZombie = (GameObject)Instantiate (zombie, destroy.transform.position ,destroy.transform.rotation);
				zombieList.Add(newZombie);

				// remove the human from the humanList
				humanList.Remove(humanList[i]);

				// destroy the human
				Destroy(destroy);

				// get out of the loop
				return;
			}
		}
	}
}
