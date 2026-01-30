namespace TasteBox.Abstractions.Consts;

public class RegexPatterns
{
    public const string Password =
        "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";

    public const string EgyptianPhoneNumber = "^(010|011|012|015)[0-9]{8}$";
}