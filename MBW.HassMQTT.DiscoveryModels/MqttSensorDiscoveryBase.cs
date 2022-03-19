#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using JetBrains.Annotations;
using MBW.HassMQTT.Abstracts.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Availability;
using MBW.HassMQTT.DiscoveryModels.Device;
using MBW.HassMQTT.DiscoveryModels.Enum;
using MBW.HassMQTT.DiscoveryModels.Helpers;
using MBW.HassMQTT.DiscoveryModels.Interfaces;
using MBW.HassMQTT.DiscoveryModels.Validation;
using Newtonsoft.Json;

namespace MBW.HassMQTT.DiscoveryModels
{
    /// <summary>
    /// All MQTT discovery types are documented here:
    /// https://www.home-assistant.io/docs/mqtt/discovery/
    /// </summary>
    public abstract class MqttSensorDiscoveryBase<T, TValidator> : IHassDiscoveryDocument, IMqttValueContainer, INotifyPropertyChanged where T : IHassDiscoveryDocument where TValidator : AbstractValidator<T>, new()
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static TValidator Validator { get; } = new TValidator();
        IValidator IHassDiscoveryDocument.Validator => Validator;

        [NotifyPropertyChangedInvocator]
        [UsedImplicitly]
        protected virtual void OnPropertyChanged(string propertyName, object before, object after)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (nameof(Dirty) != propertyName)
                Dirty = true;
        }

        /// <inheritdoc />
        [JsonIgnore]
        public bool Dirty { get; private set; }

        /// <inheritdoc />
        [JsonIgnore]
        public string PublishTopic { get; }

        /// <summary>
        /// Device details for this entity, usually this is duplicated between multiple entities to let HA link them together.
        /// At least one of identifiers or connections must be present to identify the device.
        /// </summary>
        public MqttDeviceDocument Device { get; }

        public MqttSensorDiscoveryBase(string discoveryTopic, string uniqueId)
        {
            PublishTopic = discoveryTopic;

            if (this is IHasUniqueId asHasUniqueId)
                asHasUniqueId.UniqueId = uniqueId;

#pragma warning disable 618
            if (this is IHasAvailability asAvailabilityTopic)
                asAvailabilityTopic.Availability = new List<AvailabilityModel>();
#pragma warning restore 618

            Device = new MqttDeviceDocument();
            Device.PropertyChanged += (_, _) => SetDirty();
        }

        public void SetDirty(bool dirty = true)
        {
            Dirty = dirty;
        }

        public object GetSerializedValue(bool resetDirty)
        {
            if (resetDirty)
                Dirty = false;

            return this;
        }

        public void SetTopic(HassTopicKind topicKind, string topic)
        {
            Type type = GetType();
            PropertyInfo prop = type.GetProperty($"{topicKind}Topic");

            if (prop == null)
                throw new NotSupportedException($"Unable to set topic {topicKind} on {type.Name}");

            prop.SetValue(this, topic);
        }

        public string GetTopic(HassTopicKind topicKind)
        {
            Type type = GetType();
            PropertyInfo prop = type.GetProperty($"{topicKind}Topic");

            if (prop == null)
                throw new NotSupportedException($"Unable to get the topic {topicKind} on {type.Name}");

            return prop.GetValue(this) as string;
        }

        public abstract class MqttSensorDiscoveryBaseValidator<TInner> : AbstractValidator<TInner> where TInner : MqttSensorDiscoveryBase<T, TValidator>
        {
            protected MqttSensorDiscoveryBaseValidator()
            {
                Type type = typeof(TInner);

                // Generics
                RuleFor(s => s.Device).SetValidator(MqttDeviceDocument.Validator);

                if (typeof(IHasAvailability).IsAssignableFrom(type))
                {
                    RuleForEach(s => ((IHasAvailability)s).Availability)
                        .SetValidator(AvailabilityModel.Validator);

                    RuleFor(s => ((IHasAvailability)s).AvailabilityMode)
                        .IsInEnum()
                        .When(s => ((IHasAvailability)s).AvailabilityMode.HasValue);
                }

                if (typeof(IHasQos).IsAssignableFrom(type))
                    RuleFor(s => ((IHasQos)s).Qos).IsInEnum();

                if (typeof(IHasJsonAttributes).IsAssignableFrom(type))
                    TopicAndTemplate(s => ((IHasJsonAttributes)s).JsonAttributesTopic, s => ((IHasJsonAttributes)s).JsonAttributesTemplate);

                // Enums
                IEnumerable<PropertyInfo> enumProps = type.GetProperties()
                    .Where(s => Nullable.GetUnderlyingType(s.PropertyType)?.IsEnum ?? s.PropertyType.IsEnum);

                foreach (PropertyInfo? enumProp in enumProps)
                {
                    // TODO: Needs work to operate, has type casting issues
                    if (enumProp.PropertyType.IsGenericType)
                    {
                        //// Nullable
                        //Type enumType = Nullable.GetUnderlyingType(enumProp.PropertyType) ?? enumProp.PropertyType;
                        //Expression<Func<TInner, System.Enum?>>? propExpression = type.GetPropertyExpression<TInner, System.Enum?>(enumProp);

                        //IRuleBuilderOptions<TInner, System.Enum?>? rule = RuleFor(propExpression)
                        //    .IsInEnum();

                        //if (enumProp.PropertyType.IsGenericType)
                        //{
                        //    Expression<Func<TInner, bool>>? notNullExpression = type.GetNotNullExpression<TInner>(enumProp);
                        //    Func<TInner, bool> notNullFunc = notNullExpression.Compile();

                        //    rule.When(notNullFunc);
                        //}
                    }
                    else
                    {
                        // Not nullable
                        //Type enumType = Nullable.GetUnderlyingType(enumProp.PropertyType) ?? enumProp.PropertyType;
                        //Expression<Func<TInner, System.Enum?>>? propExpression = type.GetPropertyExpression<TInner, System.Enum?>(enumProp);

                        //IRuleBuilderOptions<TInner, System.Enum?>? rule = RuleFor(propExpression)
                        //    .IsInEnum();
                    }
                }

                // Templates
                IEnumerable<PropertyInfo> templateProps = type.GetProperties().Where(s => s.Name.EndsWith("Template"));

                foreach (PropertyInfo prop in templateProps)
                {
                    Expression<Func<TInner, string>> propExpression = type.GetPropertyExpression<TInner, string>(prop);
                    Expression<Func<TInner, bool>> notNullExpression = type.GetNotNullExpression<TInner>(prop);
                    Func<TInner, bool> notNullPredicate = notNullExpression.Compile();

                    RuleFor(propExpression).IsValidJinjaTemplate().When(notNullPredicate);
                }

                // Topics
                IEnumerable<PropertyInfo> topicProps = type.GetProperties().Where(s => s.Name.EndsWith("Topic"));

                foreach (PropertyInfo prop in topicProps)
                {
                    Expression<Func<TInner, string>> propExpression = type.GetPropertyExpression<TInner, string>(prop);
                    Expression<Func<TInner, bool>> notNullExpression = type.GetNotNullExpression<TInner>(prop);
                    Func<TInner, bool> notNullPredicate = notNullExpression.Compile();

                    RuleFor(propExpression).IsValidMqttTopic().When(notNullPredicate);
                }

                // Non-nullable properties
                IEnumerable<PropertyInfo> nonNullableProps = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(s => s.SetMethod != null)
                    .Where(s => !s.PropertyType.IsValueType && !s.IsNullable());

                foreach (PropertyInfo prop in nonNullableProps)
                {
                    Expression<Func<TInner, object>> propExpression = type.GetPropertyExpression<TInner, object>(prop);

                    RuleFor(propExpression).NotNull();
                }
            }

            /// <summary>
            /// Ensures:
            /// - min &lt; max
            /// - max &gt; min
            /// - min &lt; value &lt; max
            /// </summary>
            protected void MinMax<TProperty>(Expression<Func<TInner, TProperty?>> minSelector, Expression<Func<TInner, TProperty?>> maxSelector, TProperty defaultMin, TProperty defaultMax, params (Expression<Func<TInner, TProperty?>> expression, TProperty defaultValue)[] values)
                where TProperty : struct, IComparable<TProperty>, IComparable
            {
                Func<TInner, TProperty?> minSelectorF = minSelector.Compile();
                Func<TInner, TProperty?> maxSelectorF = maxSelector.Compile();

                RuleFor(minSelector).LessThan(s => maxSelectorF(s) ?? defaultMax)
                    .When(s => minSelectorF(s).HasValue);

                RuleFor(maxSelector).GreaterThan(s => minSelectorF(s) ?? defaultMin)
                    .When(s => maxSelectorF(s).HasValue);

                foreach ((Expression<Func<TInner, TProperty?>> expression, TProperty defaultValue) in values)
                {
                    Func<TInner, TProperty?> valueSelectorF = expression.Compile();

                    Comparer<TProperty> comparer = Comparer<TProperty>.Default;

                    RuleFor(expression).Must((s, _) =>
                        {
                            TProperty min = minSelectorF(s) ?? defaultMin;
                            TProperty max = maxSelectorF(s) ?? defaultMax;
                            TProperty val = valueSelectorF(s) ?? defaultValue;

                            if (comparer.Compare(min, val) < 0)
                                return false;

                            if (comparer.Compare(val, max) > 0)
                                return false;

                            return true;
                        })
                        .When(s => valueSelectorF(s).HasValue);
                }
            }

            /// <summary>
            /// Ensures that a topic is set, when a template is set
            /// </summary>
            protected void TopicAndTemplate(Expression<Func<TInner, string?>> topicSelector, params Expression<Func<TInner, string?>>[] templateSelectors)
            {
                foreach (Expression<Func<TInner, string?>> templateSelector in templateSelectors)
                {
                    Func<TInner, string?> templateSelectorF = templateSelector.Compile();
                    string? topicName = topicSelector.GetProperty().Name;
                    string? templateName = templateSelector.GetProperty().Name;

                    RuleFor(topicSelector).NotNull().When(s => templateSelectorF(s) != null)
                        .WithMessage($"The template '{templateName}' is set, but the topic '{topicName}' is not")
                        .WithSeverity(Severity.Warning);
                }
            }
        }
    }
}