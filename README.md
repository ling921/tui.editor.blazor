# ToastUI Editor Blazor

`tui.editor.blazor` is a Blazor component that provides a Markdown editor based on the [tui.editor](https://github.com/nhn/tui.editor) library.


## Installation

Install the package using following command:

``` Package Manager
Install-Package ToastUIEditor
```

or

``` powershell
dotnet add package ToastUIEditor
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


3. Use the `Viewer` component in your Blazor page or component

``` razor
<Viewer Value="@content" />
```

- `Value`: Sets the content to be displayed in the viewer. It will update automatically when `content` changes.


4. Handle the available events by specifying event callbacks

``` razor
<Editor @bind-Value="content"
        Load="HandleLoad"
        Change="HandleChange"
        CaretChange="HandleCaretChange"
        Focus="HandleFocus"
        Blur="HandleBlur"
        KeyDown="HandleKeyDown"
        KeyUp="HandleKeyUp"
        BeforePreviewRender="HandleBeforePreviewRender"
        BeforeConvertWYSIWYGToMarkdown="HandleBeforeConvertWYSIWYGToMarkdown" />

<Viewer Value="content"
        Load="HandleLoad"
        Change="HandleChange"
        UpdatePreview="HandleUpdatePreview" />
```

These events are the same as the native public events, and the parameters are detailed in the code comments.


5. Access the `Editor` or `Viewer` instance to invoke methods

``` razor
<Editor @ref="editorRef" @bind-Value="markdown" Options="@options" />

<Editor @ref="viewerRef" Value="markdown" />

<button @onclick="HandlePreview">Preview</button>

<Editor @ref="viewerRef2"/>

@code {
    Editor editorRef = default!;
    Viewer viewerRef = default!;
    Viewer viewerRef2 = default!;
    string markdown = string.Empty;
    
    async Task HandlePreview()
    {
        var markdown = await editorRef.GetMarkdown();
        await viewerRef2.SetMarkdown(markdown);
    }
}
```

Most of all native methods have been implemented. Refer to the Editor class for available methods.


6. Add custom language

- Use `Editor.SetLanguage` static method to add custom language.
- Use `Editor.SetDefaultLanguage` static method to set default language, it will be used when no language is set in `EditorOptions`.

> Note: Please make sure Editor.SetLanguage and Editor.SetDefaultLanguage are called before `Editor` component is rendered.


7. Widget rules

Due to `BlazorServer` mode not supporting JavaScript call .NET method synchronously, the widget rules only support in `BlazorWebAssembly` mode.


## Implemented Features

- [x] `Editor` and `Viewer` basic usage
- [x] `Editor` and `Viewer` events
- [x] Language setting and custom language
- [x] `Editor` and `Viewer` instance methods
- [ ] Toolbar with custom button
- [ ] Add command and execute command
- [x] Add widget and set widget rules (only support in `BlazorWebAssembly` mode)
- [x] Link attributes
- [ ] Custom markdown renderer
- [ ] Custom HTML renderer
- [ ] Custom HTML Sanitizer


## License

This software is licensed under the [MIT License](LICENSE)
