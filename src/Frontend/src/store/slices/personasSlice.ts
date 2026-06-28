import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { personasApi, type Persona, type CreatePersonaRequest, type UpdatePersonaRequest } from '../../api/personas';

interface PersonasState {
  items: Persona[];
  loading: boolean;
  error: string | null;
}

const initialState: PersonasState = {
  items: [],
  loading: false,
  error: null,
};

export const fetchPersonas = createAsyncThunk('personas/fetchAll', async (productId: string) => {
  const response = await personasApi.list(productId);
  return response.data;
});

export const createPersona = createAsyncThunk(
  'personas/create',
  async ({ productId, data }: { productId: string; data: CreatePersonaRequest }, { dispatch }) => {
    const response = await personasApi.create(productId, data);
    dispatch(fetchPersonas(productId));
    return response.data;
  }
);

export const updatePersona = createAsyncThunk(
  'personas/update',
  async (
    { productId, personaId, data }: { productId: string; personaId: string; data: UpdatePersonaRequest },
    { dispatch }
  ) => {
    const response = await personasApi.update(productId, personaId, data);
    dispatch(fetchPersonas(productId));
    return response.data;
  }
);

export const deletePersona = createAsyncThunk(
  'personas/delete',
  async ({ productId, personaId }: { productId: string; personaId: string }, { dispatch }) => {
    await personasApi.delete(productId, personaId);
    dispatch(fetchPersonas(productId));
    return personaId;
  }
);

const personasSlice = createSlice({
  name: 'personas',
  initialState,
  reducers: {
    clearPersonas: (state) => {
      state.items = [];
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchPersonas.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchPersonas.fulfilled, (state, action: PayloadAction<Persona[]>) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchPersonas.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch personas';
      })
      .addCase(createPersona.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createPersona.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(createPersona.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to create persona';
      })
      .addCase(updatePersona.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updatePersona.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(updatePersona.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to update persona';
      })
      .addCase(deletePersona.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deletePersona.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(deletePersona.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to delete persona';
      });
  },
});

export const { clearPersonas } = personasSlice.actions;
export default personasSlice.reducer;
