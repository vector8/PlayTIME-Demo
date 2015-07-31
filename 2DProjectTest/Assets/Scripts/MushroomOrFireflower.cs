using UnityEngine;
using System.Collections;

public class MushroomOrFireflower : CustomAction
{
    private Spawn[] spawns = null;
    private bool done = false;

    public override void run(GameObject other = null, int id = 0)
    {
        if(spawns == null)
        {
            spawns = gameObject.GetComponents<Spawn>();
        }

        if(!done && other != null && other.gameObject.tag == "Player")
        {
            Health h = other.gameObject.GetComponent<Health>();
            if(h.hp > 1)
            {
                // Spawn fireflower
                foreach(Spawn s in spawns)
                {
                    if(s.toSpawn.name == "FireFlower")
                    {
                        s.run();
                        done = true;
                        return;
                    }
                }
            }
            else
            {
                // Spawn mushroom
                foreach (Spawn s in spawns)
                {
                    if (s.toSpawn.name == "Mushroom")
                    {
                        s.run();
                        done = true;
                        return;
                    }
                }
            }
        }
    }

    public override void reset()
    {
        if(spawns == null)
        {
            spawns = gameObject.GetComponents<Spawn>();
        }

        foreach(Spawn s in spawns)
        {
            s.reset();
        }

        done = false;
    }
}
