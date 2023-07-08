import "./toastui-editor-all.min.js";

export class ToastUI {
  instance = null;
  type = "editor";
  dotNetRefs = [];
  constructor(options) {
    if (!options.el) {
      throw new Error("element is required in options");
    }

    options.events = Object.assign(
      {},
      ...Object.keys(options.events || {}).map((key) => {
        const handler = options.events[key];
        this.dotNetRefs.push(handler);
        return {
          [key]: (...args) => {
            // console.log(`${this.type} event ${key} fired`, args);
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

    if (options.hasOwnProperty("previewStyle")) {
      options.previewStyle = options.previewStyle?.toLowerCase() || "tab";
    }
    if (options.hasOwnProperty("initialEditType")) {
      options.initialEditType =
        options.initialEditType?.toLowerCase() || "markdown";
    } else {
      options.viewer = true;
      this.type = "viewer";
    }
    let theme = options.theme?.toLowerCase();
    if (theme === "auto") {
      const colorScheme = window.matchMedia("(prefers-color-scheme: dark)");
      theme = colorScheme.matches ? "dark" : "light";
    }
    options.theme = theme === "dark" ? "dark" : "light";

    console.log(`ToastUI initial options: `, options);
    this.instance = toastui.Editor.factory(options);
    window.editor = this.instance;
  }
  static factory(options) {
    return new ToastUI(options);
  }
  static setLanguage(code, data) {
    toastui.Editor.setLanguage(code, data);
  }
  static setLanguages(languages) {
    Object.keys(languages).forEach((key) => {
      ToastUI.setLanguage(key, languages[key]);
    });
  }
  static setDefaultLanguage(code) {
    toastui.Editor.i18n.setCode(code);
  }
  destroy() {
    this.instance?.destroy();
    this.dotNetRefs.forEach((ref) => {
      ref.dispose();
    });
  }
}
