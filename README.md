# ToastUI Editor Blazor [![NuGet](https://img.shields.io/nuget/v/ToastUIEditor.svg)](https://www.nuget.org/packages/ToastUIEditor/)

English | [简体中文](README.zh-CN.md)

`tui.editor.blazor` is a Blazor wrapper for the [Toast UI Editor](https://github.com/nhn/tui.editor).
It supports Blazor Server and Blazor WebAssembly on .NET 6, .NET 7, .NET 8, .NET 9, and .NET 10.

## Installation

```shell
dotnet add package ToastUIEditor
```

## Quick start

Add `@using ToastUI` to `_Imports.razor`:

```razor
<Editor @bind-Value="content" Options="options" />
<Viewer Value="content" />

@code {
    private string content = "# Hello, Toast UI!";
    private EditorOptions options = new();
}
```

The components expose editor/viewer events and methods through `@ref`. See the package README for
a concise API example and the source code XML documentation for the complete API.

## Development

```shell
dotnet build ToastUIEditor.sln --configuration Release
dotnet test tests/ToastUIEditor.Tests/ToastUIEditor.Tests.csproj --configuration Release
```

GitHub Actions build and test pull requests. Publishing a GitHub Release or manually dispatching
the publish workflow creates and publishes the NuGet package.

## License

This project is licensed under the [MIT License](LICENSE).
