using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Domain.Entities;
public class ContainerInfo
{
    public required string DatabaseId { get; set; }
    public required string ContainerId { get; set; }
    public required string ValidationScheme { get; set; }
    public required PartitionKeyInfo PkInfo { get; set; }
}

public class PartitionKeyInfo
{
    public required string PartitionKey { get; set; }
    public required string[] PartitionKeyProperties { get; set; }
    public required string Template { get; set; }
    public required string Pattern { get; set; }

    public string ResolvePartitionKey(params string[] args)
    {
        string input = string.Format(Template, args);
        return !Regex.IsMatch(input, Pattern) ? throw new Exception("Error resolving partition key.") : input;
    }

    public string GetPrimaryKeyName()
    {
        // remove characters partition key, for sample: \listId -> listId
        var match = Regex.Match(PartitionKey, @"^[^a-zA-Z]*(\w+)[^a-zA-Z]*$");
        return match.Success
            ? match.Groups[1].Value
            : string.Empty;
    }

    public void MatchesPrimaryKey(JObject jsonObject, JToken pkArgs)
    {
        var args = PartitionKeyProperties.Where(property => jsonObject.ContainsKey(property))
                                         .Select(property => jsonObject[property]!.ToString())
                                         .ToList();

        var equal = ResolvePartitionKey([.. args]).ToString() != pkArgs.ToString()
            ? throw new Exception("Partition key of data does not match partition key of arguments.")
            : true;
    }

    public MemoryStream AddPartitionKeyToStream(Stream stream, JToken pkArgs)
    {
        var partitionKeyName = GetPrimaryKeyName();

        using var reader = new StreamReader(stream);
        var jsonObject = JObject.Parse(reader.ReadToEnd());
        MatchesPrimaryKey(jsonObject, pkArgs);
        jsonObject[partitionKeyName] = pkArgs;

        return new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));
    }
}
