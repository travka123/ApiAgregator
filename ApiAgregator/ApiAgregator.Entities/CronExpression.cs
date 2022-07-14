using System.Runtime.Serialization;

namespace ApiAgregator.Entities;

public class CronExpression
{
    private readonly string _expression;
    private readonly HashSet<int> _minutes = new HashSet<int>();
    private readonly HashSet<int> _hours = new HashSet<int>();
    private readonly bool _useDaysOfMonth = false;
    private readonly HashSet<int> _daysOfMonth = new HashSet<int>();
    private readonly HashSet<int> _months = new HashSet<int>();
    private readonly bool _useDaysOfWeek = false;
    private readonly HashSet<int> _daysOfWeek = new HashSet<int>();

    public CronExpression(string exp)
    {
        var expParts = exp.Split(' ').Where(p => p.Length > 0).ToArray();

        _expression = String.Join(" ", expParts);

        if (expParts.Length != 5)
            throw new CroneExpressionParseException();

        //Minutes
        Parse(expParts[0], 0, 59, ParseInt, val => _minutes.Add(val));

        //Hours
        Parse(expParts[1], 0, 23, ParseInt, val => _hours.Add(val));

        //Days of month
        _useDaysOfMonth = expParts[2] != "*";
        if (_useDaysOfMonth)
            Parse(expParts[2], 1, 31, ParseInt, val => _daysOfMonth.Add(val));

        //Months
        Parse(expParts[3], 1, 12, (str) =>
        {
            return str switch
            {
                "JAN" => 1,
                "FEB" => 2,
                "MAR" => 3,
                "APR" => 4,
                "MAY" => 5,
                "JUN" => 6,
                "JUL" => 7,
                "AUG" => 8,
                "SEP" => 9,
                "OCT" => 10,
                "NOV" => 11,
                "DEC" => 12,
                _ => ParseInt(str)
            };
        }, val => _months.Add(val));

        //Days of week
        _useDaysOfWeek = expParts[4] != "*";
        if (_useDaysOfWeek)
        {
            Parse(expParts[4], 0, 6, (str) =>
            {
                return str switch
                {
                    "SUN" => 0,
                    "MON" => 1,
                    "TUE" => 2,
                    "WED" => 3,
                    "THU" => 4,
                    "FRI" => 5,
                    "SAT" => 6,
                    _ => ParseInt(str)
                };
            }, val => _daysOfWeek.Add(val));
        }
    }

    private void Parse(string exp, int rMin, int rMax, Func<string, int> parse, Action<int> add)
    {
        if (exp.Contains(','))
        {
            foreach(var expPart in exp.Split(','))
            {
                Parse(expPart, rMin, rMax, parse, add);
            }
        }
        else if (exp.Contains('/'))
        {
            var expParts = exp.Split('/');

            if (expParts.Length != 2)
                throw new CroneExpressionParseException();

            int step = parse(expParts[1]);
            if (expParts[0] == "*")
            {
                for (int i = rMin; i <= rMax; i += step)
                    add(i);
            }
            else if (expParts[0].Contains("-"))
            {
                (int min, int max) = ParseRange(expParts[0], rMin, rMax, parse);
                for (int i = min; i <= max; i += step)
                    add(i);
            }
            else
            {
                throw new CroneExpressionParseException();
            }
        }
        else if (exp.Contains('-'))
        {
            (int min, int max) = ParseRange(exp, rMin, rMax, parse);
            for (int i = min; i <= max; i++)
                add(i);
        }
        else if (exp == "*")
        {
            for (int i = rMin; i <= rMax; i++)
                add(i);
        }
        else
        {
            int value = parse(exp);

            if ((value < rMin) || (value > rMax))
                throw new CroneExpressionParseException();

            add(value);
        }
    }

    private (int min, int max) ParseRange(string exp, int aMin, int aMax, Func<string, int> parse)
    {
        var expParts = exp.Split('-');
        if (expParts.Length != 2)
            throw new CroneExpressionParseException();

        int min = parse(expParts[0]);
        if (min < aMin)
            throw new CroneExpressionParseException();

        int max = parse(expParts[1]);
        if (max > aMax)
            throw new CroneExpressionParseException();

        return (min, max);
    }

    private static int ParseInt(string str)
    {
        if (int.TryParse(str, out var value))
        {
            return value;
        }
        else
        {
            throw new CroneExpressionParseException();
        }
    }

    public bool Match(DateTime time)
    {
        var dayCheck = (!_useDaysOfMonth && !_useDaysOfWeek) ||
            (_useDaysOfMonth && _daysOfMonth.Contains(time.Day)) ||
            (_useDaysOfWeek && _daysOfWeek.Contains((int)time.DayOfWeek));

        return dayCheck && _minutes.Contains(time.Minute) && _hours.Contains(time.Hour) && _months.Contains(time.Month);
    }

    public static bool TryParse(string str, out CronExpression? exp)
    {
        try
        {
            exp = new CronExpression(str);
            return true;
        }
        catch
        {
            exp = null;
            return false;
        }
    }

    public override string ToString() => _expression;
}

class CroneExpressionParseException : Exception { }
