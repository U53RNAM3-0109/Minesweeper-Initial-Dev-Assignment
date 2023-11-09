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
        //Setup board variables
        public List<List<int>> board = new List<List<int>>();
        public bool boardFilled = false;
        public int boardRows = 0;
        public int boardCols = 0;
        public int mineCount = 51;

        public List<int> clearedCells = new List<int>();

        //Image Resource variables
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

        public static Image FLAG_RED = Resources.FLAG_RED;
        public static Image FLAG_BLUE = Resources.FLAG_BLUE;

        //Player variables
        public int turnPlayer = 1;
        public Player player1 = new Player("Player 1", Color.Red, FLAG_RED);
        public Player player2 = new Player("Player 2", Color.Blue, FLAG_BLUE);



        public MinesweeperFlags()
        {
            InitializeComponent();
        }

        private void MinesweeperFlags_Load(object sender, EventArgs e)
        {
            //Set up board pictureBoxes
            boardRows = tblpnlMineBoard.RowCount;
            boardCols = tblpnlMineBoard.ColumnCount;

            for (int i = 0; i < boardRows; i++)
            {
                for (int j = 0; j < boardCols; j++) {

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

        private List<List<int>> generateMineBoard(int mineCount, int cellRow, int cellCol)
        {
            //Filling mine board with mines, and creating numbers
            List<List<int>> mineBoard = new List<List<int>>() { };

            List<List<int>> cells = new List<List<int>>();

            //Make list of possible cell locations
            for (int i = 0; i < boardRows; i++)
            {
                mineBoard.Add(new List<int>());

                for (int j = 0; j < boardCols; j++)
                {
                    mineBoard[i].Add(0);

                    if (i == cellRow && j == cellCol) { continue; }
                    cells.Append(new List<int> { i, j });
                }
                
            }

            Random rnd = new Random();

            for (int k = 0; k < mineCount; k++)
            {
                int row = rnd.Next(1, boardRows);
                int col = rnd.Next(1, boardCols);

                //Prevent stacked mines
                if (mineBoard[row][col] == -1)
                {
                    k--;
                    continue;
                }

                mineBoard[row][col] = -1;

                //Increment surrounding tiles (for numbers)
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (CellInBoardRange(row+i,col+j)) {
                            if (mineBoard[row + i][col + j] != -1)
                            {
                                mineBoard[row + i][col + j] += 1;
                            }
                        }
                    }
                }
            }

            
            var outputString = "";
            for(int i = 0; i < mineBoard.Count; i++)
            {
                for (int j = 0; j < mineBoard[i].Count; j++)
                {
                    outputString+=mineBoard[i][j].ToString() + " ";
                }
                outputString += "\n";
            }
            Console.WriteLine(outputString);
            MessageBox.Show(outputString);
               
            

            boardFilled = true;
            return mineBoard;
        }

        private Player getPlayer(int player, bool getOpponent=false) {
            switch (player)
            {
                case 1:
                    if (getOpponent) { return player2; }
                    return player1;
                case 2:
                    if (getOpponent) { return player1; }
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
            else if (e.Button == MouseButtons.Right)
            {
                //BOMB
                if(getPlayer(turnPlayer).canBomb(getPlayer(turnPlayer, true)))
                {
                    actionBomb(cell);
                }
            }
        }

        private void actionMineClear(PictureBox cell)
        {
            var cellRow = getCellLoc(cell)[0];
            var cellCol = getCellLoc(cell)[1];

            //initialise board (if not already)
            //passes first tiled clicked to ensure no instant flagging
            if (boardFilled != true) { board = generateMineBoard(mineCount, cellRow, cellCol); }

            if (!IsCellCleared(cellRow, cellCol))
            {
                switch (board[cellRow][cellCol])
                {
                    case -1:
                        //MINE
                        getPlayer(turnPlayer).flags++;
                        cell.BackgroundImage = getPlayer(turnPlayer).flagImage;
                        clearedCells.Add(cellRow * 16 + cellCol);
                        break;
                    case 0:
                        //EMPTY
                        PlaceNumber(cellRow, cellCol, 0);
                        ClearSurroundings(cellRow, cellCol);
                        NextPlayer();
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
                        PlaceNumber(cellRow, cellCol, board[cellRow][cellCol]);
                        clearedCells.Add(cellRow * 16 + cellCol);
                        NextPlayer();
                        break;
                    default:
                        //UNKOWN
                        break;
                }
            }
        }

        private void actionBomb(PictureBox cell)
        {
            /*
            var cellRow = getCellLoc(cell)[0];
            var cellCol = getCellLoc(cell)[1];

            if (!IsCleared(cellRow, cellCol))
            {
                getPlayer(turnPlayer).hasBombed = true;
                switch (board[cellRow][cellCol])
                {
                    case -1:
                        //MINE
                        getPlayer(turnPlayer).flags++;
                        cell.BackgroundImage = getPlayer(turnPlayer).flagImage;
                        clearedCells.Add(new List<int> { cellRow, cellCol });
                        break;
                    case 0:
                        //EMPTY
                        PlaceNumber(cellRow, cellCol, 0);
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (CellInBoardRange(cellRow + i, cellCol + j))
                                {
                                    //Clear surrounding cells
                                    Point newCoord = new Point(cellRow + i, cellCol + j);
                                    var newCell = tblpnlMineBoard.GetChildAtPoint(newCoord) as PictureBox;

                                    ClearSurroundings(newCeRow,newCol);
                                }
                                ..
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
                        //NUMBERS
                        PlaceNumber(cellRow, cellCol, board[cellRow][cellCol]);
                        clearedCells.Add(new List<int> { cellRow, cellCol });
                        break;
                    default:
                        //UNKOWN
                        break;
                }

                NextPlayer();
            }
            */


        }

        private void ClearSurroundings(int cellRow, int cellCol)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var newRow = cellRow + i;
                    var newCol = cellCol + j;

                    if (!IsCellCleared(newRow, newCol))
                    {   if (CellInBoardRange(newRow, newCol))
                        { 
                            Console.WriteLine(board[newRow][newCol]);
                            if (board[newRow][newCol] == 0)
                            {
                                clearedCells.Add(newRow*16+newCol);
                                PlaceNumber(newRow, newCol, 0);
                                ClearSurroundings(newRow, newCol);

                            }
                            else { PlaceNumber(newRow, newCol, board[newRow][newCol]); }
                        }
                    }
                }
            }
        }

        private void IncrementFlags()
        {
            getPlayer(turnPlayer).flags++;


        }


        private void PlaceNumber(int cellRow, int cellCol, int number)
        {
            var cell = tblpnlMineBoard.Controls[16*cellRow+cellCol];

            switch (number)
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
            if (row >= 0 && col >= 0 && row < boardRows && col < boardCols)
            {
                return true;
            } else
            {
                return false;
            };
        }

        private bool IsCellCleared(int cellRow, int cellCol)
        {
            return clearedCells.Contains(cellRow * 16 + cellCol);
        }
    }
    public class Player
    {
        public int flags;
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

        public bool canBomb(Player opponent)
        {
            if (!this.hasBombed && this.flags < opponent.flags)
            {
                return true;
            }
            return false; 
        }
    }

}









/*
 
func mineClearing {
    if on clear list: ignore click
    else:
        if mine: place flag, increment flag counter, add to clear list
        if number: reveal number, add to clear list, pass turn
        if 0: call clearSurroundings(cellRow, cellCol), pass turn
} 

func clearSurroundings(cellRow, cellCol) {
    place 0
    add to clear list
    
    for surrounding cells:
        if cell not in clear list:
            if number: place number, add to clear list
            if 0: clearSurroundings(cellRow+mod, cellCol+mod)
}

 */