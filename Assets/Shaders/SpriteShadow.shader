Shader "Customs/SpriteShadow"
{
	Properties
	{
		[NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
		_Tint("_Tint", Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Opaque"
			"Queue" = "Geometry+0"
		}

		Pass
		{
			Name "Universal Forward"
			Tags
			{
				"LightMode" = "UniversalForward"
			}

		// Render State
		Blend One Zero, One Zero
		Cull Back
		ZTest LEqual
		ZWrite On
		// ColorMask: <None>


		HLSLPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		// Debug
		// <None>

		// --------------------------------------------------
		// Pass

		// Pragmas
		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x
		#pragma target 2.0
		#pragma multi_compile_fog
		#pragma multi_compile_instancing

		// Keywords
		#pragma multi_compile _ LIGHTMAP_ON
		#pragma multi_compile _ DIRLIGHTMAP_COMBINED
		#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
		#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
		#pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
		#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
		#pragma multi_compile _ _SHADOWS_SOFT
		#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
		// GraphKeywords: <None>

		// Defines
		#define _AlphaClip 1
		#define ATTRIBUTES_NEED_NORMAL
		#define ATTRIBUTES_NEED_TANGENT
		#define ATTRIBUTES_NEED_TEXCOORD0
		#define ATTRIBUTES_NEED_TEXCOORD1
		#define VARYINGS_NEED_POSITION_WS 
		#define VARYINGS_NEED_NORMAL_WS
		#define VARYINGS_NEED_TANGENT_WS
		#define VARYINGS_NEED_TEXCOORD0
		#define VARYINGS_NEED_VIEWDIRECTION_WS
		#define VARYINGS_NEED_BITANGENT_WS
		#define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
		#define SHADERPASS_FORWARD

		// Includes
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		// --------------------------------------------------
		// Graph

		// Graph Properties
		CBUFFER_START(UnityPerMaterial)
		CBUFFER_END
		TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_TexelSize;
		float4 _Tint;
		SAMPLER(_SampleTexture2D_817D7838_Sampler_3_Linear_Repeat);

		// Graph Functions
		// GraphFunctions: <None>

		// Graph Vertex
		// GraphVertex: <None>

		// Graph Pixel
		struct SurfaceDescriptionInputs
		{
			float3 TangentSpaceNormal;
			float4 uv0;
		};

		struct SurfaceDescription
		{
			float3 Albedo;
			float3 Normal;
			float3 Emission;
			float Metallic;
			float Smoothness;
			float Occlusion;
			float Alpha;
			float AlphaClipThreshold;
		};

		SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
		{
			SurfaceDescription surface = (SurfaceDescription)0;
			float4 _SampleTexture2D_817D7838_RGBA_0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0.xy);
			float _SampleTexture2D_817D7838_R_4 = _SampleTexture2D_817D7838_RGBA_0.r;
			float _SampleTexture2D_817D7838_G_5 = _SampleTexture2D_817D7838_RGBA_0.g;
			float _SampleTexture2D_817D7838_B_6 = _SampleTexture2D_817D7838_RGBA_0.b;
			float _SampleTexture2D_817D7838_A_7 = _SampleTexture2D_817D7838_RGBA_0.a;
			float Slider_672C0532 = 0.5;
			float Slider_3490ED61 = 0.5;
			surface.Albedo = (_SampleTexture2D_817D7838_RGBA_0.xyz) * _Tint.rgb;
			surface.Normal = IN.TangentSpaceNormal;
			surface.Emission = IsGammaSpace() ? float3(0, 0, 0) : SRGBToLinear(float3(0, 0, 0));
			surface.Metallic = Slider_672C0532;
			surface.Smoothness = 0.5;
			surface.Occlusion = 1;
			surface.Alpha = _SampleTexture2D_817D7838_A_7;
			surface.AlphaClipThreshold = Slider_3490ED61;
			return surface;
		}

		// --------------------------------------------------
		// Structs and Packing

		// Generated Type: Attributes
		struct Attributes
		{
			float3 positionOS : POSITION;
			float3 normalOS : NORMAL;
			float4 tangentOS : TANGENT;
			float4 uv0 : TEXCOORD0;
			float4 uv1 : TEXCOORD1;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : INSTANCEID_SEMANTIC;
			#endif
		};

		// Generated Type: Varyings
		struct Varyings
		{
			float4 positionCS : SV_Position;
			float3 positionWS;
			float3 normalWS;
			float4 tangentWS;
			float4 texCoord0;
			float3 viewDirectionWS;
			float3 bitangentWS;
			#if defined(LIGHTMAP_ON)
			float2 lightmapUV;
			#endif
			#if !defined(LIGHTMAP_ON)
			float3 sh;
			#endif
			float4 fogFactorAndVertexLight;
			float4 shadowCoord;
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : CUSTOM_INSTANCE_ID;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
			#endif
		};

		// Generated Type: PackedVaryings
		struct PackedVaryings
		{
			float4 positionCS : SV_Position;
			#if defined(LIGHTMAP_ON)
			#endif
			#if !defined(LIGHTMAP_ON)
			#endif
			#if UNITY_ANY_INSTANCING_ENABLED
			uint instanceID : CUSTOM_INSTANCE_ID;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
			#endif
			float3 interp00 : TEXCOORD0;
			float3 interp01 : TEXCOORD1;
			float4 interp02 : TEXCOORD2;
			float4 interp03 : TEXCOORD3;
			float3 interp04 : TEXCOORD4;
			float3 interp05 : TEXCOORD5;
			float2 interp06 : TEXCOORD6;
			float3 interp07 : TEXCOORD7;
			float4 interp08 : TEXCOORD8;
			float4 interp09 : TEXCOORD9;
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
			#endif
		};

		// Packed Type: Varyings
		PackedVaryings PackVaryings(Varyings input)
		{
			PackedVaryings output = (PackedVaryings)0;
			output.positionCS = input.positionCS;
			output.interp00.xyz = input.positionWS;
			output.interp01.xyz = input.normalWS;
			output.interp02.xyzw = input.tangentWS;
			output.interp03.xyzw = input.texCoord0;
			output.interp04.xyz = input.viewDirectionWS;
			output.interp05.xyz = input.bitangentWS;
			#if defined(LIGHTMAP_ON)
			output.interp06.xy = input.lightmapUV;
			#endif
			#if !defined(LIGHTMAP_ON)
			output.interp07.xyz = input.sh;
			#endif
			output.interp08.xyzw = input.fogFactorAndVertexLight;
			output.interp09.xyzw = input.shadowCoord;
			#if UNITY_ANY_INSTANCING_ENABLED
			output.instanceID = input.instanceID;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			output.cullFace = input.cullFace;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
			#endif
			return output;
		}

		// Unpacked Type: Varyings
		Varyings UnpackVaryings(PackedVaryings input)
		{
			Varyings output;
			output.positionCS = input.positionCS;
			output.positionWS = input.interp00.xyz;
			output.normalWS = input.interp01.xyz;
			output.tangentWS = input.interp02.xyzw;
			output.texCoord0 = input.interp03.xyzw;
			output.viewDirectionWS = input.interp04.xyz;
			output.bitangentWS = input.interp05.xyz;
			#if defined(LIGHTMAP_ON)
			output.lightmapUV = input.interp06.xy;
			#endif
			#if !defined(LIGHTMAP_ON)
			output.sh = input.interp07.xyz;
			#endif
			output.fogFactorAndVertexLight = input.interp08.xyzw;
			output.shadowCoord = input.interp09.xyzw;
			#if UNITY_ANY_INSTANCING_ENABLED
			output.instanceID = input.instanceID;
			#endif
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			output.cullFace = input.cullFace;
			#endif
			#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
			output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
			#endif
			#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
			output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
			#endif
			return output;
		}

		// --------------------------------------------------
		// Build Graph Inputs

		SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
		{
			SurfaceDescriptionInputs output;
			ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

			output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
			output.uv0 = input.texCoord0;
		#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
		#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
		#else
		#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
		#endif
		#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

			return output;
		}


		// --------------------------------------------------
		// Main

		#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

		ENDHLSL
	}

	Pass
	{
		Name "ShadowCaster"
		Tags
		{
			"LightMode" = "ShadowCaster"
		}

			// Render State
			Blend One Zero, One Zero
			Cull Back
			ZTest LEqual
			ZWrite On
			// ColorMask: <None>


			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Debug
			// <None>

			// --------------------------------------------------
			// Pass

			// Pragmas
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
			#pragma multi_compile_instancing

			// Keywords
			// PassKeywords: <None>
			// GraphKeywords: <None>

			// Defines
			#define _AlphaClip 1
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define ATTRIBUTES_NEED_TEXCOORD0
			#define VARYINGS_NEED_TEXCOORD0
			#define SHADERPASS_SHADOWCASTER

			// Includes
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			// --------------------------------------------------
			// Graph

			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			CBUFFER_END
			TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_TexelSize;
			SAMPLER(_SampleTexture2D_817D7838_Sampler_3_Linear_Repeat);

			// Graph Functions
			// GraphFunctions: <None>

			// Graph Vertex
			// GraphVertex: <None>

			// Graph Pixel
			struct SurfaceDescriptionInputs
			{
				float3 TangentSpaceNormal;
				float4 uv0;
			};

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
			{
				SurfaceDescription surface = (SurfaceDescription)0;
				float4 _SampleTexture2D_817D7838_RGBA_0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0.xy);
				float _SampleTexture2D_817D7838_R_4 = _SampleTexture2D_817D7838_RGBA_0.r;
				float _SampleTexture2D_817D7838_G_5 = _SampleTexture2D_817D7838_RGBA_0.g;
				float _SampleTexture2D_817D7838_B_6 = _SampleTexture2D_817D7838_RGBA_0.b;
				float _SampleTexture2D_817D7838_A_7 = _SampleTexture2D_817D7838_RGBA_0.a;
				float Slider_3490ED61 = 0.5;
				surface.Alpha = _SampleTexture2D_817D7838_A_7;
				surface.AlphaClipThreshold = Slider_3490ED61;
				return surface;
			}

			// --------------------------------------------------
			// Structs and Packing

			// Generated Type: Attributes
			struct Attributes
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv0 : TEXCOORD0;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};

			// Generated Type: Varyings
			struct Varyings
			{
				float4 positionCS : SV_Position;
				float4 texCoord0;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
			};

			// Generated Type: PackedVaryings
			struct PackedVaryings
			{
				float4 positionCS : SV_Position;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
				#endif
				float4 interp00 : TEXCOORD0;
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			// Packed Type: Varyings
			PackedVaryings PackVaryings(Varyings input)
			{
				PackedVaryings output;
				output.positionCS = input.positionCS;
				output.interp00.xyzw = input.texCoord0;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				return output;
			}

			// Unpacked Type: Varyings
			Varyings UnpackVaryings(PackedVaryings input)
			{
				Varyings output;
				output.positionCS = input.positionCS;
				output.texCoord0 = input.interp00.xyzw;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				output.cullFace = input.cullFace;
				#endif
				#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
				output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
				#endif
				#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
				output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
				#endif
				return output;
			}

			// --------------------------------------------------
			// Build Graph Inputs

			SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

				output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
				output.uv0 = input.texCoord0;
			#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
			#else
			#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
			#endif
			#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

				return output;
			}


			// --------------------------------------------------
			// Main

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags
			{
				"LightMode" = "DepthOnly"
			}

				// Render State
				Blend One Zero, One Zero
				Cull Back
				ZTest LEqual
				ZWrite On
				ColorMask 0


				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				// Debug
				// <None>

				// --------------------------------------------------
				// Pass

				// Pragmas
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 2.0
				#pragma multi_compile_instancing

				// Keywords
				// PassKeywords: <None>
				// GraphKeywords: <None>

				// Defines
				#define _AlphaClip 1
				#define ATTRIBUTES_NEED_NORMAL
				#define ATTRIBUTES_NEED_TANGENT
				#define ATTRIBUTES_NEED_TEXCOORD0
				#define VARYINGS_NEED_TEXCOORD0
				#define SHADERPASS_DEPTHONLY

				// Includes
				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

				// --------------------------------------------------
				// Graph

				// Graph Properties
				CBUFFER_START(UnityPerMaterial)
				CBUFFER_END
				TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_TexelSize;
				SAMPLER(_SampleTexture2D_817D7838_Sampler_3_Linear_Repeat);

				// Graph Functions
				// GraphFunctions: <None>

				// Graph Vertex
				// GraphVertex: <None>

				// Graph Pixel
				struct SurfaceDescriptionInputs
				{
					float3 TangentSpaceNormal;
					float4 uv0;
				};

				struct SurfaceDescription
				{
					float Alpha;
					float AlphaClipThreshold;
				};

				SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
				{
					SurfaceDescription surface = (SurfaceDescription)0;
					float4 _SampleTexture2D_817D7838_RGBA_0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0.xy);
					float _SampleTexture2D_817D7838_R_4 = _SampleTexture2D_817D7838_RGBA_0.r;
					float _SampleTexture2D_817D7838_G_5 = _SampleTexture2D_817D7838_RGBA_0.g;
					float _SampleTexture2D_817D7838_B_6 = _SampleTexture2D_817D7838_RGBA_0.b;
					float _SampleTexture2D_817D7838_A_7 = _SampleTexture2D_817D7838_RGBA_0.a;
					float Slider_3490ED61 = 0.5;
					surface.Alpha = _SampleTexture2D_817D7838_A_7;
					surface.AlphaClipThreshold = Slider_3490ED61;
					return surface;
				}

				// --------------------------------------------------
				// Structs and Packing

				// Generated Type: Attributes
				struct Attributes
				{
					float3 positionOS : POSITION;
					float3 normalOS : NORMAL;
					float4 tangentOS : TANGENT;
					float4 uv0 : TEXCOORD0;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : INSTANCEID_SEMANTIC;
					#endif
				};

				// Generated Type: Varyings
				struct Varyings
				{
					float4 positionCS : SV_Position;
					float4 texCoord0;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
				};

				// Generated Type: PackedVaryings
				struct PackedVaryings
				{
					float4 positionCS : SV_Position;
					#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
					#endif
					float4 interp00 : TEXCOORD0;
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
					#endif
				};

				// Packed Type: Varyings
				PackedVaryings PackVaryings(Varyings input)
				{
					PackedVaryings output;
					output.positionCS = input.positionCS;
					output.interp00.xyzw = input.texCoord0;
					#if UNITY_ANY_INSTANCING_ENABLED
					output.instanceID = input.instanceID;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					output.cullFace = input.cullFace;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					return output;
				}

				// Unpacked Type: Varyings
				Varyings UnpackVaryings(PackedVaryings input)
				{
					Varyings output;
					output.positionCS = input.positionCS;
					output.texCoord0 = input.interp00.xyzw;
					#if UNITY_ANY_INSTANCING_ENABLED
					output.instanceID = input.instanceID;
					#endif
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					output.cullFace = input.cullFace;
					#endif
					#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
					output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
					#endif
					#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
					output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
					#endif
					return output;
				}

				// --------------------------------------------------
				// Build Graph Inputs

				SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
				{
					SurfaceDescriptionInputs output;
					ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

					output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
					output.uv0 = input.texCoord0;
				#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
				#else
				#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
				#endif
				#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

					return output;
				}


				// --------------------------------------------------
				// Main

				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

				ENDHLSL
			}

			Pass
			{
				Name "Meta"
				Tags
				{
					"LightMode" = "Meta"
				}

					// Render State
					Blend One Zero, One Zero
					Cull Back
					ZTest LEqual
					ZWrite On
					// ColorMask: <None>


					HLSLPROGRAM
					#pragma vertex vert
					#pragma fragment frag

					// Debug
					// <None>

					// --------------------------------------------------
					// Pass

					// Pragmas
					#pragma prefer_hlslcc gles
					#pragma exclude_renderers d3d11_9x
					#pragma target 2.0

					// Keywords
					#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
					// GraphKeywords: <None>

					// Defines
					#define _AlphaClip 1
					#define ATTRIBUTES_NEED_NORMAL
					#define ATTRIBUTES_NEED_TANGENT
					#define ATTRIBUTES_NEED_TEXCOORD0
					#define ATTRIBUTES_NEED_TEXCOORD1
					#define ATTRIBUTES_NEED_TEXCOORD2
					#define VARYINGS_NEED_TEXCOORD0
					#define SHADERPASS_META

					// Includes
					#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

					// --------------------------------------------------
					// Graph

					// Graph Properties
					CBUFFER_START(UnityPerMaterial)
					CBUFFER_END
					TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_TexelSize;
					SAMPLER(_SampleTexture2D_817D7838_Sampler_3_Linear_Repeat);

					// Graph Functions
					// GraphFunctions: <None>

					// Graph Vertex
					// GraphVertex: <None>

					// Graph Pixel
					struct SurfaceDescriptionInputs
					{
						float3 TangentSpaceNormal;
						float4 uv0;
					};

					struct SurfaceDescription
					{
						float3 Albedo;
						float3 Emission;
						float Alpha;
						float AlphaClipThreshold;
					};

					SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
					{
						SurfaceDescription surface = (SurfaceDescription)0;
						float4 _SampleTexture2D_817D7838_RGBA_0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0.xy);
						float _SampleTexture2D_817D7838_R_4 = _SampleTexture2D_817D7838_RGBA_0.r;
						float _SampleTexture2D_817D7838_G_5 = _SampleTexture2D_817D7838_RGBA_0.g;
						float _SampleTexture2D_817D7838_B_6 = _SampleTexture2D_817D7838_RGBA_0.b;
						float _SampleTexture2D_817D7838_A_7 = _SampleTexture2D_817D7838_RGBA_0.a;
						float Slider_3490ED61 = 0.5;
						surface.Albedo = (_SampleTexture2D_817D7838_RGBA_0.xyz);
						surface.Emission = IsGammaSpace() ? float3(0, 0, 0) : SRGBToLinear(float3(0, 0, 0));
						surface.Alpha = _SampleTexture2D_817D7838_A_7;
						surface.AlphaClipThreshold = Slider_3490ED61;
						return surface;
					}

					// --------------------------------------------------
					// Structs and Packing

					// Generated Type: Attributes
					struct Attributes
					{
						float3 positionOS : POSITION;
						float3 normalOS : NORMAL;
						float4 tangentOS : TANGENT;
						float4 uv0 : TEXCOORD0;
						float4 uv1 : TEXCOORD1;
						float4 uv2 : TEXCOORD2;
						#if UNITY_ANY_INSTANCING_ENABLED
						uint instanceID : INSTANCEID_SEMANTIC;
						#endif
					};

					// Generated Type: Varyings
					struct Varyings
					{
						float4 positionCS : SV_Position;
						float4 texCoord0;
						#if UNITY_ANY_INSTANCING_ENABLED
						uint instanceID : CUSTOM_INSTANCE_ID;
						#endif
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
						#endif
					};

					// Generated Type: PackedVaryings
					struct PackedVaryings
					{
						float4 positionCS : SV_Position;
						#if UNITY_ANY_INSTANCING_ENABLED
						uint instanceID : CUSTOM_INSTANCE_ID;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
						#endif
						float4 interp00 : TEXCOORD0;
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
						#endif
					};

					// Packed Type: Varyings
					PackedVaryings PackVaryings(Varyings input)
					{
						PackedVaryings output;
						output.positionCS = input.positionCS;
						output.interp00.xyzw = input.texCoord0;
						#if UNITY_ANY_INSTANCING_ENABLED
						output.instanceID = input.instanceID;
						#endif
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						output.cullFace = input.cullFace;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
						#endif
						return output;
					}

					// Unpacked Type: Varyings
					Varyings UnpackVaryings(PackedVaryings input)
					{
						Varyings output;
						output.positionCS = input.positionCS;
						output.texCoord0 = input.interp00.xyzw;
						#if UNITY_ANY_INSTANCING_ENABLED
						output.instanceID = input.instanceID;
						#endif
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						output.cullFace = input.cullFace;
						#endif
						#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
						output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
						#endif
						#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
						output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
						#endif
						return output;
					}

					// --------------------------------------------------
					// Build Graph Inputs

					SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
					{
						SurfaceDescriptionInputs output;
						ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

						output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
						output.uv0 = input.texCoord0;
					#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
					#else
					#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
					#endif
					#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

						return output;
					}


					// --------------------------------------------------
					// Main

					#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
					#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

					ENDHLSL
				}

				Pass
				{
						// Name: <None>
						Tags
						{
							"LightMode" = "Universal2D"
						}

						// Render State
						Blend One Zero, One Zero
						Cull Back
						ZTest LEqual
						ZWrite On
						// ColorMask: <None>


						HLSLPROGRAM
						#pragma vertex vert
						#pragma fragment frag

						// Debug
						// <None>

						// --------------------------------------------------
						// Pass

						// Pragmas
						#pragma prefer_hlslcc gles
						#pragma exclude_renderers d3d11_9x
						#pragma target 2.0
						#pragma multi_compile_instancing

						// Keywords
						// PassKeywords: <None>
						// GraphKeywords: <None>

						// Defines
						#define _AlphaClip 1
						#define ATTRIBUTES_NEED_NORMAL
						#define ATTRIBUTES_NEED_TANGENT
						#define ATTRIBUTES_NEED_TEXCOORD0
						#define VARYINGS_NEED_TEXCOORD0
						#define SHADERPASS_2D

						// Includes
						#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
						#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
						#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
						#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

						// --------------------------------------------------
						// Graph

						// Graph Properties
						CBUFFER_START(UnityPerMaterial)
						CBUFFER_END
						TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex); float4 _MainTex_TexelSize;
						SAMPLER(_SampleTexture2D_817D7838_Sampler_3_Linear_Repeat);

						// Graph Functions
						// GraphFunctions: <None>

						// Graph Vertex
						// GraphVertex: <None>

						// Graph Pixel
						struct SurfaceDescriptionInputs
						{
							float3 TangentSpaceNormal;
							float4 uv0;
						};

						struct SurfaceDescription
						{
							float3 Albedo;
							float Alpha;
							float AlphaClipThreshold;
						};

						SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
						{
							SurfaceDescription surface = (SurfaceDescription)0;
							float4 _SampleTexture2D_817D7838_RGBA_0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv0.xy);
							float _SampleTexture2D_817D7838_R_4 = _SampleTexture2D_817D7838_RGBA_0.r;
							float _SampleTexture2D_817D7838_G_5 = _SampleTexture2D_817D7838_RGBA_0.g;
							float _SampleTexture2D_817D7838_B_6 = _SampleTexture2D_817D7838_RGBA_0.b;
							float _SampleTexture2D_817D7838_A_7 = _SampleTexture2D_817D7838_RGBA_0.a;
							float Slider_3490ED61 = 0.5;
							surface.Albedo = (_SampleTexture2D_817D7838_RGBA_0.xyz);
							surface.Alpha = _SampleTexture2D_817D7838_A_7;
							surface.AlphaClipThreshold = Slider_3490ED61;
							return surface;
						}

						// --------------------------------------------------
						// Structs and Packing

						// Generated Type: Attributes
						struct Attributes
						{
							float3 positionOS : POSITION;
							float3 normalOS : NORMAL;
							float4 tangentOS : TANGENT;
							float4 uv0 : TEXCOORD0;
							#if UNITY_ANY_INSTANCING_ENABLED
							uint instanceID : INSTANCEID_SEMANTIC;
							#endif
						};

						// Generated Type: Varyings
						struct Varyings
						{
							float4 positionCS : SV_Position;
							float4 texCoord0;
							#if UNITY_ANY_INSTANCING_ENABLED
							uint instanceID : CUSTOM_INSTANCE_ID;
							#endif
							#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
							FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
							#endif
							#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
							uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
							#endif
							#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
							uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
							#endif
						};

						// Generated Type: PackedVaryings
						struct PackedVaryings
						{
							float4 positionCS : SV_Position;
							#if UNITY_ANY_INSTANCING_ENABLED
							uint instanceID : CUSTOM_INSTANCE_ID;
							#endif
							#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
							uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
							#endif
							#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
							uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
							#endif
							float4 interp00 : TEXCOORD0;
							#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
							FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
							#endif
						};

						// Packed Type: Varyings
						PackedVaryings PackVaryings(Varyings input)
						{
							PackedVaryings output;
							output.positionCS = input.positionCS;
							output.interp00.xyzw = input.texCoord0;
							#if UNITY_ANY_INSTANCING_ENABLED
							output.instanceID = input.instanceID;
							#endif
							#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
							output.cullFace = input.cullFace;
							#endif
							#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
							output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
							#endif
							#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
							output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
							#endif
							return output;
						}

						// Unpacked Type: Varyings
						Varyings UnpackVaryings(PackedVaryings input)
						{
							Varyings output;
							output.positionCS = input.positionCS;
							output.texCoord0 = input.interp00.xyzw;
							#if UNITY_ANY_INSTANCING_ENABLED
							output.instanceID = input.instanceID;
							#endif
							#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
							output.cullFace = input.cullFace;
							#endif
							#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
							output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
							#endif
							#if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
							output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
							#endif
							return output;
						}

						// --------------------------------------------------
						// Build Graph Inputs

						SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
						{
							SurfaceDescriptionInputs output;
							ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

							output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
							output.uv0 = input.texCoord0;
						#if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
						#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
						#else
						#define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
						#endif
						#undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

							return output;
						}


						// --------------------------------------------------
						// Main

						#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
						#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

						ENDHLSL
					}

	}
		FallBack "Hidden/Shader Graph/FallbackError"
}
