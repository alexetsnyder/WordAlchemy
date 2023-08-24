
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

        public List<Edge> EdgeList { get; set;}

        public Node(int id)
        {
            Id = id;
            EdgeList = new List<Edge>();      
        }
    }
}
