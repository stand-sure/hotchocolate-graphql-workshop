namespace ConferencePlanner.Models;

using System.ComponentModel.DataAnnotations;

public class Speaker
{
    /// <summary>
    /// Gets or sets the Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Name.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Bio.
    /// </summary>
    [StringLength(4000)]
    public string Bio { get; set; }

    /// <summary>
    /// Gets or sets the Website.
    /// </summary>
    [StringLength(1000)]
    public string Website { get; set; }
}