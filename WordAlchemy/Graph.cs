
namespace WordAlchemy
{
    public class Graph
    {
        public List<Node> NodeList { get; set; }
        
        public List<Edge> EdgeList { get; set; }

        public Graph() 
        {
            NodeList = new List<Node>();
            EdgeList = new List<Edge>();
        }

        public void AddEdge(Edge edge)
        {
            EdgeList.Add(edge);
            edge.V1.EdgeList.Add(edge);
            edge.V2.EdgeList.Add(edge);
        }
    }

    public class Edge
    {
        public Node V1 { get; set; }

        public Node V2 { get; set; }

        public Edge(Node v1, Node v2)
        {
            V1 = v1;
            V2 = v2;
        }
    }

    public class Node
    {
        public int Id { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public object? Reference { get; set; }

        public List<Edge> EdgeList { get; set;}

        public Node(int id, int x, int y)
        {
            Id = id;   
            X = x;
            Y = y;
            EdgeList = new List<Edge>();      
        }
    }
}
