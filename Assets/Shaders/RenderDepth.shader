// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RenderDepth"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
             
            uniform sampler2D _CameraDepthTexture;
            uniform half4 _MainTex_TexelSize;
 
            struct input
            {
                float4 pos : POSITION;
                half2 uv : TEXCOORD0;
            };
 
            struct output
            {
                float4 pos : SV_POSITION;
                half2 uv : TEXCOORD0;
            };
 
 
            output vert(input i)
            {
                output o;
                o.pos = UnityObjectToClipPos(i.pos);
                o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, i.uv);
                // why do we need this? cause sometimes the image I get is flipped. see: http://docs.unity3d.com/Manual/SL-PlatformDifferences.html
                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
                        o.uv.y = 1 - o.uv.y;
                #endif
 
                return o;
            }
             
            fixed4 frag(output o) : COLOR
            {
                float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, o.uv));
				depth = Linear01Depth(depth);

				// rainbow mode
				/*float R = clamp(abs(depth * 4.0 - 3.0) - 1.0, 0.0, 1.0);
				float G = clamp(2.0 - abs(depth * 4.0 - 2.0), 0.0, 1.0);
				float B = clamp(2.0 - abs(depth * 4.0 - 4.0), 0.0, 1.0);
                return fixed4(R,G,B,1.0)*/

				return fixed4(depth,depth,depth,1.0);
            }
             
            ENDCG
        }
    } 
 }