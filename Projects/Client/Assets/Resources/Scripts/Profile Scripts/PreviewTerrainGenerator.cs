using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class PreviewTerrainGenerator : MonoBehaviour
{
	#region Singleton

	public static PreviewTerrainGenerator instance;

	// Called before Start
	void Awake()
	{
		// Set the instance of ourselves
		instance = this;
	}

	#endregion

	[HideInInspector]
	public Dictionary<Vector2, PreviewTerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, PreviewTerrainChunk>();

	public delegate void ChunksAdded();
	public ChunksAdded ChunksAddedCallback;

	[Header("All the scriptable objects needed")]
	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;

	[Header("Basic vars")]
	public Material mapMaterial;
	public float ViewDst = 50f;

	[HideInInspector]
	public float meshWorldSize;
	[HideInInspector]
	public int chunksVisibleInViewDst;

	void Start()
	{
		meshWorldSize = meshSettings.meshWorldSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(ViewDst / meshWorldSize);

		heightMapSettings.noiseSettings.seed = 0;

		PreviewTerrainChunk chunk0 = new PreviewTerrainChunk(new Vector2(0, 0), heightMapSettings, meshSettings, transform, mapMaterial);
		chunk0.Load();
		PreviewTerrainChunk chunk1 = new PreviewTerrainChunk(new Vector2(1, 0), heightMapSettings, meshSettings, transform, mapMaterial);
		chunk1.Load();
		PreviewTerrainChunk chunk2 = new PreviewTerrainChunk(new Vector2(0, -1), heightMapSettings, meshSettings, transform, mapMaterial);
		chunk2.Load();
		PreviewTerrainChunk chunk3 = new PreviewTerrainChunk(new Vector2(1, -1), heightMapSettings, meshSettings, transform, mapMaterial);
		chunk3.Load();
	}
}