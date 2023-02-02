namespace ConferencePlanner.Data;

using ConferencePlanner.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Speakers.
    /// </summary>
    public DbSet<Speaker> Speakers { get; set; }
}