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
        public MinesweeperFlags()
        {
            InitializeComponent();
        }

        private void MinesweeperFlags_Load(object sender, EventArgs e)
        {
            int rowCount = tblpnlMineBoard.RowCount;
            int colCount = tblpnlMineBoard.ColumnCount;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++) {

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

        private void Cell_Click(object sender, MouseEventArgs e)
        {
            PictureBox cell = sender as PictureBox;
            int rowNum = tblpnlMineBoard.GetRow(cell);
            int colNum = tblpnlMineBoard.GetColumn(cell);

            // handle click event
            if (e.Button == MouseButtons.Left)
            {
                cell.BackgroundImage = Resources.TILE_CLEAR;
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
    }
}
