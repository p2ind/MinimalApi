namespace MinimalAPIsMovies.Validations
{
    public static class ValidationUtilities
    {
        public static string NoEmptyMessage = "The field {PropertyName} is required";
        public static string MaximumLengthMessage = "The field {PropertyName} should be less then {MaxLength} characters.";
        public static string FirstLetterIsUpperCaseMessage = "The field {PropertyName} should start with uppercase";
        public static string GreaterThanDate(DateTime value) => "The field {PropertyName} should be greater then " + value.ToString("yyyy-MM-dd");

        public static bool FirstLetterIsUpperCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;

            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }
    }
}
