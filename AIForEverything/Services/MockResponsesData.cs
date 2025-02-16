namespace AIForEverything.Services;

public class MockResponsesData
{
    public List<MockResponse> Responses { get; set; } = new();
}
public class MockResponse
{
    public string Type { get; set; } = "";
    public string Content { get; set; } = "";
}