# Guided PRD Builder Phase 5: Polish & Export

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add PRD preview mode, export capabilities (PDF, Markdown, DOCX), completion percentage calculation, and section validation warnings.

**Architecture:** Backend export endpoints that generate formatted documents. Frontend preview page that renders the full PRD. Completion calculation based on section data presence.

**Tech Stack:** .NET 8.0, FastEndpoints, MediatR, QuestPDF (for PDF), React 18, TypeScript 5

## Global Constraints

- All endpoints require JWT authentication
- Export endpoints return file downloads
- Backend port: 5153, Frontend port: 5173
- Follow existing rustic theme (stone/amber colors)

---

### Task 1: Completion Percentage Calculation

**Files:**
- Modify: `src/Backend/Braavo.Core/Entities/Product.cs`
- Create: `src/Backend/Braavo.Core/Services/CompletionCalculator.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Products/RecalculateCompletionCommand.cs`

**Implementation:**
- Calculate completion based on section presence:
  - Vision present: 10%
  - Problem statement: 10%
  - At least 1 persona: 15%
  - At least 3 user stories: 15%
  - At least 3 features: 15%
  - At least 1 timeline phase: 15%
  - Value proposition: 10%
  - Target market: 10%
- Auto-recalculate when sections are updated
- Store in Product.CompletionPercentage

- [ ] Step 1: Create CompletionCalculator service
- [ ] Step 2: Create RecalculateCompletionCommand
- [ ] Step 3: Call recalculation from create/update handlers
- [ ] Step 4: Test calculation
- [ ] Step 5: Commit

---

### Task 2: Section Validation Service

**Files:**
- Create: `src/Backend/Braavo.Core/Services/SectionValidator.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Products/ValidateSectionsQuery.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Products/ValidateSectionsEndpoint.cs`

**Implementation:**
- Validate each section for completeness:
  - Overview: vision and problem statement required
  - Personas: at least 1 with name and role
  - Stories: at least 1 with all three parts (as a, I want, so that)
  - Features: at least 1 per phase (MVP required)
  - Timeline: at least 1 phase with duration
- Return validation warnings per section

```csharp
public record SectionValidation(
    string Section,
    bool IsValid,
    string[] Warnings
);
```

- [ ] Step 1: Create SectionValidator service
- [ ] Step 2: Create ValidateSectionsQuery and handler
- [ ] Step 3: Create endpoint GET /api/products/{id}/validate
- [ ] Step 4: Test validation
- [ ] Step 5: Commit

---

### Task 3: Export to Markdown Endpoint

**Files:**
- Create: `src/Backend/Braavo.Core/Services/MarkdownExporter.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Export/ExportMarkdownQuery.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Export/ExportMarkdownEndpoint.cs`

**Implementation:**
- Generate full PRD as Markdown document
- Include all sections: Overview, Personas, Stories, Features, Timeline
- Format with proper headers, lists, tables
- Return as file download with .md extension

```markdown
# {Product Name} - Product Requirements Document

## Overview
### Vision
{vision}

### Problem Statement
{problem}

## User Personas
### {Persona Name}
- **Role:** {role}
- **Technical Level:** {level}
- **Goals:** 
  - {goal1}
  - {goal2}
...
```

- [ ] Step 1: Create MarkdownExporter service
- [ ] Step 2: Create ExportMarkdownQuery and handler
- [ ] Step 3: Create endpoint GET /api/products/{id}/export/markdown
- [ ] Step 4: Test export
- [ ] Step 5: Commit

---

### Task 4: Export to PDF Endpoint

**Files:**
- Create: `src/Backend/Braavo.Core/Services/PdfExporter.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Export/ExportPdfQuery.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Export/ExportPdfEndpoint.cs`

**Implementation:**
- Install QuestPDF NuGet package
- Generate PDF with professional styling
- Include header with product name and version
- Format sections with typography hierarchy
- Return as file download with .pdf extension

- [ ] Step 1: Add QuestPDF package
- [ ] Step 2: Create PdfExporter service with QuestPDF Fluent API
- [ ] Step 3: Create ExportPdfQuery and handler
- [ ] Step 4: Create endpoint GET /api/products/{id}/export/pdf
- [ ] Step 5: Test export
- [ ] Step 6: Commit

---

### Task 5: Frontend Export API Client

**Files:**
- Create: `src/Frontend/src/api/export.ts`

**Implementation:**
```typescript
export const exportApi = {
  markdown: (productId: string) =>
    api.get(`/products/${productId}/export/markdown`, { responseType: 'blob' }),
  
  pdf: (productId: string) =>
    api.get(`/products/${productId}/export/pdf`, { responseType: 'blob' }),
  
  validate: (productId: string) =>
    api.get<{ validations: SectionValidation[] }>(`/products/${productId}/validate`),
};
```

- [ ] Step 1: Create export.ts API client
- [ ] Step 2: Add download helper function
- [ ] Step 3: Commit

---

### Task 6: PRD Preview Page

**Files:**
- Create: `src/Frontend/src/pages/PreviewPage.tsx`
- Create: `src/Frontend/src/store/slices/exportSlice.ts`
- Modify: `src/Frontend/src/store/store.ts`
- Modify: `src/Frontend/src/App.tsx`

**Implementation:**
- Full-page PRD preview rendering all sections
- Print-friendly styling
- Validation warnings banner at top if incomplete
- Export buttons (PDF, Markdown)
- Progress indicator showing completion percentage

Layout:
```
┌─────────────────────────────────────────────────────┐
│ [Back] {Product Name} PRD          [PDF] [Markdown] │
├─────────────────────────────────────────────────────┤
│ ⚠️ 2 warnings: Missing personas, No MVP features    │
├─────────────────────────────────────────────────────┤
│ Progress: ████████░░ 75%                            │
├─────────────────────────────────────────────────────┤
│                                                     │
│ # Overview                                          │
│ ## Vision                                           │
│ {vision text}                                       │
│                                                     │
│ ## Problem Statement                                │
│ {problem text}                                      │
│                                                     │
│ # User Personas                                     │
│ ┌──────────┐ ┌──────────┐                          │
│ │ Persona1 │ │ Persona2 │                          │
│ └──────────┘ └──────────┘                          │
│                                                     │
│ # User Stories                                      │
│ ...                                                 │
└─────────────────────────────────────────────────────┘
```

- [ ] Step 1: Create exportSlice with validation and download thunks
- [ ] Step 2: Register in store
- [ ] Step 3: Create PreviewPage component
- [ ] Step 4: Add route /products/:id/preview
- [ ] Step 5: Style for print
- [ ] Step 6: Commit

---

### Task 7: Add Preview Link to Product Detail

**Files:**
- Modify: `src/Frontend/src/pages/ProductDetailPage.tsx`

**Implementation:**
- Add "Preview PRD" button in header next to status badge
- Add completion percentage progress bar (already exists, ensure it uses actual data)
- Add validation warnings if present

- [ ] Step 1: Add Preview button
- [ ] Step 2: Fetch and display validation warnings
- [ ] Step 3: Commit

---

### Task 8: Integration Testing

- [ ] Step 1: Start backend and frontend
- [ ] Step 2: Create a product with all sections filled
- [ ] Step 3: Verify completion percentage updates
- [ ] Step 4: Check validation warnings
- [ ] Step 5: Test Markdown export
- [ ] Step 6: Test PDF export
- [ ] Step 7: Verify Preview page renders correctly
- [ ] Step 8: Fix any issues
- [ ] Step 9: Final commit

---

## Summary

Phase 5 delivers:
1. **Completion percentage** - Auto-calculated based on section presence
2. **Section validation** - Warnings for incomplete/missing sections
3. **Markdown export** - Full PRD as .md file
4. **PDF export** - Professional formatted PDF document
5. **Preview page** - Full PRD view with export buttons
6. **Product detail enhancements** - Preview link, validation warnings
