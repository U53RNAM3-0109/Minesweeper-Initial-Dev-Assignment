using Minesweeper_Initial_Dev_Assignment.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Minesweeper_Initial_Dev_Assignment
{
    public partial class MinesweeperFlags : Form
    {
        public bool gameOver = false;
        public Player player1 = new Player("Player 1", Color.Red);
        public Player player2 = new Player("Player 2", Color.Blue);
        public int turnPlayer = 0;
        public List<List<int>> board = new List<List<int>>();
        public bool boardFilled = false;
        public int boardRows = 0;
        public int boardCols = 0;

        public MinesweeperFlags()
        {
            InitializeComponent();
        }

        private void MinesweeperFlags_Load(object sender, EventArgs e)
        {
            this.boardRows = tblpnlMineBoard.RowCount;
            this.boardCols = tblpnlMineBoard.ColumnCount;

            for (int i = 0; i < this.boardRows; i++)
            {
                for (int j = 0; j < this.boardCols; j++) {

                    var cell = new PictureBox();

                    cell.Dock = DockStyle.Top;
                    cell.Padding = new Padding(0);
                    cell.Margin = new Padding(0);

                    cell.BackgroundImageLayout = ImageLayout.Stretch;
                    cell.BackgroundImage = Resources.TILE;

                    cell.MouseClick += new MouseEventHandler(Cell_Click);

                    tblpnlMineBoard.Controls.Add(cell, j, i);
                }
            }
        }

        private List<List<int>> generateMineBoard(int rowCount, int colCount)
        {
            List<List<int>> mineBoard = new List<List<int>>();

            //TODO
            //get all tiles and place randomly

            return mineBoard;
        }

        private void Cell_Click(object sender, MouseEventArgs e)
        {
            PictureBox cell = sender as PictureBox;
            int rowNum = tblpnlMineBoard.GetRow(cell);
            int colNum = tblpnlMineBoard.GetColumn(cell);

            // handle click event
            if (e.Button == MouseButtons.Left)
            {
                actionMineClear(cell);
            }
            else if (e.Button == MouseButtons.Right)
            {
                cell.BackgroundImage = Resources.FLAG_RED;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                cell.BackgroundImage = Resources.FLAG_BLUE;
                MessageBox.Show($"Row: {rowNum}, Col: {colNum}");
            };
        }

        private void actionMineClear(PictureBox cell)
        {   
            if(boardFilled == false)
            {
                this.board = generateMineBoard(this.boardRows, this.boardCols);
            }
            switch (turnPlayer) {
                case 1:
                    cell.BackgroundImage = Resources.FLAG_RED;
                    turnPlayer = 2;
                    break;
                case 2:
                    cell.BackgroundImage = Resources.FLAG_BLUE;
                    turnPlayer = 1;
                    break;
                default:
                    MessageBox.Show("turnPlayer error");
                    break;
            }

            
        }
    }
    public class Player
    {
        public int flags = 0;
        public bool hasBombed = false;
        public string name = string.Empty;
        public Color color = Color.Gray;

        public Player(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }
    }

}
