import { api } from './client';

export type StoryPriority = 'Must' | 'Should' | 'Could' | 'Wont';

export interface UserStory {
  id: string;
  personaId: string | null;
  asA: string;
  iWant: string;
  soThat: string;
  priority: StoryPriority;
  acceptanceCriteria: string[];
  sortOrder: number;
}

export interface CreateUserStoryRequest {
  personaId?: string | null;
  asA: string;
  iWant: string;
  soThat: string;
  priority: StoryPriority;
  acceptanceCriteria?: string[];
  sortOrder?: number;
}

export interface UpdateUserStoryRequest {
  personaId?: string | null;
  asA?: string;
  iWant?: string;
  soThat?: string;
  priority?: StoryPriority;
  acceptanceCriteria?: string[];
  sortOrder?: number;
}

export const storiesApi = {
  list: (productId: string) =>
    api.get<UserStory[]>(`/products/${productId}/stories`),

  create: (productId: string, data: CreateUserStoryRequest) =>
    api.post<{ success: boolean; storyId: string }>(`/products/${productId}/stories`, data),

  update: (productId: string, storyId: string, data: UpdateUserStoryRequest) =>
    api.put<{ success: boolean }>(`/products/${productId}/stories/${storyId}`, data),

  delete: (productId: string, storyId: string) =>
    api.delete(`/products/${productId}/stories/${storyId}`),
};
