Shader "Custom/UIBlurShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "red" {}  // 텍스처 속성
        _Color ("Text Color", Color) = (1, 1, 1, 1) // 텍스트 색상 추가 (기본 흰색)
        _BlurSize ("Blur Size", Range(0.0, 10.0)) = 1.0  // 블러 크기
    }
    SubShader
    {
        Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurSize;
            float4 _Color;  // 텍스트 색상 변수 추가

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 텍스처에서 색상 정보 가져오기
                float4 texColor = tex2D(_MainTex, uv);
                
                // 텍스트 색상과 텍스처 색상 곱하기
                float4 finalColor = texColor * _Color;
                
                // Gaussian Blur approximation
                float2 offset1 = float2(_BlurSize / _ScreenParams.x, _BlurSize / _ScreenParams.y);
                finalColor += tex2D(_MainTex, uv + offset1) * _Color;
                finalColor += tex2D(_MainTex, uv - offset1) * _Color;
                finalColor += tex2D(_MainTex, uv + 2.0 * offset1) * _Color;
                finalColor += tex2D(_MainTex, uv - 2.0 * offset1) * _Color;

                return finalColor / 5.0;  // 평균 블러 색상
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
