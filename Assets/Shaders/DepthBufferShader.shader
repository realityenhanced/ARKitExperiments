Shader "Unlit/DepthBufferShader"
{
	Properties
	{
    	_texture ("MainTexture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
            ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct Vertex
			{
				float4 position : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct TexCoordInOut
			{
				float4 position : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			TexCoordInOut vert (Vertex vertex)
			{
				TexCoordInOut o;
				o.position = UnityObjectToClipPos(vertex.position/5); 
				o.texcoord = vertex.texcoord;
	            
				return o;
			}
			
            // samplers
            sampler2D _texture;

			fixed4 frag (TexCoordInOut i) : SV_Target
			{
				// sample the texture
                float2 texcoord = i.texcoord;
                float r = tex2D(_texture, texcoord).r;
                float4 rgba = float4(r, r, r, 1.0);

                return rgba;
			}
			ENDCG
		}
	}
}
