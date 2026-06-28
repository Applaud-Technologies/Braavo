import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchPersonas, createPersona, updatePersona, deletePersona } from '../store/slices/personasSlice';
import { fetchProduct } from '../store/slices/productsSlice';
import { generatePersona, clearSuggestedPersona } from '../store/slices/aiSlice';
import type { RootState } from '../store/store';
import type { Persona, CreatePersonaRequest } from '../api/personas';
import { PersonaCard } from '../components/prd/PersonaCard';
import { PersonaEditor } from '../components/prd/PersonaEditor';
import AIButton from '../components/ai/AIButton';
import AISuggestionCard from '../components/ai/AISuggestionCard';

export function PersonasPage() {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const { items: personas, loading, error } = useAppSelector((state: RootState) => state.personas);
  const { currentProduct } = useAppSelector((state: RootState) => state.products);
  const { suggestedPersona, loading: aiLoading } = useAppSelector((state: RootState) => state.ai);

  const [editorOpen, setEditorOpen] = useState(false);
  const [editingPersona, setEditingPersona] = useState<Persona | undefined>(undefined);
  const [aiDescription, setAiDescription] = useState('');
  const [deleteTarget, setDeleteTarget] = useState<Persona | null>(null);
  const [deleteConfirming, setDeleteConfirming] = useState(false);

  useEffect(() => {
    if (id) {
      dispatch(fetchProduct(id));
      dispatch(fetchPersonas(id));
    }
  }, [dispatch, id]);

  const handleOpenCreate = () => {
    setEditingPersona(undefined);
    setEditorOpen(true);
  };

  const handleOpenEdit = (persona: Persona) => {
    setEditingPersona(persona);
    setEditorOpen(true);
  };

  const handleCloseEditor = () => {
    setEditorOpen(false);
    setEditingPersona(undefined);
  };

  const handleSave = async (data: CreatePersonaRequest) => {
    if (!id) return;

    if (editingPersona?.id) {
      await dispatch(updatePersona({ productId: id, personaId: editingPersona.id, data })).unwrap();
    } else {
      await dispatch(createPersona({ productId: id, data })).unwrap();
    }

    handleCloseEditor();
  };

  const handleGeneratePersona = () => {
    if (id && aiDescription.trim()) {
      dispatch(generatePersona({ productId: id, description: aiDescription.trim() }));
    }
  };

  const handleAcceptPersona = () => {
    if (suggestedPersona) {
      setEditingPersona({ ...suggestedPersona, id: '', sortOrder: 0 });
      setEditorOpen(true);
      dispatch(clearSuggestedPersona());
    }
  };

  const handleDeleteRequest = (persona: Persona) => {
    setDeleteTarget(persona);
  };

  const handleDeleteConfirm = async () => {
    if (!id || !deleteTarget) return;

    setDeleteConfirming(true);
    try {
      await dispatch(deletePersona({ productId: id, personaId: deleteTarget.id })).unwrap();
    } finally {
      setDeleteConfirming(false);
      setDeleteTarget(null);
    }
  };

  const handleDeleteCancel = () => {
    setDeleteTarget(null);
  };

  if (loading && personas.length === 0) {
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
        <div className="flex items-center justify-between mb-8">
          <div>
            <h1 className="text-3xl font-display font-semibold text-stone-800">User Personas</h1>
            {currentProduct && (
              <p className="text-stone-500 mt-1 text-sm">{currentProduct.name}</p>
            )}
          </div>
          <button onClick={handleOpenCreate} className="btn-primary flex items-center gap-2">
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Add Persona
          </button>
        </div>

        {/* Error state */}
        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm">
            <p className="font-medium">Error loading personas</p>
            <p className="mt-1">{error}</p>
          </div>
        )}

        {/* AI Generate section */}
        <div className="card p-6 mb-8">
          <h3 className="font-display font-semibold text-lg mb-4">Generate Persona with AI</h3>
          <div className="flex gap-4">
            <input
              type="text"
              value={aiDescription}
              onChange={(e) => setAiDescription(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && handleGeneratePersona()}
              placeholder="Describe a user type (e.g., 'A busy project manager who needs to track multiple projects')"
              className="input flex-1"
            />
            <AIButton
              onClick={handleGeneratePersona}
              loading={aiLoading}
              disabled={!aiDescription.trim()}
            >
              Generate
            </AIButton>
          </div>

          {suggestedPersona && (
            <div className="mt-4">
              <AISuggestionCard
                title={suggestedPersona.name}
                onAccept={handleAcceptPersona}
                onReject={() => dispatch(clearSuggestedPersona())}
                acceptLabel="Open in Editor"
              >
                <p className="text-stone-600">{suggestedPersona.role}</p>
                {suggestedPersona.goals.length > 0 && (
                  <p className="text-sm text-stone-500 mt-2">
                    Goals: {suggestedPersona.goals.join(', ')}
                  </p>
                )}
                {suggestedPersona.painPoints.length > 0 && (
                  <p className="text-sm text-stone-500 mt-1">
                    Pain points: {suggestedPersona.painPoints.join(', ')}
                  </p>
                )}
                {suggestedPersona.quote && (
                  <p className="text-sm text-stone-400 italic mt-2">"{suggestedPersona.quote}"</p>
                )}
              </AISuggestionCard>
            </div>
          )}
        </div>

        {/* Empty state */}
        {!loading && personas.length === 0 && !error && (
          <div className="card p-12 text-center">
            <div className="text-5xl mb-4">👥</div>
            <h2 className="text-xl font-display font-semibold text-stone-700 mb-2">No personas yet</h2>
            <p className="text-stone-500 mb-6">
              Create your first user persona to define who your product is for.
            </p>
            <button onClick={handleOpenCreate} className="btn-primary">
              + Add Persona
            </button>
          </div>
        )}

        {/* Persona grid */}
        {personas.length > 0 && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {personas.map((persona) => (
              <PersonaCard
                key={persona.id}
                persona={persona}
                onEdit={handleOpenEdit}
                onDelete={handleDeleteRequest}
              />
            ))}
          </div>
        )}
      </div>

      {/* Editor modal */}
      {editorOpen && (
        <PersonaEditor
          persona={editingPersona}
          onSave={handleSave}
          onCancel={handleCloseEditor}
        />
      )}

      {/* Delete confirm dialog */}
      {deleteTarget && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-stone-900/50 backdrop-blur-sm">
          <div className="bg-white rounded-2xl shadow-xl w-full max-w-sm p-8">
            <h2 className="text-lg font-display font-semibold text-stone-800 mb-2">Delete Persona</h2>
            <p className="text-stone-600 text-sm mb-6">
              Are you sure you want to delete <strong>{deleteTarget.name}</strong>? This action cannot be undone.
            </p>
            <div className="flex gap-3">
              <button
                onClick={handleDeleteConfirm}
                disabled={deleteConfirming}
                className="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg font-medium hover:bg-red-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {deleteConfirming ? 'Deleting...' : 'Delete'}
              </button>
              <button
                onClick={handleDeleteCancel}
                disabled={deleteConfirming}
                className="btn-secondary"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
