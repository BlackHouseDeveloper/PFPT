// <copyright file="ClassNames.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Shared.Components.UI.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Utility for merging CSS class strings similar to clsx/tailwind-merge.
/// Later duplicates win, and empty entries are ignored.
/// </summary>
public static class ClassNames
{
    /// <summary>
    /// Merge multiple class strings into a single space-separated value.
    /// </summary>
    /// <param name="inputs">Classes to merge.</param>
    /// <returns>Normalized class string.</returns>
    public static string Cn(params string?[] inputs) => Cn((IEnumerable<string?>)inputs);

    /// <summary>
    /// Merge an enumerable of class strings into a single space-separated value.
    /// </summary>
    /// <param name="inputs">Classes to merge.</param>
    /// <returns>Normalized class string.</returns>
    public static string Cn(IEnumerable<string?> inputs)
    {
        if (inputs is null)
        {
            return string.Empty;
        }

        var tokens = new List<string>();
        foreach (var input in inputs)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            tokens.AddRange(input.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        if (tokens.Count == 0)
        {
            return string.Empty;
        }

        var lastIndex = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var i = 0; i < tokens.Count; i++)
        {
            lastIndex[tokens[i]] = i;
        }

        var merged = new List<string>(tokens.Count);
        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (lastIndex[token] == i)
            {
                merged.Add(token);
            }
        }

        return string.Join(" ", merged);
    }
}
