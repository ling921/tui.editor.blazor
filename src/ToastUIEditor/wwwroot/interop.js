import "./toastui-editor-all.min.js";

export class Editor extends toastui.Editor {
  dotNetRefs = [];
  constructor(options) {
    if (!options.el) {
      throw new Error("element is required in options");
    }

    const dotNetRefs = [];
    options.events = Object.assign(
      {},
      ...Object.keys(options.events || {}).map((key) => {
        const handler = options.events[key];
        dotNetRefs.push(handler);
        return {
          [key]: (...args) => {
            //console.log(`event ${key} fired`, args);
            if (key === "load") args = [];
            // 1. convert keyboard event to blazor event
            // 2. fill null until args.length to 2 for invoking C# method
            for (let i = 0; i < 2; i++) {
              if (args.length <= i) {
                args.push(null);
                continue;
              }
              const arg = args[i];
              if (arg instanceof KeyboardEvent) {
                args[i] = {
                  key: arg.key,
                  code: arg.code,
                  location: arg.location,
                  repeat: arg.repeat,
                  ctrlKey: arg.ctrlKey,
                  shiftKey: arg.shiftKey,
                  altKey: arg.altKey,
                  metaKey: arg.metaKey,
                  type: arg.type,
                };
              }
            }
            handler.invokeMethodAsync("InvokeAsync", ...args);

            // these events needs to return the value
            if (
              key === "beforePreviewRender" ||
              key === "beforeConvertWysiwygToMarkdown"
            ) {
              return args[0];
            }
          },
        };
      })
    );

    options.previewStyle = options.previewStyle?.toLowerCase() || "tab";
    options.initialEditType =
      options.initialEditType?.toLowerCase() || "markdown";
    let theme = options.theme?.toLowerCase();
    if (theme === "auto") {
      const colorScheme = window.matchMedia("(prefers-color-scheme: dark)");
      theme = colorScheme.matches ? "dark" : "light";
    }
    options.theme = theme === "dark" ? "dark" : "light";

    // console.log("Editor Initializing", options);

    super(options);

    this.dotNetRefs = dotNetRefs;
    window.editor = this;
  }
  destroy() {
    super.destroy();
    this.dotNetRefs.forEach((ref) => {
      ref.dispose();
    });
  }
}

export function initEditor(options) {
  return new Editor(options);
}

export function setLanguages(languages) {
  Object.keys(languages).forEach((key) => {
    const value = languages[key];
    toastui.Editor.setLanguage(key, value);
  });
}

export function setLanguage(key, value) {
  toastui.Editor.setLanguage(key, value);
}
