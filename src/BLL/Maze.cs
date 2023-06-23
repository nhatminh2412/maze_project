using System.Windows.Forms;
using System.Collections.Generic;

namespace PathInMaze
{
    public class Maze : Control
    {
        public int rows;
        public int columns;
        public Node[,] graph;

        public MazeForm form;
        public Node endPathNode;
        public Node startPathNode;

        public Maze(MazeForm form, int rows, int columns)
        {
            InitComponent(form, rows, columns);
            Generate();
        }
        private void InitComponent(MazeForm form, int rows, int columns)
        {
            this.form = form;
            this.rows = rows;
            this.columns = columns;
            this.graph = new Node[rows, columns];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    this.graph[row, column] = new Node(this, row, column);
                    this.Controls.Add(graph[row, column]);//thêm node vào controls của maze để biểu diễn trên cửa sổ
                }
            }
        }
        private void Generate()// tạo mê cung
        {
            Stack<Node> backtrackStack = new Stack<Node>();//stack dùng để backtracking
            backtrackStack.Push(graph[0, 0]);//chọn ô ngẫu nhiên
            RecursiveBacktracking(backtrackStack);//thuật toán quay lui đệ quy
        }
        private void RecursiveBacktracking(Stack<Node> stack)
        {
            if(stack.Count == 0) return;//điều kiện ngừng

            Node currentNode = stack.Pop();//lấy ô trên cùng của stack
            Node neighborNode = currentNode.GetRandomNeighborNode();//lấy ngẫu nhiên ô nằm cạnh ô hiện tại

            if(neighborNode == null) RecursiveBacktracking(stack);// khi không có ô nằm cạnh thì đệ quy
            elses
            {
                currentNode.Adjude(neighborNode);//cho ô nằm cạnh thành ô kề với ô hiện tại
                stack.Push(currentNode);//cho ô hiện tại vào stack
                stack.Push(neighborNode);//cho ô nằm cạnh vào stack
                RecursiveBacktracking(stack);// đẹ quy
            }
        }
    }
}