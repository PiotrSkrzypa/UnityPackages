﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PSkrzypa.UnityFX
{
    [Serializable]
    public class GradientColorGraphicTweenAnimation : BaseFXComponent
    {
        [SerializeField] Graphic graphicToColor;
        [SerializeField] float gradientSamplingResolution = 30f;
        [SerializeField] Gradient gradient;

        Color[] colorSamples;
        Color originalColor;

        protected override async UniTask PlayInternal(CancellationToken cancellationToken)
        {
            if (graphicToColor == null)
            {
                Debug.LogWarning("[GradientColorGraphicTweenAnimation] graphicToColor is null.");
                return;
            }

            originalColor = graphicToColor.color;

            int steps = Mathf.Max(1, Mathf.RoundToInt(gradientSamplingResolution));
            float stepDuration = Timing.Duration / steps;
            colorSamples = new Color[steps];

            for (int i = 0; i < steps; i++)
            {
                float t = i / (steps - 1f);
                colorSamples[i] = gradient.Evaluate(t);
            }

            for (int i = 0; i < colorSamples.Length; i++)
            {
                graphicToColor.color = colorSamples[i];
                await UniTask.Delay(TimeSpan.FromSeconds(stepDuration),
                    ignoreTimeScale: Timing.TimeScaleIndependent,
                    cancellationToken: cancellationToken);
            }
        }

        protected override void StopInternal()
        {
            if (graphicToColor != null)
            {
                graphicToColor.color = originalColor;
            }
        }

        protected override void ResetInternal()
        {
            if (graphicToColor != null)
            {
                graphicToColor.color = originalColor;
            }
        }
    }
}
