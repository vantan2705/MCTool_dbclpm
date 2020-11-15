using MCTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnswerSheetProcess
{
    public class Point
    {

        public int x { get; set; }
        public int y { get; set; }

        public Point()
        {

        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsValidPoint()
        {
            return (this.x > Constant.AS_LEFT_BORDER && this.y > Constant.AS_TOP_BORDER && this.x < Constant.AS_RIGHT_BORDER && this.y < Constant.AS_BOTTOM_BORDER);
        }

        public Point AbovePoint(int step = 1)
        {
            return new Point(x, y - step);
        }

        public Point BelowPoint(int step = 1)
        {
            return new Point(x, y + step);
        }

        public Point LeftPoint(int step = 1)
        {
            return new Point(x - step, y);
        }

        public Point RightPoint(int step = 1)
        {
            return new Point(x + step, y);
        }

        public Point TopLeftPoint(int step = 1)
        {
            return new Point(x - step, y - step);
        }

        public Point TopRightPoint(int step = 1)
        {
            return new Point(x + step, y - step);
        }

        public Point BottomLeftPoint(int step = 1)
        {
            return new Point(x - step, y + step);
        }

        public Point BottomRightPoint(int step = 1)
        {
            return new Point(x + step, y + step);
        }

        public List<Point> GetAvailableMoves(int step = 1)
        {
            List<Point> adjPoints = new List<Point>();
            Point top = AbovePoint(step); if (top.IsValidPoint()) adjPoints.Add(top);
            Point bottom = BelowPoint(step); if (bottom.IsValidPoint()) adjPoints.Add(bottom);
            Point left = LeftPoint(step); if (left.IsValidPoint()) adjPoints.Add(left);
            Point right = RightPoint(step); if (right.IsValidPoint()) adjPoints.Add(right);
            Point topLeft = TopLeftPoint(step); if (topLeft.IsValidPoint()) adjPoints.Add(topLeft);
            Point topRight = TopRightPoint(step); if (topRight.IsValidPoint()) adjPoints.Add(topRight);
            Point bottomLeft = BottomLeftPoint(step); if (bottomLeft.IsValidPoint()) adjPoints.Add(bottomLeft);
            Point bottomRight = BottomRightPoint(step); if (bottomRight.IsValidPoint()) adjPoints.Add(bottomRight);
            return adjPoints;        
        }

        public int distanceTo(Point p)
        {
            return Convert.ToInt32(Math.Sqrt((p.x - this.x) * (p.x - this.x) + (p.y - this.y) * (p.y - this.y)));
        }

    }
}
