import { useState } from 'react';
import type { Feature, FeaturePhase } from '../../api/features';
import { FeatureCard } from './FeatureCard';

interface FeatureBoardProps {
  features: Feature[];
  productId: string;
  onAddFeature: (phase: FeaturePhase) => void;
  onEditFeature: (feature: Feature) => void;
  onDeleteFeature: (featureId: string) => void;
  onMoveFeature: (featureId: string, phase: FeaturePhase) => void;
}

const COLUMNS: { phase: FeaturePhase; label: string; headerClass: string; dotClass: string }[] = [
  {
    phase: 'Mvp',
    label: 'MVP (Phase 1)',
    headerClass: 'border-emerald-300',
    dotClass: 'bg-emerald-400',
  },
  {
    phase: 'Enhanced',
    label: 'Enhanced (Phase 2)',
    headerClass: 'border-amber-300',
    dotClass: 'bg-amber-400',
  },
  {
    phase: 'Future',
    label: 'Future (Phase 3)',
    headerClass: 'border-sky-300',
    dotClass: 'bg-sky-400',
  },
];

export function FeatureBoard({
  features,
  onAddFeature,
  onEditFeature,
  onDeleteFeature,
  onMoveFeature,
}: FeatureBoardProps) {
  const [dropTarget, setDropTarget] = useState<FeaturePhase | null>(null);

  const featuresByPhase = (phase: FeaturePhase) =>
    features.filter((f) => f.phase === phase).sort((a, b) => a.sortOrder - b.sortOrder);

  const handleDragOver = (e: React.DragEvent, phase: FeaturePhase) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
    setDropTarget(phase);
  };

  const handleDrop = (e: React.DragEvent, phase: FeaturePhase) => {
    e.preventDefault();
    const featureId = e.dataTransfer.getData('featureId');
    if (featureId) {
      onMoveFeature(featureId, phase);
    }
    setDropTarget(null);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    // Only clear if leaving the column entirely (not entering a child element)
    if (!e.currentTarget.contains(e.relatedTarget as Node)) {
      setDropTarget(null);
    }
  };

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
      {COLUMNS.map(({ phase, label, headerClass, dotClass }) => {
        const columnFeatures = featuresByPhase(phase);
        const isDropTarget = dropTarget === phase;

        return (
          <div
            key={phase}
            onDragOver={(e) => handleDragOver(e, phase)}
            onDrop={(e) => handleDrop(e, phase)}
            onDragLeave={handleDragLeave}
            className={`flex flex-col rounded-2xl border-2 transition-all ${
              isDropTarget
                ? 'border-amber-400 bg-amber-50/60 shadow-inner'
                : 'border-stone-200 bg-stone-50'
            }`}
          >
            {/* Column header */}
            <div
              className={`flex items-center justify-between px-4 py-3 border-b-2 ${headerClass} bg-white rounded-t-xl`}
            >
              <div className="flex items-center gap-2">
                <span className={`w-2.5 h-2.5 rounded-full ${dotClass}`} />
                <span className="text-sm font-semibold text-stone-700">{label}</span>
                <span className="text-xs text-stone-400 font-normal">({columnFeatures.length})</span>
              </div>
              <button
                onClick={() => onAddFeature(phase)}
                className="text-xs px-2.5 py-1 rounded-lg bg-stone-100 text-stone-500 hover:bg-amber-50 hover:text-amber-700 border border-stone-200 hover:border-amber-300 transition-colors font-medium"
                title={`Add feature to ${label}`}
              >
                + Add
              </button>
            </div>

            {/* Cards area */}
            <div className="flex-1 p-3 space-y-3 min-h-[200px]">
              {columnFeatures.length === 0 && (
                <div
                  className={`flex items-center justify-center h-24 rounded-xl border-2 border-dashed transition-colors text-xs ${
                    isDropTarget
                      ? 'border-amber-300 text-amber-500'
                      : 'border-stone-200 text-stone-400'
                  }`}
                >
                  {isDropTarget ? 'Drop here' : 'No features yet'}
                </div>
              )}
              {columnFeatures.map((feature) => (
                <FeatureCard
                  key={feature.id}
                  feature={feature}
                  onEdit={onEditFeature}
                  onDelete={onDeleteFeature}
                />
              ))}
            </div>
          </div>
        );
      })}
    </div>
  );
}
