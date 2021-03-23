using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSaving : MonoBehaviour
{
    public static void SaveChunkData(TerrainChunk chunk)
    {
        ChunkData chunkData = new ChunkData();

        for (int i = 0; i < chunk.props.transform.childCount; i++)
        {
            Transform prop = chunk.props.transform.GetChild(i);

            PropData propData = new PropData
            {
                Name = prop.name,
                Scale = prop.localScale,
                Position = prop.position,
                Rotation = prop.rotation
            };
            chunkData.Props.Add(propData);
        }

        for (int i = 0; i < chunk.vehicles.transform.childCount; i++)
        {
            Transform prop = chunk.vehicles.transform.GetChild(i);

            PropData propData = new PropData
            {
                Name = prop.name,
                Scale = prop.localScale,
                Position = prop.position,
                Rotation = prop.rotation
            };
            chunkData.Vehicles.Add(propData);
        }

        if (!SavingManager.SaveFile.Chunks.ContainsKey(chunk.coord))
        {
            SavingManager.SaveFile.Chunks.Add(chunk.coord, chunkData);
        }
        else
        {
            SavingManager.SaveFile.Chunks[chunk.coord].Props = chunkData.Props;
        }
    }
}
