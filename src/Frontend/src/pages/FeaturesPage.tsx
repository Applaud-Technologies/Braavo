import { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchFeatures, createFeature, updateFeature, deleteFeature, moveFeature } from '../store/slices/featuresSlice';
import { fetchProduct } from '../store/slices/productsSlice';
import { fetchDependencies } from '../store/slices/dependenciesSlice';
import { suggestFeatures, removeSuggestedFeature } from '../store/slices/aiSlice';
import type { RootState } from '../store/store';
import type { Feature, CreateFeatureRequest, FeaturePhase } from '../api/features';
import type { SuggestedFeature } from '../api/ai';
import { FeatureBoard } from '../components/prd/FeatureBoard';
import { FeatureEditor } from '../components/prd/FeatureEditor';
import AIButton from '../components/ai/AIButton';
import AISuggestionCard from '../components/ai/AISuggestionCard';

export function FeaturesPage() {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const { items: features, loading, error } = useAppSelector((state: RootState) => state.features);
  const { currentProduct } = useAppSelector((state: RootState) => state.products);
  const { suggestedFeatures, loading: aiLoading } = useAppSelector((state: RootState) => state.ai);
  const { status: depStatus } = useAppSelector((state: RootState) => state.dependencies);

  const phaseColors: Record<string, string> = {
    Mvp: 'bg-emerald-100 text-emerald-800',
    Enhanced: 'bg-amber-100 text-amber-800',
    Future: 'bg-sky-100 text-sky-800',
  };

  const [editorOpen, setEditorOpen] = useState(false);
  const [editingFeature, setEditingFeature] = useState<Feature | undefined>(undefined);
  const [defaultPhase, setDefaultPhase] = useState<FeaturePhase>('Mvp');

  useEffect(() => {
    if (id) {
      dispatch(fetchProduct(id));
      dispatch(fetchFeatures(id));
      dispatch(fetchDependencies(id));
    }
  }, [dispatch, id]);

  const canAddFeature = depStatus?.canCreateFeatures ?? false;

  const handleOpenCreate = (phase: FeaturePhase = 'Mvp') => {
    if (!canAddFeature) return;
    setEditingFeature(undefined);
    setDefaultPhase(phase);
    setEditorOpen(true);
  };

  const handleOpenEdit = (feature: Feature) => {
    setEditingFeature(feature);
    setEditorOpen(true);
  };

  const handleCloseEditor = () => {
    setEditorOpen(false);
    setEditingFeature(undefined);
  };

  const handleSave = async (data: CreateFeatureRequest) => {
    if (!id) return;

    if (editingFeature) {
      await dispatch(updateFeature({ productId: id, featureId: editingFeature.id, data })).unwrap();
    } else {
      await dispatch(createFeature({ productId: id, data })).unwrap();
    }

    handleCloseEditor();
  };

  const handleDelete = async (featureId: string) => {
    if (!id) return;
    if (!window.confirm('Delete this feature?')) return;
    await dispatch(deleteFeature({ productId: id, featureId })).unwrap();
  };

  const handleSuggestFeatures = () => {
    if (id) {
      dispatch(suggestFeatures({ productId: id }));
    }
  };

  const handleAcceptFeature = (feature: SuggestedFeature, index: number) => {
    setEditingFeature({
      id: '',
      name: feature.name,
      description: feature.description,
      phase: feature.phase,
      effort: feature.effort,
      linkedStoryIds: feature.linkedStoryIds,
      sortOrder: 0,
    });
    setDefaultPhase(feature.phase as FeaturePhase);
    setEditorOpen(true);
    dispatch(removeSuggestedFeature(index));
  };

  const handleMoveFeature = async (featureId: string, phase: FeaturePhase) => {
    if (!id) return;
    // Only dispatch if actually changing phase
    const feature = features.find((f) => f.id === featureId);
    if (feature && feature.phase === phase) return;
    await dispatch(moveFeature({ productId: id, featureId, phase })).unwrap();
  };

  if (loading && features.length === 0) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-stone-50">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-amber-600" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-stone-50">
      <div className="max-w-7xl mx-auto px-6 py-10">
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
        <div className="flex items-start justify-between mb-2">
          <div>
            <h1 className="text-3xl font-display font-semibold text-stone-800">
              Features &amp; Requirements
            </h1>
            {currentProduct && (
              <p className="text-stone-500 mt-1 text-sm">{currentProduct.name}</p>
            )}
          </div>
          <button
            onClick={() => handleOpenCreate()}
            disabled={!canAddFeature}
            className="btn-primary flex items-center gap-2 shrink-0 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Add Feature
          </button>
        </div>
        <p className="text-stone-400 text-sm mb-8">
          Drag features between phases to reprioritize
        </p>

        {/* AI Suggest Features */}
        <div className="card p-6 mb-8">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h3 className="font-display font-semibold text-lg">Suggest Features with AI</h3>
              <p className="text-sm text-stone-500">Analyzes your user stories to suggest features</p>
            </div>
            <AIButton onClick={handleSuggestFeatures} loading={aiLoading}>
              Suggest Features
            </AIButton>
          </div>

          {suggestedFeatures.length > 0 && (
            <div className="space-y-4 mt-4">
              {suggestedFeatures.map((feature, i) => (
                <AISuggestionCard
                  key={i}
                  title={feature.name}
                  onAccept={() => handleAcceptFeature(feature, i)}
                  onReject={() => dispatch(removeSuggestedFeature(i))}
                >
                  <div className="flex items-center gap-2 mb-2">
                    <span className={`badge ${phaseColors[feature.phase] ?? ''}`}>{feature.phase}</span>
                    {feature.effort && <span className="badge badge-draft">{feature.effort}</span>}
                  </div>
                  <p className="text-stone-600">{feature.description}</p>
                </AISuggestionCard>
              ))}
            </div>
          )}
        </div>

        {/* Dependency warning */}
        {depStatus && !depStatus.canCreateFeatures && (
          <div className="mb-6 p-4 bg-amber-50 border border-amber-200 rounded-xl">
            <div className="flex items-start gap-3">
              <svg className="w-5 h-5 text-amber-600 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
              <div>
                <p className="font-medium text-amber-800">Personas Required</p>
                <p className="text-sm text-amber-700 mt-1">
                  Create at least one persona before adding features.{' '}
                  <Link to={`/products/${id}/personas`} className="underline hover:text-amber-900">
                    Go to Personas
                  </Link>
                </p>
              </div>
            </div>
          </div>
        )}

        {/* Error state */}
        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm">
            <p className="font-medium">Error loading features</p>
            <p className="mt-1">{error}</p>
          </div>
        )}

        {/* Empty state */}
        {!loading && features.length === 0 && !error && (
          <div className="card p-12 text-center mb-8">
            <div className="text-5xl mb-4">🗂️</div>
            <h2 className="text-xl font-display font-semibold text-stone-700 mb-2">
              No features yet
            </h2>
            <p className="text-stone-500 mb-6">
              Add features to define what your product will do and organize them into phases.
            </p>
            <button onClick={() => handleOpenCreate()} className="btn-primary">
              + Add Feature
            </button>
          </div>
        )}

        {/* Board — show even when empty to allow drops */}
        {!error && (
          <FeatureBoard
            features={features}
            productId={id ?? ''}
            onAddFeature={handleOpenCreate}
            onEditFeature={handleOpenEdit}
            onDeleteFeature={handleDelete}
            onMoveFeature={handleMoveFeature}
          />
        )}
      </div>

      {/* Editor modal */}
      {editorOpen && (
        <FeatureEditor
          feature={editingFeature}
          defaultPhase={defaultPhase}
          onSave={handleSave}
          onCancel={handleCloseEditor}
        />
      )}
    </div>
  );
}
