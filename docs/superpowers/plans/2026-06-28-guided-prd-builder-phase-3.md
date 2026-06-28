# Guided PRD Builder Phase 3: AI Integration

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add AI-powered generation and suggestion capabilities for personas, stories, features, and content refinement.

**Architecture:** Backend AI endpoints using existing `ILlmProvider` interface. Frontend AI panel component with streaming responses. Redux state for AI suggestions.

**Tech Stack:** .NET 8.0, FastEndpoints, MediatR, ILlmProvider (Anthropic/OpenAI), React 18, TypeScript 5, Redux Toolkit

## Global Constraints

- Use existing `ILlmProvider` interface for LLM calls
- All AI endpoints require JWT authentication
- AI suggestions are stored temporarily in Redux, not persisted until user accepts
- Streaming responses for longer generations
- Backend port: 5153, Frontend port: 5173

---

### Task 1: AI Generate Persona Endpoint

**Files:**
- Create: `src/Backend/Braavo.Core/UseCases/AI/GeneratePersonaCommand.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/AI/GeneratePersonaEndpoint.cs`

**Interfaces:**
- Consumes: `ILlmProvider`, `IProductRepository`
- Produces: `POST /api/ai/generate-persona` → Generated persona data

**Implementation:**
- Takes a description string (e.g., "A busy project manager who needs to track multiple projects")
- Uses LLM to generate: name, role, technicalLevel, goals[], painPoints[], quote
- Returns structured persona data (not saved to DB - user must accept)

```csharp
public record GeneratePersonaCommand(
    Guid ProductId,
    Guid UserId,
    string Description
) : IRequest<GeneratePersonaResult>;

public record GeneratedPersona(
    string Name,
    string Role,
    string TechnicalLevel,
    string[] Goals,
    string[] PainPoints,
    string Quote
);

public record GeneratePersonaResult(
    bool Success,
    GeneratedPersona? Persona = null,
    string? Error = null
);
```

System prompt for LLM:
```
You are a product manager assistant. Generate a detailed user persona based on the description.
Return JSON with: name, role, technicalLevel (Low/Medium/High), goals (3-5 items), painPoints (3-5 items), quote.
```

- [ ] Step 1: Create GeneratePersonaCommand with handler
- [ ] Step 2: Create GeneratePersonaEndpoint
- [ ] Step 3: Test with backend running
- [ ] Step 4: Commit

---

### Task 2: AI Suggest Stories Endpoint

**Files:**
- Create: `src/Backend/Braavo.Core/UseCases/AI/SuggestStoriesCommand.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/AI/SuggestStoriesEndpoint.cs`

**Interfaces:**
- Consumes: `ILlmProvider`, `IProductRepository`, `IPersonaRepository`
- Produces: `POST /api/ai/suggest-stories` → Array of suggested stories

**Implementation:**
- Takes personaId and optional context
- Fetches persona details, uses LLM to suggest 3-5 user stories
- Returns array of story suggestions with asA, iWant, soThat, priority, acceptanceCriteria

```csharp
public record SuggestStoriesCommand(
    Guid ProductId,
    Guid UserId,
    Guid PersonaId,
    string? AdditionalContext = null
) : IRequest<SuggestStoriesResult>;

public record SuggestedStory(
    string AsA,
    string IWant,
    string SoThat,
    string Priority,
    string[] AcceptanceCriteria
);
```

- [ ] Step 1: Create SuggestStoriesCommand with handler
- [ ] Step 2: Create SuggestStoriesEndpoint
- [ ] Step 3: Test endpoint
- [ ] Step 4: Commit

---

### Task 3: AI Suggest Features Endpoint

**Files:**
- Create: `src/Backend/Braavo.Core/UseCases/AI/SuggestFeaturesCommand.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/AI/SuggestFeaturesEndpoint.cs`

**Interfaces:**
- Consumes: `ILlmProvider`, `IProductRepository`, `IUserStoryRepository`
- Produces: `POST /api/ai/suggest-features` → Array of suggested features

**Implementation:**
- Takes list of story IDs or all stories for product
- Uses LLM to suggest features that would address the stories
- Returns features with name, description, phase, effort, linkedStoryIds

- [ ] Step 1: Create SuggestFeaturesCommand with handler
- [ ] Step 2: Create SuggestFeaturesEndpoint
- [ ] Step 3: Test endpoint
- [ ] Step 4: Commit

---

### Task 4: AI Refine Content Endpoint

**Files:**
- Create: `src/Backend/Braavo.Core/UseCases/AI/RefineContentCommand.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/AI/RefineContentEndpoint.cs`

**Interfaces:**
- Consumes: `ILlmProvider`
- Produces: `POST /api/ai/refine-content` → Refined text

**Implementation:**
- Generic endpoint for refining any text content
- Takes content, contentType (goal, painPoint, story, description), and instruction
- Returns improved version of the content

```csharp
public record RefineContentCommand(
    Guid UserId,
    string Content,
    string ContentType,  // "goal", "painPoint", "story", "description", "acceptanceCriteria"
    string? Instruction  // Optional: "make more specific", "add metrics", etc.
) : IRequest<RefineContentResult>;
```

- [ ] Step 1: Create RefineContentCommand with handler
- [ ] Step 2: Create RefineContentEndpoint
- [ ] Step 3: Test endpoint
- [ ] Step 4: Commit

---

### Task 5: Frontend AI API Client

**Files:**
- Create: `src/Frontend/src/api/ai.ts`

**Interfaces:**
- Consumes: `apiClient`
- Produces: `aiApi` object with generate/suggest methods

```typescript
export const aiApi = {
  generatePersona: (productId: string, description: string) =>
    apiClient.post<GeneratedPersona>('/ai/generate-persona', { productId, description }),
  
  suggestStories: (productId: string, personaId: string, context?: string) =>
    apiClient.post<SuggestedStory[]>('/ai/suggest-stories', { productId, personaId, additionalContext: context }),
  
  suggestFeatures: (productId: string, storyIds?: string[]) =>
    apiClient.post<SuggestedFeature[]>('/ai/suggest-features', { productId, storyIds }),
  
  refineContent: (content: string, contentType: string, instruction?: string) =>
    apiClient.post<{ refined: string }>('/ai/refine-content', { content, contentType, instruction }),
};
```

- [ ] Step 1: Create ai.ts API client
- [ ] Step 2: Add TypeScript types for AI responses
- [ ] Step 3: Commit

---

### Task 6: AI Redux Slice

**Files:**
- Create: `src/Frontend/src/store/slices/aiSlice.ts`
- Modify: `src/Frontend/src/store/store.ts`

**State:**
```typescript
interface AIState {
  loading: boolean;
  error: string | null;
  suggestedPersona: GeneratedPersona | null;
  suggestedStories: SuggestedStory[];
  suggestedFeatures: SuggestedFeature[];
}
```

**Thunks:**
- generatePersona
- suggestStories
- suggestFeatures
- refineContent

**Reducers:**
- clearSuggestions
- acceptPersona (clears suggestedPersona)
- acceptStory (removes from suggestedStories)
- acceptFeature (removes from suggestedFeatures)

- [ ] Step 1: Create aiSlice
- [ ] Step 2: Register in store
- [ ] Step 3: Commit

---

### Task 7: AI Suggestion Panel Component

**Files:**
- Create: `src/Frontend/src/components/ai/AISuggestionPanel.tsx`
- Create: `src/Frontend/src/components/ai/AIButton.tsx`

**AISuggestionPanel:**
- Slide-in panel from right side
- Shows current suggestions (personas, stories, features)
- Accept/Reject buttons for each suggestion
- Loading spinner during generation

**AIButton:**
- Sparkle icon button (🪄)
- Triggers AI action based on context
- Shows loading state

- [ ] Step 1: Create AIButton component
- [ ] Step 2: Create AISuggestionPanel component
- [ ] Step 3: Style with rustic theme
- [ ] Step 4: Commit

---

### Task 8: Integrate AI into PersonasPage

**Files:**
- Modify: `src/Frontend/src/pages/PersonasPage.tsx`
- Modify: `src/Frontend/src/components/prd/PersonaEditor.tsx`

**Changes:**
- Add "Generate with AI" section at bottom of personas list
- Text input for persona description + Generate button
- When AI generates persona, show in editor modal for review/edit before save
- Add AI refine buttons for goals and pain points in editor

- [ ] Step 1: Add AI generation section to PersonasPage
- [ ] Step 2: Add AI refinement buttons to PersonaEditor
- [ ] Step 3: Connect to aiSlice thunks
- [ ] Step 4: Test flow
- [ ] Step 5: Commit

---

### Task 9: Integrate AI into StoriesPage

**Files:**
- Modify: `src/Frontend/src/pages/StoriesPage.tsx`
- Modify: `src/Frontend/src/components/prd/StoryEditor.tsx`

**Changes:**
- Add "Suggest Stories" button that takes selected persona
- Show suggested stories in a preview list
- User can accept/edit/reject each suggestion
- Add AI refine for acceptance criteria

- [ ] Step 1: Add AI suggestion UI to StoriesPage
- [ ] Step 2: Add suggestion preview cards
- [ ] Step 3: Add accept/reject flow
- [ ] Step 4: Test flow
- [ ] Step 5: Commit

---

### Task 10: Integrate AI into FeaturesPage

**Files:**
- Modify: `src/Frontend/src/pages/FeaturesPage.tsx`
- Modify: `src/Frontend/src/components/prd/FeatureEditor.tsx`

**Changes:**
- Add "Suggest Features" button based on current stories
- Show suggested features with phase recommendation
- User can accept to specific phase or reject

- [ ] Step 1: Add AI suggestion UI to FeaturesPage
- [ ] Step 2: Add suggestion preview with phase pills
- [ ] Step 3: Add accept flow (picks phase)
- [ ] Step 4: Test flow
- [ ] Step 5: Commit

---

### Task 11: Integration Testing

- [ ] Step 1: Start backend and frontend
- [ ] Step 2: Test full AI flow: generate persona → suggest stories → suggest features
- [ ] Step 3: Test refinement on various content types
- [ ] Step 4: Fix any issues
- [ ] Step 5: Run test suites
- [ ] Step 6: Final commit

---

## Summary

Phase 3 delivers:
1. **4 AI backend endpoints** - Generate persona, suggest stories, suggest features, refine content
2. **AI API client** - TypeScript wrapper for AI endpoints
3. **AI Redux slice** - State management for suggestions
4. **AI UI components** - Suggestion panel, AI buttons
5. **Integrated AI in all section pages** - Personas, Stories, Features
