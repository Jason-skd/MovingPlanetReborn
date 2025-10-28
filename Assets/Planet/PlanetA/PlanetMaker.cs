using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
[ExecuteInEditMode] 
public class SphereTerrainComplete : MonoBehaviour
{
    [Header("基础参数")]
    [Range(20, 150)] public int resolution = 60;
    public float radius = 500f; // 球体半径
    [Range(5, 25)] public float height = 15f; // 地形起伏高度

    [Header("噪声参数（控制地形形状）")]
    public float noiseScale = 0.08f;
    [Range(1, 4)] public int octaves = 2;
    [Range(0.1f, 2f)] public float persistence = 0.5f;

    [Header("植被设置")]
    public GameObject grassPrefab;
    public GameObject treePrefab;
    [Range(1000, 8000)] public int grassAmount = 3000;
    [Range(100, 400)] public int treeAmount = 150;

  //初始坐标
    public bool initializePosition = true;

    private Mesh sphereMesh;
    private Vector3[] vertices;
    private int[] triangles;
    private bool isGenerated = false;

    //自动刷新
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            GenerateSphere();
        }
    }
    void Awake()
    {
        GenerateSphere();
    }

    [ContextMenu("生成完整球体")]
    public void GenerateSphere()
    {
        // 初始化坐标(0,0,0)
        if (initializePosition)
        {
            transform.position = Vector3.zero;
        }

        // 初始化网格
        if (sphereMesh == null)
        {
            sphereMesh = new Mesh();
            sphereMesh.name = "ClosedSphere";
            GetComponent<MeshFilter>().mesh = sphereMesh;
        }
        else
        {
            sphereMesh.Clear(); 
        }

        // 顶点数组
        vertices = new Vector3[(resolution + 1) * resolution];
        // 三角形数组
        triangles = new int[resolution * resolution * 6];

        int triIndex = 0;

        // 生成顶点与三角形
        for (int y = 0; y <= resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                // 1. 计算顶点坐标
                float lat = Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, y / (float)resolution);
                float lon = Mathf.Lerp(0, 2 * Mathf.PI, x / (float)(resolution - 1));

                // 球面坐标
                float xPos = Mathf.Cos(lat) * Mathf.Cos(lon);
                float yPos = Mathf.Sin(lat);
                float zPos = Mathf.Cos(lat) * Mathf.Sin(lon);

                //山丘
                float noise = 0;
                float freq = 1;
                float amp = 1;
                for (int o = 0; o < octaves; o++)
                {
                    float nX = x * noiseScale * freq;
                    float nY = y * noiseScale * freq;
                    noise += Mathf.PerlinNoise(nX, nY) * amp;
                    amp *= persistence;
                    freq *= 2;
                }
                float heightOffset = (noise - 0.5f) * height; // 高度偏移

                // 最终顶点位置
                vertices[y * resolution + x] = new Vector3(xPos, yPos, zPos) * (radius + heightOffset);

                // 2. 生成三角形
                if (y < resolution)
                {
                    // 四个顶点索引
                    int v0 = y * resolution + x;
                    int v1 = y * resolution + (x + 1) % resolution; 
                    int v2 = (y + 1) * resolution + x;
                    int v3 = (y + 1) * resolution + (x + 1) % resolution;
                    if (y == 0)
                    {
                        triangles[triIndex++] = v0;
                        triangles[triIndex++] = v2;
                        triangles[triIndex++] = v3;
                    }
                    else if (y == resolution - 1)
                    {
                        triangles[triIndex++] = v0;
                        triangles[triIndex++] = v1;
                        triangles[triIndex++] = v2;
                    }
                    else
                    {
                        triangles[triIndex++] = v0;
                        triangles[triIndex++] = v1;
                        triangles[triIndex++] = v2;
                        triangles[triIndex++] = v1;
                        triangles[triIndex++] = v3;
                        triangles[triIndex++] = v2;
                    }
                }
            }
        }

        // 应用网格数据
        sphereMesh.vertices = vertices;
        sphereMesh.triangles = triangles;
        sphereMesh.RecalculateNormals();
        sphereMesh.RecalculateBounds();
        sphereMesh.Optimize();

        // 碰撞体
        GetComponent<MeshCollider>().sharedMesh = sphereMesh;

        isGenerated = true;
        Debug.Log("完整球体生成完成，中心坐标已初始化到(0,0,0)");
    }

    // 生成植被
    [ContextMenu("生成植被")]
    public void GenerateVegetation()
    {
        if (!isGenerated)
        {
            Debug.LogWarning("请先点击「生成完整球体」");
            return;
        }

        // 清除旧植被
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Grass") || child.name.StartsWith("Tree"))
            {
                DestroyImmediate(child.gameObject);
            }
        }

        // 生成草
        if (grassPrefab != null)
        {
            for (int i = 0; i < grassAmount; i++)
            {
                int randVert = Random.Range(0, vertices.Length);
                Vector3 pos = vertices[randVert];
                Vector3 normal = sphereMesh.normals[randVert];

                if (Vector3.Angle(normal, Vector3.up) < 30f)
                {
                    GameObject grass = Instantiate(grassPrefab, pos, Quaternion.identity, transform);
                    grass.name = "Grass_" + i;
                    grass.transform.up = normal;
                    grass.transform.localScale = Vector3.one * Random.Range(0.7f, 1.1f);
                }
            }
        }

        // 生成树
        if (treePrefab != null)
        {
            for (int i = 0; i < treeAmount; i++)
            {
                int randVert = Random.Range(0, vertices.Length);
                Vector3 pos = vertices[randVert];
                Vector3 normal = sphereMesh.normals[randVert];

                if (Vector3.Angle(normal, Vector3.up) < 25f)
                {
                    GameObject tree = Instantiate(treePrefab, pos, Quaternion.identity, transform);
                    tree.name = "Tree_" + i;
                    tree.transform.up = normal;
                    tree.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
                }
            }
        }
    }

    // 编辑模式绘制辅助线
    void OnDrawGizmos()
    {
        if (vertices == null) return;

        // 绘制顶点
        Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.6f);
        foreach (var v in vertices)
        {
            Gizmos.DrawSphere(transform.position + v, 0.15f);
        }

        // 绘制球体轮廓
        Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius + height);

        // 绘制坐标原点
        if (initializePosition)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero - Vector3.right * 2, Vector3.zero + Vector3.right * 2);
            Gizmos.DrawLine(Vector3.zero - Vector3.up * 2, Vector3.zero + Vector3.up * 2);
            Gizmos.DrawLine(Vector3.zero - Vector3.forward * 2, Vector3.zero + Vector3.forward * 2);
        }
    }
}
