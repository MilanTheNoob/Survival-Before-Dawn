using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Linq;

public static class MeshGenerator
{
	public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail)
	{
		//The increment skip
		int skipIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		//The number of vertices each line
		int numVertsPerLine = meshSettings.numVertsPerLine;

		//The top left point of the terrain
		Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2f;

		//The mesh data of the terrain
		MeshData meshData = new MeshData(numVertsPerLine, skipIncrement, meshSettings.useFlatShading);

		//Map of vertice indices
		int[,] vertexIndicesMap = new int[numVertsPerLine, numVertsPerLine];

		int meshVertexIndex = 0;
		int outOfMeshVertexIndex = -1;

		for (int y = 0; y < numVertsPerLine; y++)
		{
			for (int x = 0; x < numVertsPerLine; x++)
			{
				//Some data about the vertice
				bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
				bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

				if (isOutOfMeshVertex)
				{
					vertexIndicesMap[x, y] = outOfMeshVertexIndex;
					outOfMeshVertexIndex--;
				}
				else if (!isSkippedVertex)
				{
					vertexIndicesMap[x, y] = meshVertexIndex;
					meshVertexIndex++;
				}
			}
		}

		for (int y = 0; y < numVertsPerLine; y++)
		{
			for (int x = 0; x < numVertsPerLine; x++)
			{
				//Lil bit of data about the vertice
				bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

				if (!isSkippedVertex)
				{
					//Maybe not a lil bit of data about the vertice	
					bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
					bool isMeshEdgeVertex = (y == 1 || y == numVertsPerLine - 2 || x == 1 || x == numVertsPerLine - 2) && !isOutOfMeshVertex;
					bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
					bool isEdgeConnectionVertex = (y == 2 || y == numVertsPerLine - 3 || x == 2 || x == numVertsPerLine - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

					int vertexIndex = vertexIndicesMap[x, y];
					Vector2 percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);
					Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshSettings.meshWorldSize;
					float height = heightMap[x, y];

					if (isEdgeConnectionVertex)
					{
						//Do some magic if it is an edge vertice to make it (somewhat) seamless next to the other terrain
						bool isVertical = x == 2 || x == numVertsPerLine - 3;
						int dstToMainVertexA = ((isVertical) ? y - 2 : x - 2) % skipIncrement;
						int dstToMainVertexB = skipIncrement - dstToMainVertexA;
						float dstPercentFromAToB = dstToMainVertexA / (float)skipIncrement;

						float heightMainVertexA = heightMap[(isVertical) ? x : x - dstToMainVertexA, (isVertical) ? y - dstToMainVertexA : y];
						float heightMainVertexB = heightMap[(isVertical) ? x : x + dstToMainVertexB, (isVertical) ? y + dstToMainVertexB : y];

						height = heightMainVertexA * (1 - dstPercentFromAToB) + heightMainVertexB * dstPercentFromAToB;
					}

					//Add a vertice to the mesh data
					meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);

					//Decide if a new triangle should be created
					bool createTriangle = x < numVertsPerLine - 1 && y < numVertsPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));

					if (createTriangle)
					{
						//Do some magic to create the triangle
						int currentIncrement = (isMainVertex && x != numVertsPerLine - 3 && y != numVertsPerLine - 3) ? skipIncrement : 1;

						int a = vertexIndicesMap[x, y];
						int b = vertexIndicesMap[x + currentIncrement, y];
						int c = vertexIndicesMap[x, y + currentIncrement];
						int d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];
						meshData.AddTriangle(a, d, c);
						meshData.AddTriangle(d, a, b);
					}
				}
			}
		}

		//Process the meshdata
		meshData.ProcessMesh();

		//Return the meshdata
		return meshData;

	}
}

public class MeshData
{
	[HideInInspector]
	public Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;
	Vector3[] bakedNormals;

	Vector3[] outOfMeshVertices;
	int[] outOfMeshTriangles;

	int triangleIndex;
	int outOfMeshTriangleIndex;

	bool useFlatShading;

	public MeshData(int numVertsPerLine, int skipIncrement, bool useFlatShading)
	{
		//Get if peep wants to use flat shading
		this.useFlatShading = useFlatShading;

		//Numbers of different types of vertices
		int numMeshEdgeVertices = (numVertsPerLine - 2) * 4 - 4;
		int numEdgeConnectionVertices = (skipIncrement - 1) * (numVertsPerLine - 5) / skipIncrement * 4;
		int numMainVerticesPerLine = (numVertsPerLine - 5) / skipIncrement + 1;
		int numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

		//Number of different types of triangles
		int numMeshEdgeTriangles = 8 * (numVertsPerLine - 4);
		int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;

		//Get the vertices, uvs & triangles of the meshData
		vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];
		uvs = new Vector2[vertices.Length];
		triangles = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];

		//Get the border vertices & triangles
		outOfMeshVertices = new Vector3[numVertsPerLine * 4 - 4];
		outOfMeshTriangles = new int[24 * (numVertsPerLine - 2)];
	}

	public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
	{
		if (vertexIndex < 0)
		{
			//Add a border vertice
			outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
		}
		else
		{
			//Add a standard vertice
			vertices[vertexIndex] = vertexPosition;
			uvs[vertexIndex] = uv;
		}
	}

	public void AddTriangle(int a, int b, int c)
	{
		if (a < 0 || b < 0 || c < 0)
		{
			//Get the triangle points and add it to the triangle index
			outOfMeshTriangles[outOfMeshTriangleIndex] = a;
			outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
			outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
			outOfMeshTriangleIndex += 3;
		}
		else
		{
			//Do the same here
			triangles[triangleIndex] = a;
			triangles[triangleIndex + 1] = b;
			triangles[triangleIndex + 2] = c;
			triangleIndex += 3;
		}
	}

	Vector3[] CalculateNormals()
	{

		Vector3[] vertexNormals = new Vector3[vertices.Length];
		int triangleCount = triangles.Length / 3;
		for (int i = 0; i < triangleCount; i++)
		{
			//Get the points of a triangle and do some magic here
			int normalTriangleIndex = i * 3;
			int vertexIndexA = triangles[normalTriangleIndex];
			int vertexIndexB = triangles[normalTriangleIndex + 1];
			int vertexIndexC = triangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			vertexNormals[vertexIndexA] += triangleNormal;
			vertexNormals[vertexIndexB] += triangleNormal;
			vertexNormals[vertexIndexC] += triangleNormal;
		}

		int borderTriangleCount = outOfMeshTriangles.Length / 3;
		for (int i = 0; i < borderTriangleCount; i++)
		{
			//Same again here but with border vertices
			int normalTriangleIndex = i * 3;
			int vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
			int vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
			int vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			if (vertexIndexA >= 0)
			{
				vertexNormals[vertexIndexA] += triangleNormal;
			}
			if (vertexIndexB >= 0)
			{
				vertexNormals[vertexIndexB] += triangleNormal;
			}
			if (vertexIndexC >= 0)
			{
				vertexNormals[vertexIndexC] += triangleNormal;
			}
		}


		for (int i = 0; i < vertexNormals.Length; i++)
		{
			//Normalize a vertex
			vertexNormals[i].Normalize();
		}

		//Return the vertexNormals
		return vertexNormals;

	}

	Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
	{
		//Get the points of a triangle
		Vector3 pointA = (indexA < 0) ? outOfMeshVertices[-indexA - 1] : vertices[indexA];
		Vector3 pointB = (indexB < 0) ? outOfMeshVertices[-indexB - 1] : vertices[indexB];
		Vector3 pointC = (indexC < 0) ? outOfMeshVertices[-indexC - 1] : vertices[indexC];

		//Get the sides, cross them and return it
		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;
		return Vector3.Cross(sideAB, sideAC).normalized;
	}

	public void ProcessMesh()
	{
		if (useFlatShading)
		{
			FlatShading();
		}
		else
		{
			BakeNormals();
		}
	}

	void BakeNormals()
	{
		//Call the CalculateNormals
		bakedNormals = CalculateNormals();
	}

	void FlatShading()
	{
		//Create a new empty array for the triangles & uvs
		Vector3[] flatShadedVertices = new Vector3[triangles.Length];
		Vector2[] flatShadedUvs = new Vector2[triangles.Length];

		for (int i = 0; i < triangles.Length; i++)
		{
			//Input vertices & uvs inside the arrays
			flatShadedVertices[i] = vertices[triangles[i]];
			flatShadedUvs[i] = uvs[triangles[i]];
			triangles[i] = i;
		}

		//Put the arrays into the other arrays i guess?
		vertices = flatShadedVertices;
		uvs = flatShadedUvs;
	}

	public Mesh CreateMesh()
	{
		//Create a new mesh, put data in it, and return it
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		if (useFlatShading)
		{
			//Recalculate Normals
			mesh.RecalculateNormals();
		}
		else
		{
			//Set the normals to bakedNormals
			mesh.normals = bakedNormals;
		}
		return mesh;
	}

}