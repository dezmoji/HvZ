using UnityEngine;
using System.Collections;

/*
 * Author: Dezmon Gilbert
 * Purpose: Handles destroying objects through button clicks
 * */

public class Destruction : MonoBehaviour 
{

	// needed for the lists of objects
	private SceneManager manager;

	private CameraCycle cam;
	// Use this for initialization
	void Start () 
	{
		manager = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<SceneManager> ();
		cam = GameObject.FindGameObjectWithTag ("SceneManager").GetComponent<CameraCycle> ();

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	// destroy agents on mouse clicks
	void OnMouseDown()
	{
		// only available in First Person 
		if (cam.currentCameraIndex == 5) 
		{
			// check if object is zombie
			if (gameObject.tag == "Zombie") 
			{
				// always leave one zombie alive
				if(manager.zombieList.Count > 1)
				{
					// remove and destroy
					manager.zombieList.Remove(gameObject);
					Destroy(gameObject);
				}
			}

			// check if gameobject is human
			if (gameObject.tag == "Human") 
			{
				// remove and destroy
				manager.humanList.Remove(gameObject);
				Destroy(gameObject);
			}
		}
	}
}
