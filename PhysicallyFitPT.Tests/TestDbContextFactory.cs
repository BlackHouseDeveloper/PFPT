// <copyright file="TestDbContextFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PhysicallyFitPT.Tests
{
    using Microsoft.EntityFrameworkCore;
    using PhysicallyFitPT.Infrastructure.Data;

    /// <summary>
    /// Test helper to create DbContext factory for testing.
    /// </summary>
    public class TestDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        private readonly DbContextOptions<ApplicationDbContext> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDbContextFactory"/> class.
        /// </summary>
        /// <param name="options">Database context options to use for creating contexts.</param>
        public TestDbContextFactory(DbContextOptions<ApplicationDbContext> options)
        {
            this.options = options;
        }

        /// <inheritdoc/>
        public ApplicationDbContext CreateDbContext()
        {
            var context = new ApplicationDbContext(this.options);
            context.Database.EnsureCreated();
            return context;
        }

        /// <summary>
        /// Creates a new database context asynchronously with schema initialization.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ApplicationDbContext> CreateDbContextAsync()
        {
            var context = new ApplicationDbContext(this.options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }
    }
}
