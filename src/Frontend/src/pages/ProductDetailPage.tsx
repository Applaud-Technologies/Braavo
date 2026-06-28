import { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchProduct, clearCurrentProduct } from '../store/slices/productsSlice';
import type { RootState } from '../store/store';

export function ProductDetailPage() {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { currentProduct, loading, error } = useAppSelector((state: RootState) => state.products);

  useEffect(() => {
    if (id) {
      dispatch(fetchProduct(id));
    }
    return () => {
      dispatch(clearCurrentProduct());
    };
  }, [dispatch, id]);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-stone-50">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-stone-50 p-8">
        <div className="max-w-md mx-auto p-6 bg-red-50 border border-red-200 rounded-xl text-red-700">
          <p className="font-medium">Error loading product</p>
          <p className="text-sm mt-1">{error}</p>
          <button onClick={() => navigate('/products')} className="mt-4 text-primary-600 hover:underline">
            ← Back to Products
          </button>
        </div>
      </div>
    );
  }

  if (!currentProduct) {
    return (
      <div className="min-h-screen bg-stone-50 p-8">
        <div className="max-w-md mx-auto text-center">
          <p className="text-stone-500 mb-4">Product not found</p>
          <button onClick={() => navigate('/products')} className="text-primary-600 hover:underline">
            ← Back to Products
          </button>
        </div>
      </div>
    );
  }

  const statusConfig: Record<string, { bg: string; text: string; label: string }> = {
    Draft: { bg: 'bg-stone-100', text: 'text-stone-600', label: 'Draft' },
    InProgress: { bg: 'bg-amber-100', text: 'text-amber-800', label: 'In Progress' },
    Review: { bg: 'bg-sky-100', text: 'text-sky-800', label: 'Review' },
    Final: { bg: 'bg-emerald-100', text: 'text-emerald-800', label: 'Complete' },
  };

  const status = statusConfig[currentProduct.status] ?? statusConfig.Draft;

  const sections = [
    { label: 'Personas', count: currentProduct.personas.length, icon: '👥', path: 'personas' },
    { label: 'User Stories', count: currentProduct.userStories.length, icon: '📖', path: 'stories' },
    { label: 'Features', count: currentProduct.features.length, icon: '🎯', path: 'features' },
  ];

  return (
    <div className="min-h-screen bg-stone-50">
      <div className="max-w-4xl mx-auto px-6 py-10">
        <button
          onClick={() => navigate('/products')}
          className="mb-8 text-stone-500 hover:text-primary-600 transition-colors flex items-center gap-1 text-sm font-medium"
        >
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
          </svg>
          Back to Products
        </button>

        <div className="card p-8">
          <div className="flex justify-between items-start mb-6">
            <div>
              <h1 className="text-3xl font-display font-semibold text-stone-800">{currentProduct.name}</h1>
              <p className="text-stone-500 mt-2 leading-relaxed">{currentProduct.description}</p>
            </div>
            <span className={`px-3 py-1.5 text-sm font-medium rounded-full ${status.bg} ${status.text}`}>
              {status.label}
            </span>
          </div>

          <div className="mb-8">
            <div className="flex justify-between items-center text-sm mb-2">
              <span className="text-stone-400 font-medium uppercase tracking-wide">PRD Progress</span>
              <span className="text-stone-700 font-semibold">{currentProduct.completionPercentage}%</span>
            </div>
            <div className="w-full bg-stone-200 rounded-full h-2.5 overflow-hidden">
              <div
                className="bg-gradient-to-r from-primary-500 to-primary-600 h-full rounded-full transition-all duration-500"
                style={{ width: `${currentProduct.completionPercentage}%` }}
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-6 mb-8 py-6 border-y border-stone-100">
            <div>
              <p className="text-xs text-stone-400 uppercase tracking-wide font-medium mb-1">Version</p>
              <p className="text-stone-800 font-semibold">{currentProduct.version}</p>
            </div>
            <div>
              <p className="text-xs text-stone-400 uppercase tracking-wide font-medium mb-1">Last Updated</p>
              <p className="text-stone-800 font-semibold">{new Date(currentProduct.updatedAt).toLocaleDateString()}</p>
            </div>
          </div>

          {currentProduct.vision && (
            <div className="mb-6">
              <h2 className="text-lg font-display font-semibold text-stone-800 mb-2">Vision</h2>
              <p className="text-stone-600 leading-relaxed">{currentProduct.vision}</p>
            </div>
          )}

          {currentProduct.problemStatement && (
            <div className="mb-6">
              <h2 className="text-lg font-display font-semibold text-stone-800 mb-2">Problem Statement</h2>
              <p className="text-stone-600 leading-relaxed">{currentProduct.problemStatement}</p>
            </div>
          )}

          {currentProduct.valueProposition && (
            <div className="mb-6">
              <h2 className="text-lg font-display font-semibold text-stone-800 mb-2">Value Proposition</h2>
              <p className="text-stone-600 leading-relaxed">{currentProduct.valueProposition}</p>
            </div>
          )}

          <div className="pt-6 border-t border-stone-100">
            <h2 className="text-lg font-display font-semibold text-stone-800 mb-5">PRD Sections</h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {sections.map((section) => (
                <button
                  key={section.path}
                  onClick={() => navigate(`/products/${id}/${section.path}`)}
                  className="p-5 bg-stone-50 hover:bg-primary-50 border border-stone-200 hover:border-primary-300 rounded-xl text-left transition-all group"
                >
                  <div className="flex items-center justify-between mb-2">
                    <span className="text-2xl">{section.icon}</span>
                    <span className="text-2xl font-display font-bold text-primary-600">{section.count}</span>
                  </div>
                  <p className="text-stone-700 font-medium group-hover:text-primary-700 transition-colors">
                    {section.label}
                  </p>
                </button>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
