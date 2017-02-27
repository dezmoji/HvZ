using UnityEngine;
using System.Collections;

/*
 * Author: Dezmon Gilbert
 * Purpose: To change the weights of the agents
 * */

public class ChangeWeight : MonoBehaviour 
{

	// will help to cap the changes
	public float seekChange;
	public float fleeChange;

	// Use this for initialization
	void Start () 
	{
		seekChange = 0;
		fleeChange = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Change ();
	}

	// handles changing the weights 
	void Change()
	{
		// decrement the seek weight
		if (Input.GetKey (KeyCode.S) == true && Input.GetKeyDown (KeyCode.LeftArrow) == true) 
		{
			if(seekChange >= -10)
			{
				seekChange--;
				if(gameObject.tag == "Human")
				{
					gameObject.GetComponent<Human>().seekWeight--;
				}
				if(gameObject.tag == "Zombie")
				{
					gameObject.GetComponent<Zombie>().seekWeight--;
				}
			}
		}
		
		// increment the seek weight
		if (Input.GetKey (KeyCode.S) == true && Input.GetKeyDown (KeyCode.RightArrow) == true) 
		{
			if(seekChange <= 10)
			{
				seekChange++;
				if(gameObject.tag == "Human")
				{
					gameObject.GetComponent<Human>().seekWeight++;
				}
				if(gameObject.tag == "Zombie")
				{
					gameObject.GetComponent<Zombie>().seekWeight++;
				}
			}
		}
		
		// decrement the flee weight
		if (Input.GetKey (KeyCode.F) == true && Input.GetKeyDown (KeyCode.LeftArrow) == true) 
		{
			if(fleeChange >= -20)
			{
				fleeChange--;
				if(gameObject.tag == "Human")
				{
					gameObject.GetComponent<Human>().fleeWeight--;
				}
			}
		}
		
		// incrmement the seek weight
		if (Input.GetKey (KeyCode.F) == true && Input.GetKeyDown (KeyCode.RightArrow) == true) 
		{
			if(fleeChange <= 20)
			{
				fleeChange++;
				if(gameObject.tag == "Human")
				{
					gameObject.GetComponent<Human>().fleeWeight++;
				}
			}
		}
	}
}
