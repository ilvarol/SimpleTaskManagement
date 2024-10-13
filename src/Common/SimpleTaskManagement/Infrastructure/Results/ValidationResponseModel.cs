using System.Text.Json.Serialization;

namespace SimpleTaskManagement.Common.Infrastructure.Results;

public class ValidationResponseModel
{
    public IEnumerable<string>? Errors { get; set; }

    public ValidationResponseModel()
    {
    }

    public ValidationResponseModel(IEnumerable<string> Errors)
    {
        this.Errors = Errors;
    }

    public ValidationResponseModel(string message) : this(new List<string>() { message })
    {
    }

    [JsonIgnore]
    public string FlattenErrors => Errors != null
        ? string.Join(Environment.NewLine, Errors)
        : string.Empty;
}
