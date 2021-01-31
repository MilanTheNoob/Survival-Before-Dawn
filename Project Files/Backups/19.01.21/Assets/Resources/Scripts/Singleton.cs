using UnityEngine;
/*
 * Base class for singletons. Inherit from this class to create a monobehavior singleton.
 * This is useful for manager classes, main game logic, or any 'static' class that you want handy access to or need monobehavior functionality.
 * 
 * Usage:
 * public class SomeManagerClass : Singleton<SomeManagerClass>
 * {
 * 	public bool someBool;
 * }
 * 
 * to get the value of someBool anywhere in code:
 * SomeManagerClass.Instance.someBool;
 * */

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	private static object _lock = new object();

	public static T instance
	{
		get
		{

			lock (_lock)
			{
				_instance = (T)FindObjectOfType(typeof(T));

				if (FindObjectsOfType(typeof(T)).Length > 1)
				{
					return _instance;
				}

				if (_instance == null)
				{
					GameObject singleton = new GameObject();
					_instance = singleton.AddComponent<T>();
					singleton.name = "(singleton) " + typeof(T).ToString();
				}

				return _instance;
			}
		}
	}

}