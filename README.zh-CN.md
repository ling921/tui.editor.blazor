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

组件支持通过 `@ref` 访问编辑器/查看器事件和方法。更多 API 请查看 NuGet 包说明和源码中的 XML 文档。

## 开发

```shell
dotnet build ToastUIEditor.sln --configuration Release
dotnet test tests/ToastUIEditor.Tests/ToastUIEditor.Tests.csproj --configuration Release
```

GitHub Actions 会构建和测试 Pull Request；发布 GitHub Release 或手动运行发布工作流即可创建并发布 NuGet 包。

## 许可证

本项目采用 [MIT 许可证](LICENSE)。
