import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { storiesApi, type UserStory, type CreateUserStoryRequest, type UpdateUserStoryRequest } from '../../api/stories';

interface StoriesState {
  items: UserStory[];
  loading: boolean;
  error: string | null;
}

const initialState: StoriesState = {
  items: [],
  loading: false,
  error: null,
};

export const fetchStories = createAsyncThunk('stories/fetchAll', async (productId: string) => {
  const response = await storiesApi.list(productId);
  return response.data;
});

export const createStory = createAsyncThunk(
  'stories/create',
  async ({ productId, data }: { productId: string; data: CreateUserStoryRequest }, { dispatch }) => {
    const response = await storiesApi.create(productId, data);
    dispatch(fetchStories(productId));
    return response.data;
  }
);

export const updateStory = createAsyncThunk(
  'stories/update',
  async (
    { productId, storyId, data }: { productId: string; storyId: string; data: UpdateUserStoryRequest },
    { dispatch }
  ) => {
    const response = await storiesApi.update(productId, storyId, data);
    dispatch(fetchStories(productId));
    return response.data;
  }
);

export const deleteStory = createAsyncThunk(
  'stories/delete',
  async ({ productId, storyId }: { productId: string; storyId: string }, { dispatch }) => {
    await storiesApi.delete(productId, storyId);
    dispatch(fetchStories(productId));
    return storyId;
  }
);

const storiesSlice = createSlice({
  name: 'stories',
  initialState,
  reducers: {
    clearStories: (state) => {
      state.items = [];
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchStories.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchStories.fulfilled, (state, action: PayloadAction<UserStory[]>) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchStories.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch stories';
      })
      .addCase(createStory.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createStory.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(createStory.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to create story';
      })
      .addCase(updateStory.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateStory.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(updateStory.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to update story';
      })
      .addCase(deleteStory.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteStory.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(deleteStory.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to delete story';
      });
  },
});

export const { clearStories } = storiesSlice.actions;
export default storiesSlice.reducer;
