﻿using AutofacSerilogIntegration;

namespace RobinTTY.PersonalFinanceDashboard.API.Utility;

/// <summary>
/// Container managing dependency resolution.
/// </summary>
public class ApplicationModule : Module
{
    /// <summary>
    /// Adds registrations to the dependency container.
    /// </summary>
    /// <param name="builder">The passed down <see cref="ContainerBuilder"/>.</param>
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterLogger(LoggerManager.GetDefaultLogger());
    }
}