using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Scripting;

namespace JD.TiltShifts
{
    [Serializable]
    [PostProcess(typeof(TiltShiftRenderer), PostProcessEvent.AfterStack, "Custom/TiltShift")]
    public sealed class TiltShift : PostProcessEffectSettings
    {
        [Range(0f, 25f), Tooltip("Max blur size")]
        public FloatParameter maxBlurSize = new FloatParameter { value = 5.0f };

        [Range(0f, 15f), Tooltip("Blur area")] public FloatParameter blurArea = new FloatParameter { value = 1.0f };

        [Range(-0.5f, 0.5f), Tooltip("Offset Blur Position")]
        public FloatParameter offset = new FloatParameter { value = 0f };

        [Tooltip("Small Kernel Size")] public BoolParameter smallKernelSize = new BoolParameter { value = false };
    }

    [Preserve]
    public sealed class TiltShiftRenderer : PostProcessEffectRenderer<TiltShift>
    {
        private static readonly int _blurSize = Shader.PropertyToID("_BlurSize");
        private static readonly int _blurArea = Shader.PropertyToID("_BlurArea");
        private static readonly int _offset = Shader.PropertyToID("_Offset");
        private static readonly int _mode = Shader.PropertyToID("_Mode");

        private Shader _shader;
        private bool _hasShader;

        public override void Render(PostProcessRenderContext context)
        {
            if (!_hasShader)
            {
                _shader = Shader.Find("Hidden/PostFX/TiltShift");
                _hasShader = true;
            }

            var sheet = context.propertySheets.Get(_shader);
            sheet.properties.SetFloat(_blurSize, settings.maxBlurSize < 0.0f ? 0.0f : settings.maxBlurSize);
            sheet.properties.SetFloat(_blurArea, settings.blurArea);
            sheet.properties.SetFloat(_offset, settings.offset);
            sheet.properties.SetFloat(_mode, settings.smallKernelSize ? 1 : 0);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}