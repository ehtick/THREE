﻿using System.Runtime.Serialization;

namespace THREE
{
	[Serializable]
    public class SSAOShader : ShaderMaterial
    {
        public SSAOShader()
        {
            Defines.Add("PERSPECTIVE_CAMERA", "1");
            Defines.Add("KERNEL_SIZE", "32");

            Uniforms = new GLUniforms{

                { "tDiffuse", new GLUniform{{ "value", null } } },
                { "tNormal", new GLUniform{{ "value", null } } },
                { "tDepth", new GLUniform{{ "value", null } } },
                { "tNoise", new GLUniform{{ "value", null } } },
                { "kernel", new GLUniform{{ "value", null } } },
                { "cameraNear", new GLUniform{{ "value", null } } },
                { "cameraFar", new GLUniform{{ "value", null } } },
                { "resolution", new GLUniform{{ "value", new Vector2() } } },
                { "cameraProjectionMatrix", new GLUniform{{ "value", new Matrix4() } } },
                { "cameraInverseProjectionMatrix", new GLUniform{{ "value", new Matrix4() } } },
                { "kernelRadius", new GLUniform{{ "value", 8.0f } } },
                { "minDistance", new GLUniform{{ "value", 0.005f } } },
                { "maxDistance", new GLUniform{{ "value", 0.05f } } },

            };

            VertexShader = @"
			varying vec2 vUv; 

             void main() {

				vUv = uv;
			    gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );
		     }


                ";

            FragmentShader = @"
			uniform sampler2D tDiffuse; 
			uniform sampler2D tNormal;
			uniform sampler2D tDepth;
			uniform sampler2D tNoise;

			uniform vec3 kernel[ KERNEL_SIZE ];

			uniform vec2 resolution;

			uniform float cameraNear;
			uniform float cameraFar;
			uniform mat4 cameraProjectionMatrix;
			uniform mat4 cameraInverseProjectionMatrix;

			uniform float kernelRadius;
			uniform float minDistance; // avoid artifacts caused by neighbour fragments with minimal depth difference
			uniform float maxDistance; // avoid the influence of fragments which are too far away

			varying vec2 vUv;

			#include <packing>

			float getDepth( const in vec2 screenPosition ) {

				return texture2D( tDepth, screenPosition ).x;

			}

			float getLinearDepth( const in vec2 screenPosition ) {

				#if PERSPECTIVE_CAMERA == 1

					float fragCoordZ = texture2D( tDepth, screenPosition ).x;
					float viewZ = perspectiveDepthToViewZ( fragCoordZ, cameraNear, cameraFar );
					return viewZToOrthographicDepth( viewZ, cameraNear, cameraFar );

				#else

					return texture2D( tDepth, screenPosition ).x;

				#endif

			}

			float getViewZ( const in float depth ) {

				#if PERSPECTIVE_CAMERA == 1

					return perspectiveDepthToViewZ( depth, cameraNear, cameraFar );

				#else

					return orthographicDepthToViewZ( depth, cameraNear, cameraFar );

				#endif

			}

			vec3 getViewPosition( const in vec2 screenPosition, const in float depth, const in float viewZ ) {

				float clipW = cameraProjectionMatrix[2][3] * viewZ + cameraProjectionMatrix[3][3];

				vec4 clipPosition = vec4( ( vec3( screenPosition, depth ) - 0.5 ) * 2.0, 1.0 );

				clipPosition *= clipW; // unprojection.

				return ( cameraInverseProjectionMatrix * clipPosition ).xyz;

			}

			vec3 getViewNormal( const in vec2 screenPosition ) {

				return unpackRGBToNormal( texture2D( tNormal, screenPosition ).xyz );

			}

			void main() {

				float depth = getDepth( vUv );
				float viewZ = getViewZ( depth );

				vec3 viewPosition = getViewPosition( vUv, depth, viewZ );
				vec3 viewNormal = getViewNormal( vUv );

			 vec2 noiseScale = vec2( resolution.x / 4.0, resolution.y / 4.0 );
				vec3 random = texture2D( tNoise, vUv * noiseScale ).xyz;

			// compute matrix used to reorient a kernel vector

				vec3 tangent = normalize( random - viewNormal * dot( random, viewNormal ) );
				vec3 bitangent = cross( viewNormal, tangent );
				mat3 kernelMatrix = mat3( tangent, bitangent, viewNormal );

			 float occlusion = 0.0;

			 for ( int i = 0; i < KERNEL_SIZE; i ++ ) {

					vec3 sampleVector = kernelMatrix * kernel[ i ]; // reorient sample vector in view space
					vec3 samplePoint = viewPosition + ( sampleVector * kernelRadius ); // calculate sample point

					vec4 samplePointNDC = cameraProjectionMatrix * vec4( samplePoint, 1.0 ); // project point and calculate NDC
					samplePointNDC /= samplePointNDC.w;

					vec2 samplePointUv = samplePointNDC.xy * 0.5 + 0.5; // compute uv coordinates

					float realDepth = getLinearDepth( samplePointUv ); // get linear depth from depth texture
					float sampleDepth = viewZToOrthographicDepth( samplePoint.z, cameraNear, cameraFar ); // compute linear depth of the sample view Z value
					float delta = sampleDepth - realDepth;

					if ( delta > minDistance && delta < maxDistance ) { // if fragment is before sample point, increase occlusion

						occlusion += 1.0;

					}

				}

				occlusion = clamp( occlusion / float( KERNEL_SIZE ), 0.0, 1.0 );

				gl_FragColor = vec4( vec3( 1.0 - occlusion ), 1.0 );

			}

		";

        }

        public SSAOShader(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }

	[Serializable]
    public class SSAODepthShader : ShaderMaterial
    {
        public SSAODepthShader()
        {
            Defines.Add("PERSPECTIVE_CAMERA", "1");

            Uniforms = new GLUniforms{

                { "tDepth", new GLUniform{{ "value", null } } },
                { "cameraNear", new GLUniform{{ "value", null } } },
                { "cameraFar", new GLUniform{{ "value", null } } }
                };

            VertexShader = @"
			varying vec2 vUv; 

             void main() {

				vUv = uv;
			    gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );
		     }


                ";

            FragmentShader = @"
		uniform sampler2D tDepth; 

		uniform float cameraNear;
		uniform float cameraFar;

		varying vec2 vUv;

		#include <packing>

		float getLinearDepth( const in vec2 screenPosition ) {

			#if PERSPECTIVE_CAMERA == 1

				float fragCoordZ = texture2D( tDepth, screenPosition ).x;
				float viewZ = perspectiveDepthToViewZ( fragCoordZ, cameraNear, cameraFar );
				return viewZToOrthographicDepth( viewZ, cameraNear, cameraFar );

			#else

				return texture2D( tDepth, screenPosition ).x;

			#endif

		}

		void main() {

			float depth = getLinearDepth( vUv );
			gl_FragColor = vec4( vec3( 1.0 - depth ), 1.0 );

		}

		";
        }

        public SSAODepthShader(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

	[Serializable]
    public class SSAOBlurShader : ShaderMaterial
    {
        public SSAOBlurShader()
        {

            Uniforms = new GLUniforms{
                { "tDiffuse", new GLUniform{{ "value", null } } },
                { "resolution", new GLUniform{{ "value", new Vector2() } } }
                };

            VertexShader = @"
				varying vec2 vUv; 

				 void main() {

					vUv = uv;
					gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );
				 }


                ";

            FragmentShader = @"
		uniform sampler2D tDiffuse; 

		uniform vec2 resolution;

		varying vec2 vUv;

		void main() {

			vec2 texelSize = ( 1.0 / resolution );
			float result = 0.0;

			for ( int i = - 2; i <= 2; i ++ ) {

				for ( int j = - 2; j <= 2; j ++ ) {

					vec2 offset = ( vec2( float( i ), float( j ) ) ) * texelSize;
					result += texture2D( tDiffuse, vUv + offset ).r;

				}

			}

			gl_FragColor = vec4( vec3( result / ( 5.0 * 5.0 ) ), 1.0 );

		}
";

        }

        public SSAOBlurShader(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}
