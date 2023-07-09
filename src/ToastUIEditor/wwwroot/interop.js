import "./toastui-editor-all.min.js";

function getPrefersColorScheme() {
  const colorScheme = window.matchMedia("(prefers-color-scheme: dark)");
  return colorScheme.matches ? "dark" : "light";
}

function getKeyboardEventArgs(event) {
  return {
    key: event.key,
    code: event.code,
    location: event.location,
    repeat: event.repeat,
    ctrlKey: event.ctrlKey,
    shiftKey: event.shiftKey,
    altKey: event.altKey,
    metaKey: event.metaKey,
    type: event.type,
  };
}

function decycle(obj, stack = new Set(), refs = [], idMap = new WeakMap()) {
  if (!obj || typeof obj !== "object") return obj;

  if (stack.has(obj)) {
    const index = refs.indexOf(obj);
    const id = `${index + 1}`;
    idMap.set(obj, id);
    return { $ref: id };
  }

  let newObj;
  const id = refs.length;
  refs.push(obj);

  if (Array.isArray(obj)) {
    newObj = [];
    stack.add(obj);
    for (let i = 0; i < obj.length; i++)
      newObj[i] = decycle(obj[i], stack, refs, idMap);
    stack.delete(obj);
  } else {
    newObj = {};
    stack.add(obj);
    for (let key in obj) {
      if (obj.hasOwnProperty(key))
        newObj[key] = decycle(obj[key], stack, refs, idMap);
    }
    stack.delete(obj);
  }

  if (idMap.has(obj)) newObj.$id = idMap.get(obj);

  return newObj;
}

function escapeRegExp(string) {
  return string.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

export class ToastUI {
  instance = null;
  type = "editor";
  dotNetRefs = [];
  constructor(options) {
    if (!options.el) {
      throw new Error("element is required in options");
    }

    if (options.viewer) {
      options.events = this._viewerEvents(options.ref);
      this.type = "viewer";
    } else {
      options.events = this._editorEvents(options.ref);
      options.widgetRules = this._resolveEditorWidgetRules(options.widgetRules);
      this.type = "editor";
    }

    options.theme =
      options.theme === "auto"
        ? getPrefersColorScheme()
        : options.theme || "light";

    console.log(`Init ${this.type} with options: `, options);
    this.instance = toastui.Editor.factory(options);
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
  _commonEvents(dotNetRef) {
    if (!dotNetRef) {
      throw new Error("ref in options is required.");
    }
    return {
      load: (editor) => {
        dotNetRef.invokeMethodAsync("load");
      },
    };
  }
  _editorEvents(dotNetRef) {
    return {
      ...this._commonEvents(dotNetRef),
      change: (editorType) => {
        const value =
          editorType === "wysiwyg"
            ? this.instance.getHTML()
            : this.instance.getMarkdown();
        dotNetRef.invokeMethodAsync("change", editorType, value);
      },
      caretChange: (editorType) => {
        dotNetRef.invokeMethodAsync("caretChange", editorType);
      },
      focus: (editorType) => {
        dotNetRef.invokeMethodAsync("focus", editorType);
      },
      blur: (editorType) => {
        dotNetRef.invokeMethodAsync("blur", editorType);
      },
      keydown: (editorType, ev) => {
        dotNetRef.invokeMethodAsync(
          "keydown",
          editorType,
          getKeyboardEventArgs(ev)
        );
      },
      keyup: (editorType, ev) => {
        dotNetRef.invokeMethodAsync(
          "keyup",
          editorType,
          getKeyboardEventArgs(ev)
        );
      },
      beforePreviewRender: (html) => {
        dotNetRef.invokeMethodAsync("beforePreviewRender", html);
        return html;
      },
      beforeConvertWysiwygToMarkdown: (markdownText) => {
        dotNetRef.invokeMethodAsync(
          "beforeConvertWysiwygToMarkdown",
          markdownText
        );
        return markdownText;
      },
    };
  }
  _viewerEvents(dotNetRef) {
    return {
      ...this._commonEvents(dotNetRef),
      change: (...args) => {
        console.log("viewer change", args);
        dotNetRef.invokeMethodAsync("change", args);
      },
      updatePreview: (editResult) => {
        dotNetRef.invokeMethodAsync("updatePreview", decycle(editResult));
      },
    };
  }
  _resolveEditorWidgetRules(widgetRules) {
    if (widgetRules && Array.isArray(widgetRules)) {
      return widgetRules.map((item) => {
        this.dotNetRefs.push(item.ref);
        return {
          rule: new RegExp(item.rule, "m"),
          toDOM: (text) => {
            const domText = item.ref.invokeMethod("toDOM", text);
            const element = document.createElement("span");
            element.innerHTML = domText;
            return element;
          }
        };
      });
    } else {
      return null;
    }
  }
  destroy() {
    this.instance?.destroy();
    this.dotNetRefs.forEach((ref) => {
      ref.dispose();
    });
  }
}
