namespace OptiNetNine.Features.Common
{
    using System.Text.Json;

    public class UpperCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) =>
            name.ToUpper();
    }
}