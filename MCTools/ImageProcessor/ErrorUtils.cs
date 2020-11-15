using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerSheetProcess
{
    public class ErrorUtils
    {
        public const int CAN_NOT_FIND_DIFF_COLOR = 1000;
        public const int CAN_NOT_FIND_THE_RECTANGLE_CORNER = 1001;
        public const int CAN_NOT_FIND_THE_TOP_RIGHT_FLAG_RECT = 1002;
        public const int TOP_RIGHT_FLAG_RECT_NOT_VALID = 1003;

        public const int NULL_ON_REQUIRE_NOT_NULL_OBJ = 4000;

        public const int CAN_NOT_GET_ENOUGH_VERTICAL_FLAG_RECT = 5000;
        public const int CAN_NOT_GET_ENOUGH_HORIZONTAL_FLAG_RECT = 5000;

        public static string getError(int errorCode, string message)
        {
            return string.Format("[{0}] {1}", errorCode, message);
        }
    }
}
