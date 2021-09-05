using StockApp.BaseClasses.Zielschiessen;
using System.Globalization;
using System.Windows.Controls;

namespace StockApp.ValidationRules
{
    public class ZielbewerbValueValidationRule : ValidationRule
    {
        public Disziplinart Disziplinart { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Disziplinart != Disziplinart.Schiessen)
            {
                bool canConvert = (int.TryParse(value as string, out int result)) &&
                (result == 0 || result == 2 || result == 4 || result == 6 || result == 8 || result == 10);

                return new ValidationResult(canConvert, "Kein gültiger Wert bei einem MASS-Versuch");
            }
            else
            {
                bool canConvert = (int.TryParse(value as string, out int result)) &&
                    (result == 0 || result == 2 || result == 5 || result == 10);

                return new ValidationResult(canConvert, "Kein gültiger Wert bei einem SCHUSS-Versuch");
            }
        }
    }
}
