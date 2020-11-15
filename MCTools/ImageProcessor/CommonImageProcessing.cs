using Emgu.CV;
using Emgu.CV.Structure;
using MCTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerSheetProcess
{
    public class CommonImageProcessing
    {
        public static int getPixelColor(Point src, Bitmap image) {
            return (image.GetPixel(src.x, src.y).R < 10 && image.GetPixel(src.x, src.y).G < 10 && image.GetPixel(src.x, src.y).B < 10) ? 0 : 1;
        }
        public static bool IsBlackColor(Point src, Bitmap image) {
            return IsTheSameColor(src, image, Constant.BLACK);
        }

        public static bool IsTheSameColor(Point src, Bitmap image, int color)
        {
            return getPixelColor(src, image) == color;
        }

        public static bool IsTopLeftPointOfRect(Point p, Bitmap image)
        {
            Point below = p.BelowPoint();
            Point right = p.RightPoint();
            Point above = p.AbovePoint();
            Point left = p.LeftPoint();
            return (IsBlackColor(below, image) && IsBlackColor(right, image) && !IsBlackColor(above, image) && !IsBlackColor(left, image));
        }

        public static bool IsBottomRightPointOfRect(Point p, Bitmap image)
        {
            Point below = p.BelowPoint();
            Point right = p.RightPoint();
            Point above = p.AbovePoint();
            Point left = p.LeftPoint();
            return (IsBlackColor(left, image) && IsBlackColor(above, image) && !IsBlackColor(right, image) && !IsBlackColor(below, image));
        }

        public static bool IsBottomLeftPointOfRect(Point p, Bitmap image)
        {
            Point below = p.BelowPoint();
            Point right = p.RightPoint();
            Point above = p.AbovePoint();
            Point left = p.LeftPoint();
            return (IsBlackColor(right, image) && IsBlackColor(above, image) && !IsBlackColor(below, image) && !IsBlackColor(left, image));
        }

        public static bool IsTopRightPointOfRect(Point p, Bitmap image)
        {
            Point below = p.BelowPoint();
            Point right = p.RightPoint();
            Point above = p.AbovePoint();
            Point left = p.LeftPoint();
            return (IsBlackColor(left, image) && IsBlackColor(below, image) && !IsBlackColor(above, image) && !IsBlackColor(right, image));
        }

        public static Bitmap SmoothImage(Bitmap image)
        {
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(image);
            CvInvoke.GaussianBlur(img, img, new Size(1, 1),0, 0);
            return img.ToBitmap();
        }

        public static Bitmap ConvertToBinary(Bitmap image)
        {
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(image);
            Image<Gray, Byte> grayIMG = img.Convert<Gray, Byte>();
            CvInvoke.Threshold(grayIMG, grayIMG, 230, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
            return grayIMG.ToBitmap();
        }

        public static Bitmap ResizeImage(Bitmap image, int newWidth, int newHeight)
        {
            //Image<Bgr, Byte> img = new Image<Bgr, Byte>(image);
            //CvInvoke.Resize(img, img, new Size(newWidth, newHeight), 0, 0, Emgu.CV.CvEnum.Inter.Linear);
            //return img.ToBitmap();

            Bitmap b = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage((Image)b);

            g.DrawImage(image, 0, 0, newWidth, newHeight);
            g.Dispose();

            return b;
        }

        //public static List<Point> ExpandTheBlackPixel(Point src, Bitmap image)
        //{
        //    var marker = new bool[image.Height, image.Width];
        //    var points = new List<Point>();
        //    var stack = new Stack<Point>();
        //    Point top;
        //    stack.Push(src);
        //    marker[src.x, src.y] = true;
        //    while (stack.Count != 0)
        //    {
        //        top = stack.Pop();
        //        points.Add(top);
                
                
        //        List<Point> availableMoves = top.GetAvailableMoves();

        //        foreach (Point point in availableMoves)
        //        {
        //            if (!marker[point.x, point.y] && IsBlackColor(point, image))
        //            {
        //                marker[point.x, point.y] = true;
        //                stack.Push(point);
        //            }
        //        }
        //    }
        //    return points;
        //}

        //Flag Rectangle is a Rect with size 14x10
        //We can assume a rect is flag rect when its area between FLAG_RECT_LOWER_AREA and FLAG_RECT_UPPER_AREA
        public static bool IsFlagRectangle(Rect rect)
        {
            int smallerEdge = rect.bottomRight.x - rect.topLeft.x;
            int biggerEdge = rect.bottomRight.y - rect.topLeft.y;

            int tmp;
            if (smallerEdge > biggerEdge) {
                tmp = smallerEdge;
                smallerEdge = biggerEdge;
                biggerEdge = tmp;
            }

            if (smallerEdge > Constant.ACCEPTABLE_FLAG_RECT_SMALLER_EDGE_MAX || smallerEdge < Constant.ACCEPTABLE_FLAG_RECT_SMALLER_EDGE_MIN)
            {
                return false;
            }
            if (biggerEdge > Constant.ACCEPTABLE_FLAG_RECT_BIGGER_EDGE_MAX || biggerEdge < Constant.ACCEPTABLE_FLAG_RECT_BIGGER_EDGE_MIN)
            {
                return false;
            }

            return true;
        }

        public static int CountNumberOfBlackPixelInCircle(Bitmap image, Circle circle, int range)
        {
            Point src = circle.center;
            var marker = new bool[image.Width, image.Height];
            var points = new List<Point>();
            var stack = new Stack<Point>();
            Point top;
            bool foundBlackPoint = true;
            if (!IsBlackColor(src, image))
            {
                foundBlackPoint = false;
                List<Point> availPs = src.GetAvailableMoves();
                foreach(Point p in availPs) {
                    foreach (Point q in p.GetAvailableMoves())
                    {
                        if (IsBlackColor(q, image))
                        {
                            src = q;
                            foundBlackPoint = true;
                            break;
                        }
                    }
                }
            }
            if (!foundBlackPoint)
            {
                return 0;
            }
            stack.Push(src);
            marker[src.x, src.y] = true;
            
            int nOfBlackPixel = 0;
            while (stack.Count != 0)
            {
                top = stack.Pop();
                points.Add(top);
                nOfBlackPixel++;

                List<Point> availableMoves = top.GetAvailableMoves();

                foreach (Point point in availableMoves)
                {
                    if (!marker[point.x, point.y] && IsBlackColor(point, image) && point.distanceTo(src) <= circle.radius+range)
                    {
                        marker[point.x, point.y] = true;
                        stack.Push(point);
                    }
                }
            }
            return nOfBlackPixel;
        }

        
        //Expanded the black pixel but return the rectangle object
        //Used to recognize the rectangle that specified for the answer on AS
        //In case the shape expanded not match the Flag Rect - return null
        public static Rect ExpandTheRectBlackPixel(Point src, Bitmap image, ref bool[,] marker)
        {
            if (marker == null)
            {
                marker = new bool[image.Width, image.Height];
            }
            Point topLeft = null, bottomRight = null;
            var queue = new Queue<Point>();
            Point top;
            int numberOfPoints = 0;
            queue.Enqueue(src);
            marker[src.x, src.y] = true;
            topLeft = new Point(src.x, src.y);
            bottomRight = new Point(src.x, src.y);
            while (queue.Count != 0)
            {
                top = queue.Dequeue();
                
                numberOfPoints++;

                if (numberOfPoints > Constant.FLAG_RECT_UPPER_AREA)
                {
                    return null;
                }
                topLeft.x = Math.Min(topLeft.x, top.x);
                topLeft.y = Math.Min(topLeft.y, top.y);
                bottomRight.x = Math.Max(bottomRight.x, top.x);
                bottomRight.y = Math.Max(bottomRight.y, top.y);

                List<Point> availableMoves = top.GetAvailableMoves();

                foreach (Point point in availableMoves) 
                {
                    if (!marker[point.x, point.y] && IsBlackColor(point, image))
                    {
                        marker[point.x, point.y] = true;
                        queue.Enqueue(point);
                    }
                }
            }
            if (bottomRight == null || topLeft == null)
            {
                throw new Exception(ErrorUtils.getError(ErrorUtils.CAN_NOT_FIND_THE_RECTANGLE_CORNER, "Can not find the flag rectangle corner"));
            }
            Rect result = new Rect(topLeft, bottomRight);
            if (!IsFlagRectangle(result))
            {
                return null;
            }
            return new Rect(topLeft, bottomRight);
        }

        //BFS and find the first point that different to the specific color
        //Used to detect the top right and bottom right flag rectangle
        public static Point FindDifferentColorPoint(Point src, Bitmap image, int color, ref bool[,] marker)
        {
            if (marker == null)
            {
                marker = new bool[image.Height, image.Width];
            }
            Point top;
            Point res = new Point();
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(src);
            marker[src.x, src.y] = true; 
            while (queue.Count != 0)
            {
                top = queue.Dequeue();
                if (!IsTheSameColor(top, image, color))
                {
                    return top;
                }

                List<Point> availableMoves = top.GetAvailableMoves();

                foreach (Point point in availableMoves)
                {
                    if (!marker[point.x, point.y])
                    {
                        marker[point.x, point.y] = true; 
                        queue.Enqueue(point);
                    }
                }
            }
            throw new Exception(ErrorUtils.getError(ErrorUtils.CAN_NOT_FIND_DIFF_COLOR, "Can not find the pixel with diferrent color"));
        }

        public static Bitmap RotateImage(Bitmap img, double angle)
        {
            Image<Gray, Byte> image = new Image<Gray, Byte>(img);
            image = image.Rotate(angle, new Gray(255), true);
            return image.ToBitmap();
        }
    }
}
