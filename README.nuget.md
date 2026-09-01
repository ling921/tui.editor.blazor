# ToastUI Editor Blazor

English | [简体中文](README.nuget.zh-CN.md)

`ToastUIEditor` provides Toast UI Editor components for Blazor Server and Blazor WebAssembly.
It supports .NET 6, .NET 7, .NET 8, .NET 9, and .NET 10.

## Installation

```shell
dotnet add package ToastUIEditor
```

## Quick start

Add `@using ToastUI` to `_Imports.razor`, then use the editor or viewer:

```razor
<Editor @bind-Value="content" Options="options" />
<Viewer Value="content" />

@code {
    private string content = "# Hello, Toast UI!";
    private EditorOptions options = new();
}
```

Use `@ref` to access component methods such as `GetMarkdown`, `SetMarkdown`, and `GetHTML`.
See the [repository README](https://github.com/ling921/tui.editor.blazor) for complete usage,
events, languages, and widget rules.

## 简体中文

`ToastUIEditor` 为 Blazor Server 和 Blazor WebAssembly 提供 Toast UI Editor 组件，支持
.NET 6、.NET 7、.NET 8、.NET 9 和 .NET 10。

### 安装

```shell
dotnet add package ToastUIEditor
```

### 快速开始

在 `_Imports.razor` 中添加 `@using ToastUI`，然后使用编辑器或查看器：

```razor
<Editor @bind-Value="content" Options="options" />
<Viewer Value="content" />

@code {
    private string content = "# 你好，Toast UI！";
    private EditorOptions options = new();
}
```

可以通过 `@ref` 调用 `GetMarkdown`、`SetMarkdown` 和 `GetHTML` 等组件方法。完整用法请
参考[仓库 README](https://github.com/ling921/tui.editor.blazor)。
