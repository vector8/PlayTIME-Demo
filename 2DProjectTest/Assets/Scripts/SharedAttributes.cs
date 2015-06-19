using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SharedAttributes : MonoBehaviour 
{
	public Dictionary<string, object> attributes = new Dictionary<string, object>();

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public bool tryGetAttribute<T>(string name, out T value)
	{
		object attribute;

		if(attributes.TryGetValue(name, out attribute))
		{
			if(attribute.GetType().Equals(typeof(T)) || attribute.GetType().IsSubclassOf(typeof(T)))
			{
				value = (T)attribute;
				return true;
			}
			else
			{
				print (attribute.GetType().ToString());
				print("ERROR: Attribute " + name + " is not of type " + typeof(T).ToString() + ".");
				value = default(T);
				return false;
			}
		}
		else
		{
			print("Attribute " + name + " not found.");
			value = default(T);
			return false;
		}
	}

	public void addAttribute<T>(string name, T value)
	{
		attributes.Add(name, value);
	}

	public bool containsAttribute(string name)
	{
		return attributes.ContainsKey(name);
	}

	public bool trySetAttribute<T>(string name, T value)
	{
		object attribute;
		
		if(attributes.TryGetValue(name, out attribute))
		{
			if(attribute.GetType().Equals(typeof(T)) || attribute.GetType().IsSubclassOf(typeof(T)))
			{
				attributes[name] = value;
				return true;
			}
			else
			{
				print("ERROR: Attribute " + name + " is not of type " + typeof(T).ToString() + ".");
				return false;
			}
		}
		else
		{
			print("Attribute " + name + " not found.");
			return false;
		}
	}
}
