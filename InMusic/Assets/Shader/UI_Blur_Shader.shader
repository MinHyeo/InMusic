Shader "Custom/UIBlurShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "red" {}  // �ؽ�ó �Ӽ�
        _Color ("Text Color", Color) = (1, 1, 1, 1) // �ؽ�Ʈ ���� �߰� (�⺻ ���)
        _BlurSize ("Blur Size", Range(0.0, 10.0)) = 1.0  // �� ũ��
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
            float4 _Color;  // �ؽ�Ʈ ���� ���� �߰�

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

                // �ؽ�ó���� ���� ���� ��������
                float4 texColor = tex2D(_MainTex, uv);
                
                // �ؽ�Ʈ ����� �ؽ�ó ���� ���ϱ�
                float4 finalColor = texColor * _Color;
                
                // Gaussian Blur approximation
                float2 offset1 = float2(_BlurSize / _ScreenParams.x, _BlurSize / _ScreenParams.y);
                finalColor += tex2D(_MainTex, uv + offset1) * _Color;
                finalColor += tex2D(_MainTex, uv - offset1) * _Color;
                finalColor += tex2D(_MainTex, uv + 2.0 * offset1) * _Color;
                finalColor += tex2D(_MainTex, uv - 2.0 * offset1) * _Color;

                return finalColor / 5.0;  // ��� �� ����
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
