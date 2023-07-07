# ToastUI Editor Blazor

`TUIEditor.Blazor` is a Blazor component that provides a Markdown editor based on the ToastUI Editor library.

The original library is [tui.editor](https://github.com/nhn/tui.editor)

## Installation

Install the package using following command:

``` Package Manager
Install-Package TUIEditor.Blazor
```

or

``` powershell
dotnet add package TUIEditor.Blazor
```

or just use nuget package manager.

## Usage

1. Add the following import statement to your `_Imports.razor` file

``` razor
@using ToastUI
```

2. Use the `Editor` component in your Blazor page or component

``` razor
<Editor @bind-Value="content" Options="@options" />
```

- `@bind-Value`: Binds the editor's content to a string property in your Blazor component.
- `Options`: Sets the configuration options for the editor. Refer to the `EditorOptions` class for available options.

3. Optional, Handle the available events by specifying event callbacks

``` razor
<Editor @bind-Value="content"
        OnLoad="HandleLoad"
        OnChange="HandleChange"
        OnCaretChange="HandleCaretChange"
        OnFocus="HandleFocus"
        OnBlur="HandleBlur"
        OnKeydown="HandleKeydown"
        OnKeyup="HandleKeyup"
        BeforePreviewRender="HandleBeforePreviewRender"
        BeforeConvertWysiwygToMarkdown="HandleBeforeConvertWysiwygToMarkdown" />
```

These events are the same as the native public events, and the parameters are detailed in the comments.

4. Optional, Access the TUIEditor instance to invoke methods

``` razor
<Editor @ref="editorRef" @bind-Value="content" Options="@options" />

<button @onclick="HandleButtonClick">Get Markdown</button>

@code {
    Editor editorRef = default!;
    
    async Task HandleButtonClick()
    {
        var markdown = await editorRef.GetMarkdown();
        var html = await editorRef.GetHTML();
        ...
    }
}
```

Most of all native methods have been implemented. Refer to the Editor class for available methods.

## License

This software is licensed under the [MIT License](LICENSE)
