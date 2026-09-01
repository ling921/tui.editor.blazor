# ToastUI Editor Blazor

English | 简体中文

`ToastUIEditor` 为 Blazor Server 和 Blazor WebAssembly 提供 Toast UI Editor 组件，支持
.NET 6、.NET 7、.NET 8、.NET 9 和 .NET 10。

## 安装

```shell
dotnet add package ToastUIEditor
```

## 快速开始

在 `_Imports.razor` 中添加 `@using ToastUI`，然后使用 `<Editor>` 或 `<Viewer>` 组件：

```razor
<Editor @bind-Value="content" Options="options" />
<Viewer Value="content" />

@code {
    private string content = "# 你好，Toast UI！";
    private EditorOptions options = new();
}
```

可以通过 `@ref` 调用 `GetMarkdown`、`SetMarkdown` 和 `GetHTML` 等方法。更多事件、语言和
Widget Rule 用法，请参考[仓库 README](https://github.com/ling921/tui.editor.blazor)。
