using SDL2;

namespace WordAlchemy
{
    public static class Terrain
    {
        public static TerrainInfo Water = new TerrainInfo(TerrainType.WATER, "~", Colors.Blue(), 1, 4);

        public static TerrainInfo Sand = new TerrainInfo(TerrainType.SAND, "~", Colors.Sand(), 1, 4);

        public static TerrainInfo Grass = new TerrainInfo(TerrainType.GRASS, ",", Colors.Green(), 0, -3);

        public static TerrainInfo Dirt = new TerrainInfo(TerrainType.DIRT, ".", Colors.Brown(), 0, -3);

        public static TerrainInfo Hill = new TerrainInfo(TerrainType.HILL, "∩", Colors.DarkGreen(), 0, 0);

        public static TerrainInfo Mountain = new TerrainInfo(TerrainType.MOUNTAIN, "▲", Colors.Grey(), 0, 0);
    }

    public enum TerrainType
    {
        NONE,
        WATER,
        SAND,
        GRASS,
        DIRT,
        HILL,
        MOUNTAIN,
    }

    public struct TerrainInfo
    {
        public TerrainType Type { get; set; }

        public string Symbol { get; set; }
        public SDL.SDL_Color Color { get; set; }

        public int XMod { get; set; }
        public int YMod { get; set; }

        public TerrainInfo(TerrainType type, string symbol, SDL.SDL_Color color, int xMod, int yMod)
        {
            Type = type;
            Symbol = symbol;
            Color = color;
            XMod = xMod;
            YMod = yMod;
        }

        public bool Equals(TerrainInfo other)
        {
            return this.Symbol == other.Symbol &&
                   this.Color.r == other.Color.r &&
                   this.Color.g == other.Color.g &&
                   this.Color.b == other.Color.b &&
                   this.Color.a == other.Color.a &&
                   this.XMod == other.XMod &&
                   this.YMod == other.YMod;
        }
    }
}
