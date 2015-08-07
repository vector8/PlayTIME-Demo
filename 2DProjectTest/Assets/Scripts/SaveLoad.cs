using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SaveLoad : MonoBehaviour
{
	public Camera topCamera, bottomCamera;
    public CameraControl cameraControl;
    public LevelDesignTIME ldTime;
    private LevelManager lvlManager = LevelManager.instance;

	public void save(string path)
	{
		StreamWriter writer = new StreamWriter(path);

        string entry = "";
        entry = topCamera.transform.position.x + "," + topCamera.transform.position.y + "," + topCamera.transform.position.z + "," + topCamera.orthographicSize + ",";
        writer.WriteLine(entry);
        entry = bottomCamera.transform.position.x + "," + bottomCamera.transform.position.y + "," + bottomCamera.transform.position.z + ",";
        writer.WriteLine(entry);

        if(cameraControl.staticFollowTarget != null)
        {
            entry = cameraControl.staticFollowTarget.transform.position.x + "," + cameraControl.staticFollowTarget.transform.position.y + "," + cameraControl.staticFollowTarget.transform.position.z + ",";
            entry += cameraControl.followOffset.x + "," + cameraControl.followOffset.y + ",";
        }
        else
        {
            entry = ",,,,,";
        }
        writer.WriteLine(entry);

        for (int i = 0; i < lvlManager.staticPlacedObjects.Count; i++)
        {
            entry = getSaveEntryString(lvlManager.placedObjects[i], lvlManager.staticPlacedObjects[i]);
            writer.WriteLine(entry);
        }

        for (int i = 0; i < lvlManager.staticBackgroundPlacedObjects.Count; i++)
        {
            entry = getSaveEntryString(lvlManager.backgroundPlacedObjects[i], lvlManager.staticBackgroundPlacedObjects[i]);
            writer.WriteLine(entry);
        }

        writer.Close();
	}

	public IEnumerator loadGame(string path)
	{
	    StreamReader reader = new StreamReader(path);

        try
        {
            lvlManager.wipeLevel();

            string[] delimiters = { "," };

            // Load top camera state
            string entry = reader.ReadLine();
            string[] tokens = entry.Split(delimiters, StringSplitOptions.None);
            Vector3 position = new Vector3(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]));
            topCamera.transform.position = position;
            topCamera.orthographicSize = float.Parse(tokens[3]);

            // Load bottom camera state
            entry = reader.ReadLine();
            tokens = entry.Split(delimiters, StringSplitOptions.None);
            position = new Vector3(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]));
            bottomCamera.transform.position = position;

            entry = reader.ReadLine();
            tokens = entry.Split(delimiters, StringSplitOptions.None);
            bool camHasFollowTarget = tokens[0].Length > 0;
            Vector2 followOffset = new Vector2();
            if(camHasFollowTarget)
            {
                position = new Vector3(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]));
                followOffset.x = float.Parse(tokens[3]);
                followOffset.y = float.Parse(tokens[4]);
            }

            // Populate the level with all game objects
            while (reader.Peek() != -1)
            {
                entry = reader.ReadLine();
                yield return StartCoroutine(processSaveEntry(entry));
            }

            if(camHasFollowTarget)
            {
                cameraControl.setFollowTarget(position);
                cameraControl.followOffset = followOffset;
            }
            else
            {
                cameraControl.clearFollowTarget();
            }
        }
        finally
        {
            reader.Close();
            lvlManager.loading = false;
        }
	}

    private IEnumerator processSaveEntry(string entry)
    {
        string[] delimiters = {","};
        string[] tokens = entry.Split(delimiters, StringSplitOptions.None);

        if(tokens.Length > 0)
        {
            int i = 0;
            string rfidKey = tokens[i++];

            if (rfidKey == "0a074861d4")
            {
                print("Stop here.");
            }

            yield return StartCoroutine(ldTime.rfidFoundCoroutine(rfidKey));
            
            Vector3 position = new Vector3(float.Parse(tokens[i++]), float.Parse(tokens[i++]), float.Parse(tokens[i++]));
            Pair<GameObject, GameObject> p;
            //ldTime.activeKey = rfidKey; // In case the object has a child object or spawn component, which will change the activeKey
            if (ldTime.database[ldTime.activeKey].first.tag != "Background")
            {
                ldTime.PlaceObject(position, true, true);
                p = lvlManager.getObjectExactlyAtPosition(position);
            }
            else
            {
                ldTime.PlaceObject(position, true, true);
                p = lvlManager.getBackgroundObjectExactlyAtPosition(position);
            }
            Vector3 scale = new Vector3(float.Parse(tokens[i++]), float.Parse(tokens[i++]), float.Parse(tokens[i++]));
            Resize r = p.second.GetComponent<Resize>();
            if(r != null)
            {
                r.setNewScale(scale);
            }
            else
            {
                p.first.transform.localScale = scale;
                p.second.transform.localScale = scale;
            }

            PathFollowing pf = p.first.GetComponent<PathFollowing>();
            if(pf != null)
            {
                pf.pathSpeed = float.Parse(tokens[i++]);
                int pathPointsCount = int.Parse(tokens[i++]);

                for(int k = 0; k < pathPointsCount; k++)
                {

                    Vector3 point = new Vector3(float.Parse(tokens[i++]), float.Parse(tokens[i++]), float.Parse(tokens[i++]));
                    pf.pathPoints.Add(point);
                }
            }

            Jump j = p.first.GetComponent<Jump>();
            if (j != null)
            {
                j.burst = float.Parse(tokens[i++]);
            }

            Move m = p.first.GetComponent<Move>();
            if (m != null)
            {
                m.maxSpeed[0] = float.Parse(tokens[i++]);
                m.maxSpeed[1] = float.Parse(tokens[i++]);
                m.maxSpeed[2] = float.Parse(tokens[i++]);
                m.maxSpeed[3] = float.Parse(tokens[i++]);
            }

            MoveHorizontalUntilCollision mh = p.first.GetComponent<MoveHorizontalUntilCollision>();
            if (mh != null)
            {
                mh.speed = float.Parse(tokens[i++]);
            }
        }
    }

	private string getSaveEntryString(GameObject objectToSave, GameObject staticObjectToSave)
	{
		string entry = "";
		entry += staticObjectToSave.GetComponent<RFIDKey>().rfidKey + ",";
		Vector3 position = staticObjectToSave.transform.position;
		entry += position.x + "," + position.y + "," + position.z + ",";
        Vector3 scale = staticObjectToSave.transform.localScale;
        entry += scale.x + "," + scale.y + "," + scale.z + ",";

		PathFollowing p = objectToSave.GetComponent<PathFollowing>();
		if(p!= null)
		{
			entry += p.pathSpeed + ",";
            entry += p.pathPoints.Count + ",";

			for(int i = 0; i < p.pathPoints.Count; i++)
			{
				entry += p.pathPoints[i].x + "," + p.pathPoints[i].y + "," + p.pathPoints[i].z + ",";
			}
		}

        Jump j = objectToSave.GetComponent<Jump>();
        if(j != null)
        {
            entry += j.burst + ",";
        }

        Move m = objectToSave.GetComponent<Move>();
        if (m != null)
        {
            entry += m.maxSpeed[0] + "," + m.maxSpeed[1] + "," + m.maxSpeed[2] + "," + m.maxSpeed[3] + ",";
        }

        MoveHorizontalUntilCollision mh = objectToSave.GetComponent<MoveHorizontalUntilCollision>();
        if(mh != null)
        {
            entry += mh.speed;
        }

        return entry;
	}
}