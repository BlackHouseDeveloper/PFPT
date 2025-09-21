// <copyright file="GlobalSuppressions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
using System.Diagnostics.CodeAnalysis;

// Suppress CA1859 for test projects - interfaces are preferred in tests for readability and mocking
[assembly: SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance", Justification = "Interface usage in tests provides better readability and supports mocking patterns", Scope = "namespaceanddescendants", Target = "~N:PhysicallyFitPT.Tests")]

// Suppress RCS1015 for test projects - ToString() usage in tests can be more readable than nameof
[assembly: SuppressMessage("Readability", "RCS1015:Use nameof operator", Justification = "ToString() usage in tests can be more readable than nameof for enum values", Scope = "namespaceanddescendants", Target = "~N:PhysicallyFitPT.Tests")]
