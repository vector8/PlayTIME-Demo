using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathTrigger : MonoBehaviour 
{
	public List<CustomAction> actions = new List<CustomAction>();

	public void run()
	{
		foreach(CustomAction a in actions)
		{
			a.run();
		}
	}
	
	public void initialize()
	{
		foreach(CustomAction a in actions)
		{
			a.initialize();
		}
	}
	
	public void reset() 
	{
		foreach(CustomAction a in actions)
		{
			a.reset();
		}
	}
}
