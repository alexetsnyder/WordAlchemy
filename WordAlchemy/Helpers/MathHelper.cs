
namespace WordAlchemy.Helpers
{
    public static class MathHelper
    {
        public static float Remap(float inputValue, float inputStart, float inputEnd, float outputStart, float outputEnd)
        {
            float r = inputEnd - inputStart;
            float R = outputEnd - outputStart;

            return outputStart + (R / r) * (inputValue - inputStart);
        }

        public static float FallOffMapCircular(float x, float y, int width, int height)
        {
            int midX = width / 2;
            int midY = height / 2;

            return 1 - MathF.Sqrt(DistanceSquared(x, y, midX, midY)) / MathF.Sqrt(DistanceSquared(0, 0, midX, midY));
        }

        public static float DistanceSquared(float x1, float y1, float x2, float y2)
        {
            float xDiff = x2 - x1;
            float yDiff = y2 - y1;

            return (xDiff * xDiff) + (yDiff * yDiff);
        }
    }
}
