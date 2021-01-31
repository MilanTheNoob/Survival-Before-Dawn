using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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
	public Dictionary<Vector2, TerrainChunk> chunkDictionary = new Dictionary<Vector2, TerrainChunk>();

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public PropsSettings propsSettings;

	[HideInInspector]
	public float meshWorldSize;
	[HideInInspector]
	public int chunksVisibleInViewDst;

	void Start()
	{
		float maxViewDst = meshSettings.detailLevels[meshSettings.detailLevels.Length - 1].visibleDstThreshold;
		meshWorldSize = meshSettings.meshWorldSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);
	}

	public void UpdateChunks(Dictionary<int, Player> players)
	{
		for (int i = 0; i < players.Count; i++)
        {
			Player player = players.ElementAt(i).Value;

			Vector2 viewerPosition = new Vector2(player.transform.position.x, player.transform.position.z);
			Vector2 viewerPositionOld = new Vector2(player.oldPos.x, player.oldPos.y);

			if (viewerPosition != viewerPositionOld)
			{
				viewerPositionOld = viewerPosition;

				HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

				int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
				int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

				for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
				{
					for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
					{
						Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
						if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
						{
							if (!chunkDictionary.ContainsKey(viewedChunkCoord))
							{
								TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, player);
								chunkDictionary.Add(viewedChunkCoord, newChunk);
								newChunk.Load();
							}
							else if (!player.chunks.Contains(viewedChunkCoord))
                            {
								ServerSend.ChunkData(chunkDictionary[viewedChunkCoord].chunkData, player, meshSettings.numVertsPerLine, meshSettings.numVertsPerLine);
								player.chunks.Add(viewedChunkCoord);
							}
						}
					}
				}
			}
		}
	}

	public void SpawnChunksOnPlayer(Player player)
	{
		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

		int currentChunkCoordX = Mathf.RoundToInt(player.transform.position.x / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt(player.transform.position.y / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
			{
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
				{
					if (!chunkDictionary.ContainsKey(viewedChunkCoord))
					{
						TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, transform, player);
						chunkDictionary.Add(viewedChunkCoord, newChunk);
						newChunk.Load();
					}
					else
					{
						ServerSend.ChunkData(chunkDictionary[viewedChunkCoord].chunkData, player, meshSettings.numVertsPerLine, meshSettings.numVertsPerLine);
					}

				}

			}
		}
	}

	public void UpdateChunk(Dictionary<Vector3, PropDataStruct> newProps, Vector2 coord)
    {
		//if (chunkDictionary.ContainsKey(coord))

		chunkDictionary[coord]
    }
}

[System.Serializable]
public class PropDataStruct
{
	public Vector3 rot;
	public int group;
	public int prop;
}

[System.Serializable]
public class ChunkDataStruct
{
	public Vector2 coord;
	public HeightMap heightMap;

	public Dictionary<Vector3, PropDataStruct> props = new Dictionary<Vector3, PropDataStruct>();
	public List<PropDataStruct> items = new List<PropDataStruct>();
	public List<PropDataStruct> structures = new List<PropDataStruct>();
}