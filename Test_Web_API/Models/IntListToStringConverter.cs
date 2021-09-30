using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class IntListToString
{
    public static string ListToString(List<int> value)
    {
        if (value == null || value.Count() == 0)
        {
            return null;
        }

        return String.Join(',', value.ToArray());
    }

    public static List<int> StringToList(string value)
    {
        if (value == null || value == string.Empty)
        {
            return null;
        }

        return value.Split(',')
            .Where(x => int.TryParse(x, out _))
            .Select(int.Parse)
            .ToList();

    }
}
