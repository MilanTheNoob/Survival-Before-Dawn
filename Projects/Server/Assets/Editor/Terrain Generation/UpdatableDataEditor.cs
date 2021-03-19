using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(UpdatableSettings), true)]
public class UpdatableDataEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		UpdatableSettings data = (UpdatableSettings)target;

		if (GUILayout.Button ("Update")) {
			data.NotifyOfUpdatedValues ();
			EditorUtility.SetDirty (target);
		}
	}
	
}
