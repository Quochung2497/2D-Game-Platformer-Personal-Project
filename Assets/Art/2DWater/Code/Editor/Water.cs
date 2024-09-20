using UnityEngine;

namespace Bundos.WaterSystem
{
    public class Spring
    {
        public Vector2 weightPosition, sineOffset, velocity, acceleration;
    }

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Water : MonoBehaviour
    {
        [Header("Dynamic Wave Settings")]
        public bool interactive = true;
        public float splashInfluence = 0.005f;
        public float waveHeight = .25f;

        [Header("Constant Waves Settings")]
        public bool hasConstantWaves = true;
        public float waveAmplitude = 1f;
        public float waveSpeed = 1f;
        public int waveStep = 1;

        [Header("Spring Settings")]
        public int numSprings = 10;
        public float spacing = 1f;
        public float springConstant = 0.05f;
        public float springDamping = 0.025f;

        [Header("Particles")]
        public GameObject splashParticle;

        [HideInInspector]
        Spring[] springs;
        MeshFilter meshFilter;
        Mesh mesh;

        [HideInInspector]
        public Vector2[] vertices, baseVertecies;
        [HideInInspector]
        public int[] triangles;
        [HideInInspector]
        Vector2[] uvs;


        private void Start()
        {
            Initialize();
            InitializeSprings();
            CreateShape();
        }

        public void Initialize()
        {
            mesh = new Mesh()
            {
                name = "WaterMesh"
            };

            meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        private void InitializeSprings()
        {
            springs = new Spring[numSprings];

            for (int i = 0; i < numSprings; i++)
            {
                springs[i] = new Spring
                {
                    weightPosition = new Vector2()
                };
            }
        }

        public void CreateShape()
        {
            // Vertices
            vertices = new Vector2[numSprings * 2];

            for (int i = 0, x = 0; x < numSprings; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    vertices[i] = new Vector3(x, y);
                    i++;
                }
            }

            baseVertecies = new Vector2[numSprings * 2];
            vertices.CopyTo(baseVertecies, 0);

            // Triangles
            triangles = new int[((numSprings * 2) - 2) * 3];

            int vert = 0;
            int tris = 0;
            for (int x = 0; x < numSprings - 1; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + 1;
                triangles[tris + 2] = vert + 3;

                triangles[tris + 3] = vert + 0;
                triangles[tris + 4] = vert + 3;
                triangles[tris + 5] = vert + 2;

                vert += 2;
                tris += 6;
            }

            // UV's
            uvs = new Vector2[vertices.Length];

            for (int i = 0, x = 0; x < numSprings; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    uvs[i] = new Vector3((float)x / numSprings, (float)y / 2);
                    i++;
                }
            }
        }

        private Vector3[] ConvertVector2ArrayToVector3(Vector2[] vector2Array)
        {
            Vector3[] vector3Array = new Vector3[vector2Array.Length];
            for (int i = 0; i < vector2Array.Length; i++)
            {
                vector3Array[i] = new Vector3(vector2Array[i].x, vector2Array[i].y, 0);
            }
            return vector3Array;
        }

        private void Update()
        {
            UpdateSpringPositions();
            UpdateMeshVerticePositions();
            UpdateMesh();
        }

        private void UpdateMeshVerticePositions()
        {
            for (int i = 0; i < numSprings; i++)
            {
                vertices[(2 * i) + 1] = baseVertecies[(2 * i) + 1] + springs[i].weightPosition + springs[i].sineOffset;
            }
        }

        private void UpdateSpringPositions()
        {
            // Random spring movement
            for (int i = 0; i < springs.Length; i++)
            {
                springs[i].acceleration = (-springConstant * springs[i].weightPosition.y) * Vector2.up - (springs[i].velocity * springDamping);

                if (i > 0)
                {
                    float leftDelta = splashInfluence * (springs[i].acceleration.y - springs[i - 1].acceleration.y);
                    springs[i].velocity += leftDelta * Vector2.up;
                }

                if (i < springs.Length - 1)
                {
                    float rightDelta = splashInfluence * (springs[i].acceleration.y - springs[i + 1].acceleration.y);
                    springs[i].velocity += rightDelta * Vector2.up;
                }

                springs[i].velocity += springs[i].acceleration;

                if (hasConstantWaves)
                    springs[i].sineOffset = new Vector2(0, waveAmplitude * Mathf.Sin((Time.realtimeSinceStartup * waveSpeed) + i * waveStep));

                springs[i].weightPosition += springs[i].velocity;
            }
        }

        public void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = ConvertVector2ArrayToVector3(vertices);
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
        }

        private void Ripple(Vector3 contactPoint, bool sink)
        {
            Instantiate(splashParticle, contactPoint, Quaternion.identity);

            Vector3 localContactPoint = transform.InverseTransformPoint(contactPoint);

            float currSmallestDistance = 10000f;
            int index = 0;
            for (int i = 0; i < numSprings; i++)
            {
                float distance = Mathf.Abs(Vector2.Distance(vertices[(2 * i) + 1], localContactPoint));
                if (distance < currSmallestDistance)
                {
                    currSmallestDistance = distance;
                    index = i;
                }
            }

            springs[index].weightPosition = (sink ? Vector2.down : Vector2.up) * waveHeight;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!interactive)
                return;

            Rigidbody2D otherRigidbody = other.GetComponent<Rigidbody2D>();
            if (otherRigidbody != null)
            {
                Vector2 contactPoint = other.ClosestPoint(transform.position);

                Ripple(contactPoint, false);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (!interactive)
                return;

            Rigidbody2D otherRigidbody = other.GetComponent<Rigidbody2D>();
            if (otherRigidbody != null)
            {
                Vector2 contactPoint = other.ClosestPoint(transform.position);
                Ripple(contactPoint, true);
            }
        }
    }
}