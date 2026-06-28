import { useState } from 'react';
import type { Feature, EffortSize } from '../../api/features';

interface FeatureCardProps {
  feature: Feature;
  onEdit: (feature: Feature) => void;
  onDelete: (featureId: string) => void;
}

const effortConfig: Record<EffortSize, { label: string; badgeClass: string }> = {
  Small: { label: 'S', badgeClass: 'bg-emerald-100 text-emerald-800 border-emerald-200' },
  Medium: { label: 'M', badgeClass: 'bg-amber-100 text-amber-800 border-amber-200' },
  Large: { label: 'L', badgeClass: 'bg-red-100 text-red-800 border-red-200' },
};

export function FeatureCard({ feature, onEdit, onDelete }: FeatureCardProps) {
  const [dragging, setDragging] = useState(false);

  const effort = feature.effort ? effortConfig[feature.effort] : null;

  return (
    <div
      draggable
      onDragStart={(e) => {
        e.dataTransfer.setData('featureId', feature.id);
        e.dataTransfer.effectAllowed = 'move';
        setDragging(true);
      }}
      onDragEnd={() => setDragging(false)}
      className={`card p-4 flex flex-col gap-3 select-none transition-all ${
        dragging
          ? 'opacity-40 cursor-grabbing shadow-lg rotate-1'
          : 'cursor-grab hover:shadow-md'
      }`}
    >
      {/* Header: effort badge + actions */}
      <div className="flex items-start justify-between gap-2">
        <div className="flex items-center gap-2 flex-wrap">
          {effort && (
            <span
              className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-semibold border ${effort.badgeClass}`}
              title={`Effort: ${feature.effort}`}
            >
              {effort.label}
            </span>
          )}
          {feature.linkedStoryIds.length > 0 && (
            <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-stone-100 text-stone-500 border border-stone-200">
              <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M13.828 10.172a4 4 0 00-5.656 0l-4 4a4 4 0 105.656 5.656l1.102-1.101m-.758-4.899a4 4 0 005.656 0l4-4a4 4 0 00-5.656-5.656l-1.1 1.1"
                />
              </svg>
              {feature.linkedStoryIds.length}
            </span>
          )}
        </div>

        <div className="flex items-center gap-1 shrink-0">
          <button
            onClick={(e) => {
              e.stopPropagation();
              onEdit(feature);
            }}
            className="text-xs px-2 py-1 rounded-md bg-stone-100 text-stone-500 hover:bg-amber-50 hover:text-amber-700 border border-stone-200 hover:border-amber-300 transition-colors"
            title="Edit feature"
          >
            Edit
          </button>
          <button
            onClick={(e) => {
              e.stopPropagation();
              onDelete(feature.id);
            }}
            className="text-xs px-2 py-1 rounded-md bg-stone-100 text-stone-500 hover:bg-red-50 hover:text-red-600 border border-stone-200 hover:border-red-200 transition-colors"
            title="Delete feature"
          >
            Del
          </button>
        </div>
      </div>

      {/* Name */}
      <p className="text-sm font-semibold text-stone-800 leading-snug">{feature.name}</p>

      {/* Description (truncated) */}
      {feature.description && (
        <p className="text-xs text-stone-500 line-clamp-2 leading-relaxed">{feature.description}</p>
      )}
    </div>
  );
}
