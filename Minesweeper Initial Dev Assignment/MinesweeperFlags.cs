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
        public int turnPlayer = 1;
        public List<List<int>> board = new List<List<int>>();
        public bool boardFilled = false;
        public int mineCount = 50;
        public int boardRows = 0;
        public int boardCols = 0;

        public Image TILE = Resources.TILE;
        public Image TILE_CLEAR = Resources.TILE_CLEAR;
        public Image NUM_1 = Resources.NUM_1;
        public Image NUM_2 = Resources.NUM_2;
        public Image NUM_3 = Resources.NUM_3;
        public Image NUM_4 = Resources.NUM_4;
        public Image NUM_5 = Resources.NUM_5;
        public Image NUM_6 = Resources.NUM_6;
        public Image NUM_7 = Resources.NUM_7;
        public Image NUM_8 = Resources.NUM_8;
        public Image FLAG_BLUE = Resources.FLAG_BLUE;
        public Image FLAG_RED = Resources.FLAG_RED;



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
                    cell.BackgroundImage = TILE;

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
                    mineBoard[i].Add(0);
                    cells.Append(new List<int> { });
                }
            }

            if (cell != null) {
                cells.Remove(getCellLoc(cell));
            }

            Random rnd = new Random();

            for (int k = 0; k < mineCount; k++)
            {
                int row = rnd.Next(1, this.boardRows);
                int col = rnd.Next(1, this.boardCols);

                mineBoard[row][col] = -1;

                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++)
                    {
                        try
                        {
                            if (mineBoard[row + i][col + j] != -1) {
                                mineBoard[row + i][col + j] += 1;
                            }

                        }
                        catch (ArgumentOutOfRangeException)
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

        private void NextPlayer() {
            switch (turnPlayer)
            {
                case 1:
                    turnPlayer = 2;
                    break;
                case 2:
                    turnPlayer = 1;
                    break;
                default:
                    MessageBox.Show("nextPlayer error");
                    break;
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
            //initialise board (if not already)
            //passes first tiled clicked to ensure no instant flagging
            if (boardFilled != true) { board = generateMineBoard(mineCount, cell); }

            var cellRow = getCellLoc(cell)[0];
            var cellCol = getCellLoc(cell)[1];

            if (cell.BackgroundImage == TILE)
            {
                switch (board[cellRow][cellCol])
                {
                    case -1:
                        //MINE
                        getPlayer(turnPlayer).flags++;
                        cell.BackgroundImage = getPlayer(turnPlayer).flagImage;
                        NextPlayer();
                        break;
                    case 0:
                        //EMPTY
                        PlaceNumber(cell, 0);
                        ClearSurroundings(cell);
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        //NUMBERS
                        PlaceNumber(cell, board[cellRow][cellCol]);
                        break;
                    default:
                        //UNKOWN
                        break;
                }
            }


        }

        private void ClearSurroundings(PictureBox cell)
        {
            var cellRow = getCellLoc(cell)[0];
            var cellCol = getCellLoc(cell)[1];
            if (cell.BackgroundImage == TILE) {
                switch (board[cellRow][cellCol])
                {
                    case -1:
                        //MINE (gets ignored)
                        break;
                    case 0:
                        //EMPTY (recursion)
                        PlaceNumber(cell, 0);
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if(CellInBoardRange(cellRow+i, cellCol+j))
                                {
                                    //Clear surrounding cells
                                    Point newCoord = new Point(cellRow+i, cellCol+j);
                                    var newCell = tblpnlMineBoard.GetChildAtPoint(newCoord) as PictureBox;

                                    ClearSurroundings(newCell);
                                }
                                
                            }
                        }
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        //NUMBERS (places number)
                        PlaceNumber(cell, board[cellRow][cellCol]);
                        break;
                    default:
                        //UNKOWN (show error box)
                        MessageBox.Show("Surroundings clearing error");
                        break;
                }
            }
        }
            

            

        private void PlaceNumber(PictureBox cell, int number)
        {
            switch(number)
            {
                case 0:
                    cell.BackgroundImage = TILE_CLEAR;
                    break;
                case 1:
                    cell.BackgroundImage = NUM_1;
                    break;
                case 2:
                    cell.BackgroundImage = NUM_2;
                    break;
                case 3:
                    cell.BackgroundImage = NUM_3;
                    break;
                case 4:
                    cell.BackgroundImage = NUM_4;
                    break;
                case 5:
                    cell.BackgroundImage = NUM_5;
                    break;
                case 6:
                    cell.BackgroundImage = NUM_6;
                    break;
                case 7:
                    cell.BackgroundImage = NUM_7;
                    break;
                case 8:
                    cell.BackgroundImage = NUM_8;
                    break;
                default:
                    MessageBox.Show("Place number error");
                    break;
            }
        }

        private bool CellInBoardRange(int row, int col) {
            if (row >= 0 && col >= 0 && row <= boardRows && col <= boardCols)
            {
                return true;
            } else
            {
                return false;
            };
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
