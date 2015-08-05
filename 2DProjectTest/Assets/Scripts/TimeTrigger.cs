using UnityEngine;
using System;
using System.Collections.Generic;

public class TimeTrigger : MonoBehaviour 
{
	public List<CustomAction> actions = new List<CustomAction>();
	public List<float> originalTimes = new List<float>();
	public List<bool> repeats = new List<bool>();

	private List<float> times = new List<float>();

	void Update()
	{
        if(!LevelManager.instance.paused)
        {
		    for(int i = 0; i < times.Count; i++)
		    {
			    if(times[i] > 0f)
			    {
				    times[i] -= Time.deltaTime;
				    if(times[i] <= 0f)
				    {
                        LevelManager.instance.addActionQueueItem(actions[i], null, i);
					    if(repeats[i])
					    {
						    times[i] = originalTimes[i];
					    }
				    }
			    }
		    }
        }
    }

	public void initialize()
	{
		times = new List<float>(originalTimes);

		foreach(CustomAction a in actions)
		{
			a.initialize();
		}
	}

	public void reset() 
	{
		for(int i = 0; i < actions.Count; i++)
		{
			times[i] = originalTimes[i];
			actions[i].reset();
		}
	}
}