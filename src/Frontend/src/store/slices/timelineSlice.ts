import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { timelineApi, type TimelinePhase } from '../../api/timeline';

interface TimelineState {
  phases: TimelinePhase[];
  loading: boolean;
  saving: boolean;
  error: string | null;
  ganttCode: string | null;
  ganttLoading: boolean;
  ganttError: string | null;
}

const initialState: TimelineState = {
  phases: [],
  loading: false,
  saving: false,
  error: null,
  ganttCode: null,
  ganttLoading: false,
  ganttError: null,
};

export const fetchTimeline = createAsyncThunk(
  'timeline/fetch',
  async (productId: string) => {
    const response = await timelineApi.get(productId);
    const data = response.data as { phases?: TimelinePhase[] } | TimelinePhase[];
    if (Array.isArray(data)) return data;
    return data.phases ?? [];
  }
);

export const updateTimeline = createAsyncThunk(
  'timeline/update',
  async ({ productId, phases }: { productId: string; phases: TimelinePhase[] }, { dispatch }) => {
    await timelineApi.update(productId, phases);
    dispatch(fetchTimeline(productId));
    return phases;
  }
);

export const generateGanttFromTimeline = createAsyncThunk(
  'timeline/generateGantt',
  async (productId: string) => {
    // Use the diagrams API to generate a Gantt diagram from the backend
    const { diagramsApi } = await import('../../api/diagrams');
    const response = await diagramsApi.generateGantt(productId);
    const data = response.data as { mermaidCode?: string } | string;
    if (typeof data === 'string') return data;
    return (data as { mermaidCode?: string }).mermaidCode ?? '';
  }
);

const timelineSlice = createSlice({
  name: 'timeline',
  initialState,
  reducers: {
    clearTimeline: (state) => {
      state.phases = [];
      state.error = null;
      state.ganttCode = null;
    },
    clearGantt: (state) => {
      state.ganttCode = null;
      state.ganttError = null;
    },
    setPhases: (state, action: PayloadAction<TimelinePhase[]>) => {
      state.phases = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      // fetchTimeline
      .addCase(fetchTimeline.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchTimeline.fulfilled, (state, action: PayloadAction<TimelinePhase[]>) => {
        state.loading = false;
        state.phases = action.payload;
      })
      .addCase(fetchTimeline.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch timeline';
      })
      // updateTimeline
      .addCase(updateTimeline.pending, (state) => {
        state.saving = true;
        state.error = null;
      })
      .addCase(updateTimeline.fulfilled, (state) => {
        state.saving = false;
      })
      .addCase(updateTimeline.rejected, (state, action) => {
        state.saving = false;
        state.error = action.error.message ?? 'Failed to save timeline';
      })
      // generateGanttFromTimeline
      .addCase(generateGanttFromTimeline.pending, (state) => {
        state.ganttLoading = true;
        state.ganttError = null;
        state.ganttCode = null;
      })
      .addCase(generateGanttFromTimeline.fulfilled, (state, action: PayloadAction<string>) => {
        state.ganttLoading = false;
        state.ganttCode = action.payload;
      })
      .addCase(generateGanttFromTimeline.rejected, (state, action) => {
        state.ganttLoading = false;
        state.ganttError = action.error.message ?? 'Failed to generate Gantt diagram';
      });
  },
});

export const { clearTimeline, clearGantt, setPhases } = timelineSlice.actions;
export default timelineSlice.reducer;
