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

	[HideInInspector]
	public float meshWorldSize;
	[HideInInspector]
	public int chunksVisibleInViewDst;

	// Called at the beginning of the game
	void Start()
	{
		// Get the max view distance
		float maxViewDst = meshSettings.detailLevels[meshSettings.detailLevels.Length - 1].visibleDstThreshold;
		// Get the terrain size
		meshWorldSize = meshSettings.meshWorldSize;
		// Get the chunks visible in the view distance
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

		// Set the seed for the world
		heightMapSettings.noiseSettings.seed = 0;

		// Get the current chunk the player is on
		int currentChunkCoordX = Mathf.RoundToInt(0 / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt(0 / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
			{
				// Get the chunk we are looking at
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (!terrainChunkDictionary.ContainsKey(viewedChunkCoord))
				{
					// Create a new chunk
					PreviewTerrainChunk newChunk = new PreviewTerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, mapMaterial);
					// Add it to the chunk dictionary
					terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
					// Load the new chunk
					newChunk.Load();
				}
			}
		}
	}
}