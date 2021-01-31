using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class TerrainGenerator : MonoBehaviour
{
	public static TerrainGenerator instance;

	public List<Vector2> biomesDictionary = new List<Vector2>();
	[HideInInspector]
	public Dictionary<Vector2, int> biomesIndex = new Dictionary<Vector2, int>();

	[HideInInspector]
	public Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	[HideInInspector]
	public List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

	[HideInInspector]
	public Dictionary<Vector2, MultiplayerTerrainChunk> multiplayerChunkDictionary = new Dictionary<Vector2, MultiplayerTerrainChunk>();
	[HideInInspector]
	public List<MultiplayerTerrainChunk> visibleMultiplayerChunks = new List<MultiplayerTerrainChunk>();

	public delegate void ChunksAdded();
	public ChunksAdded ChunksAddedCallback;

	[Header("All the scriptable objects needed")]
	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;

	[Header("Basic vars")]
	public Transform viewer;
	public Material mapMaterial;

	public enum ChildType
    {
		Prop,
		Structure,
		StructurePiece,
		Item
    }

	public enum GenerateType
    {
		Standard,
		PreGen
    }
	[HideInInspector]
	public GenerateType generateType;

	[HideInInspector]
	public float meshWorldSize;
	[HideInInspector]
	public int chunksVisibleInViewDst;

	Vector2 viewerPosition;
	Vector2 viewerPositionOld;

	void Awake()
    {
		instance = this;

		heightMapSettings.noiseSettings.seed = SavingManager.SaveFile.seed;
	}

	// Called at the beginning of the game
	void Start()
	{
		// Get the max view distance
		float maxViewDst = meshSettings.detailLevels[meshSettings.detailLevels.Length - 1].visibleDstThreshold;
		// Get the terrain size
		meshWorldSize = meshSettings.meshWorldSize;
		// Get the chunks visible in the view distance
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

		UpdateVisibleChunks();
	}

	// Called every frame
	void Update()
	{
		// Get ther player's Vector2 pos
		viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

		if (viewerPosition != viewerPositionOld)
		{
				// Set the current viewer position to the old one
				viewerPositionOld = viewerPosition;
				// Update the visible chunks
				UpdateVisibleChunks();

				// Update colliders of all active chunks
				for (int i = 0; i < visibleTerrainChunks.Count; i++) { visibleTerrainChunks[i].UpdateCollisionMesh(); }
		}
	}


	// Called to update the view distance
	public void UpdateViewDist()
    {
		// Get the max view distance
		float maxViewDst = meshSettings.detailLevels[meshSettings.detailLevels.Length - 1].visibleDstThreshold;
		// Get the terrain size
		meshWorldSize = meshSettings.meshWorldSize;
		// Get the chunks visible in the view distance
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

		// Remove all chunks
		ResetChunks();
	}

	public static Vector2 GetNearestChunk(Vector3 pos)
    {
		return new Vector2(Mathf.RoundToInt(pos.x / instance.meshWorldSize), Mathf.RoundToInt(pos.x / instance.meshWorldSize));
    }

	public static bool AddToNearestChunk(GameObject obj, ChildType childType)
	{
		Vector2 chunkLoc = new Vector2(Mathf.RoundToInt(obj.transform.position.x / instance.meshWorldSize), Mathf.RoundToInt(obj.transform.position.x / instance.meshWorldSize));

        try
        {
			if (childType == ChildType.Prop)
			{
				obj.transform.parent = instance.terrainChunkDictionary[chunkLoc].meshObject.transform.Find("Props Holder");
			}
			else if (childType == ChildType.Structure)
			{
				obj.transform.parent = instance.terrainChunkDictionary[chunkLoc].meshObject.transform.Find("Structures Holder");
			}
			else if (childType == ChildType.Item)
			{
				obj.transform.parent = instance.terrainChunkDictionary[chunkLoc].meshObject.transform.Find("Items Holder");
			}
			else if (childType == ChildType.StructurePiece)
			{
				obj.transform.parent = instance.terrainChunkDictionary[chunkLoc].meshObject.transform.Find("Structure Pieces Holder");
			}
		}
        catch (Exception ex) { return false; }

		return true;
	}

	public static bool CanAddToNearestChunk(Vector3 t)
	{
		Vector2 chunkLoc = new Vector2(Mathf.RoundToInt(t.x / instance.meshWorldSize), Mathf.RoundToInt(t.x / instance.meshWorldSize));
		if (instance.terrainChunkDictionary.ContainsKey(chunkLoc)) { Debug.Log("true"); return true; } else { Debug.Log("false"); return false; }
	}

	#region Singleplayer Code

	// Called to reset all the current chunks
	public void ResetChunks()
	{
		// Loops through all chunks in the dictionary
		for (int i = 0; i < terrainChunkDictionary.Count; i++)
		{
			terrainChunkDictionary.ElementAt(i).Value.RemoveChunk();
		}
		// Loops through all multiplayer chunks in the dictionary
		for (int i = 0; i < multiplayerChunkDictionary.Count; i++)
		{
			Destroy(multiplayerChunkDictionary.ElementAt(i).Value.meshObject);
		}

		// Clear the dictionary & visible list
		terrainChunkDictionary.Clear();
		visibleTerrainChunks.Clear();
		multiplayerChunkDictionary.Clear();
		visibleMultiplayerChunks.Clear();

		UpdateVisibleChunks();
	}

	// Called to Update all the visible chunks
	public void UpdateVisibleChunks()
	{
		// Update biomes first
		UpdateBiomes();

		// Create a new array for the already updated chunk coordinates
		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

		for (int i = 0; i < visibleTerrainChunks.Count; i++)
		{
			// Add the chunk positions to it
			alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
			// Update the chunk we added to it
			visibleTerrainChunks[i].UpdateTerrainChunk();
		}

		// A var
		bool addedNewChunk = false;

		// Get the current chunk the player is on
		int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
			{
				// Get the chunk we are looking at
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
				{
					if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
					{
						// Update the chunk we are looking at
						terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
					}
					else
					{
						// Create a new chunk
						TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, meshSettings.detailLevels, meshSettings.colliderLODIndex, transform, viewer, mapMaterial, generateType, GetChunkBiome(viewedChunkCoord));
						// Add it to the chunk dictionary
						terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
						// Toggle if it should be visible
						newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
						// Load the new chunk
						newChunk.Load();

						// Yes we added a new chunk
						addedNewChunk = true;
					}
				}

			}
		}
	}

	// Update the biomes
	public void UpdateBiomes()
	{
		// Get the biome dist
		int biomesInViewDst = chunksVisibleInViewDst;

		// Get the current biome coords
		int currentBiomeCoordX = Mathf.RoundToInt(viewerPosition.x / (meshWorldSize * 3));
		int currentBiomeCoordY = Mathf.RoundToInt(viewerPosition.y / (meshWorldSize * 3));

		// Loop through the biome positions
		for (int yOffset = -biomesInViewDst; yOffset <= biomesInViewDst; yOffset++)
		{
			for (int xOffset = -biomesInViewDst; xOffset <= biomesInViewDst; xOffset++)
			{
				// Get this biome coord
				Vector2 viewedBiomeCoord = new Vector2(currentBiomeCoordX + xOffset, currentBiomeCoordY + yOffset);

				if (!biomesDictionary.Contains(viewedBiomeCoord))
                {
					// Get a random biome and chunk pos vars
					int biomeId = PropsGeneration.instance.GetRandomBiome();
					float x = viewedBiomeCoord.x * 3;
					float y = viewedBiomeCoord.y * 3;

					// Add the biome & all the chunks
					biomesDictionary.Add(viewedBiomeCoord);

					if (!biomesIndex.ContainsKey(new Vector2(x - 1, y + 1))) biomesIndex.Add(new Vector2(x - 1, y + 1), biomeId);
					if (!biomesIndex.ContainsKey(new Vector2(x, y + 1))) biomesIndex.Add(new Vector2(x, y + 1), biomeId);
					if (!biomesIndex.ContainsKey(new Vector2(x + 1, y + 1))) biomesIndex.Add(new Vector2(x + 1, y + 1), biomeId);

					if (!biomesIndex.ContainsKey(new Vector2(x - 1, y))) biomesIndex.Add(new Vector2(x - 1, y), biomeId);
					if (!biomesIndex.ContainsKey(new Vector2(x, y))) biomesIndex.Add(viewedBiomeCoord * 3, biomeId);
					if (!biomesIndex.ContainsKey(new Vector2(x + 1, y))) biomesIndex.Add(new Vector2(x + 1, y), biomeId);

					if (!biomesIndex.ContainsKey(new Vector2(x - 1, y - 1))) biomesIndex.Add(new Vector2(x - 1, y - 1), biomeId);
					if (!biomesIndex.ContainsKey(new Vector2(x, y - 1))) biomesIndex.Add(new Vector2(x, y - 1), biomeId);
					if (!biomesIndex.ContainsKey(new Vector2(x + 1, y - 1))) biomesIndex.Add(new Vector2(x + 1, y - 1), biomeId);
				}
			}
		}
	}

	// Called to get the biome for a chunk
	int GetChunkBiome(Vector2 coord)
    {
		if (biomesIndex.ContainsKey(coord))
			return biomesIndex[coord];

		return -1;
    }

	// Called to add a chunk to the visible list if its visisble
	void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
	{
		if (isVisible)
		{
			// Add to the visible chunk array
			visibleTerrainChunks.Add(chunk);
		}
		else
		{
			// Remove from the visible chunk array
			visibleTerrainChunks.Remove(chunk);
		}
	}

	#endregion
}