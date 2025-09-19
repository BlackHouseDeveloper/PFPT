//-----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="BlackHouseDeveloper">
//     Copyright (c) BlackHouseDeveloper. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// Suppress SA1633 (file header required) for generated files from Microsoft.Maui.Controls.SourceGen
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "Generated file", Scope = "NamespaceAndDescendants", Target = "~N:Microsoft.Maui.Controls.SourceGen")]

// Suppress SA1633 for all generated files from source generators
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "Auto-generated file from source generator", Scope = "type", Target = "~T:Microsoft.Maui.Controls.SourceGen.CodeBehindGenerator.Components_Layout_MainLayout")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "Auto-generated file from source generator", Scope = "type", Target = "~T:Microsoft.Maui.Controls.SourceGen.CodeBehindGenerator.Components_Layout_NavMenu")]

// Global suppression for any files in SourceGen namespace
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "Source generated files do not require file headers", Scope = "namespaceanddescendants", Target = "Microsoft.Maui.Controls.SourceGen")]
