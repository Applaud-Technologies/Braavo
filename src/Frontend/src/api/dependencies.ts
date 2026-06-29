import { api } from './client';

export interface DependencyStatus {
  hasPersonas: boolean;
  hasFeatures: boolean;
  hasStories: boolean;
  canCreateFeatures: boolean;
  canCreateStories: boolean;
}

export const dependenciesApi = {
  get: (productId: string) =>
    api.get<DependencyStatus>(`/products/${productId}/dependencies`),
};
