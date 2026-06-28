import { api } from './client';

export interface GeneratedPersona {
  name: string;
  role: string;
  technicalLevel: 'Low' | 'Medium' | 'High';
  goals: string[];
  painPoints: string[];
  quote: string;
}

export interface SuggestedStory {
  asA: string;
  iWant: string;
  soThat: string;
  priority: 'Must' | 'Should' | 'Could' | 'Wont';
  acceptanceCriteria: string[];
}

export interface SuggestedFeature {
  name: string;
  description: string;
  phase: 'Mvp' | 'Enhanced' | 'Future';
  effort: 'Small' | 'Medium' | 'Large' | null;
  linkedStoryIds: string[];
}

export const aiApi = {
  generatePersona: (productId: string, description: string) =>
    api.post<{ success: boolean; persona?: GeneratedPersona; error?: string }>(
      '/ai/generate-persona',
      { productId, description }
    ),

  suggestStories: (productId: string, personaId: string, additionalContext?: string) =>
    api.post<{ success: boolean; stories?: SuggestedStory[]; error?: string }>(
      '/ai/suggest-stories',
      { productId, personaId, additionalContext }
    ),

  suggestFeatures: (productId: string, storyIds?: string[]) =>
    api.post<{ success: boolean; features?: SuggestedFeature[]; error?: string }>(
      '/ai/suggest-features',
      { productId, storyIds }
    ),

  refineContent: (content: string, contentType: string, instruction?: string) =>
    api.post<{ success: boolean; refinedContent?: string; error?: string }>(
      '/ai/refine-content',
      { content, contentType, instruction }
    ),
};
