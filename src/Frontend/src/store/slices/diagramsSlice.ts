import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { diagramsApi } from '../../api/diagrams';

interface DiagramsState {
  // User Journey
  userJourneyCode: string | null;
  userJourneyLoading: boolean;
  userJourneyError: string | null;
  // Feature Hierarchy
  featureHierarchyCode: string | null;
  featureHierarchyLoading: boolean;
  featureHierarchyError: string | null;
  // Component Architecture
  componentCode: string | null;
  componentLoading: boolean;
  componentError: string | null;
}

const initialState: DiagramsState = {
  userJourneyCode: null,
  userJourneyLoading: false,
  userJourneyError: null,
  featureHierarchyCode: null,
  featureHierarchyLoading: false,
  featureHierarchyError: null,
  componentCode: null,
  componentLoading: false,
  componentError: null,
};

export const generateUserJourney = createAsyncThunk(
  'diagrams/generateUserJourney',
  async ({ productId, personaId }: { productId: string; personaId?: string }) => {
    const response = await diagramsApi.generateUserJourney(productId, personaId);
    const data = response.data as { mermaidCode?: string } | string;
    if (typeof data === 'string') return data;
    return (data as { mermaidCode?: string }).mermaidCode ?? '';
  }
);

export const generateFeatureHierarchy = createAsyncThunk(
  'diagrams/generateFeatureHierarchy',
  async (productId: string) => {
    const response = await diagramsApi.generateFeatureHierarchy(productId);
    const data = response.data as { mermaidCode?: string } | string;
    if (typeof data === 'string') return data;
    return (data as { mermaidCode?: string }).mermaidCode ?? '';
  }
);

export const generateComponentDiagram = createAsyncThunk(
  'diagrams/generateComponent',
  async (productId: string) => {
    const response = await diagramsApi.generateComponent(productId);
    const data = response.data as { mermaidCode?: string } | string;
    if (typeof data === 'string') return data;
    return (data as { mermaidCode?: string }).mermaidCode ?? '';
  }
);

const diagramsSlice = createSlice({
  name: 'diagrams',
  initialState,
  reducers: {
    clearDiagrams: (state) => {
      state.userJourneyCode = null;
      state.userJourneyError = null;
      state.featureHierarchyCode = null;
      state.featureHierarchyError = null;
      state.componentCode = null;
      state.componentError = null;
    },
    clearUserJourney: (state) => {
      state.userJourneyCode = null;
      state.userJourneyError = null;
    },
    clearFeatureHierarchy: (state) => {
      state.featureHierarchyCode = null;
      state.featureHierarchyError = null;
    },
    clearComponent: (state) => {
      state.componentCode = null;
      state.componentError = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // generateUserJourney
      .addCase(generateUserJourney.pending, (state) => {
        state.userJourneyLoading = true;
        state.userJourneyError = null;
        state.userJourneyCode = null;
      })
      .addCase(generateUserJourney.fulfilled, (state, action: PayloadAction<string>) => {
        state.userJourneyLoading = false;
        state.userJourneyCode = action.payload;
      })
      .addCase(generateUserJourney.rejected, (state, action) => {
        state.userJourneyLoading = false;
        state.userJourneyError = action.error.message ?? 'Failed to generate user journey diagram';
      })
      // generateFeatureHierarchy
      .addCase(generateFeatureHierarchy.pending, (state) => {
        state.featureHierarchyLoading = true;
        state.featureHierarchyError = null;
        state.featureHierarchyCode = null;
      })
      .addCase(generateFeatureHierarchy.fulfilled, (state, action: PayloadAction<string>) => {
        state.featureHierarchyLoading = false;
        state.featureHierarchyCode = action.payload;
      })
      .addCase(generateFeatureHierarchy.rejected, (state, action) => {
        state.featureHierarchyLoading = false;
        state.featureHierarchyError = action.error.message ?? 'Failed to generate feature hierarchy diagram';
      })
      // generateComponentDiagram
      .addCase(generateComponentDiagram.pending, (state) => {
        state.componentLoading = true;
        state.componentError = null;
        state.componentCode = null;
      })
      .addCase(generateComponentDiagram.fulfilled, (state, action: PayloadAction<string>) => {
        state.componentLoading = false;
        state.componentCode = action.payload;
      })
      .addCase(generateComponentDiagram.rejected, (state, action) => {
        state.componentLoading = false;
        state.componentError = action.error.message ?? 'Failed to generate component architecture diagram';
      });
  },
});

export const { clearDiagrams, clearUserJourney, clearFeatureHierarchy, clearComponent } = diagramsSlice.actions;
export default diagramsSlice.reducer;
