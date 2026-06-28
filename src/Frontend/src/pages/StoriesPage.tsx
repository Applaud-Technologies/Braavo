import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchStories, createStory, updateStory } from '../store/slices/storiesSlice';
import { fetchPersonas } from '../store/slices/personasSlice';
import { fetchProduct } from '../store/slices/productsSlice';
import { suggestStories, removeSuggestedStory, clearSuggestedStories } from '../store/slices/aiSlice';
import type { RootState } from '../store/store';
import type { UserStory, CreateUserStoryRequest, StoryPriority } from '../api/stories';
import type { SuggestedStory } from '../api/ai';
import { StoryCard } from '../components/prd/StoryCard';
import { StoryEditor } from '../components/prd/StoryEditor';
import AIButton from '../components/ai/AIButton';
import AISuggestionCard from '../components/ai/AISuggestionCard';

const PRIORITY_SECTIONS: { key: StoryPriority; label: string; emptyLabel: string }[] = [
  { key: 'Must', label: 'Must Have', emptyLabel: 'No must-have stories yet' },
  { key: 'Should', label: 'Should Have', emptyLabel: 'No should-have stories yet' },
  { key: 'Could', label: 'Could Have', emptyLabel: 'No could-have stories yet' },
  { key: 'Wont', label: "Won't Have", emptyLabel: 'No out-of-scope stories yet' },
];

const priorityHeaderClass: Record<StoryPriority, string> = {
  Must: 'border-l-4 border-emerald-400 pl-3',
  Should: 'border-l-4 border-amber-400 pl-3',
  Could: 'border-l-4 border-sky-400 pl-3',
  Wont: 'border-l-4 border-stone-300 pl-3',
};

export function StoriesPage() {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const { items: stories, loading, error } = useAppSelector((state: RootState) => state.stories);
  const { items: personas } = useAppSelector((state: RootState) => state.personas);
  const { currentProduct } = useAppSelector((state: RootState) => state.products);
  const { suggestedStories, loading: aiLoading } = useAppSelector((state: RootState) => state.ai);

  const [editorOpen, setEditorOpen] = useState(false);
  const [editingStory, setEditingStory] = useState<UserStory | undefined>(undefined);
  const [filterPersonaId, setFilterPersonaId] = useState<string>('');
  const [selectedPersonaId, setSelectedPersonaId] = useState<string>('');
  const [storyContext, setStoryContext] = useState('');

  useEffect(() => {
    if (id) {
      dispatch(fetchProduct(id));
      dispatch(fetchPersonas(id));
      dispatch(fetchStories(id));
    }
  }, [dispatch, id]);

  const handleOpenCreate = () => {
    setEditingStory(undefined);
    setEditorOpen(true);
  };

  const handleOpenEdit = (story: UserStory) => {
    setEditingStory(story);
    setEditorOpen(true);
  };

  const handleCloseEditor = () => {
    setEditorOpen(false);
    setEditingStory(undefined);
  };

  const handleSuggestStories = () => {
    if (id && selectedPersonaId) {
      dispatch(suggestStories({
        productId: id,
        personaId: selectedPersonaId,
        context: storyContext.trim() || undefined,
      }));
    }
  };

  const handleAcceptStory = (story: SuggestedStory, index: number) => {
    setEditingStory({
      id: '',
      personaId: selectedPersonaId,
      asA: story.asA,
      iWant: story.iWant,
      soThat: story.soThat,
      priority: story.priority,
      acceptanceCriteria: story.acceptanceCriteria,
      sortOrder: 0,
    } as unknown as UserStory);
    setEditorOpen(true);
    dispatch(removeSuggestedStory(index));
  };

  const handleSave = async (data: CreateUserStoryRequest) => {
    if (!id) return;

    if (editingStory) {
      await dispatch(updateStory({ productId: id, storyId: editingStory.id, data })).unwrap();
    } else {
      await dispatch(createStory({ productId: id, data })).unwrap();
    }

    handleCloseEditor();
  };

  // Apply persona filter
  const filteredStories = filterPersonaId
    ? stories.filter((s) => s.personaId === filterPersonaId)
    : stories;

  // Build a quick lookup: personaId -> Persona
  const personaMap = new Map(personas.map((p) => [p.id, p]));

  const totalCount = filteredStories.length;

  if (loading && stories.length === 0) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-stone-50">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-amber-600" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-stone-50">
      <div className="max-w-5xl mx-auto px-6 py-10">
        {/* Back link */}
        <button
          onClick={() => navigate(`/products/${id}`)}
          className="mb-8 text-stone-500 hover:text-amber-700 transition-colors flex items-center gap-1 text-sm font-medium"
        >
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
          </svg>
          {currentProduct ? `Back to ${currentProduct.name}` : 'Back to Product'}
        </button>

        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-3xl font-display font-semibold text-stone-800">User Stories</h1>
            {currentProduct && (
              <p className="text-stone-500 mt-1 text-sm">{currentProduct.name}</p>
            )}
          </div>
          <button onClick={handleOpenCreate} className="btn-primary flex items-center gap-2">
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Add Story
          </button>
        </div>

        {/* AI Story Suggestions */}
        <div className="card p-6 mb-8">
          <h3 className="font-display font-semibold text-lg mb-4">Suggest Stories with AI</h3>
          <div className="flex gap-4 mb-4 flex-wrap">
            <select
              value={selectedPersonaId}
              onChange={(e) => setSelectedPersonaId(e.target.value)}
              className="input"
            >
              <option value="">Select a persona...</option>
              {personas.map((p) => (
                <option key={p.id} value={p.id}>{p.name}</option>
              ))}
            </select>
            <input
              type="text"
              value={storyContext}
              onChange={(e) => setStoryContext(e.target.value)}
              placeholder="Additional context (optional)"
              className="input flex-1"
            />
            <AIButton
              onClick={handleSuggestStories}
              loading={aiLoading}
              disabled={!selectedPersonaId}
            >
              Suggest Stories
            </AIButton>
          </div>

          {suggestedStories.length > 0 && (
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <p className="text-sm text-stone-500">{suggestedStories.length} suggestion(s)</p>
                <button
                  onClick={() => dispatch(clearSuggestedStories())}
                  className="text-xs text-stone-400 hover:text-red-600 underline"
                >
                  Dismiss all
                </button>
              </div>
              {suggestedStories.map((story, i) => (
                <AISuggestionCard
                  key={i}
                  title={`As a ${story.asA}...`}
                  onAccept={() => handleAcceptStory(story, i)}
                  onReject={() => dispatch(removeSuggestedStory(i))}
                >
                  <p className="text-stone-700">I want {story.iWant}</p>
                  <p className="text-stone-600 text-sm mt-1">So that {story.soThat}</p>
                </AISuggestionCard>
              ))}
            </div>
          )}
        </div>

        {/* Persona filter */}
        {personas.length > 0 && (
          <div className="mb-8 flex items-center gap-3">
            <label className="text-sm font-medium text-stone-600 shrink-0">Filter by persona:</label>
            <select
              value={filterPersonaId}
              onChange={(e) => setFilterPersonaId(e.target.value)}
              className="input max-w-xs"
            >
              <option value="">All personas</option>
              {personas.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name} — {p.role}
                </option>
              ))}
            </select>
            {filterPersonaId && (
              <button
                onClick={() => setFilterPersonaId('')}
                className="text-xs text-stone-500 hover:text-amber-700 underline"
              >
                Clear
              </button>
            )}
          </div>
        )}

        {/* Error state */}
        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm">
            <p className="font-medium">Error loading stories</p>
            <p className="mt-1">{error}</p>
          </div>
        )}

        {/* Empty state */}
        {!loading && totalCount === 0 && !error && (
          <div className="card p-12 text-center">
            <div className="text-5xl mb-4">📖</div>
            <h2 className="text-xl font-display font-semibold text-stone-700 mb-2">
              {filterPersonaId ? 'No stories for this persona' : 'No user stories yet'}
            </h2>
            <p className="text-stone-500 mb-6">
              {filterPersonaId
                ? 'Try selecting a different persona or clear the filter.'
                : 'Create your first user story to define what your product should do.'}
            </p>
            {!filterPersonaId && (
              <button onClick={handleOpenCreate} className="btn-primary">
                + Add Story
              </button>
            )}
          </div>
        )}

        {/* Priority sections */}
        {totalCount > 0 && (
          <div className="space-y-10">
            {PRIORITY_SECTIONS.map(({ key, label }) => {
              const sectionStories = filteredStories.filter((s) => s.priority === key);
              if (sectionStories.length === 0) return null;

              return (
                <section key={key}>
                  <h2
                    className={`text-lg font-display font-semibold text-stone-700 mb-4 ${priorityHeaderClass[key]}`}
                  >
                    {label}
                    <span className="ml-2 text-sm font-normal text-stone-400">
                      ({sectionStories.length})
                    </span>
                  </h2>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    {sectionStories.map((story) => (
                      <StoryCard
                        key={story.id}
                        story={story}
                        persona={story.personaId ? personaMap.get(story.personaId) : undefined}
                        onEdit={handleOpenEdit}
                      />
                    ))}
                  </div>
                </section>
              );
            })}
          </div>
        )}
      </div>

      {/* Editor modal */}
      {editorOpen && (
        <StoryEditor
          story={editingStory}
          personas={personas}
          onSave={handleSave}
          onCancel={handleCloseEditor}
        />
      )}
    </div>
  );
}
