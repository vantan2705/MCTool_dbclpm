using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnswerSheetProcess
{
    public class Rect
    {
        public Point topLeft { get; set; }
        public Point bottomRight { get; set; }

        public Rect()
        {

        }

        public Rect(Point topLeft, Point bottomRight)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }

        public Rect(int x1, int y1, int x2, int y2)
        {
            this.topLeft = new Point(x1, y1);
            this.bottomRight = new Point(x2, y2);
        }

        public Point GetCenterPoint()
        {
            return new Point((topLeft.x + bottomRight.x) / 2, (topLeft.y + bottomRight.y) / 2);
        }

    }
}
