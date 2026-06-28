import { Link } from 'react-router-dom';
import type { ProductSummary } from '../../api/products';

interface ProductCardProps {
  product: ProductSummary;
}

export function ProductCard({ product }: ProductCardProps) {
  const statusColors: Record<string, string> = {
    Draft: 'bg-gray-100 text-gray-800',
    InProgress: 'bg-blue-100 text-blue-800',
    Review: 'bg-yellow-100 text-yellow-800',
    Final: 'bg-green-100 text-green-800',
  };

  return (
    <Link
      to={`/products/${product.id}`}
      className="block p-6 bg-white rounded-lg border border-gray-200 hover:border-blue-500 transition-colors"
    >
      <div className="flex justify-between items-start mb-2">
        <h3 className="text-lg font-semibold text-gray-900">{product.name}</h3>
        <span className={`px-2 py-1 text-xs rounded-full ${statusColors[product.status] ?? statusColors.Draft}`}>
          {product.status}
        </span>
      </div>
      <p className="text-gray-600 text-sm mb-4 line-clamp-2">{product.description}</p>
      <div className="flex justify-between items-center">
        <div className="w-full bg-gray-200 rounded-full h-2">
          <div
            className="bg-blue-600 h-2 rounded-full"
            style={{ width: `${product.completionPercentage}%` }}
          />
        </div>
        <span className="ml-2 text-sm text-gray-500">{product.completionPercentage}%</span>
      </div>
    </Link>
  );
}
