#ifndef TEST
#define TEST

struct Pos {
    float x;
    float y;
    float z;
};

RWStructuredBuffer<Pos> Positions;
RWStructuredBuffer<Pos> ChangedPositions;

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
    float3 _InstancePosition;
#endif
#endif