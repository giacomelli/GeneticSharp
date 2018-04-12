using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.WallBuilder
{
    public class CarController : MonoBehaviour
    {
      	private void Awake()
		{
      
            //for (int i = 0; i < verticesCount; i += 3)
            //{
            //    vectors.Insert(i, Vector2.zero);
            //    vectors.Insert(i + 2, vectors[i + 3]);
            //}


            var mesh = CreateMesh();

            var mf = GetComponent<MeshFilter>();
            mf.mesh = mesh;


		}

        private Mesh CreateMesh()
        {
            //var orderedVertices = new List<Vector3>();
            //var lastVertice = Vector2.zero;

            //for (int i = 0; i < 8; i++)
            //{
            //    orderedVertices.Add(new Vector2(Random.Range(-10, 10), Random.Range(-10, 10)));
            //    //vertices[i] = new Vector2(i * 10, i * 10);
            //}

            ////orderedVertices = orderedVertices.OrderBy(v => v.x).ThenBy(v => v.y).ToList();
            //orderedVertices.Insert(0, Vector2.zero);
            //var vertices = orderedVertices.ToArray();

            var vertices = new Vector3[]
            {
                new Vector2(Random.Range(-10, -5), Random.Range(0, 10)),
                new Vector2(Random.Range(-5, 0), Random.Range(0, 10))
            };


            var uvs = new Vector2[vertices.Length];

            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }

            var tris = new int[3 * (vertices.Length - 2)];
            var vertexIndex = 0;
          
            for(int i = 0; i < tris.Length; i += 3)
            {
                tris[i] = 0;
                tris[i + 1] = vertexIndex + 1;
                tris[i + 2] = vertexIndex + 2;
                vertexIndex++;
            }

            //var tris = new int[]
            //{
            //    0, 1, 2,
            //    0, 2, 3,
            //    0, 3, 4
            //};


            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = tris;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
           
            mesh.name = "MyMesh";

            return mesh;
        }
	}
}