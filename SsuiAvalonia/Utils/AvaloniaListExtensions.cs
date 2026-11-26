using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Collections;

namespace SolidShineUi.Utils;

/// <summary>
/// A collection of helper methods for working with an AvaloniaList.
/// </summary>
public static class AvaloniaListExtensions
{

    /// <summary>
    /// Remove a range of items from an AvaloniaList that match a certain condition.
    /// </summary>
    /// <typeparam name="T">the type of the list</typeparam>
    /// <param name="list">the source list</param>
    /// <param name="evaluate">the function by which to evaluate each item</param>
    /// <returns><c>true</c> if any items were found and removed; otherwise <c>false</c></returns>
    public static bool RemoveWhere<T>(this AvaloniaList<T> list, Func<T, bool> evaluate)
    {
        var items = list.Where(evaluate);
        if (!items.Any()) return false;
        list.RemoveAll(items);
        return true;
    }

}
