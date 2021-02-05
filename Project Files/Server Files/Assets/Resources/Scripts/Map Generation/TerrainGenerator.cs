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

    void FixedUpdate()
    {
		for (int i = 0; i < NetworkManager.instance.players.Count; i++)
		{
			Player player = NetworkManager.instance.players.ElementAt(i).Value;

			Vector2 viewerPosition = new Vector2(player.transform.position.x, player.transform.position.z);
			Vector2 viewerPositionOld = new Vector2(player.oldPos.x, player.oldPos.y);

			if (viewerPosition != viewerPositionOld)
			{
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

	public void UpdateChunk(Vector3 propPos, PropDataStruct newProp)
    {
		Vector2 coord = new Vector3(Mathf.RoundToInt(propPos.x / meshWorldSize), Mathf.RoundToInt(propPos.z / meshWorldSize));
		if (!chunkDictionary.ContainsKey(coord)) { print("shiiitt"); return; }

		Vector3 localPropPos = chunkDictionary[coord].meshObject.transform.InverseTransformPoint(propPos);

		if (newProp != null)
        {
			if (!chunkDictionary[coord].chunkData.props.ContainsKey(localPropPos))
			{
				chunkDictionary[coord].chunkData.props.Add(localPropPos, newProp);

				GameObject g = Instantiate(propsSettings.PropGroups[newProp.group].Props[newProp.prop].prop);
				g.transform.name = propsSettings.PropGroups[newProp.group].Props[newProp.prop].prop.name;
				g.transform.parent = chunkDictionary[coord].props.transform;
				g.transform.localPosition = localPropPos;
				g.transform.eulerAngles = newProp.rot;
			}
		}
        else
        {
			print("not shit");
			if (chunkDictionary[coord].chunkData.props.ContainsKey(localPropPos)) 
			{
				if (chunkDictionary[coord].propsDict.ContainsKey(propPos))
                {
					Destroy(chunkDictionary[coord].propsDict[propPos]);
					chunkDictionary[coord].propsDict.Remove(propPos);
				}
				chunkDictionary[coord].chunkData.props.Remove(localPropPos); 
			}
        }

		for (int i = 0; i < NetworkManager.instance.players.Count; i++)
        {
			Player player = NetworkManager.instance.players.ElementAt(i).Value;
			if (player.chunks.Contains(coord)) { ServerSend.PropChunkUpdate(chunkDictionary[coord].chunkData, player); }
        }
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