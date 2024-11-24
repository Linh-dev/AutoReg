namespace ToolRegGoethe.Utilities
{
    public static class DateUtil
    {
        public static DateTime? Parse(string dateStr, string dateFormat = "MM/dd/yyyy")
        {
            try
            {
                //return DateTime.SpecifyKind(DateTime.ParseExact(dateStr, dateFormat, null), DateTimeKind.Utc);
                return DateTime.ParseExact(dateStr, dateFormat, null);
            }
            catch (Exception ex)
            {
                //LogUtil.Error(ex);
                return null;
            }
        }
        public static string DateToString(DateTime? date, string dateFormat = "dd/MM/yyyy")
        {
            try
            {
                return date == null ? "" : ((DateTime)date).ToString(dateFormat);
            }
            catch (Exception ex)
            {
                //LogUtil.Error(ex);
                return null;
            }
        }
        public static string DateTimeToString(DateTime? date, string dateFormat = "d/M/yyyy H:m:s")
        {
            try
            {
                return date == null ? "" : ((DateTime)date).ToLocalTime().ToString(dateFormat);
            }
            catch (Exception ex)
            {
                //LogUtil.Error(ex);
                return null;
            }
        }
        public static DateTime? StringToDate(string dateStr, string dateFormat = "d/M/yyyy", bool logEx = false)
        {
            try
            {
                return DateTime.SpecifyKind(DateTime.ParseExact(dateStr, dateFormat, null), DateTimeKind.Local);
            }
            catch (Exception ex)
            {
                if (logEx)
                {
                    //LogUtil.Error(ex);
                }
                return null;
            }
        }
        public static DateTime? StringToDateTime(string dateStr, string dateFormat = "d/M/yyyy H:m:s", bool logEx = false)
        {
            try
            {
                return DateTime.SpecifyKind(DateTime.ParseExact(dateStr, dateFormat, null), DateTimeKind.Local);
            }
            catch (Exception ex)
            {
                if (logEx)
                {
                    //LogUtil.Error(ex);
                }
                return null;
            }
        }
        public static string GetWeekDayName(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "CN";

                case DayOfWeek.Monday:
                    return "Hai";

                case DayOfWeek.Tuesday:
                    return "Ba";

                case DayOfWeek.Wednesday:
                    return "Tư";

                case DayOfWeek.Thursday:
                    return "Năm";

                case DayOfWeek.Friday:
                    return "Sáu";

                case DayOfWeek.Saturday:
                    return "Bảy";
            }
            return "";
        }
    }
}
