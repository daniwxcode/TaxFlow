using System.Collections.ObjectModel;
using System.Text;

namespace Core.Domain.Tax.Calculation;

/// <summary>
/// Normalizes rule expressions to a deterministic NCalc-friendly form.
/// </summary>
internal static class RulePreprocessor
{
    public static RulePreprocessorResult Process(string? expression)
    {
        var context = new RulePreprocessorContext(expression ?? string.Empty);

        NormalizeInput(context);
        RemoveNumericSeparators(context);
        NormalizeTernaryLineBreaks(context);
        NormalizeWhitespace(context);

        return context.ToResult();
    }

    private static void NormalizeInput(RulePreprocessorContext context)
    {
        var trimmed = (context.Expression ?? string.Empty).Trim();
        context.UpdateExpression(trimmed);
    }

    private static void RemoveNumericSeparators(RulePreprocessorContext context)
    {
        var expression = context.Expression;
        if (string.IsNullOrEmpty(expression) || !expression.Contains('_'))
        {
            return;
        }

        var sanitized = RemoveNumericSeparatorsInternal(expression);
        if (!sanitized.Equals(expression, StringComparison.Ordinal))
        {
            context.UpdateExpression(sanitized, "Removed numeric separators from numeric literals.");
        }
    }

    private static string RemoveNumericSeparatorsInternal(string expression)
    {
        StringBuilder sb = new(expression.Length);
        bool inString = false;
        char delimiter = '\0';

        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];

            if (inString)
            {
                sb.Append(c);
                if (c == delimiter && (i == 0 || expression[i - 1] != '\\'))
                {
                    inString = false;
                }

                continue;
            }

            if (c == '"' || c == '\'')
            {
                inString = true;
                delimiter = c;
                sb.Append(c);
                continue;
            }

            if (c == '_' && i > 0 && i < expression.Length - 1 && char.IsDigit(expression[i - 1]) && char.IsDigit(expression[i + 1]))
            {
                continue;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    private static void NormalizeWhitespace(RulePreprocessorContext context)
    {
        var expression = context.Expression;
        if (string.IsNullOrEmpty(expression))
        {
            return;
        }

        var replaced = expression.ReplaceLineEndings("\n");
        var sb = new StringBuilder(replaced.Length);
        var inString = false;
        char delimiter = '\0';
        var lastWasWhitespace = false;

        for (var i = 0; i < replaced.Length; i++)
        {
            var c = replaced[i];

            if (inString)
            {
                sb.Append(c);
                if (c == delimiter && (i == 0 || replaced[i - 1] != '\\'))
                {
                    inString = false;
                }

                continue;
            }

            if (c == '"' || c == '\'')
            {
                inString = true;
                delimiter = c;
                sb.Append(c);
                lastWasWhitespace = false;
                continue;
            }

            if (char.IsWhiteSpace(c))
            {
                if (lastWasWhitespace)
                {
                    continue;
                }

                sb.Append(' ');
                lastWasWhitespace = true;
                continue;
            }

            sb.Append(c);
            lastWasWhitespace = false;
        }

        var normalized = sb.ToString().Trim();
        if (!normalized.Equals(expression, StringComparison.Ordinal))
        {
            context.UpdateExpression(normalized, "Normalized whitespace and line endings.");
        }
    }

    private static void NormalizeTernaryLineBreaks(RulePreprocessorContext context)
    {
        var expression = context.Expression;
        if (string.IsNullOrEmpty(expression))
        {
            return;
        }

        StringBuilder sb = new(expression.Length);
        bool inString = false;
        char delimiter = '\0';
        bool changed = false;

        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];

            if (inString)
            {
                sb.Append(c);
                if (c == delimiter && (i == 0 || expression[i - 1] != '\\'))
                {
                    inString = false;
                }

                continue;
            }

            if (c == '"' || c == '\'')
            {
                inString = true;
                delimiter = c;
                sb.Append(c);
                continue;
            }

            if ((c == '?' || c == ':') && HasLineBreakImmediatelyAfter(expression, i + 1, out var skip))
            {
                sb.Append(c);
                sb.Append(' ');
                i += skip;
                changed = true;
                continue;
            }

            sb.Append(c);
        }

        if (changed)
        {
            context.UpdateExpression(sb.ToString(), "Flattened multi-line ternary operands.");
        }
    }

    private static bool HasLineBreakImmediatelyAfter(string expression, int startIndex, out int skipLength)
    {
        var index = startIndex;
        var sawLineBreak = false;

        while (index < expression.Length)
        {
            var c = expression[index];
            if (c == '\r' || c == '\n')
            {
                sawLineBreak = true;
            }
            else if (!char.IsWhiteSpace(c))
            {
                break;
            }

            index++;
        }

        skipLength = Math.Max(0, index - startIndex);
        return sawLineBreak;
    }

    private sealed class RulePreprocessorContext
    {
        private string _expression;
        private readonly List<RulePreprocessorDiagnostic> _diagnostics = new();

        public RulePreprocessorContext(string initialExpression)
        {
            _expression = initialExpression ?? string.Empty;
        }

        public string Expression => _expression;

        public bool UpdateExpression(string expression, string? reason = null)
        {
            var newValue = expression ?? string.Empty;
            if (string.Equals(_expression, newValue, StringComparison.Ordinal))
            {
                return false;
            }

            _expression = newValue;
            if (!string.IsNullOrWhiteSpace(reason))
            {
                _diagnostics.Add(RulePreprocessorDiagnostic.Info(reason));
            }

            return true;
        }

        public void AddDiagnostic(RulePreprocessorDiagnostic diagnostic)
        {
            _diagnostics.Add(diagnostic);
        }

        public RulePreprocessorResult ToResult()
            => new(_expression, new ReadOnlyCollection<RulePreprocessorDiagnostic>(_diagnostics));
    }
}

/// <summary>
/// Result of preprocessing a rule expression.
/// </summary>
internal sealed record RulePreprocessorResult(string Expression, IReadOnlyList<RulePreprocessorDiagnostic> Diagnostics);

/// <summary>
/// Diagnostic emitted during preprocessing.
/// </summary>
internal sealed record RulePreprocessorDiagnostic(RulePreprocessorDiagnosticSeverity Severity, string Message)
{
    public static RulePreprocessorDiagnostic Info(string message) => new(RulePreprocessorDiagnosticSeverity.Info, message);
    public static RulePreprocessorDiagnostic Warning(string message) => new(RulePreprocessorDiagnosticSeverity.Warning, message);
}

internal enum RulePreprocessorDiagnosticSeverity
{
    Info,
    Warning
}
