using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessSettingsApplierBuiltIn : GraphicSettingsApplier
{
    public PostProcessVolume PostProcessVolume { get; set; }
    GraphicSettingsManager GraphicSettingsManager { get; set; }

    private void Awake()
    {
        GraphicSettingsManager = FindObjectOfType<GraphicSettingsManager>();
        PostProcessVolume = GetComponent<PostProcessVolume>();
    }

    protected override void Start()
    {
        base.Start();
        ApplySettings();
    }

    public override void ApplySettings()
    {
        ApplySetting<BloomSetting, Bloom>();
        ApplySetting<GrainSetting, Grain>();
        ApplySetting<MotionBlurSetting, MotionBlur>();
        ApplySetting<AmbientOcclusionSetting, AmbientOcclusion>();
        ApplySetting<ChromaticAberrationSetting, ChromaticAberration>();
    }
    
    void ApplySetting<TGraphicSetting, TPostProcessEffect>()
        where TGraphicSetting : ToggleGraphicSetting
        where TPostProcessEffect : PostProcessEffectSettings
    {
        if (PostProcessVolume.profile == null)
        {
            return;
        }

        if (PostProcessVolume.profile.TryGetSettings<TPostProcessEffect>(out var effect) &&
            GraphicSettingsManager.TryGet<TGraphicSetting>(out var setting))
        {
            effect.enabled.value = setting.IsEnabled();
        }
    }
}