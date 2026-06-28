import { api } from './client';

export interface Persona {
  id: string;
  name: string;
  role: string;
  technicalLevel: 'Low' | 'Medium' | 'High';
  goals: string[];
  painPoints: string[];
  quote: string;
  sortOrder: number;
}

export interface CreatePersonaRequest {
  name: string;
  role: string;
  technicalLevel: 'Low' | 'Medium' | 'High';
  goals: string[];
  painPoints: string[];
  quote: string;
  sortOrder?: number;
}

export interface UpdatePersonaRequest {
  name?: string;
  role?: string;
  technicalLevel?: 'Low' | 'Medium' | 'High';
  goals?: string[];
  painPoints?: string[];
  quote?: string;
  sortOrder?: number;
}

export const personasApi = {
  list: (productId: string) =>
    api.get<Persona[]>(`/products/${productId}/personas`),

  create: (productId: string, data: CreatePersonaRequest) =>
    api.post<{ success: boolean; personaId: string }>(`/products/${productId}/personas`, data),

  update: (productId: string, personaId: string, data: UpdatePersonaRequest) =>
    api.put<{ success: boolean }>(`/products/${productId}/personas/${personaId}`, data),

  delete: (productId: string, personaId: string) =>
    api.delete(`/products/${productId}/personas/${personaId}`),
};
