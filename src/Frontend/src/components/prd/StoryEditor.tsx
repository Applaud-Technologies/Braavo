import { useState, useEffect } from 'react';
import type { UserStory, CreateUserStoryRequest, StoryPriority } from '../../api/stories';
import type { Persona } from '../../api/personas';

interface StoryEditorProps {
  story?: UserStory;
  personas: Persona[];
  onSave: (data: CreateUserStoryRequest) => Promise<void>;
  onCancel: () => void;
}

const PRIORITIES: { value: StoryPriority; label: string; description: string }[] = [
  { value: 'Must', label: 'Must Have', description: 'Critical for launch' },
  { value: 'Should', label: 'Should Have', description: 'Important but not vital' },
  { value: 'Could', label: 'Could Have', description: 'Nice to have' },
  { value: 'Wont', label: "Won't Have", description: 'Out of scope' },
];

export function StoryEditor({ story, personas, onSave, onCancel }: StoryEditorProps) {
  const [personaId, setPersonaId] = useState<string>('');
  const [asA, setAsA] = useState('');
  const [iWant, setIWant] = useState('');
  const [soThat, setSoThat] = useState('');
  const [priority, setPriority] = useState<StoryPriority>('Should');
  const [criteria, setCriteria] = useState<string[]>(['']);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (story) {
      setPersonaId(story.personaId ?? '');
      setAsA(story.asA);
      setIWant(story.iWant);
      setSoThat(story.soThat);
      setPriority(story.priority);
      setCriteria(story.acceptanceCriteria.length > 0 ? story.acceptanceCriteria : ['']);
    }
  }, [story]);

  const handleCriterionChange = (index: number, value: string) => {
    const updated = [...criteria];
    updated[index] = value;
    setCriteria(updated);
  };

  const handleCriterionAdd = () => {
    setCriteria([...criteria, '']);
  };

  const handleCriterionRemove = (index: number) => {
    if (criteria.length === 1) {
      setCriteria(['']);
    } else {
      setCriteria(criteria.filter((_, i) => i !== index));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!asA.trim()) {
      setError('"As a" is required');
      return;
    }
    if (!iWant.trim()) {
      setError('"I want" is required');
      return;
    }

    setSaving(true);
    setError(null);

    try {
      await onSave({
        personaId: personaId || null,
        asA: asA.trim(),
        iWant: iWant.trim(),
        soThat: soThat.trim(),
        priority,
        acceptanceCriteria: criteria.map((c) => c.trim()).filter(Boolean),
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save story');
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-stone-900/50 backdrop-blur-sm">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <div className="px-8 py-6 border-b border-stone-100">
          <h2 className="text-xl font-display font-semibold text-stone-800">
            {story ? 'Edit User Story' : 'New User Story'}
          </h2>
          <p className="text-stone-500 text-sm mt-1">
            Describe a feature from the user's perspective
          </p>
        </div>

        <form onSubmit={handleSubmit} className="px-8 py-6 space-y-6">
          {error && (
            <div className="p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm">
              {error}
            </div>
          )}

          {/* Persona selector */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">
              Linked Persona
            </label>
            <select
              value={personaId}
              onChange={(e) => setPersonaId(e.target.value)}
              className="input"
              disabled={saving}
            >
              <option value="">No persona</option>
              {personas.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name} — {p.role}
                </option>
              ))}
            </select>
          </div>

          {/* As a */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">
              As a…
            </label>
            <input
              type="text"
              value={asA}
              onChange={(e) => setAsA(e.target.value)}
              className="input"
              placeholder="e.g., product manager, new user"
              disabled={saving}
            />
          </div>

          {/* I want */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">
              I want…
            </label>
            <input
              type="text"
              value={iWant}
              onChange={(e) => setIWant(e.target.value)}
              className="input"
              placeholder="e.g., to export my data as CSV"
              disabled={saving}
            />
          </div>

          {/* So that */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">
              So that…
            </label>
            <input
              type="text"
              value={soThat}
              onChange={(e) => setSoThat(e.target.value)}
              className="input"
              placeholder="e.g., I can analyse it in my own tools"
              disabled={saving}
            />
          </div>

          {/* Priority */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-3">Priority</label>
            <div className="grid grid-cols-2 gap-2 sm:grid-cols-4">
              {PRIORITIES.map(({ value, label, description }) => (
                <label
                  key={value}
                  className={`flex flex-col items-center justify-center gap-1 py-3 px-2 rounded-lg border cursor-pointer transition-all text-center ${
                    priority === value
                      ? 'bg-amber-50 border-amber-400 text-amber-800 font-medium'
                      : 'border-stone-200 text-stone-600 hover:bg-stone-50'
                  }`}
                >
                  <input
                    type="radio"
                    name="priority"
                    value={value}
                    checked={priority === value}
                    onChange={() => setPriority(value)}
                    className="sr-only"
                    disabled={saving}
                  />
                  <span className="text-sm font-semibold">{label}</span>
                  <span className="text-xs text-stone-400">{description}</span>
                </label>
              ))}
            </div>
          </div>

          {/* Acceptance criteria */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">
              Acceptance Criteria
            </label>
            <div className="space-y-2">
              {criteria.map((criterion, i) => (
                <div key={i} className="flex gap-2">
                  <input
                    type="text"
                    value={criterion}
                    onChange={(e) => handleCriterionChange(i, e.target.value)}
                    className="input"
                    placeholder={`Criterion ${i + 1}`}
                    disabled={saving}
                  />
                  <button
                    type="button"
                    onClick={() => handleCriterionRemove(i)}
                    className="shrink-0 w-9 h-10 flex items-center justify-center rounded-lg border border-stone-200 text-stone-400 hover:text-red-500 hover:border-red-200 hover:bg-red-50 transition-colors"
                    disabled={saving}
                    aria-label="Remove criterion"
                  >
                    <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M6 18L18 6M6 6l12 12"
                      />
                    </svg>
                  </button>
                </div>
              ))}
              <button
                type="button"
                onClick={handleCriterionAdd}
                className="text-sm text-amber-700 hover:text-amber-800 font-medium flex items-center gap-1 mt-1"
                disabled={saving}
              >
                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M12 4v16m8-8H4"
                  />
                </svg>
                Add criterion
              </button>
            </div>
          </div>

          {/* Actions */}
          <div className="flex gap-3 pt-2 border-t border-stone-100">
            <button
              type="submit"
              disabled={saving || !asA.trim() || !iWant.trim()}
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
              ) : story ? (
                'Save Changes'
              ) : (
                'Create Story'
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
