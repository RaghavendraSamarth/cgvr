using System;
using System.Collections.Generic;
using UnityEngine;

namespace LumiLab.Lamps
{
    /// <summary>
    /// Utility for procedurally generating 3D meshes for LumiLab lamps and showroom objects.
    /// Implements profile revolution (2D CAD to 3D), Boundary Representation (B-Rep) hollow solids,
    /// toroid geometry, and tessellated parametric surfaces.
    /// </summary>
    public static class ProceduralMeshUtility
    {
        /// <summary>
        /// Case Study 2 - Part A: 2D Profile Revolution (Lathe)
        /// Revolves a 2D curve in the XY plane around the Y-axis.
        /// </summary>
        public static Mesh CreateRevolvedMesh(Vector2[] profile, int radialSegments, string meshName = "RevolvedMesh", bool closeBottom = false, bool closeTop = false)
        {
            if (profile == null || profile.Length < 2)
            {
                Debug.LogError("CreateRevolvedMesh requires at least 2 profile points.");
                return new Mesh();
            }

            radialSegments = Mathf.Clamp(radialSegments, 3, 128);
            Mesh mesh = new Mesh { name = meshName };

            int profileCount = profile.Length;
            int vertCount = profileCount * (radialSegments + 1);

            Vector3[] vertices = new Vector3[vertCount];
            Vector3[] normals = new Vector3[vertCount];
            Vector2[] uvs = new Vector2[vertCount];
            List<int> triangles = new List<int>();

            // Calculate total profile arc length for UV mapping
            float totalLength = 0f;
            float[] cumulativeLength = new float[profileCount];
            cumulativeLength[0] = 0f;
            for (int i = 1; i < profileCount; i++)
            {
                totalLength += Vector2.Distance(profile[i], profile[i - 1]);
                cumulativeLength[i] = totalLength;
            }
            if (totalLength < 0.0001f) totalLength = 1f;

            // Generate vertices around the Y-axis
            for (int r = 0; r <= radialSegments; r++)
            {
                float u = (float)r / radialSegments;
                float angle = u * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                for (int p = 0; p < profileCount; p++)
                {
                    int index = r * profileCount + p;
                    float radius = profile[p].x;
                    float y = profile[p].y;

                    vertices[index] = new Vector3(radius * cos, y, radius * sin);
                    float v = cumulativeLength[p] / totalLength;
                    uvs[index] = new Vector2(u, v);
                }
            }

            // Generate quads between profile lines
            for (int r = 0; r < radialSegments; r++)
            {
                for (int p = 0; p < profileCount - 1; p++)
                {
                    int curr = r * profileCount + p;
                    int next = (r + 1) * profileCount + p;

                    // Triangle 1
                    triangles.Add(curr);
                    triangles.Add(next);
                    triangles.Add(curr + 1);

                    // Triangle 2
                    triangles.Add(next);
                    triangles.Add(next + 1);
                    triangles.Add(curr + 1);
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }

        /// <summary>
        /// Case Study 2 - Part B: Boundary Representation (B-Rep) Hollow Solid.
        /// Generates an explicit hollow cylinder with outward outer wall,
        /// inward inner wall, and top/bottom connecting rim annuli.
        /// </summary>
        public static Mesh CreateHollowCylinder(float outerRadius, float innerRadius, float height, int radialSegments, string meshName = "BRep_HollowCylinder")
        {
            radialSegments = Mathf.Clamp(radialSegments, 6, 96);
            if (innerRadius >= outerRadius) innerRadius = outerRadius * 0.8f;

            Mesh mesh = new Mesh { name = meshName };
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            float halfH = height * 0.5f;

            // 1. Outer Cylinder (Normals point OUTWARD)
            int outerStart = verts.Count;
            for (int i = 0; i <= radialSegments; i++)
            {
                float t = (float)i / radialSegments;
                float angle = t * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Vector3 normal = new Vector3(cos, 0, sin);

                // Bottom vertex
                verts.Add(new Vector3(outerRadius * cos, -halfH, outerRadius * sin));
                norms.Add(normal);
                uvs.Add(new Vector2(t, 0f));

                // Top vertex
                verts.Add(new Vector3(outerRadius * cos, halfH, outerRadius * sin));
                norms.Add(normal);
                uvs.Add(new Vector2(t, 1f));
            }

            for (int i = 0; i < radialSegments; i++)
            {
                int b0 = outerStart + i * 2;
                int t0 = b0 + 1;
                int b1 = outerStart + (i + 1) * 2;
                int t1 = b1 + 1;

                tris.Add(b0); tris.Add(t0); tris.Add(b1);
                tris.Add(b1); tris.Add(t0); tris.Add(t1);
            }

            // 2. Inner Cylinder (Normals point INWARD)
            int innerStart = verts.Count;
            for (int i = 0; i <= radialSegments; i++)
            {
                float t = (float)i / radialSegments;
                float angle = t * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Vector3 normal = new Vector3(-cos, 0, -sin); // Inward facing normal

                verts.Add(new Vector3(innerRadius * cos, -halfH, innerRadius * sin));
                norms.Add(normal);
                uvs.Add(new Vector2(t, 0f));

                verts.Add(new Vector3(innerRadius * cos, halfH, innerRadius * sin));
                norms.Add(normal);
                uvs.Add(new Vector2(t, 1f));
            }

            for (int i = 0; i < radialSegments; i++)
            {
                int b0 = innerStart + i * 2;
                int t0 = b0 + 1;
                int b1 = innerStart + (i + 1) * 2;
                int t1 = b1 + 1;

                // Winding order inverted so front faces inwards
                tris.Add(b0); tris.Add(b1); tris.Add(t0);
                tris.Add(b1); tris.Add(t1); tris.Add(t0);
            }

            // 3. Top Rim Annulus (Normal points +Y)
            int topRimStart = verts.Count;
            for (int i = 0; i <= radialSegments; i++)
            {
                float t = (float)i / radialSegments;
                float angle = t * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                // Inner rim vert
                verts.Add(new Vector3(innerRadius * cos, halfH, innerRadius * sin));
                norms.Add(Vector3.up);
                uvs.Add(new Vector2(cos * 0.5f + 0.5f, sin * 0.5f + 0.5f));

                // Outer rim vert
                verts.Add(new Vector3(outerRadius * cos, halfH, outerRadius * sin));
                norms.Add(Vector3.up);
                uvs.Add(new Vector2(cos * 0.5f + 0.5f, sin * 0.5f + 0.5f));
            }

            for (int i = 0; i < radialSegments; i++)
            {
                int in0 = topRimStart + i * 2;
                int out0 = in0 + 1;
                int in1 = topRimStart + (i + 1) * 2;
                int out1 = in1 + 1;

                tris.Add(in0); tris.Add(out0); tris.Add(in1);
                tris.Add(out0); tris.Add(out1); tris.Add(in1);
            }

            // 4. Bottom Rim Annulus (Normal points -Y)
            int botRimStart = verts.Count;
            for (int i = 0; i <= radialSegments; i++)
            {
                float t = (float)i / radialSegments;
                float angle = t * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                verts.Add(new Vector3(innerRadius * cos, -halfH, innerRadius * sin));
                norms.Add(Vector3.down);
                uvs.Add(new Vector2(cos * 0.5f + 0.5f, sin * 0.5f + 0.5f));

                verts.Add(new Vector3(outerRadius * cos, -halfH, outerRadius * sin));
                norms.Add(Vector3.down);
                uvs.Add(new Vector2(cos * 0.5f + 0.5f, sin * 0.5f + 0.5f));
            }

            for (int i = 0; i < radialSegments; i++)
            {
                int in0 = botRimStart + i * 2;
                int out0 = in0 + 1;
                int in1 = botRimStart + (i + 1) * 2;
                int out1 = in1 + 1;

                tris.Add(in0); tris.Add(in1); tris.Add(out0);
                tris.Add(out0); tris.Add(in1); tris.Add(out1);
            }

            mesh.vertices = verts.ToArray();
            mesh.normals = norms.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }

        /// <summary>
        /// Creates a Torus (ring) mesh for Halo Contemporary lamp or decorative trim.
        /// </summary>
        public static Mesh CreateTorus(float mainRadius, float tubeRadius, int radialSegments = 32, int tubularSegments = 16, string meshName = "Torus")
        {
            Mesh mesh = new Mesh { name = meshName };
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            for (int i = 0; i <= radialSegments; i++)
            {
                float u = (float)i / radialSegments;
                float phi = u * Mathf.PI * 2f;
                Vector3 center = new Vector3(Mathf.Cos(phi) * mainRadius, 0, Mathf.Sin(phi) * mainRadius);

                for (int j = 0; j <= tubularSegments; j++)
                {
                    float v = (float)j / tubularSegments;
                    float theta = v * Mathf.PI * 2f;

                    Vector3 normal = new Vector3(
                        Mathf.Cos(phi) * Mathf.Cos(theta),
                        Mathf.Sin(theta),
                        Mathf.Sin(phi) * Mathf.Cos(theta)
                    );

                    Vector3 pos = center + normal * tubeRadius;

                    verts.Add(pos);
                    norms.Add(normal);
                    uvs.Add(new Vector2(u, v));
                }
            }

            int stride = tubularSegments + 1;
            for (int i = 0; i < radialSegments; i++)
            {
                for (int j = 0; j < tubularSegments; j++)
                {
                    int a = i * stride + j;
                    int b = (i + 1) * stride + j;
                    int c = i * stride + (j + 1);
                    int d = (i + 1) * stride + (j + 1);

                    tris.Add(a); tris.Add(b); tris.Add(c);
                    tris.Add(b); tris.Add(d); tris.Add(c);
                }
            }

            mesh.vertices = verts.ToArray();
            mesh.normals = norms.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }

        /// <summary>
        /// Creates a tapered frustum lampshade with open bottom and open top.
        /// </summary>
        public static Mesh CreateFrustumShade(float bottomRadius, float topRadius, float height, int radialSegments = 32, string meshName = "FrustumShade")
        {
            Mesh mesh = new Mesh { name = meshName };
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            float halfH = height * 0.5f;
            float slantAngle = Mathf.Atan2(bottomRadius - topRadius, height);
            float normY = Mathf.Sin(slantAngle);
            float normR = Mathf.Cos(slantAngle);

            for (int i = 0; i <= radialSegments; i++)
            {
                float t = (float)i / radialSegments;
                float angle = t * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector3 norm = new Vector3(cos * normR, normY, sin * normR).normalized;

                // Bottom vertex
                verts.Add(new Vector3(bottomRadius * cos, -halfH, bottomRadius * sin));
                norms.Add(norm);
                uvs.Add(new Vector2(t, 0f));

                // Top vertex
                verts.Add(new Vector3(topRadius * cos, halfH, topRadius * sin));
                norms.Add(norm);
                uvs.Add(new Vector2(t, 1f));
            }

            for (int i = 0; i < radialSegments; i++)
            {
                int b0 = i * 2;
                int t0 = b0 + 1;
                int b1 = (i + 1) * 2;
                int t1 = b1 + 1;

                // Double-sided shading for open shade
                tris.Add(b0); tris.Add(t0); tris.Add(b1);
                tris.Add(b1); tris.Add(t0); tris.Add(t1);

                // Backfaces
                tris.Add(b0); tris.Add(b1); tris.Add(t0);
                tris.Add(b1); tris.Add(t1); tris.Add(t0);
            }

            mesh.vertices = verts.ToArray();
            mesh.normals = norms.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }

        /// <summary>
        /// Creates a wireframe mesh with lines for depth buffer & hidden surface removal inspection.
        /// </summary>
        public static Mesh CreateWireframeMesh(Mesh srcMesh, string name = "Wireframe")
        {
            if (srcMesh == null) return new Mesh();

            Mesh wire = new Mesh { name = name };
            Vector3[] origVerts = srcMesh.vertices;
            int[] origTris = srcMesh.triangles;

            HashSet<ulong> uniqueEdges = new HashSet<ulong>();
            List<int> lineIndices = new List<int>();

            for (int i = 0; i < origTris.Length; i += 3)
            {
                int i0 = origTris[i];
                int i1 = origTris[i + 1];
                int i2 = origTris[i + 2];

                AddEdge(i0, i1, uniqueEdges, lineIndices);
                AddEdge(i1, i2, uniqueEdges, lineIndices);
                AddEdge(i2, i0, uniqueEdges, lineIndices);
            }

            wire.vertices = origVerts;
            wire.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
            return wire;
        }

        private static void AddEdge(int a, int b, HashSet<ulong> edges, List<int> lines)
        {
            int min = Mathf.Min(a, b);
            int max = Mathf.Max(a, b);
            ulong key = ((ulong)(uint)min << 32) | (uint)max;

            if (edges.Add(key))
            {
                lines.Add(min);
                lines.Add(max);
            }
        }
    }
}
