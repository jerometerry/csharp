using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Connect4.Api
{
    public class Wall : IWall
    {
        public Size Size { get; private set; }
        private DiskColour[,] Disks { get; set; }

        public Wall(Size size)
        {
            this.Size = size;
            this.Disks = new DiskColour[size.Width, size.Height];
        }

        public DiskColour Get(Point point)
        {
            return this.Disks[point.X, point.Y];
        }

        public void Push(int column, DiskColour colour)
        {
            int row = GetNextEmptyRow(column);
            this.Disks[column, row] = colour;
        }

        private int GetNextEmptyRow(int column)
        {
            return 0;
        }
    }
}
