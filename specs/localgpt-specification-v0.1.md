# localGpt - Specification v0.1

A cross-platform desktop application (C# / .NET 8 + Avalonia UI) that lets users chat locally with OpenAI's Responses API, manage conversations as Git-friendly JSON files, and keep full control of cost, context and attachments.

---

## 1. Purpose and Vision

localGpt is a brainstorming and specification-writing companion. It removes the need for a browser-based UI so that every session (and its attachments) can be version-controlled as plain files. The app delivers:

- Omni-modal chat with GPT-4o (or any model listed in models.json) via the official OpenAI .NET SDK.
- Self-contained operation—no Git commands, no cloud storage, no recording hardware; the user decides when and where to save.
- Precise context control—messages can be archived (excluded from prompts) or deleted/regenerated in bulk.
- Transparent cost tracking—token counts and money spent are fixed on message creation and always visible.
- Attachment-aware workflow—images, audio and documents travel with their session in a deterministic folder layout.
- Multilingual support—UI strings stored in language files with initial support for en-us and ja-jp.

---

## 2. Key Features

| Category | Details |
|----------|---------|
| Chat core | GPT-4o through Responses API; optional Web Search tool when the "Search" checkbox is on. |
| Conversation management | Archive toggle; delete / regenerate from any user message → everything after it (incl. archived) is purged. |
| Attachments | Thumbnails for images, icon + duration for audio, icon + name for other files. Saved under {sessionBasename}_files/. |
| Cost & tokens | Prompt/Completion token counts + costs are stored per message, summed live in the main window. Models without both prices are greyed-out and unselectable. |
| Full-text search | Instant search across active & archived messages and attachments' OCR / text. |
| Localization | Language files for en-us (default) and ja-jp with configuration option to switch between them. |
| HTML export (opt-in) | One-click, pretty, static reproduction of the whole conversation next to the JSON. |
| Logging (debug) | Continuous ND-JSON stream of every request / token-stream event in logs/. |

---

## 3. Domain Model

### 3.1 Core Classes

#### SessionFile
The root container for a conversation session.
- **Id**: Guid - Unique identifier
- **Title**: string - User-friendly name for the session
- **SystemPrompt**: string - Initial instructions for the AI
- **Messages**: List<Message> - Collection of all messages in the session
- **Parameters**: List<ParameterSetting> - Configuration parameters for the session
- **StateMeta**: AppStateMeta - Metadata about the session state

#### Message
Represents a single message in the conversation.
- **Id**: Guid - Unique identifier
- **Role**: Role - Who sent the message (User, Assistant, System, Tool)
- **Text**: string - Content of the message
- **Timestamp**: DateTimeOffset - When the message was created (UTC)
- **Archived**: bool - Whether the message is excluded from context
- **Attachments**: List<IAttachment> - Files attached to the message
- **Model**: string - AI model used for this message
- **PromptTokens**: int - Number of tokens in the prompt
- **CompletionTokens**: int - Number of tokens in the completion
- **PromptCost**: double - Cost of the prompt tokens
- **CompletionCost**: double - Cost of the completion tokens
- **TotalCost**: double - Total cost of the message

#### AppStateMeta
Tracks aggregate statistics for the session.
- **SelectedModel**: string - Currently selected AI model
- **TotalPromptTokens**: int - Sum of all prompt tokens
- **TotalCompletionTokens**: int - Sum of all completion tokens
- **TotalPromptCost**: double - Sum of all prompt costs
- **TotalCompletionCost**: double - Sum of all completion costs
- **TotalCost**: double - Total cost of the session

### 3.2 Parameters and Models

#### ParameterSetting
Configurable parameter for the session.
- **Key**: string - Parameter name
- **Value**: string - Parameter value
- **Enabled**: bool - Whether the parameter is active

#### ModelInfo
Information about an AI model.
- **Name**: string - Model identifier
- **PromptPricePer1M**: double? - Cost per million prompt tokens
- **CompletionPricePer1M**: double? - Cost per million completion tokens
- **IsPricingConfigured**: bool - Whether pricing is set up

### 3.3 Attachments

#### IAttachment (interface)
Common interface for all attachment types.
- **Id**: Guid - Unique identifier
- **FileName**: string - Original file name
- **Type**: AttachmentType - Type of attachment
- **RelativePath**: string - Path relative to session folder

#### AttachmentType (enum)
- **Image** - Image files (png, jpg, etc.)
- **Audio** - Audio files (mp3, wav, etc.)
- **Document** - Document files (pdf, docx, etc.)
- **Other** - Other file types

#### Concrete Attachment Types
- **ImageAttachment** - Implements IAttachment for images
- **AudioAttachment** - Implements IAttachment for audio
- **DocumentAttachment** - Implements IAttachment for documents

#### Role (enum)
- **User** - Message from the human user
- **Assistant** - Message from the AI assistant
- **System** - System message (instructions)
- **Tool** - Message from a tool (e.g., web search)

### 3.4 Class Relationships

1. **SessionFile to Message** (Composition, One-to-Many)
   - A SessionFile contains multiple Messages
   - Each Message belongs to exactly one SessionFile
   - Messages cannot exist independently of their SessionFile

2. **Message to IAttachment** (Association, Many-to-Many)
   - Messages can reference multiple Attachments
   - An Attachment can be referenced by multiple Messages
   - This allows sharing attachments between messages

3. **SessionFile to ParameterSetting** (Aggregation, One-to-Many)
   - A SessionFile contains multiple ParameterSettings
   - Each ParameterSetting belongs to one SessionFile
   - Less strict ownership than composition

4. **SessionFile to AppStateMeta** (Composition, One-to-One)
   - A SessionFile contains exactly one AppStateMeta
   - The AppStateMeta cannot exist without its SessionFile
   - Strong ownership relationship

5. **IAttachment to Concrete Attachments** (Implementation)
   - ImageAttachment, AudioAttachment, and DocumentAttachment all implement the IAttachment interface
   - Each provides type-specific functionality while sharing common properties

### 3.5 Implementation Notes

- Modern collection types (List<T>) are used instead of IList<T> for better performance and compatibility.
- Timestamps use DateTimeOffset for round-trippable ISO-8601 values. All timestamps are stored in UTC internally, while local time may be used for display purposes.
- Costs are calculated once from the ModelInfo prices and never recomputed.

---

## 4. Persistent Files

| File | Format | When created |
|------|--------|--------------|
| *.json | UTF-8, indented, SessionFile root | On Save / Save As |
| {basename}_files/ | raw attachments | When first file attached |
| models.json | Array<ModelInfo> | Comes with app; editable in "Model Manager" dialog |
| localization/{lang-code}.json | Localization strings | Included with app (en-us, ja-jp) |
| Optional *.htm | self-contained | When "Export HTML" toggled |
| logs/{yyyyMMdd}.ndjson | ND-JSON events | Always, for debugging |

---

## 5. User Interface

### 5.1 Main Window (single session)

```
┌─────────────────────────────────────────────────────────────────────────┐
│ File | Edit | View | Tools | Settings | Help                            │
├─────────────────────────────────────────────────────────────────────────┤
│ Model selector [Combo]   ☐ Search   │  Parameter list [+] [✎] [−]       │
├─────────────────────────────────────────────────────────────────────────┤
│                     User Input TextBox (multiline)  [Send]              │
├─────────────────────────────────────────────────────────────────────────┤
│ Non-Archived (left)           │ Archived (right)                        │
│  ────────────────             │ ────────────────                        │
│  • message card               │  • message card                         │
│    text                       │    text                                 │
│    thumbnails / icons         │    …                                    │
│    tokens + cost              │                                         │
└─────────────────────────────────────────────────────────────────────────┘
  Total Prompt Tokens / Total Completion Tokens | Prompt Cost / Completion Cost | Total Cost
```

Message cards scroll freely (CanContentScroll = false) so long answers are never clipped.

### 5.2 Menus (minimum)

| Menu | Items |
|------|-------|
| File | New Window Ctrl+N, Open Ctrl+O, Save Ctrl+S, Save As Ctrl+Shift+S, Exit |
| Edit | Copy, Paste, Delete, Regenerate Ctrl+R, Undo/Redo (text box only) |
| View | Toggle model/parameter panes, toggle meta info |
| Tools | Full-text Search, Token-price Quick Chart, HTML Export |
| Settings | Model Manager (add/edit prices), Language (en-us/ja-jp) |
| Help | About, Shortcuts, Open Config Folder |

Shortcuts list is embedded in Help and mirrors the table given in the conversation.

### 5.3 Dialogs & Panels

- Parameter Panel — permanent; items grey when Enabled == false. (+ / edit / − buttons).
- Model Manager — data-grid bound to ModelInfo list; rows without both prices are shaded/disabled.
- Language Settings — allows switching between available language files (en-us, ja-jp).
- Save As — initial filename comes from session Title → kebab-case ASCII. Titles typed in Japanese invoke automatic English suggestion via GPT.

---

## 6. Business Rules

1. Self-contained operation — no file is generated on app start; saving occurs only on explicit Save / Save As or after a send/receive event.
2. Conversation management — deletion / regeneration cascades forward from the selected user message.
3. Cost immutability — once a message is logged, its costs never change, even if model prices are edited later.
4. Model choice gating — selector disables models lacking price data.
5. Time handling — all timestamps are stored in UTC internally while displayed in local time where appropriate.
6. Attachments location — always inside {basename}_files/; session directories are created lazily.

---

## 7. External Dependencies

| Layer | Library | Notes |
|-------|---------|-------|
| UI | Avalonia UI 11 | cross-platform, custom docking |
| HTTP & DTO | OpenAI .NET prerelease | official Responses API client |
| HTTP helper | Refit | declarative interfaces |
| JSON | System.Text.Json | JsonSerializerOptions.Default + JsonPolymorphic for attachments |
| Time | .NET DateTimeOffset | precision + round-trip |
| Localization | Custom ResourceManager | loads strings from language JSON files |

---

## 8. Open Points (future versions)

- WYSIWYG layout for HTML export
- Live token cost chart
- Token-level diff when regenerating
- Drag-and-drop between session windows
- OAuth-free local voice capture / camera input
- Additional language support beyond en-us and ja-jp

---

## 9. Appendices

### A. Sample models.json

```json
[
    {
        "Name": "gpt-4o",
        "PromptPricePer1M": 5.00,
        "CompletionPricePer1M": 15.00
    },
    {
        "Name": "gpt-4.1",
        "PromptPricePer1M": 2.00,
        "CompletionPricePer1M": 8.00
    }
]
```

### B. Sample message excerpt

```json
{
    "id": "b7da…",
    "role": "assistant",
    "text": "Sure, here's a summary …",
    "timestamp": "2025-04-21T10:30:45Z",
    "model": "gpt-4o",
    "promptTokens": 120,
    "completionTokens": 440,
    "promptCost": 0.00060,
    "completionCost": 0.00660
}
```

### C. Sample localization file (en-us.json)

```json
{
    "app.title": "localGpt",
    "menu.file": "File",
    "menu.edit": "Edit",
    "menu.view": "View",
    "menu.tools": "Tools",
    "menu.settings": "Settings",
    "menu.help": "Help",
    "button.send": "Send",
    "label.search": "Search",
    "status.totalPromptTokens": "Total Prompt Tokens",
    "status.totalCompletionTokens": "Total Completion Tokens",
    "status.promptCost": "Prompt Cost",
    "status.completionCost": "Completion Cost",
    "status.totalCost": "Total Cost"
}
```

---

End of localGpt specification v0.1