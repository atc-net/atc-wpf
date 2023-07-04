namespace Atc.Wpf.Controls.Layouts.Grid.Internal;

internal sealed class TokenizerHelper
{
    private string str = string.Empty;
    private char quoteChar;
    private char argSeparator;
    private int strLen;
    private int charIndex;
    private int currentTokenIndex;
    private int currentTokenLength;

    public bool FoundSeparator { get; private set; }

    public TokenizerHelper(
        string value,
        IFormatProvider formatProvider)
    {
        ArgumentNullException.ThrowIfNull(value);

        var numberSeparator = GetNumericListSeparator(formatProvider);
        Initialize(value, '\'', numberSeparator);
    }

    private void Initialize(
        string value,
        char quote,
        char separator)
    {
        ArgumentNullException.ThrowIfNull(value);

        str = value;
        strLen = value.Length;
        currentTokenIndex = -1;
        quoteChar = quote;
        argSeparator = separator;

        while (charIndex < strLen)
        {
            if (!char.IsWhiteSpace(str, charIndex))
            {
                break;
            }

            ++charIndex;
        }
    }

    public string? GetCurrentToken()
        => currentTokenIndex < 0
            ? null
            : str.Substring(currentTokenIndex, currentTokenLength);

    internal bool NextToken()
        => NextToken(allowQuotedToken: false);

    public bool NextToken(
        bool allowQuotedToken)
        => NextToken(allowQuotedToken, argSeparator);

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    public bool NextToken(
        bool allowQuotedToken,
        char separator)
    {
        currentTokenIndex = -1;
        FoundSeparator = false;

        if (charIndex >= strLen)
        {
            return false;
        }

        var currentChar = str[charIndex];
        var quoteCount = 0;

        if (allowQuotedToken &&
            currentChar == quoteChar)
        {
            quoteCount++;
            ++charIndex;
        }

        var newTokenIndex = charIndex;
        var newTokenLength = 0;

        while (charIndex < strLen)
        {
            currentChar = str[charIndex];
            if (quoteCount > 0)
            {
                if (currentChar == quoteChar)
                {
                    --quoteCount;

                    if (quoteCount == 0)
                    {
                        ++charIndex;
                        break;
                    }
                }
            }
            else if (char.IsWhiteSpace(currentChar) || currentChar == separator)
            {
                if (currentChar == separator)
                {
                    FoundSeparator = true;
                }

                break;
            }

            ++charIndex;
            ++newTokenLength;
        }

        if (quoteCount > 0)
        {
            throw new InvalidOperationException("Missing end-quote");
        }

        ScanToNextToken(separator);

        currentTokenIndex = newTokenIndex;
        currentTokenLength = newTokenLength;

        if (currentTokenLength < 1)
        {
            throw new InvalidOperationException("Empty token");
        }

        return true;
    }

    private void ScanToNextToken(
        char separator)
    {
        if (charIndex >= strLen)
        {
            return;
        }

        var currentChar = str[charIndex];

        if (currentChar != separator && !char.IsWhiteSpace(currentChar))
        {
            throw new InvalidOperationException("Extra data encountered");
        }

        var argSepCount = 0;
        while (charIndex < strLen)
        {
            currentChar = str[charIndex];

            if (currentChar == separator)
            {
                FoundSeparator = true;
                ++argSepCount;
                charIndex++;

                if (argSepCount > 1)
                {
                    throw new InvalidOperationException("Empty token");
                }
            }
            else if (char.IsWhiteSpace(currentChar))
            {
                ++charIndex;
            }
            else
            {
                break;
            }
        }

        if (argSepCount > 0 && charIndex >= strLen)
        {
            throw new InvalidOperationException("Empty token");
        }
    }

    internal static char GetNumericListSeparator(
        IFormatProvider provider)
    {
        var numericSeparator = ',';
        var numberFormat = NumberFormatInfo.GetInstance(provider);

        if (numberFormat.NumberDecimalSeparator.Length > 0 && numericSeparator == numberFormat.NumberDecimalSeparator[0])
        {
            numericSeparator = ';';
        }

        return numericSeparator;
    }
}