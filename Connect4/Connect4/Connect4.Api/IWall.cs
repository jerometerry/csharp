using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Connect4.Api
{
    public interface IWall
    {
        Size Size { get; }
        DiskColour Get(Point point);
        void Push(int column, DiskColour colour);
    }
}
