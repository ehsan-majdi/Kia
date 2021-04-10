namespace KiaGallery.Common
{
    public class ValidateUtility
    {
        /// <summary>
        /// بررسی شماره تلفن همراه
        /// </summary>
        /// <param name="mobileNumber">شماره تلفن همراه</param>
        /// <returns>نتیجه بررسی تلفن وارد شده</returns>
        public static bool ValidateMobileNumber(string mobileNumber)
        {
            if (string.IsNullOrEmpty(mobileNumber)) return false;

            if (mobileNumber.Length != 11) return false;

            if (!mobileNumber.StartsWith("09")) return false;

            return true;
        }
    }
}
