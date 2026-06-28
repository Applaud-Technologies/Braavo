import { api } from './client';

export type FeaturePhase = 'Mvp' | 'Enhanced' | 'Future';
export type EffortSize = 'Small' | 'Medium' | 'Large';

export interface Feature {
  id: string;
  name: string;
  description: string;
  phase: FeaturePhase;
  effort: EffortSize | null;
  linkedStoryIds: string[];
  sortOrder: number;
}

export interface CreateFeatureRequest {
  name: string;
  description: string;
  phase: FeaturePhase;
  effort?: EffortSize | null;
  linkedStoryIds?: string[];
  sortOrder?: number;
}

export interface UpdateFeatureRequest {
  name?: string;
  description?: string;
  phase?: FeaturePhase;
  effort?: EffortSize | null;
  linkedStoryIds?: string[];
  sortOrder?: number;
}

export interface MoveFeatureRequest {
  phase: FeaturePhase;
  sortOrder?: number;
}

export const featuresApi = {
  list: (productId: string) =>
    api.get<Feature[]>(`/products/${productId}/features`),

  create: (productId: string, data: CreateFeatureRequest) =>
    api.post<{ success: boolean; featureId: string }>(`/products/${productId}/features`, data),

  update: (productId: string, featureId: string, data: UpdateFeatureRequest) =>
    api.put<{ success: boolean }>(`/products/${productId}/features/${featureId}`, data),

  move: (productId: string, featureId: string, data: MoveFeatureRequest) =>
    api.put<{ success: boolean }>(`/products/${productId}/features/${featureId}/move`, data),

  delete: (productId: string, featureId: string) =>
    api.delete(`/products/${productId}/features/${featureId}`),
};
