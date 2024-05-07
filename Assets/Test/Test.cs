using UnityEngine;

public class Test : MonoBehaviour
{
    public int count;
    public ComputeShader shader;
    public Mesh mesh;
    public Material material;

    int m_KernelIndex;

    ComputeBuffer m_PositionsBuffer;
    ComputeBuffer m_ChangedPositionsBuffer;

    void Start()
    {
        m_KernelIndex = shader.FindKernel("CSMain");
        UpdateBuffer();
    }

    void Update()
    {
        DispatchKernel();
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, new Bounds(Vector3.zero, 10000 * Vector3.one), count);
    }
    void OnValidate()
    {
        UpdateBuffer();
    }
    void UpdateBuffer()
    {
        if (m_PositionsBuffer != null)
            m_PositionsBuffer.Release();
        m_PositionsBuffer = new ComputeBuffer(count, sizeof(float) * 3);

        var positions = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            positions[i] = new Vector3(i, 0, 0);
        }
        m_PositionsBuffer.SetData(positions);
        shader.SetBuffer(m_KernelIndex, "Positions", m_PositionsBuffer);

        if (m_ChangedPositionsBuffer != null)
            m_ChangedPositionsBuffer.Release();
        m_ChangedPositionsBuffer = new ComputeBuffer(count, sizeof(float) * 3);
        //m_ChangedPositionsBuffer.SetData(positions);
        shader.SetBuffer(m_KernelIndex, "ChangedPositions", m_ChangedPositionsBuffer);

        material.SetBuffer("ChangedPositions", m_ChangedPositionsBuffer);
    }
    void OnDestroy()
    {
        if (m_PositionsBuffer != null) m_PositionsBuffer.Release();
        if (m_ChangedPositionsBuffer != null) m_ChangedPositionsBuffer.Release();
    }
    void DispatchKernel()
    {
        shader.Dispatch(m_KernelIndex, Mathf.CeilToInt((float)count / 128), 1, 1);
    }
}