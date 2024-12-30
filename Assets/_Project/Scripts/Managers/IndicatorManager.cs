using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class IndicatorManager : Singleton<IndicatorManager>
{
    public GameObject indicatorPrefab;
    public Transform indicatorContainer;
    [ShowInInspector]
    private Dictionary<Transform, Indicator> indicators = new();
    public void Register(Indicator indicator)
    {
        indicators.Add(indicator.Target, indicator);
    }

    private void Update()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        foreach (var indicator in indicators.Values)
        {
            indicator.UpdateIndicatorPosition(screenWidth, screenHeight);
        }
    }
    
    public Indicator UpdateTarget(Transform oldTarget, Transform newTarget)
    {
        if (!indicators.TryGetValue(oldTarget, out var indicator)) return null;
        indicator.UpdateTarget(newTarget);
        indicators.Remove(oldTarget);
        indicators.Add(newTarget, indicator);
        return indicator;
    }

    public void RemoveIndicator(Transform target)
    {
        if (indicators.ContainsKey(target))
        {
            Indicator.Destroy(indicators[target]);
            indicators.Remove(target);
        }
    }

    public void ClearIndicators()
    {
        foreach (var indicator in indicators.Values)
        {
            Indicator.Destroy(indicator);
        }
        indicators.Clear();
    }

    public Indicator GetIndicator(Transform target)
    {
        return indicators.GetValueOrDefault(target);
    }
}
