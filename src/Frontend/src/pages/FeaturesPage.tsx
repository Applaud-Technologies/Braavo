import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchFeatures, createFeature, updateFeature, deleteFeature, moveFeature } from '../store/slices/featuresSlice';
import { fetchProduct } from '../store/slices/productsSlice';
import type { RootState } from '../store/store';
import type { Feature, CreateFeatureRequest, FeaturePhase } from '../api/features';
import { FeatureBoard } from '../components/prd/FeatureBoard';
import { FeatureEditor } from '../components/prd/FeatureEditor';

export function FeaturesPage() {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const { items: features, loading, error } = useAppSelector((state: RootState) => state.features);
  const { currentProduct } = useAppSelector((state: RootState) => state.products);

  const [editorOpen, setEditorOpen] = useState(false);
  const [editingFeature, setEditingFeature] = useState<Feature | undefined>(undefined);
  const [defaultPhase, setDefaultPhase] = useState<FeaturePhase>('Mvp');

  useEffect(() => {
    if (id) {
      dispatch(fetchProduct(id));
      dispatch(fetchFeatures(id));
    }
  }, [dispatch, id]);

  const handleOpenCreate = (phase: FeaturePhase = 'Mvp') => {
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
            className="btn-primary flex items-center gap-2 shrink-0"
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
