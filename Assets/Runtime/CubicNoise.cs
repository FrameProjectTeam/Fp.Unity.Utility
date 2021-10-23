using UnityEngine;

namespace Fp.Utility
{
    public sealed class CubicNoise
    {
        private const int RndA = 134775813;
        private const int RndB = 1103515245;

        private readonly int _seed;
        private readonly int _octave;
        private readonly int _periodX = int.MaxValue;
        private readonly int _periodY = int.MaxValue;

        public CubicNoise(int seed, int octave, int periodX, int periodY)
        {
            _seed = seed;
            _octave = octave;
            _periodX = periodX;
            _periodY = periodY;
        }

        public CubicNoise(int seed, int octave)
        {
            _seed = seed;
            _octave = octave;
        }

        public float Sample(float x)
        {
            var xi = (int)Mathf.Floor(x / _octave);
            float lerp = x / _octave - xi;

            return Interpolate(
                Randomize(_seed, Tile(xi - 1, _periodX), 0),
                Randomize(_seed, Tile(xi, _periodX), 0),
                Randomize(_seed, Tile(xi + 1, _periodX), 0),
                Randomize(_seed, Tile(xi + 2, _periodX), 0),
                lerp) * 0.666666f + 0.166666f;
        }

        public float Sample(float x, float y)
        {
            var xi = (int)Mathf.Floor(x / _octave);
            float lerpX = x / _octave - xi;
            var yi = (int)Mathf.Floor(y / _octave);
            float lerpY = y / _octave - yi;

            var xSamples = new float[4];

            for (var i = 0; i < 4; ++i)
            {
                xSamples[i] = Interpolate(
                    Randomize(_seed, Tile(xi - 1, _periodX), Tile(yi - 1 + i, _periodY)),
                    Randomize(_seed, Tile(xi, _periodX), Tile(yi - 1 + i, _periodY)),
                    Randomize(_seed, Tile(xi + 1, _periodX), Tile(yi - 1 + i, _periodY)),
                    Randomize(_seed, Tile(xi + 2, _periodX), Tile(yi - 1 + i, _periodY)),
                    lerpX);
            }

            return Interpolate(xSamples[0], xSamples[1], xSamples[2], xSamples[3], lerpY) * 0.5f + 0.25f;
        }

        private static float Randomize(int seed, int x, int y)
        {
            return (float)((((x ^ y) * RndA) ^ (seed + x)) * (((RndB * x) << 16) ^ (RndB * y - RndA))) / int.MaxValue;
        }

        private static int Tile(int coordinate, int period)
        {
            return coordinate % period;
        }

        private static float Interpolate(
            float a,
            float b,
            float c,
            float d,
            float x)
        {
            float p = d - c - (a - b);

            return x * (x * (x * p + (a - b - p)) + (c - a)) + b;
        }
    }
}