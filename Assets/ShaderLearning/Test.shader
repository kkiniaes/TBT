Shader "Learning/test" {
	Properties {
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_MainTex ("Ground Texture", 2D) = "white" {}
		
		_BumpMap("Ground Normal Texture", 2D) = "bump" {}
		_BumpDepth("Normal Depth", Range(-2.0,2.0)) = 1
		
		_SpecMap("Specular Map", 2D) = "white" {}
		
		_SpecColor ("Specular Color", Color) = (1.0,1.0,1.0,1.0)
		_Shininess ("Shininess", Float) = 10
		_SpecIntensity ("Specular Intensity", Float) = 2
		
		_RimPower ("Rim Power", Float) = 10
		_RegAtmosphereColor ("Regular Atmosphere Color", Color) = (1.0,1.0,1.0,1.0)
		_SunAtmosphereColor ("Sunset Atmosphere Color", Color) = (1.0,1.0,1.0,1.0)
		_Custom ("Custom", Float) = 1
		
		_SecondaryRim ("Secondary Rim", Float) = 0
	}
	SubShader {
		Tags { "LightMode" = "ForwardBase" }
		Pass {
		
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			
			//pragmas
			#pragma vertex vert
			#pragma fragment frag
			
			//user defined vars
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;
			uniform float _SpecIntensity;
			uniform float _RimPower;
			uniform float4 _RegAtmosphereColor;
			uniform float4 _SunAtmosphereColor;
			uniform float _Custom;
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			uniform sampler2D _BumpMap;
			uniform float4 _BumpMap_ST;
			uniform float _BumpDepth;
			
			uniform sampler2D _SpecMap;
			uniform float4 _SpecMap_ST;
			
			//Unity defined vars
			uniform float4 _LightColor0;
//			float4x4 _Object2World;
//			float4x4 _World2Object;
//			float4 _WorldSpaceLightPos0;
			
			//base input structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 col : COLOR;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			};
			
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float4 tex : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				float3 tangentWorld : TEXCOORD3;
				float3 binormalWorld : TEXCOORD4;
			};
			
			//vertex function
			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				
				o.normalDir = normalize(mul(float4(v.normal,0.0), _World2Object).xyz);
				o.tangentWorld = normalize( mul(_Object2World, float4(v.tangent.xyz,0.0)).xyz);
				o.binormalWorld = normalize( cross(o.normalDir, o.tangentWorld) * v.tangent.w);
				
				o.posWorld = mul(_Object2World, v.vertex);
				o.tex = v.texcoord;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				return o;
			}
			
			//fragment function
			float4 frag(vertexOutput i) : COLOR {
			
				//texture
				float4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				float4 specTex = tex2D(_SpecMap, i.tex.xy * _SpecMap_ST.xy + _SpecMap_ST.zw);
				float4 texN = tex2D(_BumpMap, i.tex.xy * _BumpMap_ST.xy + _BumpMap_ST.zw);
				
				float3 localCoords = float3(2.0 * texN.ag - float2(1.0,1.0),0.0);
				localCoords.z = _BumpDepth;
				
				float3x3 local2WorldTranspose = float3x3(
					i.tangentWorld,
					i.binormalWorld,
					i.normalDir
				);
				
				//setting up vars
				float3 normalDirection = normalize( mul( localCoords, local2WorldTranspose ));
				float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - i.posWorld);
				float3 lightDirection = normalize( _WorldSpaceLightPos0.xyz);
				float atten = 1.0;
				
				//Rim lighting
				float3 headOnEdNess = max(0.5,dot(viewDirection,lightDirection));
				half rim = 1 - saturate(dot(normalize(viewDirection), normalDirection));
				float4 atmCol = _RegAtmosphereColor*(min(1.5,(max(0.5,headOnEdNess*2)))-0.5) + _SunAtmosphereColor*(1-(min(1.5,(max(0.5,headOnEdNess*2)))-0.5));
				float3 rimLighting = specTex.xyz * max(0.0,dot(normalDirection*_Custom/(pow(headOnEdNess,(2))), lightDirection)) * atmCol * pow(rim, _RimPower);
				
				//diffuse lambert
				float3 diffuseReflection = atten * _LightColor0.xyz * _Color.rgb * max(0.0,dot(normalDirection, lightDirection));
				
				//specular
				float3 specularReflection = specTex.xyz * _LightColor0.xyz * _SpecColor.rgb * max(0.0, dot(normalDirection, lightDirection)) * pow(max(0.0,dot(reflect(-lightDirection,normalDirection),viewDirection)), _Shininess) * _SpecIntensity;
				
				//combining lighting
				float3 lightFinal = diffuseReflection + specularReflection + rimLighting + UNITY_LIGHTMODEL_AMBIENT;
				
				
												
				return float4(tex.xyz * lightFinal * _Color.xyz,tex.w);
			}
			
			ENDCG
		}
		
		Pass {
			ZWrite Off
			Blend One One
			
			CGPROGRAM
			
			//pragmas
			#pragma vertex vert
			#pragma fragment frag
			
			//user defined vars
			uniform float _SecondaryRim;
			uniform float _RimPower;
			uniform float4 _RegAtmosphereColor;
			uniform float4 _SunAtmosphereColor;
			uniform float _Custom;
			
			//Unity defined vars
			uniform float4 _LightColor0;
			
			//base input structs
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 col : COLOR;
				float4 tangent : TANGENT;
			};
			
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				float3 tangentWorld : TEXCOORD3;
				float3 binormalWorld : TEXCOORD4;
			};
			
			//vertex function
			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				
				o.normalDir = normalize(mul(float4(v.normal,0.0), _World2Object).xyz);
				o.tangentWorld = normalize( mul(_Object2World, float4(v.tangent.xyz,0.0)).xyz);
				o.binormalWorld = normalize( cross(o.normalDir, o.tangentWorld) * v.tangent.w);
				
				o.posWorld = mul(_Object2World, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				return o;
			}
			
			//fragment function
			float4 frag(vertexOutput i) : COLOR {
				
				//setting up vars
				float3 normalDirection = i.normalDir;
				float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - i.posWorld);
				float3 lightDirection = normalize( _WorldSpaceLightPos0.xyz);
				float atten = 1.0;
				
				//Rim lighting
				float3 headOnEdNess = max(0.5,dot(viewDirection,lightDirection));
				half rim = 1 - saturate(dot(normalize(viewDirection), normalDirection));
				float4 atmCol = _RegAtmosphereColor*(min(1.5,(max(0.5,headOnEdNess*2)))-0.5) + _SunAtmosphereColor*(1-(min(1.5,(max(0.5,headOnEdNess*2)))-0.5));
				float3 rimLighting = max(0.0,dot(normalDirection*_Custom/(pow(headOnEdNess,(2))), lightDirection)) * atmCol * pow(rim, _RimPower);
				
												
				return float4(rimLighting*_SecondaryRim,1.0);
			}
			
			ENDCG
		}
	}
}