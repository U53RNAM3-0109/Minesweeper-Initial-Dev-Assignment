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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

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

        public int timeLeft = 60;

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

        public Image BOMB = Resources.BOMB;

        //Player variables
        public int turnPlayer = 1;
        public Player player1 = new Player("Player 1", Color.Red, FLAG_RED);
        public Player player2 = new Player("Player 2", Color.Blue, FLAG_BLUE);



        public MinesweeperFlags()
        {
            InitializeComponent();

            tmrTurnTimer.Interval = 1000;

            timeLeft = 30;
            tmrTurnTimer.Tick += Timer_Tick;
        }

        private void MinesweeperFlags_Load(object sender, EventArgs e)
        {
            //Sets some variables
            boardRows = tblpnlMineBoard.RowCount;
            boardCols = tblpnlMineBoard.ColumnCount;

            //Loop through board and place images
            for (int i = 0; i < boardRows; i++)
            {
                for (int j = 0; j < boardCols; j++) {
                    //Sets up pictureboxes
                    var cell = new PictureBox();

                    cell.Dock = DockStyle.Top;
                    cell.Padding = new Padding(0);
                    cell.Margin = new Padding(0);

                    cell.BackgroundImageLayout = ImageLayout.Stretch;
                    cell.BackgroundImage = TILE;

                    //Add mouse event handlers
                    cell.MouseClick += new MouseEventHandler(Cell_Click);

                    //Place image at coord
                    tblpnlMineBoard.Controls.Add(cell, j, i);
                }
            }
        }

        private void MinesweeperFlags_Shown(Object sender, EventArgs e)
        {
            MessageBox.Show("Welcome to Minesweeper Flags! This is a two-player minesweeper game, where you take turns to clear the board and place flags. Whoever marks the most mines wins!");
            MessageBox.Show("If you have less flags than your opponent, you can use the one-time only \"Bomb\" action, to automatically clear a 3x3 area.");
            MessageBox.Show("Your turn ends when you Bomb an area, clear a non-mine space, or run out of time! You have 30 seconds per turn.");
            MessageBox.Show("Controls:\nLeft mouse button - Uncover a tile/Flag a mine\nRight mouse button - Use the Bomb action");
            tmrTurnTimer.Start();
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            lblTurnTimeDisplay.Text = "(" + timeLeft + ")";

            if (timeLeft <= 0)
            {
                tmrTurnTimer.Stop();
                NextPlayer();
            }
        }

        public void ResetTimer()
        {
            timeLeft = 30;
            tmrTurnTimer.Start();
        }

        public bool CheckForWin() {
            if(player1.flags > player2.flags)
            { 
                var leadingPlayer = player1 as Player;
                var difference = player1.flags - player2.flags;
                if (mineCount < difference)
                {
                    return true;
                }
            }
            else
            { 
                var leadingPlayer = player2 as Player;
                var difference = player2.flags - player1.flags;
                if (mineCount < difference)
                {
                    return true;
                }
            }
            
            return false;



        }
        
        private void EndGame()
        {
            tmrTurnTimer.Stop();
            if (player1.flags > player2.flags)
            {
                MessageBox.Show($"Player 1 wins!\nScore was {player1.flags}-{player2.flags}");
            }
            else { MessageBox.Show($"Player 2 wins!\nScore was {player1.flags}-{player2.flags}"); }
            Application.Exit();

        }

        private void IncrementFlags()
        {
            //Increments flags and updates scoring
            getPlayer(turnPlayer).flags++;
            if (turnPlayer == 1)
            {
                lblPlayer1Flags.Text = Convert.ToString(getPlayer(turnPlayer).flags);
            }
            else
            {
                lblPlayer2Flags.Text = Convert.ToString(getPlayer(turnPlayer).flags);
            }

            //Check if current player can bomb and update accordingly
            if (getPlayer(1).canBomb(getPlayer(2)))
            {
                pctbxPlayer1BombAvailable.BackgroundImage = BOMB;
            }
            else
            {
                pctbxPlayer1BombAvailable.BackgroundImage = null;
            }

            //Check if enemy can bomb and update accordingly
            if (getPlayer(2).canBomb(getPlayer(1)))
            {
                pctbxPlayer2BombAvailable.BackgroundImage = BOMB;
            }
            else
            {
                pctbxPlayer2BombAvailable.BackgroundImage = null;
            }

            mineCount--;
            lblMineCount.Text = $"{mineCount} mines remaining";
            if (CheckForWin()) { EndGame(); }
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

                    //Skips first clicked cell
                    if (i == cellRow && j == cellCol) { continue; }
                    cells.Append(new List<int> { i, j });
                }
                
            }

            //Setup random
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

            //Write grid for debugging purposes
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
               
            boardFilled = true;
            return mineBoard;
        }

        private Player getPlayer(int player, bool getOpponent=false) {
            //Returns the current turn player object (or opponent if getOpponent is true)
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
            //Switches to the next turn and updates display accordingly
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
            lblPlayerTurn.Text = $"Player {turnPlayer}'s turn";
            tmrTurnTimer.Stop();
            MessageBox.Show($"Player {turnPlayer}'s turn");
            ResetTimer();
            
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
                    getPlayer(turnPlayer).hasBombed = true;
                    NextPlayer();
                }
            }
        }

        private void actionMineClear(PictureBox cell)
        {
            var cellRow = tblpnlMineBoard.GetRow(cell);
            var cellCol = tblpnlMineBoard.GetColumn(cell);

            //initialise board (if not already)
            //passes first tiled clicked to ensure no instant flagging
            if (boardFilled != true) { board = generateMineBoard(mineCount, cellRow, cellCol); }

            if (!IsCellCleared(cellRow, cellCol))
            {
                switch (board[cellRow][cellCol])
                {
                    case -1:
                        //MINE
                        IncrementFlags();
                        cell.BackgroundImage = getPlayer(turnPlayer).flagImage;
                        clearedCells.Add(cellRow * boardCols + cellCol);
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
                        clearedCells.Add(cellRow * boardCols + cellCol);
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
            MessageBox.Show("BOOM!");
            var cellRow = tblpnlMineBoard.GetRow(cell);
            var cellCol = tblpnlMineBoard.GetColumn(cell);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var newRow = cellRow + i;
                    var newCol = cellCol + j;
                    Console.WriteLine($"{i}, {j}");
                    if (!IsCellCleared(newRow, newCol))
                    {
                        Console.WriteLine($"Cell not cleared");
                        if (CellInBoardRange(newRow, newCol))
                        {
                            Console.WriteLine("Cell in range");
                            Console.WriteLine("Cell is:"+board[cellRow][newCol]);
                            switch (board[newRow][newCol])
                            {
                                case -1:
                                    //MINE
                                    IncrementFlags();
                                    cell = tblpnlMineBoard.Controls[boardCols * newRow + newCol] as PictureBox;
                                    cell.BackgroundImage = getPlayer(turnPlayer).flagImage;
                                    clearedCells.Add(newRow * boardCols + newCol);
                                    break;
                                case 0:
                                    //EMPTY
                                    PlaceNumber(newRow, newCol, 0);
                                    ClearSurroundings(newRow, newCol);
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
                                    PlaceNumber(newRow, newCol, board[newRow][newCol]);
                                    clearedCells.Add(newRow * boardCols + newCol);
                                    break;
                                default:
                                    //UNKOWN
                                    break;
                            }
                        }
                    }
                }
            }
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
                                clearedCells.Add(newRow*boardCols +newCol);
                                PlaceNumber(newRow, newCol, 0);
                                ClearSurroundings(newRow, newCol);

                            }
                            else { PlaceNumber(newRow, newCol, board[newRow][newCol]); }
                        }
                    }
                }
            }
        }

        


        private void PlaceNumber(int cellRow, int cellCol, int number)
        {
            //Get cell index
            var cell = tblpnlMineBoard.Controls[boardCols*cellRow+cellCol];
            
            //Update background image for cell
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
            //Checks whether given coord is in range of the grid
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
            return clearedCells.Contains(cellRow * boardCols + cellCol);
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
            if (!hasBombed && this.flags < opponent.flags)
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