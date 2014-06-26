 Shader "Spectrum/glow" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color("Diffuse Material Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader {
    	Tags { "LightMode" = "ForwardBase" }
    	Pass {
    
CGPROGRAM

#pragma fragmentoption ARB_precision_hint_fastest
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "../Shaders/spectrum.cginc"

uniform sampler2D _MainTex;
uniform float4 _MainTex_ST;
uniform fixed4 _Color;
uniform fixed4 _LightColor0;

struct vertexInput {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
};

struct fragmentInput {
	float4 pos : SV_POSITION;
	float4 color : COLOR0;
	half2 uv : TEXCOORD0;
};

fragmentInput vert(vertexInput i) {
	fragmentInput o;
	o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
	o.uv = TRANSFORM_TEX(i.texcoord, _MainTex);
	
	float3 normalDirection = NORMAL_TO_WORLD(i.normal);
	
	float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
	
	float3 diffuse = _LightColor0.xyz * _Color.rgb * max( 0.0, dot(normalDirection, lightDirection));
	
	o.color = half4(diffuse, 1.0);
	
	return o;
}

half4 frag(fragmentInput i) : COLOR {
	return tex2D(_MainTex, i.uv) * i.color;
}
ENDCG	    
    	}
    } 
    Fallback "Specular"
  }