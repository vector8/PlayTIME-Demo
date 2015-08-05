using UnityEngine;
using System.Collections;

public class DespawnOther : CustomAction
{
    public int deathAnimID;

	public override void run(GameObject other = null, int id = 0)
	{
		if(other != null && isValidTag(other.tag))
		{
            Despawn d = other.GetComponent<Despawn>();
            if(d == null)
            {
                d = other.AddComponent<Despawn>();
            }
            d.deathAnimID = deathAnimID;
            d.run();
        }
	}

	public override void reset()
	{
	}
}
