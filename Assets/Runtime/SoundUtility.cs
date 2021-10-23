using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.Assertions;

namespace Fp.Utility
{
    public static class SoundUtility
    {
        public const float DecibelMax = 20;
        public const float DecibelMin = -80;
        public const float VolumeMax = 10;
        public const float VolumeMin = 0.0001f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DecibelToVolumeClamped(float decibel)
        {
            Assert.IsFalse(float.IsNaN(decibel), $"float.IsNaN({nameof(decibel)})");

            decibel = Mathf.Clamp(decibel, DecibelMin, DecibelMax);

            return DecibelToVolume(decibel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DecibelToVolume(float decibel)
        {
            Assert.IsFalse(float.IsNaN(decibel), $"float.IsNaN({nameof(decibel)})");

            return Mathf.Pow(10.0f, 0.05f * decibel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float VolumeToDecibelClamped(float volume)
        {
            Assert.IsFalse(float.IsNaN(volume), $"float.IsNaN({nameof(volume)})");

            volume = Mathf.Clamp(volume, VolumeMin, VolumeMax);

            return VolumeToDecibel(volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float VolumeToDecibel(float volume)
        {
            Assert.IsFalse(float.IsNaN(volume), $"float.IsNaN({nameof(volume)})");
            Assert.IsTrue(volume != 0, $"{nameof(volume)} != 0");

            return 20.0f * Mathf.Log10(volume);
        }
    }
}