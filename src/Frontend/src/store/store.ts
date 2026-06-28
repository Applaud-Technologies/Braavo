import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import productsReducer from './slices/productsSlice';
import personasReducer from './slices/personasSlice';
import storiesReducer from './slices/storiesSlice';
import featuresReducer from './slices/featuresSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    products: productsReducer,
    personas: personasReducer,
    stories: storiesReducer,
    features: featuresReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
