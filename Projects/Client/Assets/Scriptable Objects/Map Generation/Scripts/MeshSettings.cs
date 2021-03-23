using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Custom Scriptable Objects/Map Generation/New Mesh Settings")]
public class MeshSettings : UpdatableSettings {

	public const int numSupportedLODs = 5;
	public const int numSupportedChunkSizes = 11;
	public const int numSupportedFlatshadedChunkSizes = 5;
	public static readonly int[] supportedChunkSizes = {16,32,48,72,96,120,144,168,192,216,240};
	
	public float ySize = 75f;
	public float meshScale = 2.5f;
	public bool useFlatShading;

	[Range(0,numSupportedChunkSizes-1)]
	public int chunkSizeIndex;
	[Range(0,numSupportedFlatshadedChunkSizes-1)]
	public int flatshadedChunkSizeIndex;


	// num verts per line of mesh rendered at LOD = 0. Includes the 2 extra verts that are excluded from final mesh, but used for calculating normals
	public int numVertsPerLine {
		get {
			return supportedChunkSizes [(useFlatShading) ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;
		}
	}

	public float meshWorldSize {
		get {
			return (numVertsPerLine - 3) * meshScale;
		}
	}


}
