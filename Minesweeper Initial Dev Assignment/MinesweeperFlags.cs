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
    //Game
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
        public Player player1 = new Player(Color.Red, FLAG_RED);
        public Player player2 = new Player(Color.Blue, FLAG_BLUE);


        //Form functions
        public MinesweeperFlags()
        {
            InitializeComponent();

            //Setup turn timer tick interval
            tmrTurnTimer.Interval = 1000;

            //Setup turn timer duration, and add the Tick event
            timeLeft = 30;
            tmrTurnTimer.Tick += Timer_Tick;
        }

        private void MinesweeperFlags_Load(object sender, EventArgs e)
        {
            //Sets the global row/col variables
            boardRows = tblpnlMineBoard.RowCount;
            boardCols = tblpnlMineBoard.ColumnCount;

            //Loop through board and place images
            for (int i = 0; i < boardRows; i++)
            {
                for (int j = 0; j < boardCols; j++) {
                    //Sets up pictureboxes with correct sizing/padding
                    var cell = new PictureBox();

                    cell.Dock = DockStyle.Top;
                    cell.Padding = new Padding(0);
                    cell.Margin = new Padding(0);

                    cell.BackgroundImageLayout = ImageLayout.Stretch;
                    cell.BackgroundImage = TILE;

                    //Add mouse event handlers to mouseclick event
                    cell.MouseClick += new MouseEventHandler(Cell_Click);

                    //Place the picturebox at the location
                    tblpnlMineBoard.Controls.Add(cell, j, i);
                }
            }
        }

        private void MinesweeperFlags_Shown(Object sender, EventArgs e)
        {
            //This event is only called once, the first time the form is shown to the user.
            lblPlayerTurn.ForeColor = getPlayer(turnPlayer).color;
            lblTurnTimeDisplay.ForeColor = getPlayer(turnPlayer).color;
            MessageBox.Show("Welcome to Minesweeper Flags! This is a two-player minesweeper game, where you take turns to clear the board and place flags. Whoever marks the most mines wins!");
            MessageBox.Show("If you have less flags than your opponent, you can use the one-time only \"Bomb\" action, to automatically clear a 3x3 area.");
            MessageBox.Show("Your turn ends when you Bomb an area, clear a non-mine space, or run out of time! You have 30 seconds per turn.");
            MessageBox.Show("Controls:\nLeft mouse button - Uncover a tile/Flag a mine\nRight mouse button - Use the Bomb action");
            //After the instructions have been read, we start the timer.
            tmrTurnTimer.Start();
        }


        //Timer functions
        public void Timer_Tick(object sender, EventArgs e)
        {
            //Timer tick event
            //Decrements the timer, and updates the timer display.
            timeLeft--;
            lblTurnTimeDisplay.Text = "(" + timeLeft + ")";

            //If we hit 0, we stop the timer and skip to the next player.
            if (timeLeft <= 0)
            {
                tmrTurnTimer.Stop();
                NextPlayer();
            }
        }

        public void ResetTimer()
        {
            //This function resets the timer, then restarts it
            timeLeft = 30;
            tmrTurnTimer.Start();
        }


        //Flags and win checking
        private void IncrementFlags()
        {
            //Increments flags and updates scoring display
            getPlayer(turnPlayer).flags++;
            if (turnPlayer == 1)
            {
                lblPlayer1Flags.Text = Convert.ToString(getPlayer(turnPlayer).flags);
            }
            else
            {
                lblPlayer2Flags.Text = Convert.ToString(getPlayer(turnPlayer).flags);
            }

            //Check if current player can bomb and update display accordingly
            if (getPlayer(1).canBomb(getPlayer(2)))
            {
                pctbxPlayer1BombAvailable.BackgroundImage = BOMB;
            }
            else
            {
                pctbxPlayer1BombAvailable.BackgroundImage = null;
            }

            //Check if opponent can bomb and update displau accordingly
            if (getPlayer(2).canBomb(getPlayer(1)))
            {
                pctbxPlayer2BombAvailable.BackgroundImage = BOMB;
            }
            else
            {
                pctbxPlayer2BombAvailable.BackgroundImage = null;
            }

            //Finally, we decrement the counter for number of total mines and update the corresponding display.
            mineCount--;
            lblMineCount.Text = $"{mineCount} mines remaining";
            if (CheckForWin()) { EndGame(); } //If there is a winner, we call the endgame function
        }

        public bool CheckForWin() {
            //After every flag placement, we check to see if it is possible for the losing player to regain a winning position.
            //If it is impossible, then we consider the leading player to have won.
            //This is found by seeing whether the difference between the two scores is bigger than the number of remaining mines. 
            if(player1.flags > player2.flags)
            { 
                var difference = player1.flags - player2.flags;
                if (mineCount < difference)
                {
                    return true;
                }
            }
            else
            { 
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
            //This is our end game function.
            //Stops the timer
            tmrTurnTimer.Stop();
            //Then checks to see who has won, and shows a messagebox
            if (player1.flags > player2.flags)
            {
                MessageBox.Show($"Player 1 wins!\nScore was {player1.flags}-{player2.flags}");
            }
            else { MessageBox.Show($"Player 2 wins!\nScore was {player1.flags}-{player2.flags}"); }
            //Once the messagebox is closed, we close the application.
            Application.Exit();

        }

        //Board functions
        private List<List<int>> generateMineBoard(int mineCount, int cellRow, int cellCol)
        {
            //Filling mine board with mines, and creating numbers
            List<List<int>> mineBoard = new List<List<int>>() { };

            List<Tuple<int, int>> cells = new List<Tuple<int, int>>();

            //Make list of possible cell locations
            for (int i = 0; i < boardRows; i++)
            {
                mineBoard.Add(new List<int>());

                for (int j = 0; j < boardCols; j++)
                {
                    mineBoard[i].Add(0);

                    if(i==cellRow && j==cellCol) { continue; }
                    cells.Add(new Tuple<int, int>(i,j));
                }
                
            }

            //Setup random
            Random rnd = new Random();

            for (int k = 0; k < mineCount; k++)
            {
                var coordNum = rnd.Next(0, cells.Count);
                Tuple<int,int> coord = cells[coordNum];
                int row = coord.Item1 ;
                int col = coord.Item2;

                //Prevent stacked mines
                if (mineBoard[row][col] == -1)
                {
                    k--;
                    continue;
                }

                mineBoard[row][col] = -1;
                cells.RemoveAt(coordNum);


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

        private bool CellInBoardRange(int row, int col)
        {
            //Checks whether given coord is in range of the grid
            if (row >= 0 && col >= 0 && row < boardRows && col < boardCols)
            {
                return true;
            }
            else
            {
                return false;
            };
        }

        private bool IsCellCleared(int cellRow, int cellCol)
        {
            //Checks whether the index is in the clearedCells list, from a given col/row
            return clearedCells.Contains(cellRow * boardCols + cellCol);
        }

        //Player functions
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
            //Changes to the next player
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
            //Update display to show the current turn player
            lblPlayerTurn.Text = $"Player {turnPlayer}'s turn";
            lblPlayerTurn.ForeColor = getPlayer(turnPlayer).color;
            lblTurnTimeDisplay.ForeColor = getPlayer(turnPlayer).color;
            //Stop the timer
            lblTurnTimeDisplay.Text = "(30)";
            tmrTurnTimer.Stop();
            //Messagebox to let players know of the turn change
            MessageBox.Show($"Player {turnPlayer}'s turn");
            //Once Msgbox is closed, we reset the timer
            ResetTimer();
            
        }

        //Click events and related
        private void Cell_Click(object sender, MouseEventArgs e)
        {
            //This event is called when a cell is clicked
            PictureBox cell = sender as PictureBox;

            // Find which button was pressed
            if (e.Button == MouseButtons.Left)
            {
                //If left click, we call the clearing function
                actionMineClear(cell);
            }
            else if (e.Button == MouseButtons.Right)
            {
                //If right click, we check whether bombing is an option
                if(getPlayer(turnPlayer).canBomb(getPlayer(turnPlayer, true)))
                {
                    //Then run the bomb function on the cell, and set the player's hasBombed value to true
                    actionBomb(cell);
                    getPlayer(turnPlayer).hasBombed = true;
                    //Bombing automaticaly switches to the next player.
                    NextPlayer();
                }
            }
        }

        private void actionMineClear(PictureBox cell)
        {
            //First we get our row and column
            var cellRow = tblpnlMineBoard.GetRow(cell);
            var cellCol = tblpnlMineBoard.GetColumn(cell);

            //initialise board (if not already)
            //passes first tiled clicked to ensure no instant flagging
            if (boardFilled != true) { board = generateMineBoard(mineCount, cellRow, cellCol); }

            //We first check that the cell hasn't already been cleared
            if (!IsCellCleared(cellRow, cellCol))
            {
                //Then we check what the tile is
                switch (board[cellRow][cellCol])
                {
                    case -1:
                        //MINE
                        //get the player's flag image
                        cell.BackgroundImage = getPlayer(turnPlayer).flagImage;
                        //add the index to the clearedCells list
                        clearedCells.Add(cellRow * boardCols + cellCol);
                        //call incrementFlags
                        IncrementFlags();
                        break;
                    case 0:
                        //EMPTY
                        //first place the 0 on the board
                        PlaceNumber(cellRow, cellCol, 0);
                        //then call the recursive ClearSurroundings function
                        ClearSurroundings(cellRow, cellCol);
                        //then go to next player
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
                        //place our number
                        PlaceNumber(cellRow, cellCol, board[cellRow][cellCol]);
                        //add to cleared cells
                        clearedCells.Add(cellRow * boardCols + cellCol);
                        //go to next player
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
            //get row and col
            var cellRow = tblpnlMineBoard.GetRow(cell);
            var cellCol = tblpnlMineBoard.GetColumn(cell);

            //loop through cell and surrounding cells
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    //find the new location
                    var newRow = cellRow + i;
                    var newCol = cellCol + j;
                    //if it hasn't been cleared
                    if (!IsCellCleared(newRow, newCol))
                    {
                        //if it is also in the board's range
                        if (CellInBoardRange(newRow, newCol))
                        {
                            //perform the right action
                            switch (board[newRow][newCol])
                            {
                                case -1:
                                    //MINE
                                    //auto places flags on mines
                                    cell = tblpnlMineBoard.Controls[boardCols * newRow + newCol] as PictureBox;
                                    cell.BackgroundImage = getPlayer(turnPlayer).flagImage;
                                    //adds to cleared cells then increments flags
                                    clearedCells.Add(newRow * boardCols + newCol);
                                    IncrementFlags();
                                    break;
                                case 0:
                                    //EMPTY
                                    //calls clearSurroundings on 0s
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
                                    //places number and adds to cleared cells
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
            //Recursive function
            //loops through surrounding cells
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var newRow = cellRow + i;
                    var newCol = cellCol + j;

                    //doesn't recurse deeper if the cell has already been cleared
                    if (!IsCellCleared(newRow, newCol))
                    {   //or if the new location is out of range
                        if (CellInBoardRange(newRow, newCol))
                        { 
                            //the recursive function is only called from a 0, which only appears if no surrounding cells are mines
                            //this means we don't have to check for mines, and only need check for 0s and 1-8
                            if (board[newRow][newCol] == 0)
                            {
                                //if we have another 0, we add the location to the cleared cells and place our 0
                                //before recursing
                                clearedCells.Add(newRow*boardCols +newCol);
                                PlaceNumber(newRow, newCol, 0);
                                ClearSurroundings(newRow, newCol);

                            }
                            else {
                                //otherwise we add to cleared cells and just place the number
                                clearedCells.Add(newRow * boardCols + newCol); 
                                PlaceNumber(newRow, newCol, board[newRow][newCol]); }
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
    }

    //Player class
    public class Player
    {
        //Player class, for managing player values such as flags, images and bomb status
        public int flags;
        public bool hasBombed = false;
        public Color color = Color.Gray;
        public Image flagImage = null;

        public Player(Color color, Image flagImage)
        {
            //Initialise Player class
            this.color = color;
            this.flagImage = flagImage;
        }

        public bool canBomb(Player opponent)
        {
            //returns whether the player can bomb
            //to be able to bomb, the player must have less flags than their opponent and have not bombed already.
            if (!hasBombed && this.flags < opponent.flags)
            {
                return true;
            }
            return false; 
        }
    }

}
