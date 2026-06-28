import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { productsApi, type Product, type ProductSummary } from '../../api/products';

interface ProductsState {
  items: ProductSummary[];
  currentProduct: Product | null;
  loading: boolean;
  error: string | null;
}

const initialState: ProductsState = {
  items: [],
  currentProduct: null,
  loading: false,
  error: null,
};

export const fetchProducts = createAsyncThunk('products/fetchAll', async () => {
  const response = await productsApi.list();
  return response.data;
});

export const fetchProduct = createAsyncThunk('products/fetchOne', async (id: string) => {
  const response = await productsApi.get(id);
  return response.data;
});

export const createProduct = createAsyncThunk(
  'products/create',
  async (data: { name: string; description: string }, { dispatch }) => {
    const response = await productsApi.create(data);
    dispatch(fetchProducts());
    return response.data;
  }
);

const productsSlice = createSlice({
  name: 'products',
  initialState,
  reducers: {
    clearCurrentProduct: (state) => {
      state.currentProduct = null;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchProducts.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchProducts.fulfilled, (state, action: PayloadAction<ProductSummary[]>) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchProducts.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch products';
      })
      .addCase(fetchProduct.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchProduct.fulfilled, (state, action: PayloadAction<Product>) => {
        state.loading = false;
        state.currentProduct = action.payload;
      })
      .addCase(fetchProduct.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch product';
      })
      .addCase(createProduct.fulfilled, (state) => {
        state.loading = false;
      });
  },
});

export const { clearCurrentProduct, clearError } = productsSlice.actions;
export default productsSlice.reducer;
