import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { dependenciesApi, type DependencyStatus } from '../../api/dependencies';

interface DependenciesState {
  status: DependencyStatus | null;
  loading: boolean;
  error: string | null;
}

const initialState: DependenciesState = {
  status: null,
  loading: false,
  error: null,
};

export const fetchDependencies = createAsyncThunk(
  'dependencies/fetch',
  async (productId: string) => {
    const response = await dependenciesApi.get(productId);
    return response.data;
  }
);

const dependenciesSlice = createSlice({
  name: 'dependencies',
  initialState,
  reducers: {
    clearDependencies: (state) => {
      state.status = null;
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchDependencies.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchDependencies.fulfilled, (state, action) => {
        state.loading = false;
        state.status = action.payload;
      })
      .addCase(fetchDependencies.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch dependencies';
      });
  },
});

export const { clearDependencies } = dependenciesSlice.actions;
export default dependenciesSlice.reducer;
