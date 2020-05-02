// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/LineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PointSize("LineSize", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
        CGPROGRAM
#pragma vertex vert
#pragma fragment frag

float _PointSize;

struct VertexInput {
    float4 v : POSITION;
    float4 color: COLOR;
};

struct VertexOutput {
    float4 pos : SV_POSITION;
    float4 col : COLOR;
    float size : PSIZE;
};

VertexOutput vert(VertexInput v) {
    VertexOutput o;
    o.pos = UnityObjectToClipPos(v.v);
    o.col = v.color;
    o.size = _PointSize;
    return o;
}

float4 frag(VertexOutput o) : COLOR {
    return o.col;
    
}
            ENDCG
        }
    }
}