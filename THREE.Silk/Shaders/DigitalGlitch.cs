﻿
using System.Runtime.Serialization;

namespace THREE
{
	[Serializable]
    public class DigitalGlitch : ShaderMaterial
    {
        public DigitalGlitch() : base()
        {
            Uniforms = new GLUniforms
            {

                { "tDiffuse", new GLUniform{{"value", null } } }, //diffuse texture
				{ "tDisp", new GLUniform{{"value", null } } }, //displacement texture for digital glitch squares
				{ "byp", new GLUniform{{"value", 0.0f } } }, //apply the glitch ?
				{ "amount", new GLUniform{{"value", 0.08f } } },
                { "angle", new GLUniform{{"value", 0.02f } } },
                { "seed", new GLUniform{{"value", 0.02f } } },
                { "seed_x", new GLUniform{{"value", 0.02f } } }, //-1,1
				{ "seed_y", new GLUniform{{"value", 0.02f } } }, //-1,1
				{ "distortion_x", new GLUniform{{"value", 0.5f } } },
                { "distortion_y", new GLUniform{{"value", 0.6f } } },
                { "col_s", new GLUniform{{"value", 0.05f } } }
            };

            VertexShader = @"

			varying vec2 vUv;
			void main() {
				vUv = uv;
				gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );
			}
			";


            FragmentShader = @"
		uniform int byp; //should we apply the glitch ?

		uniform sampler2D tDiffuse;
		uniform sampler2D tDisp;

		uniform float amount;
		uniform float angle;
		uniform float seed;
		uniform float seed_x;
		uniform float seed_y;
		uniform float distortion_x;
		uniform float distortion_y;
		uniform float col_s;

		varying vec2 vUv;


		float rand(vec2 co){
			return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
		}

		void main() {
			if(byp<1) {
				vec2 p = vUv;
				float xs = floor(gl_FragCoord.x / 0.5);
				float ys = floor(gl_FragCoord.y / 0.5);
		//based on staffantans glitch shader for unity https://github.com/staffantan/unityglitch
				vec4 normal = texture2D (tDisp, p*seed*seed);
				if(p.y<distortion_x+col_s && p.y>distortion_x-col_s*seed) {
					if(seed_x>0.){
						p.y = 1. - (p.y + distortion_y);
					}
					else {
						p.y = distortion_y;
					}
				}
				if(p.x<distortion_y+col_s && p.x>distortion_y-col_s*seed) {
					if(seed_y>0.){
						p.x=distortion_x;
					}
					else {
						p.x = 1. - (p.x + distortion_x);
					}
				}
				p.x+=normal.x*seed_x*(seed/5.);
				p.y+=normal.y*seed_y*(seed/5.);
		//base from RGB shift shader
				vec2 offset = amount * vec2( cos(angle), sin(angle));
				vec4 cr = texture2D(tDiffuse, p + offset);
				vec4 cga = texture2D(tDiffuse, p);
				vec4 cb = texture2D(tDiffuse, p - offset);
				gl_FragColor = vec4(cr.r, cga.g, cb.b, cga.a);
		//add noise
				vec4 snow = 200.*amount*vec4(rand(vec2(xs * seed,ys * seed*50.))*0.2);
				gl_FragColor = gl_FragColor+ snow;
			}
			else {
				gl_FragColor=texture2D (tDiffuse, vUv);
			}
		}
		";
        }

        public DigitalGlitch(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
