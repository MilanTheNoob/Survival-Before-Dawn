using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class TerrainGenerator : MonoBehaviour
{

	#region Singleton

	public static TerrainGenerator instance;

	// Called before Start
	void Awake()
	{
		// Set the instance of ourselves
		instance = this;
	}

	#endregion

	[HideInInspector]
	public Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	[HideInInspector]
	public Dictionary<Vector2, TerrainDataStruct> terrainDataList = new Dictionary<Vector2, TerrainDataStruct>();

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureSettings textureSettings;
	public PropsSettings propsSettings;
	public StructuresSettings structuresSettings;

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
	}
	/*
	// Called every frame
	public void UpdateChunks(List<Player> players)
	{
		for (int i = 0; i < players.Count; i++)
        {
			Vector2 viewerPosition = new Vector2(players[i].transform.position.x, players[i].transform.position.z);
			Vector2 viewerPositionOld = new Vector2(players[i].oldPos.x, players[i].oldPos.y);

			if (viewerPosition != viewerPositionOld)
			{
				// Set the current viewer position to the old one
				viewerPositionOld = viewerPosition;

				// Create a new array for the already updated chunk coordinates
				HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

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
							if (!terrainChunkDictionary.ContainsKey(viewedChunkCoord))
							{
								// Create a new chunk
								TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, players[i]);
								// Add it to the chunk dictionary
								terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
								// Load the new chunk and save the data
								terrainDataList.Add(viewedChunkCoord, newChunk.Load());
							}

							ServerSend.ChunkData(terrainDataList[viewedChunkCoord], players[i]);
						}

					}
				}
			}
		}
	}
	*/
	public void SpawnChunksOnPlayer(Player player)
    {
		// Create a new array for the already updated chunk coordinates
		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

		// Get the current chunk the player is on
		int currentChunkCoordX = Mathf.RoundToInt(player.transform.position.x / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt(player.transform.position.y / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
			{
				// Get the chunk we are looking at
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
				{
					if (!terrainChunkDictionary.ContainsKey(viewedChunkCoord))
					{
						// Create a new chunk
						TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, player);
						// Add it to the chunk dictionary
						terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
						// Load the new chunk and save the data
						terrainDataList.Add(viewedChunkCoord, newChunk.Load());
						ServerSend.ChunkData(terrainDataList[viewedChunkCoord], player);
					}
                    else
                    {
						ServerSend.ChunkData(terrainDataList[viewedChunkCoord], player);
					}

				}

			}
		}
	}

	public class TerrainDataStruct
	{
		public Vector2 coord = new Vector2();
		public Dictionary<string, Vector3> props = new Dictionary<string, Vector3>();
	}
}