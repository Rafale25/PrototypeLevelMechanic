using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ColorCompressionRenderer), PostProcessEvent.AfterStack, "Custom/ColorCompression")]
public sealed class ColorCompression : PostProcessEffectSettings
{
  [Range(0f, 1f), Tooltip("ColorCompression effect intensity.")]
   public FloatParameter blend = new FloatParameter { value = 0.5f };
}
public sealed class ColorCompressionRenderer : PostProcessEffectRenderer<ColorCompression>
{
   public override void Render(PostProcessRenderContext context)
  {
       var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/ColorCompression"));
       sheet.properties.SetFloat("_Blend", settings.blend);
       context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
  }
}