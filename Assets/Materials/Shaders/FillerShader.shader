Shader "Unlit/FillerShader"
{
    Properties
    {
        _Value("Value", Float) = 1.0
        _MainTex ("Texture", 2D) = "white" {}
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
            float _Value;
            float4 _MainTex_ST;

            fixed4 toGrayScale(fixed4 inColor){
                float pixVal = (0.3 * inColor.x) + (0.59 * inColor.y) + (0.11 * inColor.z);
                return fixed4(pixVal,pixVal,pixVal,inColor.a);
            }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                if(i.uv.y > _Value){
                    col = toGrayScale(col);
                }
                return col;
            }

          

            ENDCG
        }
    }
}
