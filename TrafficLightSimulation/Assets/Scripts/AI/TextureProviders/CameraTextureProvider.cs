using Assets.Scripts.TextureProviders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Scripts.TextureProviders {
    public class CameraTextureProvider : TextureProvider {

        [SerializeField]
        private RenderTexture renderTexture;

        public CameraTextureProvider(int width, int height, TextureFormat format = TextureFormat.RGB24) : base(width, height, format) {
            InputTexture = renderTexture;
        }

        public CameraTextureProvider(CameraTextureProvider provider, int width, int height, TextureFormat format = TextureFormat.RGB24) : this(width, height, format) {
            if (provider == null)
                return;

            renderTexture = provider.renderTexture;
            InputTexture = renderTexture;
        }

        public override void Start() {
        }

        public override void Stop() {
        }

        public override TextureProviderType.ProviderType TypeEnum() {
            return TextureProviderType.ProviderType.Camera;
        }
    }
}