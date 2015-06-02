using System;

public class Pair<T, U> where T : new()
	where U : new()
{
	public Pair()
	{
		this.first = new T();
		this.second = new U();
	}

	public Pair(T first, U second)
	{
		this.first = first;
		this.second = second;
	}

	public T first {get; set;}
	public U second {get; set;}
}
