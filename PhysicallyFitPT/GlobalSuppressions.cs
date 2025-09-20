//-----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
using System.Diagnostics.CodeAnalysis;

// Suppress SA1633 (file header required) for generated files from Microsoft.Maui.Controls.SourceGen
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "Generated file", Scope = "NamespaceAndDescendants", Target = "~N:Microsoft.Maui.Controls.SourceGen")]
