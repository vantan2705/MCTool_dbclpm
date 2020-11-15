using Emgu.CV;
using Emgu.CV.Structure;
using MCTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerSheetProcess
{
    class ASProcessor
    {
        private static void ValidateTopRightFlagRect(Rect rect)
        {
            if (rect.topLeft.x > Constant.AS_TOP_BOTTOM_RIGHT_Y_MAX || rect.topLeft.x < Constant.AS_TOP_BOTTOM_RIGHT_Y_MIN) throw new Exception(ErrorUtils.getError(ErrorUtils.TOP_RIGHT_FLAG_RECT_NOT_VALID, "Can not find the top right flag rectangle"));
        }

        private static void ValidateBottomRightFlagRect(Rect rect)
        {
            if (rect.topLeft.x > Constant.AS_TOP_BOTTOM_RIGHT_Y_MAX || rect.topLeft.x < Constant.AS_TOP_BOTTOM_RIGHT_Y_MIN) throw new Exception(ErrorUtils.getError(ErrorUtils.TOP_RIGHT_FLAG_RECT_NOT_VALID, "Can not find the top right flag rectangle"));
        }

        private static Rect DetectTopRightFlagRect(Bitmap image)
        {
            //In case the points expanded not match the template rectangle. We need to mark the visited point, so we don't go to these point again
            var marker = new bool[image.Width, image.Height];
            Point p = CommonImageProcessing.FindDifferentColorPoint(new Point(Constant.AS_RIGHT_BORDER, Constant.AS_TOP_BORDER), image, Constant.WHITE, ref marker);
            Rect topRightFlagRect = CommonImageProcessing.ExpandTheRectBlackPixel(p, image, ref marker);
            
            if (topRightFlagRect == null) {
                p = CommonImageProcessing.FindDifferentColorPoint(new Point(Constant.AS_RIGHT_BORDER, Constant.AS_TOP_BORDER), image, Constant.WHITE, ref marker);
                topRightFlagRect = CommonImageProcessing.ExpandTheRectBlackPixel(p, image, ref marker);
                if (topRightFlagRect == null)
                {
                    throw new Exception(ErrorUtils.getError(ErrorUtils.CAN_NOT_FIND_THE_TOP_RIGHT_FLAG_RECT, "Can not detect the top right flag rectangle. Tried 2 attempts!!"));
                }
                else
                {
                    ValidateTopRightFlagRect(topRightFlagRect);
                    return topRightFlagRect;
                }
            } else {
                ValidateTopRightFlagRect(topRightFlagRect);
                return topRightFlagRect;
            }
        }

        private static Rect DetectBottomRightCorner(Bitmap image)
        {
            var marker = new bool[image.Width, image.Height];
            Point p = CommonImageProcessing.FindDifferentColorPoint(new Point(Constant.AS_RIGHT_BORDER, Constant.AS_BOTTOM_BORDER), image, Constant.WHITE, ref marker);
            Rect bottomRightRect = CommonImageProcessing.ExpandTheRectBlackPixel(p, image, ref marker);

            if (bottomRightRect == null)
            {
                p = CommonImageProcessing.FindDifferentColorPoint(new Point(Constant.AS_RIGHT_BORDER, Constant.AS_BOTTOM_BORDER), image, Constant.WHITE, ref marker);
                bottomRightRect = CommonImageProcessing.ExpandTheRectBlackPixel(p, image, ref marker);
                if (bottomRightRect == null)
                {
                    throw new Exception(ErrorUtils.getError(ErrorUtils.CAN_NOT_FIND_THE_TOP_RIGHT_FLAG_RECT, "Can not detect the top right flag rectangle. Tried 2 attempts!!"));
                }
                else
                {
                    ValidateBottomRightFlagRect(bottomRightRect);
                    return bottomRightRect;
                }
            }
            else
            {
                ValidateBottomRightFlagRect(bottomRightRect);
                return bottomRightRect;
            }
        }
        static int i = 0;

        private static Bitmap PreProcess(Bitmap img)
        {
            i++;
            //img.Save(@"D:\hello.jpg");
            img = CommonImageProcessing.SmoothImage(img);
            //img.Save(@"D:\hello_smooth.jpg");
            img = CommonImageProcessing.ConvertToBinary(img);
            img.Save(@"D:\hello_smooth_black1.jpg");
            Rect topRightRect = DetectTopRightFlagRect(img);
            Rect bottomRightRect = DetectBottomRightCorner(img);
            double angle = calculateRotateAngle(topRightRect, bottomRightRect);
            if (Math.Abs(topRightRect.bottomRight.x - bottomRightRect.bottomRight.x) > Constant.ACCEPTABLE_DIFF_BETWEEN_TOP_RIGHT_FLG_RECT_AND_BOTTOM_RIGHT_FLG_RECT)
            {
                img = CommonImageProcessing.RotateImage(img, angle-90);
            }
            img.Save(@"D:\" + i + ".jpg");
            
            return img;
        }

        private static double calculateRotateAngle(Rect topRightRect, Rect bottomRightRect)
        {
            Point upper = topRightRect.GetCenterPoint();
            Point lower = bottomRightRect.GetCenterPoint();
            double angle = Math.Atan2(lower.y - upper.y, upper.x - lower.x);
            return (180.0 * angle) / ((double)3.1415);
        }

        private static Tuple<String, List<Circle>> DetectStudentCode(Bitmap img, List<Rect> horizontalFlgRects, List<Rect> verticalFlgRects)
        {
            string studentId = "";
            List<Circle> lstCircles = new List<Circle>();
            Circle c;
            bool found;
            
            for (int i = 4; i < 10; i++)
            {
                found = false;
                for (int j = 1; j < 11; j++)
                {
                    Rect r1 = horizontalFlgRects[i];
                    Rect r2 = verticalFlgRects[j];
                    Point c1 = r1.GetCenterPoint();
                    Point c2 = r2.GetCenterPoint();
                    c = new Circle(new Point(c1.x, c2.y), Constant.DEFAULT_CIRCLE_RADIUS);
                    int nOfBlackInCircle = CommonImageProcessing.CountNumberOfBlackPixelInCircle(img, c, -5);
                    if (nOfBlackInCircle >= Constant.NUMBER_OF_CIRCLE_BLACK_PIXEL_TO_BE_CONSIDERED_INFO_AS_CHOOSEN)
                    {
                        if (found)
                        {
                            studentId = "-";
                        }
                        else
                        {
                            found = true;
                            if (studentId != "-")
                            {
                                studentId = (j - 1).ToString() + studentId;
                            }
                        }
                        lstCircles.Add(c);
                    }
                }
                if (!found)
                {
                    studentId = studentId + "*";
                }
            }
            return new Tuple<string, List<Circle>>(studentId, lstCircles);
        }

        private static Tuple<String, List<Circle>> DetectExamCode(Bitmap img, List<Rect> horizontalFlgRects, List<Rect> verticalFlgRects)
        {
            string examId = "";
            List<Circle> lstCircles = new List<Circle>();
            Circle c;
            bool found;

            for (int i = 1; i < 4; i++)
            {
                found = false;
                for (int j = 1; j < 11; j++)
                {
                    Rect r1 = horizontalFlgRects[i];
                    Rect r2 = verticalFlgRects[j];
                    Point c1 = r1.GetCenterPoint();
                    Point c2 = r2.GetCenterPoint();
                    c = new Circle(new Point(c1.x, c2.y), Constant.DEFAULT_CIRCLE_RADIUS);
                    int nOfBlackInCircle = CommonImageProcessing.CountNumberOfBlackPixelInCircle(img, c, -5);
                    if (nOfBlackInCircle >= Constant.NUMBER_OF_CIRCLE_BLACK_PIXEL_TO_BE_CONSIDERED_INFO_AS_CHOOSEN)
                    {
                        if (found)
                        {
                            examId = "-";
                        }
                        else
                        {
                            found = true;
                            if (examId != "-")
                            {
                                examId = (j - 1).ToString() + examId;
                            }
                        }
                        lstCircles.Add(c);
                    }
                }
                if (!found)
                {
                    examId = examId + "*";
                }
            }
            return new Tuple<string, List<Circle>>(examId, lstCircles);
        }

        private static Tuple<List<Char>, List<Circle>, List<Point>> DetectStudentAnswer(Bitmap img, List<Rect> horizontalFlgRects, List<Rect> verticalFlgRects)
        {
            int numberOfQuestion = Globals.numberOfQuestion;

            List<Char> answers = new List<Char>();
            List<Circle> lstCircles = new List<Circle>();
            List<Point> borderPoints = new List<Point>();

            for (int i = 0; i < numberOfQuestion + 1; i++)
            {
                answers.Add('*');
                borderPoints.Add(new Point(-100, -100));
            }
            
            int nHorizontalRect = horizontalFlgRects.Count;
            int nVerticalRect = verticalFlgRects.Count;

            Char currentAns = 'A';
            int currentQuestion;
            int colStartQuestion = 1;
            int numberOfRow = (nVerticalRect - 12);
            Circle c;
            for (int i = nHorizontalRect-1; i >= 1; i--)
            {
                currentQuestion = colStartQuestion;
                for (int j = 11; j < nVerticalRect - 1; j++)
                {
                    if (currentQuestion > numberOfQuestion) { break; }
                    Rect r1 = horizontalFlgRects[i];
                    Rect r2 = verticalFlgRects[j];
                    Point c1 = r1.GetCenterPoint();
                    Point c2 = r2.GetCenterPoint();
                    if (currentAns == 'A')
                    {
                        borderPoints[currentQuestion] = new Point(c1.x, c2.y);
                    }
                    c = new Circle(new Point(c1.x, c2.y), Constant.DEFAULT_CIRCLE_RADIUS);
                    int nOfBlackInCircle = CommonImageProcessing.CountNumberOfBlackPixelInCircle(img, c, 8);
                    if (nOfBlackInCircle >= Constant.NUMBER_OF_CIRCLE_BLACK_PIXEL_TO_BE_CONSIDERED_ANSWER_AS_CHOOSEN)
                    {
                        if (answers[currentQuestion] == '*')
                        {
                            answers[currentQuestion] = currentAns;
                        }
                        else
                        {
                            answers[currentQuestion] = '-';
                        }
                        lstCircles.Add(c);
                    }
                    currentQuestion++;
                    if (currentQuestion > numberOfQuestion) { break; }
                }
                if (currentAns == 'E')
                {
                    currentAns = 'A';
                    colStartQuestion += numberOfRow;
                }
                else
                {
                    currentAns++;
                }
            }
            return new Tuple<List<char>, List<Circle>, List<Point>>(answers, lstCircles, borderPoints);
        }

        private static List<Rect> GetVerticalFlagRect(Rect aboveFlgRect, Bitmap image)
        {
            List<Rect> result = new List<Rect>();
            Rect rect;
            bool[,] marker = new bool[image.Width, image.Height];
            int y = aboveFlgRect.GetCenterPoint().y;
            int x = aboveFlgRect.GetCenterPoint().x;
            int totalFlgRect = TemplateUtils.GetNumberOfVerticalFLagRect(Globals.currentTemplate);
            Point p;
            while (result.Count < totalFlgRect)
            {
                p = new Point(x, y);
                if (!p.IsValidPoint())
                {
                    throw new Exception(ErrorUtils.getError(ErrorUtils.CAN_NOT_GET_ENOUGH_VERTICAL_FLAG_RECT, "Can not get enough vertical flag rect!!"));
                }
                if (!marker[x, y] && CommonImageProcessing.IsBlackColor(new Point(x, y), image))
                {
                    rect = CommonImageProcessing.ExpandTheRectBlackPixel(new Point(x, y), image, ref marker);
                    if (rect == null)
                    {
                        throw new Exception(ErrorUtils.getError(ErrorUtils.CAN_NOT_GET_ENOUGH_VERTICAL_FLAG_RECT, "Can not get enough vertical flag rect!!"));
                    }
                    if (CommonImageProcessing.IsFlagRectangle(rect))
                    {
                        result.Add(rect);
                    }
                }
                y++;
            }
            return result;
        }

        private static List<Rect> GetAboveHorizontalFlagRect(Rect aboveFlgRect, Bitmap image)
        {
            List<Rect> result = new List<Rect>();
            Rect rect;
            bool[,] marker = new bool[image.Width, image.Height];
            int y = aboveFlgRect.GetCenterPoint().y;
            int x = aboveFlgRect.GetCenterPoint().x;
            int totalFlgRect = TemplateUtils.GetNumberOfHorizontalFLagRect(Constant.DEV_CURRENT_TEMPLATE);
            Point p;
            while (result.Count < totalFlgRect)
            {
                p = new Point(x, y);
                if (!p.IsValidPoint())
                {
                    throw new Exception(ErrorUtils.getError(ErrorUtils.CAN_NOT_GET_ENOUGH_HORIZONTAL_FLAG_RECT, "Can not get enough horizontal flag rect!!"));
                }
                if (!marker[x, y] && CommonImageProcessing.IsBlackColor(new Point(x, y), image))
                {
                    rect = CommonImageProcessing.ExpandTheRectBlackPixel(new Point(x, y), image, ref marker);
                    if (rect == null)
                    {
                        throw new Exception(ErrorUtils.getError(ErrorUtils.CAN_NOT_GET_ENOUGH_VERTICAL_FLAG_RECT, "Can not get enough horizontal flag rect!!"));
                    }
                    if (CommonImageProcessing.IsFlagRectangle(rect))
                    {
                        result.Add(rect);
                    }
                }
                x--;
            }
            return result;
        }

        private static List<Rect> GetBelowHorizontalFlagRect(Rect belowFlgRect, Bitmap image)
        {
            List<Rect> result = new List<Rect>();
            Rect rect;
            bool[,] marker = new bool[image.Width, image.Height];
            int y = belowFlgRect.GetCenterPoint().y;
            int x = belowFlgRect.GetCenterPoint().x;
            int totalFlgRect = Constant.BELOW_HORIZONTAL_FLAG_RECT;
            Point p;
            while (result.Count < totalFlgRect)
            {
                p = new Point(x, y);
                if (!p.IsValidPoint())
                {
                    throw new Exception(ErrorUtils.getError(ErrorUtils.CAN_NOT_GET_ENOUGH_HORIZONTAL_FLAG_RECT, "Can not get enough horizontal flag rect!!"));
                }
                if (!marker[x, y] && CommonImageProcessing.IsBlackColor(new Point(x, y), image))
                {
                    rect = CommonImageProcessing.ExpandTheRectBlackPixel(new Point(x, y), image, ref marker);
                    if (rect == null)
                    {
                        throw new Exception(ErrorUtils.getError(ErrorUtils.CAN_NOT_GET_ENOUGH_HORIZONTAL_FLAG_RECT, "Can not get enough horizontal flag rect!!"));
                    }
                    if (CommonImageProcessing.IsFlagRectangle(rect))
                    {
                        result.Add(rect);
                    }
                }
                x--;
            }
            return result;
        }
        public static ProcessedAS ProcessAS(Bitmap img)
        {
            if (img.Width != Constant.AS_WIDTH || img.Height != Constant.AS_HEIGHT)
            {
                img = CommonImageProcessing.ResizeImage(img, Constant.AS_WIDTH, Constant.AS_HEIGHT);
            }
            Bitmap orgImg = img;
            img = PreProcess(img);

            Rect topRightRect = DetectTopRightFlagRect(img);
            Rect bottomRightRect = DetectBottomRightCorner(img);

            List<Rect> verticalFlagRects = GetVerticalFlagRect(topRightRect, img);
            List<Rect> aboveHorizontalFlagRects = GetAboveHorizontalFlagRect(topRightRect, img);
            List<Rect> belowHorizontalFlagRects = GetBelowHorizontalFlagRect(bottomRightRect, img);

            Tuple<String, List<Circle>> studentCodeResult = DetectStudentCode(img, belowHorizontalFlagRects, verticalFlagRects);
            Tuple<String, List<Circle>> examCodeResult = DetectExamCode(img, belowHorizontalFlagRects, verticalFlagRects);
            Tuple<List<Char>, List<Circle>, List<Point>> studentAnswersResult = DetectStudentAnswer(img, aboveHorizontalFlagRects, verticalFlagRects);

            String examId = examCodeResult.Item1;
            String studentId = studentCodeResult.Item1;

            ProcessedAS processedAS = new ProcessedAS();
            processedAS.examId = examId;
            processedAS.studentId = studentId;

            StudentAnswer studentAnswer;

            int numberOfQuestion = Globals.numberOfQuestion;

            List<Char> answerText = studentAnswersResult.Item1;
            List<Point> positions = studentAnswersResult.Item3;

            List<StudentAnswer> studentAnswers = new List<StudentAnswer>();

            for (int i = 1; i <= numberOfQuestion; i++)
            {
                Point p = positions[i];
                // 300 x 38
                // CvInvoke.Rectangle(outputImg, rect, new MCvScalar(255, 0, 255), 2);
                studentAnswer = new StudentAnswer(answerText[i], new Point(p.x - 45, p.y - 18));
                studentAnswers.Add(studentAnswer);
            }

            processedAS.answers = studentAnswers;

            //List<Circle> tmp;
            //tmp = studentCodeResult.Item2;
            //Image<Bgr, Byte> outputImg = new Image<Bgr, Byte>(img);
            //foreach (Circle circle in tmp)
            //{
            //    CvInvoke.Circle(outputImg, new System.Drawing.Point(circle.center.x, circle.center.y), circle.radius - 2, new MCvScalar(0, 255, 0), 2);
            //}

            //tmp = examCodeResult.Item2;

            //foreach (Circle circle in tmp)
            //{
            //    CvInvoke.Circle(outputImg, new System.Drawing.Point(circle.center.x, circle.center.y), circle.radius - 2, new MCvScalar(0, 255, 0), 2);
            //}

            //tmp = studentAnswersResult.Item2;

            //foreach (Circle circle in tmp)
            //{
            //    CvInvoke.Circle(outputImg, new System.Drawing.Point(circle.center.x, circle.center.y), circle.radius-2, new MCvScalar(0, 255, 0), 2);
            //}

            processedAS.processedAnswerSheetImage = orgImg;
            
            return processedAS;
        } 
    }
}
