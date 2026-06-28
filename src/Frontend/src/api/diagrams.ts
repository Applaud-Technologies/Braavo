import { api } from './client';

export interface DiagramResult {
  mermaidCode: string;
}

export const diagramsApi = {
  generateGantt: (productId: string) =>
    api.post<DiagramResult>(`/products/${productId}/diagrams/gantt`),

  generateUserJourney: (productId: string, personaId?: string) =>
    api.post<DiagramResult>(`/products/${productId}/diagrams/user-journey`, { personaId }),

  generateFeatureHierarchy: (productId: string) =>
    api.post<DiagramResult>(`/products/${productId}/diagrams/feature-hierarchy`),
};
