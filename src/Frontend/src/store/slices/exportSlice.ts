import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { exportApi, downloadBlob, type SectionValidation } from '../../api/export';

interface ExportState {
  validations: SectionValidation[];
  loading: boolean;
  error: string | null;
}

const initialState: ExportState = {
  validations: [],
  loading: false,
  error: null,
};

export const fetchValidations = createAsyncThunk(
  'export/fetchValidations',
  async (productId: string) => {
    const response = await exportApi.validate(productId);
    const data = response.data as { validations?: SectionValidation[] } | SectionValidation[];
    if (Array.isArray(data)) return data;
    return (data as { validations?: SectionValidation[] }).validations ?? [];
  }
);

export const downloadMarkdown = createAsyncThunk(
  'export/downloadMarkdown',
  async ({ productId, productName }: { productId: string; productName: string }) => {
    const response = await exportApi.markdown(productId);
    downloadBlob(response.data as Blob, `${productName}-PRD.md`);
  }
);

export const downloadPdf = createAsyncThunk(
  'export/downloadPdf',
  async ({ productId, productName }: { productId: string; productName: string }) => {
    const response = await exportApi.pdf(productId);
    downloadBlob(response.data as Blob, `${productName}-PRD.pdf`);
  }
);

const exportSlice = createSlice({
  name: 'export',
  initialState,
  reducers: {
    clearExport: (state) => {
      state.validations = [];
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchValidations.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchValidations.fulfilled, (state, action: PayloadAction<SectionValidation[]>) => {
        state.loading = false;
        state.validations = action.payload;
      })
      .addCase(fetchValidations.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch validations';
      })
      .addCase(downloadMarkdown.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(downloadMarkdown.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(downloadMarkdown.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to download Markdown';
      })
      .addCase(downloadPdf.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(downloadPdf.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(downloadPdf.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to download PDF';
      });
  },
});

export const { clearExport } = exportSlice.actions;
export default exportSlice.reducer;
