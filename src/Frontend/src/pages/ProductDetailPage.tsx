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
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-4xl mx-auto p-8">
        <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-red-700">
          Error: {error}
        </div>
        <button
          onClick={() => navigate('/products')}
          className="mt-4 px-4 py-2 text-blue-600 hover:underline"
        >
          Back to Products
        </button>
      </div>
    );
  }

  if (!currentProduct) {
    return (
      <div className="max-w-4xl mx-auto p-8">
        <p className="text-gray-500">Product not found</p>
        <button
          onClick={() => navigate('/products')}
          className="mt-4 px-4 py-2 text-blue-600 hover:underline"
        >
          Back to Products
        </button>
      </div>
    );
  }

  const statusColors: Record<string, string> = {
    Draft: 'bg-gray-100 text-gray-800',
    InProgress: 'bg-blue-100 text-blue-800',
    Review: 'bg-yellow-100 text-yellow-800',
    Final: 'bg-green-100 text-green-800',
  };

  return (
    <div className="max-w-4xl mx-auto p-8">
      <button
        onClick={() => navigate('/products')}
        className="mb-6 text-blue-600 hover:underline flex items-center gap-1"
      >
        ← Back to Products
      </button>

      <div className="bg-white rounded-lg border border-gray-200 p-6">
        <div className="flex justify-between items-start mb-4">
          <h1 className="text-2xl font-bold text-gray-900">{currentProduct.name}</h1>
          <span className={`px-3 py-1 text-sm rounded-full ${statusColors[currentProduct.status] ?? statusColors.Draft}`}>
            {currentProduct.status}
          </span>
        </div>

        <p className="text-gray-600 mb-6">{currentProduct.description}</p>

        <div className="mb-6">
          <div className="flex justify-between text-sm text-gray-500 mb-1">
            <span>Completion</span>
            <span>{currentProduct.completionPercentage}%</span>
          </div>
          <div className="w-full bg-gray-200 rounded-full h-2">
            <div
              className="bg-blue-600 h-2 rounded-full"
              style={{ width: `${currentProduct.completionPercentage}%` }}
            />
          </div>
        </div>

        <div className="grid grid-cols-2 gap-6 mb-8">
          <div>
            <h3 className="text-sm font-medium text-gray-500 mb-1">Version</h3>
            <p className="text-gray-900">{currentProduct.version}</p>
          </div>
          <div>
            <h3 className="text-sm font-medium text-gray-500 mb-1">Last Updated</h3>
            <p className="text-gray-900">{new Date(currentProduct.updatedAt).toLocaleDateString()}</p>
          </div>
        </div>

        {currentProduct.vision && (
          <div className="mb-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-2">Vision</h2>
            <p className="text-gray-600">{currentProduct.vision}</p>
          </div>
        )}

        {currentProduct.problemStatement && (
          <div className="mb-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-2">Problem Statement</h2>
            <p className="text-gray-600">{currentProduct.problemStatement}</p>
          </div>
        )}

        {currentProduct.valueProposition && (
          <div className="mb-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-2">Value Proposition</h2>
            <p className="text-gray-600">{currentProduct.valueProposition}</p>
          </div>
        )}

        <div className="border-t border-gray-200 pt-6 mt-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">PRD Sections</h2>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div className="p-4 bg-gray-50 rounded-lg text-center">
              <div className="text-2xl font-bold text-blue-600">{currentProduct.personas.length}</div>
              <div className="text-sm text-gray-500">Personas</div>
            </div>
            <div className="p-4 bg-gray-50 rounded-lg text-center">
              <div className="text-2xl font-bold text-blue-600">{currentProduct.userStories.length}</div>
              <div className="text-sm text-gray-500">User Stories</div>
            </div>
            <div className="p-4 bg-gray-50 rounded-lg text-center">
              <div className="text-2xl font-bold text-blue-600">{currentProduct.features.length}</div>
              <div className="text-sm text-gray-500">Features</div>
            </div>
            <div className="p-4 bg-gray-50 rounded-lg text-center">
              <div className="text-2xl font-bold text-blue-600">{currentProduct.version}</div>
              <div className="text-sm text-gray-500">Version</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
