import { api } from './client';

export interface ProductSummary {
  id: string;
  name: string;
  description: string;
  status: string;
  completionPercentage: number;
  updatedAt: string;
}

export interface PersonaDto {
  id: string;
  name: string;
  role: string;
  technicalLevel: string;
  goals: string[];
  painPoints: string[];
  quote: string;
}

export interface UserStoryDto {
  id: string;
  personaId: string | null;
  asA: string;
  iWant: string;
  soThat: string;
  priority: string;
  acceptanceCriteria: string[];
}

export interface FeatureDto {
  id: string;
  parentId: string | null;
  name: string;
  description: string;
  phase: string;
  effort: string | null;
  linkedStoryIds: string[];
}

export interface Product extends ProductSummary {
  version: number;
  vision: string;
  problemStatement: string;
  valueProposition: string;
  targetMarket: string[];
  businessGoals: string[];
  createdAt: string;
  personas: PersonaDto[];
  userStories: UserStoryDto[];
  features: FeatureDto[];
}

export interface CreateProductRequest {
  name: string;
  description: string;
  categories?: string[];
}

export const productsApi = {
  list: () => api.get<ProductSummary[]>('/products'),
  get: (id: string) => api.get<Product>(`/products/${id}`),
  create: (data: CreateProductRequest) => api.post<{ productId: string }>('/products', data),
  delete: (id: string) => api.delete(`/products/${id}`),
};
