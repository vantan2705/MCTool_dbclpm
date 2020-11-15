using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTools
{
    class Constant
    {
        public const int AS_WIDTH = 1653;
        public const int AS_HEIGHT = 2338;

        public const int AS_RIGHT_BORDER = 1640;
        public const int AS_TOP_BORDER = 5;
        public const int AS_BOTTOM_BORDER = 2330;
        public const int AS_LEFT_BORDER = 5;

        public const int AS_TOP_BOTTOM_RIGHT_Y_MIN = 1500;
        public const int AS_TOP_BOTTOM_RIGHT_Y_MAX = 1640;

        public const int A = 0;
        public const int B = 1;
        public const int C = 2;
        public const int D = 3;
        public const int E = 4;


        public const int BLACK = 0;
        public const int WHITE = 1;

        public const int INF = 1000000000;

        public const int FLAG_RECT_LOWER_AREA = 80;
        public const int FLAG_RECT_UPPER_AREA = 220;

        public const int NUMBER_OF_CIRCLE_BLACK_PIXEL_TO_BE_CONSIDERED_ANSWER_AS_CHOOSEN = 510;
        public const int NUMBER_OF_CIRCLE_BLACK_PIXEL_TO_BE_CONSIDERED_INFO_AS_CHOOSEN = 320;

        public const int ACCEPTABLE_DIFF_BETWEEN_TOP_RIGHT_FLG_RECT_AND_BOTTOM_RIGHT_FLG_RECT = 2;

        public const int ACCEPTABLE_FLAG_RECT_SMALLER_EDGE_MIN = 6;
        public const int ACCEPTABLE_FLAG_RECT_SMALLER_EDGE_MAX = 14;

        public const int ACCEPTABLE_FLAG_RECT_BIGGER_EDGE_MIN = 9;
        public const int ACCEPTABLE_FLAG_RECT_BIGGER_EDGE_MAX = 17;

        public const int MAXIMUM_DIST_BETWEEN_VERTICAL_FLG_RECT = 80;
        public const int EXPECTED_DIST_BETWEEN_VERTICAL_FLG_RECT = 55;

        public const int TEMPLATE_45_QUESTIONS = 0;
        public const int TEMPLATE_60_QUESTIONS = 1;
        public const int TEMPLATE_80_QUESTIONS = 2;
        public const int TEMPLATE_100_QUESTIONS = 3;

        public const int BELOW_HORIZONTAL_FLAG_RECT = 10;


        public const string DEV_OUTPUT_PATH = @"D:\graduation thesis\templates\Detected\Ans_sheet(60).jpg";
        public const string DEV_INPUT_PATH = @"D:\graduation thesis\templates\Answered\Ans_sheet(45).jpg";
        public const int DEV_CURRENT_TEMPLATE = TEMPLATE_45_QUESTIONS;


        public const int DRAW_RECT_ON_ANSWER = 1;
        public const int DRAW_RECT_ON_EXAM_ID = 2;
        public const int DRAW_RECT_ON_STUDENT_ID = 3;

        public const int DEFAULT_CIRCLE_RADIUS = 16;


       


    }
}
