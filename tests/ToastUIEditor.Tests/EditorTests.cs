using System.Text.Json;
using ToastUI;

namespace ToastUIEditor.Tests;

public class EditorTests
{
    [Fact]
    public void EditorOptions_HaveExpectedDefaults()
    {
        var options = new EditorOptions();

        Assert.Equal("300px", options.Height);
        Assert.Equal("200px", options.MinHeight);
        Assert.Equal(EditorType.Markdown, options.InitialEditType);
        Assert.Equal(6, options.ToolbarItems.Length);
        Assert.True(options.Autofocus);
        Assert.True(options.UsageStatistics);
    }

    [Fact]
    public void EditorOptions_SerializeEnumValuesAndDictionaryKeys()
    {
        var options = new EditorOptions
        {
            InitialEditType = EditorType.WYSIWYG,
            Theme = Theme.Dark,
            LinkAttributes = new() { [LinkAttributeNames.Target] = "_blank" }
        };

        using var json = JsonDocument.Parse(JsonSerializer.Serialize(options, JsonSerializerOptions.Web));
        var root = json.RootElement;

        Assert.Equal("wysiwyg", root.GetProperty("initialEditType").GetString());
        Assert.Equal("dark", root.GetProperty("theme").GetString());
        Assert.Equal("_blank", root.GetProperty("linkAttributes").GetProperty("target").GetString());
    }

    [Fact]
    public void EditorOptions_DeserializeEnumValuesAndDictionaryKeys()
    {
        var options = JsonSerializer.Deserialize<EditorOptions>(
            "{\"initialEditType\":\"wysiwyg\",\"theme\":\"dark\",\"linkAttributes\":{\"target\":\"_blank\"}}", JsonSerializerOptions.Web)!;

        Assert.Equal(EditorType.WYSIWYG, options.InitialEditType);
        Assert.Equal(Theme.Dark, options.Theme);
        Assert.Equal("_blank", options.LinkAttributes![LinkAttributeNames.Target]);
    }

    [Fact]
    public void Editor_SetHeightRejectsEmptyValue()
    {
        var editor = new Editor();

        Assert.Throws<ArgumentException>(() => editor.SetHeight(string.Empty));
    }

    [Fact]
    public void Editor_SetSelectionRejectsInvalidRange()
    {
        var editor = new Editor();

        Assert.Throws<ArgumentOutOfRangeException>(() => editor.SetSelection(2, 1));
    }

}
