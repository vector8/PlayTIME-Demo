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
		for(int i = 0; i < times.Count; i++)
		{
			if(times[i] > 0f)
			{
				times[i] -= Time.deltaTime;
				if(times[i] <= 0f)
				{
					actions[i].run(null, i);
					if(repeats[i])
					{
						times[i] = originalTimes[i];
					}
				}
			}
		}
	}

	public void initialize()
	{
		for(int i = 0; i < actions.Count; i++)
		{
			times = new List<float>(originalTimes);
			actions[i].initialize();
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