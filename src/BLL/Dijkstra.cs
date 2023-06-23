using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PathInMaze
{
    public class Dijkstra
    {
        //algorithm
        private Maze maze;// để lấy dữ liệu mê cung
        private int[,] length;// lưu độ dài đường đi từ ô(Node) bắt đầu đến các ô(Node)  khác trong mê cung
        private bool[,] visited;// tương tự dấu * khi làm giấy, true là đã tính rồi, false là chưa tính
        private Node[,] track;//để lưu đường đi từ ô(Node) bắt đầu dến ô(Node) kết thúc

        //color -> dùng để định dạng màu khi vẽ đường đi từ ô bắt đầu đến ô kết thúc
        private Color visitedColor = ExtendColor.Blue;
        private Color resetColor = ExtendColor.BlueBerry;
        private Color pathColor = ExtendColor.Pink;
        private Color startPathNodeCircleColor = ExtendColor.Yellow;
        private Color endPathNodeCircleColor = ExtendColor.Green;
        //phương thức khởi tạo 
        public Dijkstra(Maze maze)
        {
            this.maze = maze;
            this.visited = new bool[maze.rows, maze.columns];
            this.length = new int[maze.rows, maze.columns];
            this.track = new Node[maze.rows, maze.columns];

            //set up
            for(int row = 0; row < maze.rows; row++)
            {
                for(int column = 0; column < maze.columns; column++)
                {
                    visited[row, column] = false;//gán tất cả các ô là chưa được tính
                    length[row, column] = int.MaxValue;//gán tất cả các ô là vô cực
                    track[row, column] = null;//gán tất cả các ô là null, vì chưa biết ô phải đi qua
                }
            }

            //set start path node
            SetLength(maze.startPathNode, 0);//gán độ dài từ ô bắt đầu đến ô bắt đầu bằng 0
            SetTrack(maze.startPathNode, maze.startPathNode);//gán ô phải đi qua trước ô bắt đầu là ô bắt đầu
        }

        public void Run()
        {
            
            Timer timer = new Timer();//timer dùng để delay thuật toán với mục đính biểu diễn cách hoạt động của thuật toán
            timer.Interval = 13000 / (maze.rows * maze.columns);// số mili giây để gọi 1 tick
            timer.Tick += new EventHandler((obj, e) => Execute(timer));// phương thức khi 1 tick được gọi
            timer.Enabled = true;// cho timer hoạt động 
        }
        private void Execute(Timer timer)
        {
            //Dijsktra Algorithm
            if(!GetVisited(maze.endPathNode))//kiểm tra ô cuối cùng đã được tính chưa,nếu chưa thì tiếp tục chạy thuật toán
            {
                Node minNode = GetMinLengthNode();// lấy ô có độ dài nhỏ nhất trong mảng length
                SetVisited(minNode); //tương tự gán dấu * trên giấy,gán ô này trong visited là true, tức ô này đã được tính toán
                minNode.ChangeCircleColor(visitedColor);// đổi màu hình tròn của ô đang được tính toán

                foreach(Node adjNode in minNode.adjNodes)// chạy qua từng ô kề với ô đang được tính toán
                {
                    int oldLength = GetLength(adjNode);// lấy độ dài cũ của ô kề 
                    int newLength = GetLength(minNode) + 1;//tính độ dài mới của ô kề

                    if(oldLength > newLength)//nếu độ dài mới ngắn hơn độ dài cũ
                    {
                        SetLength(adjNode, newLength);// cập nhật lại độ dài của ô kề
                        SetTrack(adjNode, minNode);//gán ô đang được tính toán là ô phải đi qua trước ô kề
                    }
                }
            }
            else //vẽ đường đi từ ô bắt đầu tới ô kết thúc
            {
                timer.Enabled = false;// cho timer ngưng hoạt động
                timer.Dispose();// giải phóng tài nguyên của timer

                DrawPath();//vẽ đường
                maze.form.NextState();//trạng thái tiếp theo của cửa sổ
            }
        }
        private Node GetMinLengthNode()// lấy ra ô có độ dài nhỏ nhất trong mảng length
        {
            Node node = null;
            int min = Int32.MaxValue;

            for (int row = 0; row  < maze.rows; row++)
            {
                for (int column = 0; column < maze.columns; column++)
                {
                    if(length[row, column] < min && !visited[row, column])
                    {
                        min = length[row, column];
                        node = maze.graph[row, column];
                    }
                }
            }
            
            return node;
        }  
        private void DrawPath()// phương thức vẽ đường đi từ tô bắt đầu đến ô kết thúc
        {
            //reset all node color
            foreach(Node item in maze.graph)//chạy qua từng ô trong mê cung
            {
                if(item.position == maze.startPathNode.position)// khi ô đang xét là ô bắt đầu
                    item.ChangeCircleColor(startPathNodeCircleColor);// thì đổi màu theo chỉ định
                else if (item.position == maze.endPathNode.position)//khi ô đang xét là ô kết thúc
                    item.ChangeCircleColor(endPathNodeCircleColor);//thì đổi màu ô kết thúc theo màu chỉ định
                else
                    item.ChangeCircleColor(resetColor);//những ô còn lại thì đổi màu theo chỉ định
            }
            //tracking
            Stack<Node> stack = new Stack<Node>();//thêm ô hiện tại vào stack
            Node node = GetTrack(maze.endPathNode);//lấy ô phải đi qua trước ô hiện tại
            while(node.position != maze.startPathNode.position)//kiểm tra ô hiện tại có phải là ô bắt đầu 
            {
                stack.Push(node);
                node = GetTrack(node);
            }
            
            //draw path
            while(stack.Count != 0)//khi stack không rỗng 
            {
                Node trackNode = stack.Pop();//lấy và xóa phần tử trên cùng của stack
                trackNode.ChangeCircleColor(pathColor);// đổi màu hình tròn của ô đó
            }
        }

        private void SetLength(Node node, int value) => length[node.position.row, node.position.column] = value;
        private int GetLength(Node node) => length[node.position.row, node.position.column];
        private void SetTrack(Node node, Node trackNode) => track[node.position.row, node.position.column] = trackNode;
        private Node GetTrack(Node node) => track[node.position.row, node.position.column];
        private void SetVisited(Node node) => visited[node.position.row, node.position.column] = true;
        private bool GetVisited(Node node) => visited[node.position.row, node.position.column];
    }
}