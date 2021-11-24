using System.Text.Json;
using System.Text.RegularExpressions;

namespace RecipeManagementBE.Config {
    public class JsonSnakeCaseNamingPolicy : JsonNamingPolicy {
        public override string ConvertName(string name) => Regex.Replace(name, @"(\w)([A-Z])", "$1_$2").ToLower();
    }
}