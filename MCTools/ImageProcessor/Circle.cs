using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerSheetProcess
{
    public class Circle
    {
        public Point center { get; set; }
        public int radius { get; set; }

        public Circle()
        {

        }

        public Circle(Point center, int radius)
        {
            this.center = center;
            this.radius = radius;
        }

    }
}
