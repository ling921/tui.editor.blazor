# ToastUI Editor Blazor

`ToastUIEditor` is a Blazor wrapper for the [Toast UI Editor](https://github.com/nhn/tui.editor).
It supports Blazor Server and Blazor WebAssembly applications targeting .NET 6, .NET 7, .NET 8,
.NET 9, and .NET 10.

## Installation

```shell
dotnet add package ToastUIEditor
```

## Quick start

Add `@using ToastUI` to `_Imports.razor` and use the editor or viewer:

```razor
<Editor @bind-Value="content" Options="options" />
<Viewer Value="content" />

@code {
    private string content = "# Hello, Toast UI!";
    private EditorOptions options = new();
}
```

Use `@ref` to invoke component methods such as `GetMarkdown`, `SetMarkdown`, and `GetHTML`.
The components also expose editor and viewer events. See the repository documentation for
language registration, widget rules, and the complete API.

## Links

- [Repository](https://github.com/ling921/tui.editor.blazor)
- [Toast UI Editor](https://github.com/nhn/tui.editor)
- [NuGet](https://www.nuget.org/packages/ToastUIEditor/)
