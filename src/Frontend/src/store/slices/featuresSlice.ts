import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import {
  featuresApi,
  type Feature,
  type FeaturePhase,
  type CreateFeatureRequest,
  type UpdateFeatureRequest,
} from '../../api/features';

interface FeaturesState {
  items: Feature[];
  loading: boolean;
  error: string | null;
}

const initialState: FeaturesState = {
  items: [],
  loading: false,
  error: null,
};

export const fetchFeatures = createAsyncThunk('features/fetchAll', async (productId: string) => {
  const response = await featuresApi.list(productId);
  return response.data;
});

export const createFeature = createAsyncThunk(
  'features/create',
  async ({ productId, data }: { productId: string; data: CreateFeatureRequest }, { dispatch }) => {
    const response = await featuresApi.create(productId, data);
    dispatch(fetchFeatures(productId));
    return response.data;
  }
);

export const updateFeature = createAsyncThunk(
  'features/update',
  async (
    { productId, featureId, data }: { productId: string; featureId: string; data: UpdateFeatureRequest },
    { dispatch }
  ) => {
    const response = await featuresApi.update(productId, featureId, data);
    dispatch(fetchFeatures(productId));
    return response.data;
  }
);

export const deleteFeature = createAsyncThunk(
  'features/delete',
  async ({ productId, featureId }: { productId: string; featureId: string }, { dispatch }) => {
    await featuresApi.delete(productId, featureId);
    dispatch(fetchFeatures(productId));
    return featureId;
  }
);

export const moveFeature = createAsyncThunk(
  'features/move',
  async (
    {
      productId,
      featureId,
      phase,
      sortOrder,
    }: { productId: string; featureId: string; phase: FeaturePhase; sortOrder?: number },
    { dispatch }
  ) => {
    const response = await featuresApi.move(productId, featureId, { phase, sortOrder });
    dispatch(fetchFeatures(productId));
    return response.data;
  }
);

const featuresSlice = createSlice({
  name: 'features',
  initialState,
  reducers: {
    clearFeatures: (state) => {
      state.items = [];
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchFeatures.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchFeatures.fulfilled, (state, action: PayloadAction<Feature[]>) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchFeatures.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch features';
      })
      .addCase(createFeature.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createFeature.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(createFeature.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to create feature';
      })
      .addCase(updateFeature.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateFeature.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(updateFeature.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to update feature';
      })
      .addCase(deleteFeature.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteFeature.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(deleteFeature.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to delete feature';
      })
      .addCase(moveFeature.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(moveFeature.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(moveFeature.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to move feature';
      });
  },
});

export const { clearFeatures } = featuresSlice.actions;
export default featuresSlice.reducer;
