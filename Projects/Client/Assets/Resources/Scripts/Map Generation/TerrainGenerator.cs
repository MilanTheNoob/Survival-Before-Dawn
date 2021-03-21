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
	[HideInInspector]
	public int ViewDst;

	Vector2 viewerPosition;
	Vector2 viewerPositionOld;

	void Awake()
    {
		instance = this;
		heightMapSettings.noiseSettings.seed = SavingManager.SaveFile.seed;
		ViewDst = SavingManager.SaveData.SettingsData.RenderDistance;
	}

	void Start()
	{
		meshWorldSize = meshSettings.meshWorldSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(ViewDst / meshWorldSize);

		UpdateVisibleChunks();
	}

	void FixedUpdate()
	{
		viewerPosition = new Vector2(SavingManager.player.transform.position.x, SavingManager.player.transform.position.z);

		if (viewerPosition != viewerPositionOld)
		{
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks();
		}
	}

	public void UpdateViewDist()
    {
		meshWorldSize = meshSettings.meshWorldSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(ViewDst / meshWorldSize);

		for (int i = 0; i < terrainChunkDictionary.Count; i++) { terrainChunkDictionary.ElementAt(i).Value.UpdateViewDst(ViewDst); }
		UpdateVisibleChunks();
	}

	public static Vector2 GetNearestChunk(Vector3 pos)
    {
		return new Vector2(Mathf.RoundToInt(pos.x / instance.meshWorldSize), Mathf.RoundToInt(pos.x / instance.meshWorldSize));
    }

	public static bool AddToNearestChunk(GameObject obj, ChildType childType)
	{
		Vector2 chunkLoc = new Vector2(Mathf.RoundToInt(obj.transform.position.x / instance.meshWorldSize), Mathf.RoundToInt(obj.transform.position.x / instance.meshWorldSize));

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
		return false;
	}

	public static bool CanAddToNearestChunk(Vector3 t)
	{
		Vector2 chunkLoc = new Vector2(Mathf.RoundToInt(t.x / instance.meshWorldSize), Mathf.RoundToInt(t.x / instance.meshWorldSize));
		if (instance.terrainChunkDictionary.ContainsKey(chunkLoc)) { return true; } else { return false; }
	}

	public void ResetChunks()
	{
		for (int i = 0; i < terrainChunkDictionary.Count; i++) { terrainChunkDictionary.ElementAt(i).Value.RemoveChunk(); }

		terrainChunkDictionary.Clear();
		visibleTerrainChunks.Clear();

		UpdateVisibleChunks();
	}

	public void UpdateVisibleChunks()
	{
		UpdateBiomes();

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
					if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
					{
						terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
					}
					else
					{
						TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, ViewDst, transform, mapMaterial, generateType, GetChunkBiome(viewedChunkCoord));
						terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
						newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
						newChunk.Load();
					}
				}

			}
		}
	}

	public void UpdateBiomes()
	{
		int currentBiomeCoordX = Mathf.RoundToInt(viewerPosition.x / (meshWorldSize * 3));
		int currentBiomeCoordY = Mathf.RoundToInt(viewerPosition.y / (meshWorldSize * 3));

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
			{
				Vector2 viewedBiomeCoord = new Vector2(currentBiomeCoordX + xOffset, currentBiomeCoordY + yOffset);

				if (!biomesDictionary.Contains(viewedBiomeCoord))
                {
					int biomeId = PropsGeneration.instance.GetRandomBiome();
					float x = viewedBiomeCoord.x * 3;
					float y = viewedBiomeCoord.y * 3;

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

	int GetChunkBiome(Vector2 coord)
    {
		if (biomesIndex.ContainsKey(coord)) { return biomesIndex[coord]; }
		return -1;
    }
	void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible) { if (isVisible) { visibleTerrainChunks.Add(chunk); } else { visibleTerrainChunks.Remove(chunk); } }
}