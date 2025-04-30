localGpt - Specification v0.1

A cross-platform desktop application (C# / .NET 8 + Avalonia UI) that lets users chat locally with OpenAI’s Responses API, manage conversations as Git-friendly JSON files, and keep full control of cost, context and attachments.

⸻

1  Purpose and Vision v0.1

localGpt is a brainstorming and specification-writing companion.
It removes the need for a browser-based UI so that every session (and its attachments) can be version-controlled as plain files. The app delivers:
	•	Omni-modal chat with GPT-4o (or any model listed in models.json) via the official OpenAI .NET SDK.
	•	Precise context control—messages can be archived (excluded from prompts) or deleted/regenerated in bulk.
	•	Transparent cost tracking—token counts and money spent are fixed on message creation and always visible.
	•	Attachment-aware workflow—images, audio and documents travel with their session in a deterministic folder layout.
	•	Self-contained operation—no Git commands, no cloud storage, no recording hardware; the user decides when and where to save.

⸻

2  Key Features

Category	Details
Chat core	GPT-4o through Responses API; optional Web Search tool when the “Search” checkbox is on.ocalGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
Conversation management	Archive toggle; delete / regenerate from any user message → everything after it (incl. archived) is purged.ocalGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
Cost & tokens	Prompt/Completion token counts + costs are stored per message, summed live in the main window. Models without both prices are greyed-out and unselectable.
Attachments	Thumbnails for images, icon + duration for audio, icon + name for other files. Saved under {sessionBasename}_files/.localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
Full-text search	Instant search across active & archived messages and attachments’ OCR / text.localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
HTML export (opt-in)	One-click, pretty, static reproduction of the whole conversation next to the JSON.localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
Logging (debug)	Continuous ND-JSON stream of every request / token-stream event in logs/.localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)



⸻

3  Domain Model

classDiagram
    class SessionFile {
        +Guid Id
        +string Title
        +string SystemPrompt
        +IList~Message~ Messages
        +IList~ParameterSetting~ Parameters
        +AppStateMeta StateMeta
    }

    class Message {
        +Guid Id
        +Role Role
        +string Text
        +DateTimeOffset Timestamp
        +bool Archived
        +IList~IAttachment~ Attachments
        +string Model
        +int PromptTokens
        +int CompletionTokens
        +double PromptCost
        +double CompletionCost
        +double TotalCost
    }

    enum Role {User; Assistant; System; Tool}
    interface IAttachment {
        <<interface>>
        +Guid Id
        +string FileName
        +AttachmentType Type
        +string RelativePath
    }
    enum AttachmentType {Image; Audio; Document; Other}
    class ImageAttachment
    class AudioAttachment
    class DocumentAttachment

    class ParameterSetting {
        +string Key
        +string Value
        +bool Enabled
    }

    class ModelInfo {
        +string Name
        +double? PromptPricePer1k
        +double? CompletionPricePer1k
        +bool IsPricingConfigured
    }

    class AppStateMeta {
        +string SelectedModel
        +int TotalPromptTokens
        +int TotalCompletionTokens
        +double TotalPromptCost
        +double TotalCompletionCost
        +double TotalCost
    }

    SessionFile "1" *-- "*" Message
    Message "0..*" --> "0..*" IAttachment
    SessionFile "1" o-- "*" ParameterSetting

	•	All collections expose IList<T> to stay test-friendly and allow ObservableCollection later.ocalGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
	•	Timestamps use DateTimeOffset for round-trippable ISO-8601 values.ocalGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
	•	Costs are calculated once from the ModelInfo prices and never recomputed.localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)

⸻

4  Persistent Files

File	Format	When created
*.json	UTF-8, indented, SessionFile root	On Save / Save As
{basename}_files/	raw attachments	When first file attached
models.json	Array<ModelInfo>	Comes with app; editable in “Model Manager” dialog
logs/{yyyyMMdd}.ndjson	ND-JSON events	Always, for debugging
Optional *.html	self-contained	When “Export HTML” toggled



⸻

5  User Interface

5.1 Main Window (single session)

┌ MenuBar ─────────────────────────────────────────────────────────────────┐
│ File | Edit | View | Tools | Settings | Help                            │
├──────────────────────────────────────────────────────────────────────────┤
│ Model selector [Combo]   ☐ Search   │  Parameter list [+] [✎] [−]       │
├──────────────────────────────────────────────────────────────────────────┤
│                     User Input TextBox (multiline)  [Send]              │
├──────────────────────────────────────────────────────────────────────────┤
│ Non-Archived (left)           | Archived (right)                        │
│  ────────────────             | ────────────────                        │
│  • message card               |  • message card                         │
│    text                       |    text                                 │
│    thumbnails / icons         |    …                                    │
│    tokens + cost              |                                          │
└──────────────────────────────────────────────────────────────────────────┘
Status bar: ΣPromptTok / ΣCompTok | ΣPrompt$ / ΣComp$ | ΣTotal$

Message cards scroll freely (CanContentScroll = false) so long answers are never clipped.localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)

5.2 Menus (minimum)

Menu	Items
File	New Window Ctrl+N, Open Ctrl+O, Save Ctrl+S, Save As Ctrl+Shift+S, Exit
Edit	Copy, Paste, Delete, Regenerate Ctrl+R, Undo/Redo (text box only)
View	Toggle model/parameter panes, toggle meta info
Tools	Full-text Search, Token-price Quick Chart, HTML Export
Settings	Model Manager (add/edit prices)
Help	About, Shortcuts, Open Config Folder

Shortcuts list is embedded in Help and mirrors the table given in the conversation.‡localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)

5.3 Dialogs & Panels
	•	Model Manager — data-grid bound to ModelInfo list; rows without both prices are shaded/disabled.‡localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
	•	Parameter Panel — permanent; items grey when Enabled == false. (+ / edit / − buttons).localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
	•	Save As — initial filename comes from session Title → kebab-case ASCII. Titles typed in Japanese invoke automatic English suggestion via GPT.‡localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)

⸻

6  Business Rules
	1.	Saving occurs only on explicit Save / Save As or after a send/receive event. No file is generated on app start.‡localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
	2.	Deletion / Regeneration cascades forward from the selected user message.localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
	3.	Cost immutability — once a message is logged, its costs never change, even if model prices are edited later.‡localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
	4.	Model choice gating — selector disables models lacking price data.‡localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)
	5.	Attachments location — always inside {basename}_files/; session directories are created lazily.‡localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)

⸻

7  External Dependencies

Layer	Library	Notes
HTTP & DTO	OpenAI .NET prerelease	official Responses API client
HTTP helper	Refit	declarative interfaces
JSON	System.Text.Json	JsonSerializerOptions.Default + JsonPolymorphic for attachments
UI	Avalonia UI 11	cross-platform, custom docking
Time	.NET DateTimeOffset	precision + round-trip



⸻

8  Open Points (future versions)
	•	OAuth-free local voice capture / camera input
	•	Drag-and-drop between session windows
	•	Token-level diff when regenerating
	•	WYSIWYG layout for HTML export
	•	Live token cost chart

⸻

9  Appendices

A. Sample models.json

[
  { "Name": "gpt-4o",  "PromptPricePer1k": 0.005, "CompletionPricePer1k": 0.015 },
  { "Name": "gpt-4.1", "PromptPricePer1k": 0.002, "CompletionPricePer1k": 0.008 }
]

B. Sample message excerpt

{
  "id": "b7da…",
  "role": "assistant",
  "text": "Sure, here’s a summary …",
  "timestamp": "2025-04-21T10:30:45Z",
  "model": "gpt-4o",
  "promptTokens": 120,
  "completionTokens": 440,
  "promptCost": 0.00060,
  "completionCost": 0.00660
}

‡localGpt-conversation.json](file-service://file-M4mCGpHokojUFNkd5RAce5)

⸻

End of localGpt specification v0.1