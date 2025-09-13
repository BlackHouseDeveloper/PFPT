// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT;
/// <summary>
/// Represents the entry point for the iOS application.
/// </summary>
public static class Program
{
  /// <summary>
  /// The main entry point of the application.
  /// </summary>
  /// <param name="args">The command line arguments.</param>
  private static void Main(string[] args)
  {
    // if you want to use a different Application Delegate class from "AppDelegate"
    // you can specify it here.
    UIApplication.Main(args, null, typeof(AppDelegate));
  }
}
