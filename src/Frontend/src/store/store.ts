import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import productsReducer from './slices/productsSlice';
import personasReducer from './slices/personasSlice';
import storiesReducer from './slices/storiesSlice';
import featuresReducer from './slices/featuresSlice';
import aiReducer from './slices/aiSlice';
import timelineReducer from './slices/timelineSlice';
import diagramsReducer from './slices/diagramsSlice';
import exportReducer from './slices/exportSlice';
import dependenciesReducer from './slices/dependenciesSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    products: productsReducer,
    personas: personasReducer,
    stories: storiesReducer,
    features: featuresReducer,
    ai: aiReducer,
    timeline: timelineReducer,
    diagrams: diagramsReducer,
    export: exportReducer,
    dependencies: dependenciesReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
