using System.Globalization;
using System.Windows.Controls;

namespace KiaGallery.DailyReport.Forms.ValidationRole
{
	public class NotEmptyValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			return string.IsNullOrWhiteSpace((value ?? "").ToString())
				? new ValidationResult(false, "این فیلد اجباری است.")
				: ValidationResult.ValidResult;
		}


	}
}