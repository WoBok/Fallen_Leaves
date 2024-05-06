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
    }

    [Header("Resources")]
    public Mesh mesh;
    public Material material;
    public ComputeShader shader;

    [Header("Paramas")]
    public int count = 500;
    public Vector2 xRange = new Vector2(-5, 5);
    public Vector2 zRange = new Vector2(-5, 5);
    public Vector2 scaleRange = new Vector2(1, 2);
    public Vector2 lifeRange = new Vector2(7, 10);
    public Vector2 fallingSpeenRange = new Vector2(1, 2);
    public Vector2 xRotSpeedRange = new Vector2(1, 2);
    public Vector2 yRotSpeedRange = new Vector2(1, 2);
    public Vector3 windForce = new Vector3(1, 0, 1);
    public Vector2 windForceOffset = new Vector2(0, 0.3f);

    ComputeBuffer m_LeavesBuffer;

    float m_LifeSpan;

    int m_KernelIndex;

    void OnEnable()
    {
        Init();
        UpdateBuffer();
    }
    void Init()
    {
        m_LifeSpan = Random.Range(lifeRange.x, lifeRange.y);
        m_KernelIndex = shader.FindKernel("Fallen");
    }
    void Update()
    {
        DispatchKernel();
    }
    void OnValidate()
    {
        UpdateBuffer();
    }
    void OnDisable()
    {
        m_LeavesBuffer.Release();
    }
    void UpdateBuffer()
    {
        if (m_LeavesBuffer != null)
            m_LeavesBuffer.Release();
        m_LeavesBuffer = new ComputeBuffer(count, sizeof(float) * 3 * 2 + sizeof(float) * 2 * 1 + sizeof(float) * 1 * 2);

        var fallenLeavesData = new FallenLeavesData[count];
        for (int i = 0; i < count; i++)
        {
            var posX = Random.Range(xRange.x, xRange.y);
            var posY = transform.position.y;
            var posZ = Random.Range(zRange.x, zRange.y);
            fallenLeavesData[i].position = new Vector3(posX, posY, posZ);

            var xRotSpeed = Random.Range(xRotSpeedRange.x, xRotSpeedRange.y);
            var yRotSpeed = Random.Range(yRotSpeedRange.x, yRotSpeedRange.y);
            fallenLeavesData[i].rotationSpeed = new Vector2(xRotSpeed, yRotSpeed);

            fallenLeavesData[i].windForce = windForce + windForce.normalized * Random.Range(windForceOffset.x, windForceOffset.y);

            fallenLeavesData[i].fallingSpeed = Random.Range(fallingSpeenRange.x, fallingSpeenRange.y);

            fallenLeavesData[i].scale = Random.Range(scaleRange.x, scaleRange.y);
        }
    }
    void DispatchKernel()
    {
        shader.Dispatch(m_KernelIndex, (int)Mathf.Ceil((float)count / 128), 1, 1);
        shader.SetFloat("time", Time.time % m_LifeSpan);
    }
}