
using SDL2;

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

        public static float LinearFallOffMapCircular(float x, float y, int width, int height)
        {
            int midX = width / 2;
            int midY = height / 2;

            return 1 - Distance(x, y, midX, midY) / Distance(0, 0, midX, midY);
        }

        public static float SigmoidFallOffMapCircular(float x, float y,  int width, int height)
        {
            int midX = width / 2;
            int midY = height / 2;

            float d = Distance(x, y, midX, midY);
            float dMax = Distance(0, 0, midX, midY);

            return 1 / (1 + MathF.Pow(MathF.E, (d * 12 / dMax - 6)));
        }

        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return MathF.Sqrt(DistanceSquared(x1, y1, x2, y2));
        }

        public static float DistanceSquared(float x1, float y1, float x2, float y2)
        {
            float xDiff = x2 - x1;
            float yDiff = y2 - y1;

            return ((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static int Dot(int Ax, int Ay, int Bx, int By)
        {
            return Ax * Bx + Ay * By;
        }

        //AB must be perpendicular to BC
        public static bool IsInRectangle(int Ax, int Ay, int Bx, int By, int Cx, int Cy, int x, int y)
        {
            int ABx = Bx - Ax;
            int ABy = By - Ay;

            int BCx = Cx - Bx;
            int BCy = Cy - By;

            int AMx = x - Ax;
            int AMy = y - Ay;

            int BMx = x - Bx;
            int BMy = y - By;

            int dotABAM = Dot(ABx, ABy, AMx, AMy);
            int dotABAB = Dot(ABx, ABy, ABx, ABy);
            int dotBCBM = Dot(BCx, BCy, BMx, BMy);
            int dotBCBC = Dot(BCx, BCy, BCx, BCy);

            return dotABAM >= 0 && dotABAM <= dotABAB && dotBCBM >= 0 && dotBCBM <= dotBCBC;
        }
    }
}
