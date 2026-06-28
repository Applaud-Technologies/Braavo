import { api } from './client';

export interface TimelineMilestone {
  id: string;
  name: string;
  weekNumber: number;
  deliverables: string[];
  status: string; // "planned" | "in-progress" | "completed"
}

export interface TimelinePhase {
  id: string;
  name: string;
  durationWeeks: number;
  startDate: string | null; // ISO date string or null
  sortOrder: number;
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
