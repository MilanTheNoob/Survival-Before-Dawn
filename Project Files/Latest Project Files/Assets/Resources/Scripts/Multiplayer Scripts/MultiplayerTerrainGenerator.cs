using UnityEngine;
using System.Collections.Generic;

public class MultiplayerTerrainGenerator : MonoBehaviour
{
	public static MultiplayerTerrainGenerator instance;

	[HideInInspector]
	public Dictionary<Vector2, MultiplayerTerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, MultiplayerTerrainChunk>();
	[HideInInspector]
	public List<MultiplayerTerrainChunk> visibleTerrainChunks = new List<MultiplayerTerrainChunk>();

	public delegate void ChunksAdded();
	public ChunksAdded ChunksAddedCallback;

	[Header("All the scriptable objects needed")]
	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;

	[Header("Basic vars")]
	public Material mapMaterial;

	public enum ChildType
	{
		Prop,
		Structure,
		StructurePiece,
		Item
	}

	[HideInInspector]
	public Transform viewer;
	[HideInInspector]
	public float meshWorldSize;
	[HideInInspector]
	public int chunksVisibleInViewDst;

	Vector2 viewerPosition;
	Vector2 viewerPositionOld;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		float maxViewDst = meshSettings.detailLevels[meshSettings.detailLevels.Length - 1].visibleDstThreshold;
		meshWorldSize = meshSettings.meshWorldSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);
	}

	void FixedUpdate()
	{
		if (viewer != null)
        {
			viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

			if (viewerPosition != viewerPositionOld)
			{
				viewerPositionOld = viewerPosition;
				UpdateVisibleChunks();
			}
		}
	}

	public void UpdateVisibleChunks()
	{
		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

		for (int i = 0; i < visibleTerrainChunks.Count; i++)
		{
			alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
			visibleTerrainChunks[i].UpdateTerrainChunk();
		}

		int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
			{
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
				{
					if (terrainChunkDictionary.ContainsKey(viewedChunkCoord)) { terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk(); }
				}

			}
		}
	}

	public void SpawnChunk(ChunkDataStruct chunkData)
	{
		if (!terrainChunkDictionary.ContainsKey(chunkData.coord))
		{
			MultiplayerTerrainChunk newChunk = new MultiplayerTerrainChunk(meshSettings, transform, viewer, mapMaterial, chunkData);
			newChunk.Load();
			terrainChunkDictionary.Add(chunkData.coord, newChunk);
		}
	}

	public static MultiplayerTerrainChunk GetNearestChunk(Vector3 pos)
	{
		return instance.terrainChunkDictionary[new Vector2(Mathf.RoundToInt(pos.x / instance.meshWorldSize), Mathf.RoundToInt(pos.x / instance.meshWorldSize))];
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