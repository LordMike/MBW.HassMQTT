using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MBW.HassMQTT.DiscoveryModels.Interfaces;

namespace MBW.HassMQTT.Internal;

internal sealed class StateAndAttributesPublishingCapability
{
    public PropertyInfo StateTopicProperty { get; }
    public PropertyInfo StateTemplateProperty { get; }
    public IReadOnlyList<PropertyInfo> AdditionalStateTemplateProperties { get; }

    private StateAndAttributesPublishingCapability(
        PropertyInfo stateTopicProperty,
        PropertyInfo stateTemplateProperty,
        IReadOnlyList<PropertyInfo> additionalStateTemplateProperties)
    {
        StateTopicProperty = stateTopicProperty;
        StateTemplateProperty = stateTemplateProperty;
        AdditionalStateTemplateProperties = additionalStateTemplateProperties;
    }

    public static bool TryCreate(Type discoveryType, out StateAndAttributesPublishingCapability capability)
    {
        capability = null;
        if (!typeof(IHasJsonAttributes).IsAssignableFrom(discoveryType))
            return false;

        PropertyInfo stateTopic = GetWritableStringProperty(discoveryType, "StateTopic");
        PropertyInfo[] stateTemplates = new[]
            {
                GetWritableStringProperty(discoveryType, "ValueTemplate"),
                GetWritableStringProperty(discoveryType, "StateValueTemplate")
            }
            .Where(property => property != null)
            .ToArray();

        if (stateTopic == null || stateTemplates.Length != 1)
            return false;

        List<PropertyInfo> additionalTemplates = new List<PropertyInfo>();
        PropertyInfo lastResetTemplate = GetWritableStringProperty(discoveryType, "LastResetValueTemplate");
        if (lastResetTemplate != null)
            additionalTemplates.Add(lastResetTemplate);

        capability = new StateAndAttributesPublishingCapability(stateTopic, stateTemplates[0], additionalTemplates);
        return true;
    }

    private static PropertyInfo GetWritableStringProperty(Type type, string propertyName)
    {
        PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        return property?.PropertyType == typeof(string) && property.SetMethod != null ? property : null;
    }
}
