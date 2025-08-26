using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rogue.Domain;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Rogue.Data;

internal class AllFieldsContractResolver : DefaultContractResolver
{
    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        var contract = base.CreateObjectContract(objectType);

        contract.OverrideCreator = null;
        contract.CreatorParameters.Clear();

        contract.DefaultCreator = () =>
            RuntimeHelpers.GetUninitializedObject(objectType);

        return contract;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var props = new List<JsonProperty>();
        for (var current = type; current != null && current != typeof(object); current = current.BaseType)
        {
            var fields = current.GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly);

            foreach (var field in fields)
            {
                var prop = base.CreateProperty(field, memberSerialization);
                prop.PropertyName = field.Name;
                prop.Readable = true;
                prop.Writable = true;
                props.Add(prop);
            }
        }

        return props;
    }
}

public static class Data
{
    private static readonly string SavePath = "save.json";
    private static readonly string StatisticsPath = "statistics.json";

    private static readonly JsonSerializerSettings Settings = new()
    {
        ContractResolver = new AllFieldsContractResolver(),
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        MissingMemberHandling = MissingMemberHandling.Error,
        TypeNameHandling = TypeNameHandling.All,
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
    };

    public static void SaveData(Game game)
    {
        string text = JsonConvert.SerializeObject(game, Formatting.Indented, Settings);

        File.WriteAllText(SavePath, text);
    }

    public static Game LoadData()
    {
        string text = File.ReadAllText(SavePath);
        Game? game = JsonConvert.DeserializeObject<Game>(text, Settings);

        if (game == null)
        {
            throw new Exception("Deserialized game is null.");
        }

        return game;
    }

    public static List<Statistics> LoadStatistics()
    {
        if (!File.Exists(StatisticsPath))
        {
            return [];
        }
        string text = File.ReadAllText(StatisticsPath);
        List<Statistics>? statistics = JsonConvert.DeserializeObject<List<Statistics>>(text, Settings);
        if (statistics == null)
        {
            throw new Exception("Deserialized statistics is null.");
        }
        return statistics;
    }

    public static void SaveStatistics(List<Statistics> statistics)
    {
        string text = JsonConvert.SerializeObject(statistics, Formatting.Indented, Settings);
        File.WriteAllText(StatisticsPath, text);
    }
}
