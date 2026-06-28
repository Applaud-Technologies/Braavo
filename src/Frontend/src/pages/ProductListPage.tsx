import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import type { RootState } from '../store/store';
import { fetchProducts } from '../store/slices/productsSlice';
import type { ProductSummary } from '../api/products';
import { ProductCard } from '../components/products/ProductCard';

export function ProductListPage() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { items, loading, error } = useAppSelector((state: RootState) => state.products);

  useEffect(() => {
    dispatch(fetchProducts());
  }, [dispatch]);

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
          <p className="font-medium">Error loading products</p>
          <p className="text-sm mt-1">{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-stone-50">
      <div className="max-w-6xl mx-auto px-6 py-10">
        <div className="flex justify-between items-center mb-10">
          <div>
            <h1 className="text-3xl font-display font-semibold text-stone-800">Your Products</h1>
            <p className="text-stone-500 mt-1">Manage your product requirement documents</p>
          </div>
          <button
            onClick={() => navigate('/products/new')}
            className="btn-primary"
          >
            + New Product
          </button>
        </div>

        {items.length === 0 ? (
          <div className="text-center py-16">
            <div className="w-16 h-16 mx-auto mb-6 rounded-full bg-primary-100 flex items-center justify-center">
              <svg className="w-8 h-8 text-primary-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
            </div>
            <h2 className="text-xl font-display font-semibold text-stone-700 mb-2">No products yet</h2>
            <p className="text-stone-500 mb-6">Create your first PRD to get started</p>
            <button
              onClick={() => navigate('/products/new')}
              className="btn-primary"
            >
              Create Your First Product
            </button>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {items.map((product: ProductSummary) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
