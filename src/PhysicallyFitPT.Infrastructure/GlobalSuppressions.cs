// <copyright file="GlobalSuppressions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
using System.Diagnostics.CodeAnalysis;

// Suppress warnings for generated Entity Framework migration files
[assembly: SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance", Justification = "Generated migration code", Scope = "namespaceanddescendants", Target = "~N:PhysicallyFitPT.Infrastructure.Migrations")]
[assembly: SuppressMessage("Readability", "RCS1015:Use nameof operator", Justification = "Generated migration code", Scope = "namespaceanddescendants", Target = "~N:PhysicallyFitPT.Infrastructure.Migrations")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "Generated migration code", Scope = "namespaceanddescendants", Target = "~N:PhysicallyFitPT.Infrastructure.Migrations")]

// Suppress warnings for generated design-time database factory
[assembly: SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance", Justification = "Generated database factory code", Scope = "type", Target = "~T:PhysicallyFitPT.Infrastructure.Data.DesignTimeDbContextFactory")]
[assembly: SuppressMessage("Readability", "RCS1015:Use nameof operator", Justification = "Generated database factory code", Scope = "type", Target = "~T:PhysicallyFitPT.Infrastructure.Data.DesignTimeDbContextFactory")]
