import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { aiApi, type GeneratedPersona, type SuggestedStory, type SuggestedFeature } from '../../api/ai';

interface AIState {
  loading: boolean;
  error: string | null;
  suggestedPersona: GeneratedPersona | null;
  suggestedStories: SuggestedStory[];
  suggestedFeatures: SuggestedFeature[];
  refinedContent: string | null;
}

const initialState: AIState = {
  loading: false,
  error: null,
  suggestedPersona: null,
  suggestedStories: [],
  suggestedFeatures: [],
  refinedContent: null,
};

export const generatePersona = createAsyncThunk(
  'ai/generatePersona',
  async ({ productId, description }: { productId: string; description: string }) => {
    const response = await aiApi.generatePersona(productId, description);
    if (!response.data.success || !response.data.persona) {
      throw new Error(response.data.error ?? 'Failed to generate persona');
    }
    return response.data.persona;
  }
);

export const suggestStories = createAsyncThunk(
  'ai/suggestStories',
  async ({ productId, personaId, context }: { productId: string; personaId: string; context?: string }) => {
    const response = await aiApi.suggestStories(productId, personaId, context);
    if (!response.data.success || !response.data.stories) {
      throw new Error(response.data.error ?? 'Failed to suggest stories');
    }
    return response.data.stories;
  }
);

export const suggestFeatures = createAsyncThunk(
  'ai/suggestFeatures',
  async ({ productId, storyIds }: { productId: string; storyIds?: string[] }) => {
    const response = await aiApi.suggestFeatures(productId, storyIds);
    if (!response.data.success || !response.data.features) {
      throw new Error(response.data.error ?? 'Failed to suggest features');
    }
    return response.data.features;
  }
);

export const refineContent = createAsyncThunk(
  'ai/refineContent',
  async ({
    content,
    contentType,
    instruction,
  }: {
    content: string;
    contentType: string;
    instruction?: string;
  }) => {
    const response = await aiApi.refineContent(content, contentType, instruction);
    if (!response.data.success || !response.data.refinedContent) {
      throw new Error(response.data.error ?? 'Failed to refine content');
    }
    return response.data.refinedContent;
  }
);

const aiSlice = createSlice({
  name: 'ai',
  initialState,
  reducers: {
    clearSuggestions: (state) => {
      state.suggestedPersona = null;
      state.suggestedStories = [];
      state.suggestedFeatures = [];
      state.refinedContent = null;
      state.error = null;
    },
    clearSuggestedPersona: (state) => {
      state.suggestedPersona = null;
    },
    clearSuggestedStories: (state) => {
      state.suggestedStories = [];
    },
    clearSuggestedFeatures: (state) => {
      state.suggestedFeatures = [];
    },
    clearRefinedContent: (state) => {
      state.refinedContent = null;
    },
    removeSuggestedStory: (state, action: PayloadAction<number>) => {
      state.suggestedStories.splice(action.payload, 1);
    },
    removeSuggestedFeature: (state, action: PayloadAction<number>) => {
      state.suggestedFeatures.splice(action.payload, 1);
    },
  },
  extraReducers: (builder) => {
    builder
      // generatePersona
      .addCase(generatePersona.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(generatePersona.fulfilled, (state, action: PayloadAction<GeneratedPersona>) => {
        state.loading = false;
        state.suggestedPersona = action.payload;
      })
      .addCase(generatePersona.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to generate persona';
      })
      // suggestStories
      .addCase(suggestStories.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(suggestStories.fulfilled, (state, action: PayloadAction<SuggestedStory[]>) => {
        state.loading = false;
        state.suggestedStories = action.payload;
      })
      .addCase(suggestStories.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to suggest stories';
      })
      // suggestFeatures
      .addCase(suggestFeatures.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(suggestFeatures.fulfilled, (state, action: PayloadAction<SuggestedFeature[]>) => {
        state.loading = false;
        state.suggestedFeatures = action.payload;
      })
      .addCase(suggestFeatures.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to suggest features';
      })
      // refineContent
      .addCase(refineContent.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(refineContent.fulfilled, (state, action: PayloadAction<string>) => {
        state.loading = false;
        state.refinedContent = action.payload;
      })
      .addCase(refineContent.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to refine content';
      });
  },
});

export const {
  clearSuggestions,
  clearSuggestedPersona,
  clearSuggestedStories,
  clearSuggestedFeatures,
  clearRefinedContent,
  removeSuggestedStory,
  removeSuggestedFeature,
} = aiSlice.actions;

export default aiSlice.reducer;
