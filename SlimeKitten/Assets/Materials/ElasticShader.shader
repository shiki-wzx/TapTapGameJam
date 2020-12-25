Shader "Custom/ElasticShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Position("xyz:position,w:range",vector) = (0,0,0,1)
        _Normal("xyz:normal,w:intensity",vector) = (0,1,0,0)
        _Released("Released", int) = 1
        _PointTime("point time",float) = 0
        _Duration("duration",float) = 2
        _Frequency("frequency",float) = 5
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100
 
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                // make fog work
                #pragma multi_compile_fog
 
                #include "UnityCG.cginc"
 
                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };
 
                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };
 
                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Position;
                float4 _Normal;
                int _Released;
                float _PointTime;
                float _Duration;
                float _Frequency;
                v2f vert(appdata v)
                {
                    v2f o;
                    float t = _Time.y - _PointTime;
					
                    if (_Released == 0)
                    {
                        float r = 1 - saturate(length(v.vertex.xyz - _Position.xyz) / _Position.w);
                        v.vertex.xyz += r * _Normal.xyz * _Normal.w;
                    }
					// 在指定变化时间内弹性变化
                    else if ((_Released == 1) && t>0 && t<_Duration)
                    {
						// 归一化点击范围内点的数值，并且因为点击点应该变化最大，周边次子到最小，故 1- 
                        float r = 1 - saturate(length(v.vertex.xyz - _Position.xyz) / _Position.w);
						// 时间上的变化，时间越久，变化逐渐恢复，故要 1-
                        float l = 1 - t / _Duration;
 
						// 点击范围内的点在法线上的偏移，并且成 cos 波形变化
                        v.vertex.xyz += r * l * _Normal.xyz * _Normal.w * cos(t * _Frequency);
                    }
 
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o, o.vertex);
                    return o;
                }
 
                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
 
                    return col;
                }
            ENDCG
        }
    }
}
