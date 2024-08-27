Shader "Custom/CleanShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}    // Dirty image
		_MaskTex("Mask", 2D) = "white" {}       // Cleaning mask
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" }
		LOD 200

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float2 texcoord : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;  // Dirty image
			sampler2D _MaskTex;  // Mask texture
			float4 _MainTex_ST;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);  // Get the color of the dirty image
				fixed4 mask = tex2D(_MaskTex, i.texcoord); // Get the mask's current state

				// Fully hide the dirty image where the mask is "cleaned"
				col.a *= mask.r; // Use the red channel of the mask as alpha to fully hide parts

				// Optionally: fully discard pixels where the mask is fully "cleaned"
				if (mask.r < 0.1)
				{
					discard; // This completely removes the pixel if the mask is close to 0
				}

				return col;
			}
			ENDCG
		}
	}
}