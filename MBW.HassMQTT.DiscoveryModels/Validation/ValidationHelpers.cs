using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace MBW.HassMQTT.DiscoveryModels.Validation
{
    static class ValidationHelpers
    {
        public static IRuleBuilderOptions<T, string> IsValidMqttTopic<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(MqttTopicValidator<T>.Instance);
        }

        public static IRuleBuilderOptions<T, string> IsValidJinjaTemplate<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            // Jinja template must have start and end brackets
            Regex regex = new Regex(@"^\s*\{\{.*?\}\}\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return ruleBuilder.SetValidator(new RegularExpressionValidator<T>(regex));
        }

        private class MqttTopicValidator<T> : IPropertyValidator<T, string>
        {
            public static MqttTopicValidator<T> Instance { get; } = new MqttTopicValidator<T>();

            public bool IsValid(ValidationContext<T> context, string value)
            {
                // Get levels
                string[] levels = value.Split('/');

                if (levels.Take(levels.Length - 1).Any(x => x.Contains("#")))
                {
                    context.MessageFormatter.AppendArgument("mqttError", "# must only come at the end");
                    return false;
                }

                if (levels.Any(x => (x.Contains('+') || x.Contains('#')) && x != "#" && x != "+"))
                {
                    context.MessageFormatter.AppendArgument("mqttError", "Levels must not use + or # unless it is the only character");
                    return false;
                }

                return true;
            }

            public string GetDefaultMessageTemplate(string errorCode)
            {
                return "{PropertyName} does not contain a valid MQTT Topic: {mqttError}";
            }

            public string Name => nameof(MqttTopicValidator<T>);
        }
    }
}
