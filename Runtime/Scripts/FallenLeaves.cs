using UnityEngine;

public class FallenLeaves : MonoBehaviour
{
    struct FallenLeavesData
    {
        public Vector3 position;
        public Vector3 windForce;
        public Vector2 rotationSpeed;
        public float fallingSpeed;
        public float scale;
        public float altitude;
        public float descendingDistance;
    }

    [Header("Resources")]
    public Mesh mesh;
    public Material material;
    public ComputeShader computeShader;

    [Header("Count")]
    [Min(1)]
    public int count = 500;

    [Header("Range")]
    public float altitude = 5;
    public float descendingDistance = 5;
    public Vector2 xRange = new Vector2(-5, 5);
    public Vector2 zRange = new Vector2(-5, 5);
    public Vector2 scaleRange = new Vector2(1, 2);

    [Header("Speed")]
    public Vector2 fallingSpeedRange = new Vector2(1, 2);
    public Vector2 xRotSpeedRange = new Vector2(1, 2);
    public Vector2 yRotSpeedRange = new Vector2(1, 2);

    [Header("Wind")]
    public Vector3 windForce = new Vector3(1, 0, 1);
    public Vector2 windForceOffset = new Vector2(0, 1f);

    [Header("Warm Up")]
    public bool warmUp;

    ComputeBuffer m_LeavesBuffer;
    ComputeBuffer m_MatrixBuffer;
    ComputeBuffer m_argsBuffer;

    uint[] args = new uint[] { 0, 0, 0, 0, 0 };

    int m_KernelIndex;

    int m_CachedCount;
    void OnEnable()
    {
        Init();
        UpdateBuffer();
    }
    void Init()
    {
        m_KernelIndex = computeShader.FindKernel("Fallen");

        m_argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);

        m_CachedCount = 1;
    }
    void Update()
    {
        DispatchKernel();

        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(Vector3.zero, 10000 * Vector3.one), m_argsBuffer, 0);
    }
    void OnValidate()
    {
        UpdateBuffer();
    }
    void OnDestroy()
    {
        if (m_argsBuffer != null)
            m_argsBuffer.Dispose();
        if (m_LeavesBuffer != null)
            m_LeavesBuffer.Dispose();
        if (m_MatrixBuffer != null)
            m_MatrixBuffer.Dispose();
    }
    void UpdateBuffer()
    {
        UpdateArgsBuffer();

        UpdateDataBuffer();
    }
    void UpdateArgsBuffer()
    {
        if (m_argsBuffer != null)
        {
            if (mesh != null)
            {
                args[0] = mesh.GetIndexCount(0);
                args[1] = (uint)count;
                m_argsBuffer.SetData(args);
            }
        }
    }
    void UpdateDataBuffer()
    {
        if (count <= 0 || m_CachedCount <= 0) return;
        m_CachedCount = count;

        if (m_LeavesBuffer != null)
            m_LeavesBuffer.Dispose();
        m_LeavesBuffer = new ComputeBuffer(count, sizeof(float) * 3 * 2 + sizeof(float) * 2 * 1 + sizeof(float) * 1 * 4);

        var fallenLeavesData = new FallenLeavesData[count];
        for (int i = 0; i < count; i++)
        {
            var posX = Random.Range(xRange.x, xRange.y);
            var posY = altitude;
            if (warmUp)
            {
                posY = Random.Range(-descendingDistance, altitude);
            }
            var posZ = Random.Range(zRange.x, zRange.y);
            fallenLeavesData[i].position = new Vector3(posX, posY, posZ);

            var xRotSpeed = Random.Range(xRotSpeedRange.x, xRotSpeedRange.y);
            var yRotSpeed = Random.Range(yRotSpeedRange.x, yRotSpeedRange.y);
            fallenLeavesData[i].rotationSpeed = new Vector2(xRotSpeed, yRotSpeed);

            fallenLeavesData[i].windForce = windForce + windForce.normalized * Random.Range(windForceOffset.x, windForceOffset.y);

            fallenLeavesData[i].fallingSpeed = Random.Range(fallingSpeedRange.x, fallingSpeedRange.y);

            fallenLeavesData[i].scale = Random.Range(scaleRange.x, scaleRange.y);

            fallenLeavesData[i].altitude = altitude;

            fallenLeavesData[i].descendingDistance = descendingDistance;
        }

        m_LeavesBuffer.SetData(fallenLeavesData);
        computeShader.SetBuffer(m_KernelIndex, "fallenLeavesDataBuffer", m_LeavesBuffer);

        if (m_MatrixBuffer != null)
            m_MatrixBuffer.Dispose();
        m_MatrixBuffer = new ComputeBuffer(count, sizeof(float) * 4 * 4);

        computeShader.SetBuffer(m_KernelIndex, "objectToWorldMatrixBuffer", m_MatrixBuffer);

        material.SetBuffer("objectToWorldMatrixBuffer", m_MatrixBuffer);
    }
    void DispatchKernel()
    {
        computeShader.Dispatch(m_KernelIndex, (int)Mathf.Ceil((float)count / 128), 1, 1);
    }
}