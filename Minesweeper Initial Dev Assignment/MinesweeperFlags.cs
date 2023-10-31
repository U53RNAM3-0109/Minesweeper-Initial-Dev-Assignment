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
        public Player player1 = new Player("Player 1", Color.Red, Resources.FLAG_RED);
        public Player player2 = new Player("Player 2", Color.Blue, Resources.FLAG_BLUE);
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
            //Set up board pictureBoxes
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

                    //Add mouse event handlers
                    cell.MouseClick += new MouseEventHandler(Cell_Click);

                    tblpnlMineBoard.Controls.Add(cell, j, i);
                }
            }
        }

        private List<List<int>> generateMineBoard(int mineCount, PictureBox cell = null)
        {
            //Filling mine board with mines, and creating numbers
            List<List<int>> mineBoard = new List<List<int>>() { };

            List<List<int>> cells = new List<List<int>>();

            //Make list of possible cell locations
            for (int i = 0; i < this.boardRows; i++)
            {
                mineBoard.Add(new List<int>());

                for (int j = 0; j < this.boardCols; j++)
                {
                    mineBoard[i][j] = (0);
                    cells.Append(new List<int>{i, j});
                }
            }

            if (cell != null) {
                cells.Remove(getCellLoc(cell));
            }

            Random rnd = new Random();

            for (int k=0; k <mineCount; k++)
            {
                int row = rnd.Next(1, this.boardRows);
                int col = rnd.Next(1, this.boardCols);

                mineBoard[row][col] = -1;

                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j<= 1; j++)
                    {
                        try
                        {
                            if (mineBoard[row+i][col+j] != -1) {
                                mineBoard[row+i][col+j] += 1;
                            }
                            
                        }
                        catch (IndexOutOfRangeException)
                        {
                            break;
                        }
                    }
                }
            }

            boardFilled = true;
            return mineBoard;
        }

        private Player getPlayer(int player) {
            switch (player)
            {
                case 1:
                    return player1;
                case 2:
                    return player2;
                default:
                    MessageBox.Show("turnPlayer error");
                    return player1;
            }
        }

        private List<int> getCellLoc(PictureBox cell) {
            int rowNum = tblpnlMineBoard.GetRow(cell);
            int colNum = tblpnlMineBoard.GetColumn(cell);

            return new List<int> { rowNum, colNum };
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
            else if (e.Button == MouseButtons.Right && getPlayer(turnPlayer).canBomb())
            {
                //BOMB
            }
            else if (e.Button == MouseButtons.Middle)
            {
                cell.BackgroundImage = Resources.FLAG_BLUE;
                MessageBox.Show($"Row: {rowNum}, Col: {colNum}");
            };
        }

        private void actionMineClear(PictureBox cell)
        {
        }
    }
    public class Player
    {
        public int flags = 0;
        public bool hasBombed = false;
        public string name = string.Empty;
        public Color color = Color.Gray;
        public Image flagImage = null;

        public Player(string name, Color color, Image flagImage)
        {
            this.name = name;
            this.color = color;
            this.flagImage = flagImage;
        }

        public bool canBomb()
        {
            return true;
        }
    }

}
