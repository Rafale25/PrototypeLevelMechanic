using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ColorCompressionRenderer), PostProcessEvent.AfterStack, "Custom/ColorCompression")]
public sealed class ColorCompression : PostProcessEffectSettings
{
  [Tooltip("Colorgrading or palette, 0 or 1")]
   public FloatParameter palette = new FloatParameter { value = 0f };
}
public sealed class ColorCompressionRenderer : PostProcessEffectRenderer<ColorCompression>
{
   public override void Render(PostProcessRenderContext context)
  {
       var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/ColorCompression"));
       sheet.properties.SetFloat("_Palette", settings.palette);
       context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
  }
}