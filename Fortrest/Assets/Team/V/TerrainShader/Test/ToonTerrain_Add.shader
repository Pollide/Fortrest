// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Edits by Glynn Taylor. MIT license
// Includes code for splitmap by https://twitter.com/adamgryu and triplanar mapping by https://github.com/keijiro. MIT License

Shader "Hidden/ToonTerrainAdd" {
    Properties {
        [HideInInspector] _Control ("Control (RGBA)", 2D) = "black" {}
        [HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
        [HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
        [HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
        [HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
        [HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
        [HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
        [HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
        [HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		_VerticalTex("VerticalTex (RGB)", 2D) = "white" {}
    }

    CGINCLUDE
        #pragma surface surf Lambert decal:add vertex:SplatmapVert finalcolor:SplatmapFinalColor finalprepass:SplatmapFinalPrepass finalgbuffer:SplatmapFinalGBuffer noinstancing decal:blend
        #pragma multi_compile_fog
        #define TERRAIN_SPLAT_ADDPASS
        #include "ToonTerrain.cginc"


        void surf(Input IN, inout SurfaceOutput o)
        {
			half4 splat_control;
			half weight;
			fixed4 mixedDiffuse;

			float3 bf = normalize(abs(IN.localNormal));
			bf /= dot(bf, (float3)1);

			// Triplanar mapping
			float2 tx = IN.localCoord.yz * _VerticalTex_ST.x;
			float2 ty = IN.localCoord.zx* _VerticalTex_ST.x;
			float2 tz = IN.localCoord.xy* _VerticalTex_ST.x;

			// Base color
			half4 cx = tex2D(_VerticalTex, tz) * bf.x;
			half4 cy = tex2D(_VerticalTex, tz) * bf.y;
			half4 cz = tex2D(_VerticalTex, tz) * bf.z;
			half4 color = (cx + cy + cz);


			SplatmapMix(IN, splat_control, weight, mixedDiffuse, o.Normal);
			if (dot(IN.localNormal, fixed3(0, 1, 0)) >= 0.8)
			{
				o.Albedo = mixedDiffuse.rgb;
			}
			else {
				o.Albedo = color.rgb;
			}
			o.Alpha = weight;
        }
    ENDCG

    Category {
        Tags {
            "Queue" = "Geometry-99"
            "IgnoreProjector"="True"
            "RenderType" = "Opaque"
        }
        // TODO: Seems like "#pragma target 3.0 _TERRAIN_NORMAL_MAP" can't fallback correctly on less capable devices?
        // Use two sub-shaders to simulate different features for different targets and still fallback correctly.
        SubShader { // for sm3.0+ targets
            CGPROGRAM
                #pragma target 3.0
                #pragma multi_compile __ _TERRAIN_NORMAL_MAP
            ENDCG
        }
        SubShader { // for sm2.0 targets
            CGPROGRAM
            ENDCG
        }
    }

    Fallback off
}
