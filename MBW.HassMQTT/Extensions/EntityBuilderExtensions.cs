using System;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.Interfaces;
using MBW.HassMQTT.Internal;

namespace MBW.HassMQTT.Extensions;

public static class EntityBuilderExtensions
{
    /// <summary>
    /// Publishes the entity's primary state and JSON attributes as one retained JSON payload.
    /// </summary>
    /// <remarks>
    /// State and JSON-attribute topics must either both be unset or be configured to the same case-sensitive value.
    /// Build installs the Home Assistant templates required to extract both values and rejects custom extraction
    /// templates. No combined value payload is published until state has been assigned; an explicit null state counts
    /// as assigned and is published.
    ///
    /// The combined payload is a complete snapshot, not a patch. The application must reconstruct both state and
    /// attributes after a restart; publishing state without reconstructing attributes replaces retained attributes
    /// with an empty object. Custom state and JSON-attribute extraction templates are not supported.
    /// </remarks>
    public static IEntityBuilder<TEntity> PublishStateAndAttributesTogether<TEntity>(this IEntityBuilder<TEntity> builder)
        where TEntity : IHassDiscoveryDocument
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        if (builder is not EntityBuilder<TEntity> entityBuilder)
            throw new NotSupportedException("State and attributes can only be combined on HassMQTT entity builders");

        return entityBuilder.PublishStateAndAttributesTogether();
    }
}
