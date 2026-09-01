# ToastUI Editor Blazor

English | 简体中文

`tui.editor.blazor` 是 [Toast UI Editor](https://github.com/nhn/tui.editor) 的 Blazor 封装，
支持 Blazor Server 和 Blazor WebAssembly，目标框架包括 .NET 6、.NET 7、.NET 8、.NET 9 和 .NET 10。

## 安装

```shell
dotnet add package ToastUIEditor
```

## 快速开始

在 `_Imports.razor` 中添加 `@using ToastUI`：

```razor
<Editor @bind-Value="content" Options="options" />
<Viewer Value="content" />

@code {
    private string content = "# 你好，Toast UI！";
    private EditorOptions options = new();
}
```

组件支持通过 `@ref` 访问编辑器和查看器的事件与方法。完整 API 请参考源码中的 XML 文档。

## 开发

```shell
dotnet build ToastUIEditor.sln --configuration Release
dotnet test tests/ToastUIEditor.Tests/ToastUIEditor.Tests.csproj --configuration Release
```

## 许可证

本项目采用 [MIT 许可证](LICENSE)。
