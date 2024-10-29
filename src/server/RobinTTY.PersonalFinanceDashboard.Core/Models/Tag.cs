﻿namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// A tag can be used to add additional information to transactions.
/// E.g. a transaction could have the tags "home-entertainment" and "technology".
/// </summary>
public class Tag
{
    /// <summary>
    /// The id of the tag.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// The name of the tag.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// A description of the tag.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// The color of the tag when displayed in a client application.
    /// </summary>
    public string Color { get; set; }
    
    /// <summary>
    /// Creates a new instance of <see cref="Tag"/>.
    /// Default constructor necessary for GraphQL projections.
    /// </summary>
    public Tag()
    {
        Id = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
        Color = string.Empty;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Tag"/>.
    /// </summary>
    /// <param name="id">The id of the tag.</param>
    /// <param name="name">The name of the tag.</param>
    /// <param name="description">A description of the tag.</param>
    /// <param name="color">The color of the tag when displayed in a client application.</param>
    public Tag(string id, string name, string description, string color)
    {
        Id = id;
        Name = name;
        Description = description;
        Color = color;
    }
}
