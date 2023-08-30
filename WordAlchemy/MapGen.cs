
using SDL2;
using WordAlchemy.Helpers;

namespace WordAlchemy
{
    internal class MapGen
    {
        public int Width
        {
            get
            {
                return Cols * CharWidth;
            }
        }

        public int Height
        {
            get
            {
                return Rows * CharHeight;
            }
        }

        public int Rows { get; set; }
        public int Cols { get; set; }

        public int Seed { get; set; }

        public int CharWidth { get; private set; }
        public int CharHeight { get; private set; }

        private FastNoiseLite Noise { get; set; }
        private float[] HeightMap { get; set; }

        public MapGen(int rows, int cols, int seed)
        {
            Rows = rows;
            Cols = cols;

            Seed = seed;
            Noise = new FastNoiseLite(seed);
            Noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            Noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            SDLGraphics.Instance.SizeText(Terrain.Water.Symbol, FontName.IBM_VGA_8X14, out int width, out int height);
            CharWidth = width;
            CharHeight = height;

            HeightMap = new float[Cols * Rows];
        }

        public Map GenerateMap()
        {
            GenerateHeightMap();

            Map map = new Map(this, Rows, Cols);
            map.Graph = GenerateGraph();
            map.GroupList = GroupTerrain(map);

            return map;
        }

        private void GenerateHeightMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    float noise = Noise.GetNoise(i, j);
                    float remapNoise = MathHelper.Remap(noise, -1.0f, 1.0f, 0.0f, 100.0f);
                    float fallOffValue = MathHelper.SigmoidFallOffMapCircular(j, i, Cols, Rows);

                    HeightMap[i * Cols + j] = remapNoise * fallOffValue;
                }
            }
        }

        private Graph GenerateGraph()
        {
            Graph graph = new Graph();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    int x = j * CharWidth;
                    int y = i * CharHeight;

                    TerrainInfo terrain = GetTerrain(i, j);

                    MapNode mapNode = new MapNode(i * Cols + j, x, y, terrain);

                    graph.AddNode(mapNode);
                    if (j != 0)
                    {
                        Edge newEdge = new Edge(graph.NodeList[i * Cols + (j - 1)], mapNode);
                        graph.AddEdge(newEdge);
                    }
                    if (i != 0)
                    {
                        Edge newEdge = new Edge(graph.NodeList[(i - 1) * Cols + j], mapNode);
                        graph.AddEdge(newEdge);
                    }
                }
            }
            return graph;
        }

        private List<Group> GroupTerrain(Map map)
        {
            List<Group> groupList = new List<Group>();

            FillGroup(map.Graph, 0);

            return groupList;
        }

        private Group FillGroup(Graph graph, int groupId)
        {
            Group group = new Group(groupId);

            return group;
        }

        private void GenerateRivers()
        {
            int[] riverStarts = GetMaxHeights(6);
            
            foreach (int index in riverStarts)
            {
                int j = index % Cols;
                int i = (index - j) / Cols;

                if (j > Cols / 2)
                {
                    j++;
                    if (j < Cols)
                    {
                        int newIndex = i * Cols + j;
                        float height = HeightMap[newIndex];

                        while (height >= 20.0f && j < Cols)
                        {
                            if (height < 60.0f)
                            {
                                HeightMap[newIndex] = 19.0f;
                            }                 

                            j++;
                            newIndex = i * Cols + j;
                            height = HeightMap[newIndex];
                        }
                    }            
                }
                else
                {
                    j--;
                    if (j >= 0)
                    {
                        int newIndex = i * Cols + j;
                        float height = HeightMap[newIndex];

                        while (height >= 20.0f && j >= 0)
                        {
                            if (height < 60.0f)
                            {
                                HeightMap[newIndex] = 19.0f;
                            }

                            j--;
                            newIndex = i * Cols + j;
                            height = HeightMap[newIndex];
                        }
                    }
                }
            }
        }

        private int[] GetMaxHeights(int n = 1)
        {
            float[] maxValues = new float[n];
            int[] maxIndexes = new int[n];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    float heightValue = HeightMap[i * Cols + j];

                    for (int k = 0; k < n; k++)
                    {
                        if (heightValue > maxValues[k])
                        {
                            maxValues[k] = heightValue;
                            maxIndexes[k] = i * Cols + j;
                            break;
                        }
                    }
                }
            }

            return maxIndexes;
        }

        public TerrainInfo GetTerrain(int i, int j)
        {
            float heightValue = HeightMap[i * Cols + j];

            TerrainInfo terrain;
            if (heightValue < 20.0f)
            {
                terrain = Terrain.Water;
            }
            else if (heightValue < 25.0f)
            {
                terrain = Terrain.Sand;
            }
            else if (heightValue < 40.0f)
            {
                terrain = Terrain.Grass;
            }
            else if (heightValue < 60.0f)
            {
                terrain = Terrain.Hill;
            }
            else //(heightValue < 100.0f)
            {
                terrain = Terrain.Mountain;
            }

            return terrain;
        }
    }

    public static class Terrain
    {
        public static TerrainInfo Water = new TerrainInfo("~", Colors.Blue(), 1, 4);

        public static TerrainInfo Sand = new TerrainInfo("~", Colors.Sand(), 1, 4);

        public static TerrainInfo Grass = new TerrainInfo(",", Colors.Green(), 0, -3);

        public static TerrainInfo Dirt = new TerrainInfo(".", Colors.Brown(), 0, -3);

        public static TerrainInfo Hill = new TerrainInfo("∩", Colors.DarkGreen(), 0, 0);

        public static TerrainInfo Mountain = new TerrainInfo("▲", Colors.Grey(), 0, 0);
    }

    public struct TerrainInfo
    {
        public string Symbol { get; set; }
        public SDL.SDL_Color Color { get; set; }

        public int XMod { get; set; }
        public int YMod { get; set; }

        public TerrainInfo(string symbol, SDL.SDL_Color color, int xMod, int yMod)
        {
            Symbol = symbol;
            Color = color;
            XMod = xMod;
            YMod = yMod;
        }
    }
}
