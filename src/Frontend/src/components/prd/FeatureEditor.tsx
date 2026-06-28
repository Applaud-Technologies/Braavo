import { useState, useEffect } from 'react';
import type { Feature, CreateFeatureRequest, FeaturePhase, EffortSize } from '../../api/features';

interface FeatureEditorProps {
  feature?: Feature;
  defaultPhase?: FeaturePhase;
  onSave: (data: CreateFeatureRequest) => Promise<void>;
  onCancel: () => void;
}

const PHASES: { value: FeaturePhase; label: string }[] = [
  { value: 'Mvp', label: 'MVP (Phase 1)' },
  { value: 'Enhanced', label: 'Enhanced (Phase 2)' },
  { value: 'Future', label: 'Future (Phase 3)' },
];

const EFFORTS: { value: EffortSize; label: string }[] = [
  { value: 'Small', label: 'Small' },
  { value: 'Medium', label: 'Medium' },
  { value: 'Large', label: 'Large' },
];

export function FeatureEditor({ feature, defaultPhase = 'Mvp', onSave, onCancel }: FeatureEditorProps) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [phase, setPhase] = useState<FeaturePhase>(defaultPhase);
  const [effort, setEffort] = useState<EffortSize | ''>('');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (feature) {
      setName(feature.name);
      setDescription(feature.description);
      setPhase(feature.phase);
      setEffort(feature.effort ?? '');
    } else {
      setPhase(defaultPhase);
    }
  }, [feature, defaultPhase]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) {
      setError('Name is required');
      return;
    }

    setSaving(true);
    setError(null);

    try {
      await onSave({
        name: name.trim(),
        description: description.trim(),
        phase,
        effort: effort || null,
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save feature');
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-stone-900/50 backdrop-blur-sm">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg">
        <div className="px-8 py-6 border-b border-stone-100">
          <h2 className="text-xl font-display font-semibold text-stone-800">
            {feature ? 'Edit Feature' : 'New Feature'}
          </h2>
          <p className="text-stone-500 text-sm mt-1">
            Define a product capability and assign it to a phase
          </p>
        </div>

        <form onSubmit={handleSubmit} className="px-8 py-6 space-y-5">
          {error && (
            <div className="p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm">
              {error}
            </div>
          )}

          {/* Name */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">
              Feature name <span className="text-red-500">*</span>
            </label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="input"
              placeholder="e.g., User authentication, CSV export"
              disabled={saving}
              autoFocus
            />
          </div>

          {/* Description */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">
              Description
            </label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="input resize-none"
              rows={3}
              placeholder="Brief description of what this feature does"
              disabled={saving}
            />
          </div>

          {/* Phase */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">Phase</label>
            <select
              value={phase}
              onChange={(e) => setPhase(e.target.value as FeaturePhase)}
              className="input"
              disabled={saving}
            >
              {PHASES.map(({ value, label }) => (
                <option key={value} value={value}>
                  {label}
                </option>
              ))}
            </select>
          </div>

          {/* Effort */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">Effort</label>
            <select
              value={effort}
              onChange={(e) => setEffort(e.target.value as EffortSize | '')}
              className="input"
              disabled={saving}
            >
              <option value="">Not estimated</option>
              {EFFORTS.map(({ value, label }) => (
                <option key={value} value={value}>
                  {label}
                </option>
              ))}
            </select>
          </div>

          {/* Actions */}
          <div className="flex gap-3 pt-2 border-t border-stone-100">
            <button
              type="submit"
              disabled={saving || !name.trim()}
              className="btn-primary flex-1 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {saving ? (
                <span className="flex items-center justify-center gap-2">
                  <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24">
                    <circle
                      className="opacity-25"
                      cx="12"
                      cy="12"
                      r="10"
                      stroke="currentColor"
                      strokeWidth="4"
                      fill="none"
                    />
                    <path
                      className="opacity-75"
                      fill="currentColor"
                      d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
                    />
                  </svg>
                  Saving...
                </span>
              ) : feature ? (
                'Save Changes'
              ) : (
                'Create Feature'
              )}
            </button>
            <button
              type="button"
              onClick={onCancel}
              disabled={saving}
              className="btn-secondary"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
