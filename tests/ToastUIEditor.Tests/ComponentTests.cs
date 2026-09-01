using System.Text.Json;
using ToastUI;

namespace ToastUIEditor.Tests;

public class ComponentTests
{
    [Fact]
    public void ViewerOptions_HaveExpectedDefaults()
    {
        var options = new ViewerOptions();

        Assert.True(options.Viewer);
        Assert.True(options.UsageStatistics);
        Assert.Equal(Theme.Light, options.Theme);
    }

    [Fact]
    public void WidgetRule_InvokesDelegate()
    {
        using var rule = new WidgetRule("@\\w+", value => $"<b>{value}</b>");

        Assert.Equal("<b>@user</b>", rule.ToDOM("@user"));
    }

    [Fact]
    public void EditorLanguage_RejectsUnknownDefaultLanguage()
    {
        Assert.Throws<InvalidOperationException>(() => Editor.SetDefaultLanguage("not-a-language"));
    }

    [Fact]
    public void ViewerOptions_SerializeLinkAttributeKeys()
    {
        var options = new ViewerOptions
        {
            LinkAttributes = new() { [LinkAttributeNames.Rel] = "nofollow" }
        };

        using var json = JsonDocument.Parse(JsonSerializer.Serialize(options, JsonSerializerOptions.Web));

        Assert.Equal("nofollow", json.RootElement.GetProperty("linkAttributes").GetProperty("rel").GetString());
    }
}
