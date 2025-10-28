using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
[ExecuteInEditMode] 
public class SphereTerrainComplete : MonoBehaviour
{
    [Header("��������")]
    [Range(20, 150)] public int resolution = 60;
    public float radius = 500f; // ����뾶
    [Range(5, 25)] public float height = 15f; // ��������߶�

    [Header("�������������Ƶ�����״��")]
    public float noiseScale = 0.08f;
    [Range(1, 4)] public int octaves = 2;
    [Range(0.1f, 2f)] public float persistence = 0.5f;

    [Header("ֲ������")]
    public GameObject grassPrefab;
    public GameObject treePrefab;
    [Range(1000, 8000)] public int grassAmount = 3000;
    [Range(100, 400)] public int treeAmount = 150;

  //��ʼ����
    public bool initializePosition = true;

    private Mesh sphereMesh;
    private Vector3[] vertices;
    private int[] triangles;
    private bool isGenerated = false;

    //�Զ�ˢ��
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

    [ContextMenu("������������")]
    public void GenerateSphere()
    {
        // ��ʼ������(0,0,0)
        if (initializePosition)
        {
            transform.position = Vector3.zero;
        }

        // ��ʼ������
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

        // ��������
        vertices = new Vector3[(resolution + 1) * resolution];
        // ����������
        triangles = new int[resolution * resolution * 6];

        int triIndex = 0;

        // ���ɶ�����������
        for (int y = 0; y <= resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                // 1. ���㶥������
                float lat = Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, y / (float)resolution);
                float lon = Mathf.Lerp(0, 2 * Mathf.PI, x / (float)(resolution - 1));

                // ��������
                float xPos = Mathf.Cos(lat) * Mathf.Cos(lon);
                float yPos = Mathf.Sin(lat);
                float zPos = Mathf.Cos(lat) * Mathf.Sin(lon);

                //ɽ��
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
                float heightOffset = (noise - 0.5f) * height; // �߶�ƫ��

                // ���ն���λ��
                vertices[y * resolution + x] = new Vector3(xPos, yPos, zPos) * (radius + heightOffset);

                // 2. ����������
                if (y < resolution)
                {
                    // �ĸ���������
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

        // Ӧ����������
        sphereMesh.vertices = vertices;
        sphereMesh.triangles = triangles;
        sphereMesh.RecalculateNormals();
        sphereMesh.RecalculateBounds();
        sphereMesh.Optimize();

        // ��ײ��
        GetComponent<MeshCollider>().sharedMesh = sphereMesh;

        isGenerated = true;
        Debug.Log("��������������ɣ����������ѳ�ʼ����(0,0,0)");
    }

    // ����ֲ��
    [ContextMenu("����ֲ��")]
    public void GenerateVegetation()
    {
        if (!isGenerated)
        {
            Debug.LogWarning("���ȵ���������������塹");
            return;
        }

        // �����ֲ��
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Grass") || child.name.StartsWith("Tree"))
            {
                DestroyImmediate(child.gameObject);
            }
        }

        // ���ɲ�
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

        // ������
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

    // �༭ģʽ���Ƹ�����
    void OnDrawGizmos()
    {
        if (vertices == null) return;

        // ���ƶ���
        Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.6f);
        foreach (var v in vertices)
        {
            Gizmos.DrawSphere(transform.position + v, 0.15f);
        }

        // ������������
        Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius + height);

        // ��������ԭ��
        if (initializePosition)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero - Vector3.right * 2, Vector3.zero + Vector3.right * 2);
            Gizmos.DrawLine(Vector3.zero - Vector3.up * 2, Vector3.zero + Vector3.up * 2);
            Gizmos.DrawLine(Vector3.zero - Vector3.forward * 2, Vector3.zero + Vector3.forward * 2);
        }
    }
}
