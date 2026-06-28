import { api } from './client';

export interface TimelineMilestone {
  name: string;
  date: string; // ISO date string YYYY-MM-DD
}

export interface TimelinePhase {
  id: string;
  name: string;
  startDate: string; // ISO date string YYYY-MM-DD
  endDate: string;   // ISO date string YYYY-MM-DD
  milestones: TimelineMilestone[];
}

export interface TimelineResponse {
  phases: TimelinePhase[];
}

export const timelineApi = {
  get: (productId: string) =>
    api.get<TimelineResponse>(`/products/${productId}/timeline`),

  update: (productId: string, phases: TimelinePhase[]) =>
    api.put<{ success: boolean }>(`/products/${productId}/timeline`, { phases }),
};
