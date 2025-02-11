namespace Atc.Wpf.SourceGenerators.Builders;

internal abstract class BuilderBase
{
    private const int IndentSpaces = 4;
    private readonly StringBuilder sb = new();
    private string indent = string.Empty;
    private bool wasLastCallAppendLine = true;
    private bool isFirstMember = true;

    public int IndentLevel { get; private set; }

    public void IncreaseIndent()
    {
        IndentLevel++;
        indent += new string(' ', IndentSpaces);
    }

    public bool DecreaseIndent()
    {
        if (indent.Length < IndentSpaces)
        {
            return false;
        }

        IndentLevel--;
        indent = indent.Substring(IndentSpaces);
        return true;
    }

    public void AppendLineBeforeMember()
    {
        if (!isFirstMember)
        {
            sb.AppendLine();
        }

        isFirstMember = false;
    }

    public void AppendLine(
        string line)
    {
        if (wasLastCallAppendLine)
        {
            sb.Append(indent);
        }

        sb.AppendLine(line);
        wasLastCallAppendLine = true;
    }

    public void AppendLine()
    {
        sb.AppendLine();
        wasLastCallAppendLine = true;
    }

    public void Append(
        string stringToAppend)
    {
        if (wasLastCallAppendLine)
        {
            sb.Append(indent);
            wasLastCallAppendLine = false;
        }

        sb.Append(stringToAppend);
    }

    public void GenerateEnd()
    {
        DecreaseIndent();
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("#nullable disable");
    }

    public override string ToString()
        => sb.ToString();

    public SourceText ToSourceText()
    {
        var str = sb.ToString();
        str = str.Substring(0, str.Length - 2);
        return SourceText.From(
            str,
            Encoding.UTF8);
    }
}