using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	public static T instance
	{
		get
		{
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