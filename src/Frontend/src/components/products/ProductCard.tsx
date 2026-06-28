import { Link } from 'react-router-dom';
import type { ProductSummary } from '../../api/products';

interface ProductCardProps {
  product: ProductSummary;
}

export function ProductCard({ product }: ProductCardProps) {
  const statusConfig: Record<string, { bg: string; text: string; label: string }> = {
    Draft: { bg: 'bg-stone-100', text: 'text-stone-600', label: 'Draft' },
    InProgress: { bg: 'bg-amber-100', text: 'text-amber-800', label: 'In Progress' },
    Review: { bg: 'bg-sky-100', text: 'text-sky-800', label: 'Review' },
    Final: { bg: 'bg-emerald-100', text: 'text-emerald-800', label: 'Complete' },
  };

  const status = statusConfig[product.status] ?? statusConfig.Draft;

  return (
    <Link
      to={`/products/${product.id}`}
      className="card-hover block p-6 group"
    >
      <div className="flex justify-between items-start mb-3">
        <h3 className="text-lg font-display font-semibold text-stone-800 group-hover:text-primary-700 transition-colors">
          {product.name}
        </h3>
        <span className={`px-2.5 py-1 text-xs font-medium rounded-full ${status.bg} ${status.text}`}>
          {status.label}
        </span>
      </div>

      <p className="text-stone-500 text-sm mb-5 line-clamp-2 leading-relaxed">
        {product.description || 'No description yet'}
      </p>

      <div className="space-y-2">
        <div className="flex justify-between items-center text-xs">
          <span className="text-stone-400 font-medium uppercase tracking-wide">Progress</span>
          <span className="text-stone-600 font-semibold">{product.completionPercentage}%</span>
        </div>
        <div className="w-full bg-stone-200 rounded-full h-1.5 overflow-hidden">
          <div
            className="bg-gradient-to-r from-primary-500 to-primary-600 h-full rounded-full transition-all duration-500"
            style={{ width: `${product.completionPercentage}%` }}
          />
        </div>
      </div>
    </Link>
  );
}
