import { useState, useEffect } from 'react';
import type { Persona, CreatePersonaRequest } from '../../api/personas';

interface PersonaEditorProps {
  persona?: Persona;
  onSave: (data: CreatePersonaRequest) => Promise<void>;
  onCancel: () => void;
}

export function PersonaEditor({ persona, onSave, onCancel }: PersonaEditorProps) {
  const [name, setName] = useState('');
  const [role, setRole] = useState('');
  const [technicalLevel, setTechnicalLevel] = useState<'Low' | 'Medium' | 'High'>('Medium');
  const [goals, setGoals] = useState<string[]>(['']);
  const [painPoints, setPainPoints] = useState<string[]>(['']);
  const [quote, setQuote] = useState('');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (persona) {
      setName(persona.name);
      setRole(persona.role);
      setTechnicalLevel(persona.technicalLevel);
      setGoals(persona.goals.length > 0 ? persona.goals : ['']);
      setPainPoints(persona.painPoints.length > 0 ? persona.painPoints : ['']);
      setQuote(persona.quote);
    }
  }, [persona]);

  const handleListChange = (
    list: string[],
    setter: (v: string[]) => void,
    index: number,
    value: string
  ) => {
    const updated = [...list];
    updated[index] = value;
    setter(updated);
  };

  const handleListAdd = (list: string[], setter: (v: string[]) => void) => {
    setter([...list, '']);
  };

  const handleListRemove = (list: string[], setter: (v: string[]) => void, index: number) => {
    if (list.length === 1) {
      setter(['']);
    } else {
      setter(list.filter((_, i) => i !== index));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) {
      setError('Name is required');
      return;
    }
    if (!role.trim()) {
      setError('Role is required');
      return;
    }

    setSaving(true);
    setError(null);

    try {
      await onSave({
        name: name.trim(),
        role: role.trim(),
        technicalLevel,
        goals: goals.map((g) => g.trim()).filter(Boolean),
        painPoints: painPoints.map((p) => p.trim()).filter(Boolean),
        quote: quote.trim(),
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save persona');
      setSaving(false);
    }
  };

  return (
    /* Overlay */
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-stone-900/50 backdrop-blur-sm">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <div className="px-8 py-6 border-b border-stone-100">
          <h2 className="text-xl font-display font-semibold text-stone-800">
            {persona ? 'Edit Persona' : 'New Persona'}
          </h2>
          <p className="text-stone-500 text-sm mt-1">Define a user persona for your product</p>
        </div>

        <form onSubmit={handleSubmit} className="px-8 py-6 space-y-6">
          {error && (
            <div className="p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm">
              {error}
            </div>
          )}

          {/* Name */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">Name</label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="input"
              placeholder="e.g., Sarah the Designer"
              disabled={saving}
            />
          </div>

          {/* Role */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">Role</label>
            <input
              type="text"
              value={role}
              onChange={(e) => setRole(e.target.value)}
              className="input"
              placeholder="e.g., UX Designer, Product Manager"
              disabled={saving}
            />
          </div>

          {/* Technical Level */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-3">Technical Level</label>
            <div className="flex gap-4">
              {(['Low', 'Medium', 'High'] as const).map((level) => (
                <label
                  key={level}
                  className={`flex-1 flex items-center justify-center gap-2 py-2.5 px-4 rounded-lg border cursor-pointer transition-all ${
                    technicalLevel === level
                      ? 'bg-amber-50 border-amber-400 text-amber-800 font-medium'
                      : 'border-stone-200 text-stone-600 hover:bg-stone-50'
                  }`}
                >
                  <input
                    type="radio"
                    name="technicalLevel"
                    value={level}
                    checked={technicalLevel === level}
                    onChange={() => setTechnicalLevel(level)}
                    className="sr-only"
                    disabled={saving}
                  />
                  {level}
                </label>
              ))}
            </div>
          </div>

          {/* Goals */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">Goals</label>
            <div className="space-y-2">
              {goals.map((goal, i) => (
                <div key={i} className="flex gap-2">
                  <input
                    type="text"
                    value={goal}
                    onChange={(e) => handleListChange(goals, setGoals, i, e.target.value)}
                    className="input"
                    placeholder={`Goal ${i + 1}`}
                    disabled={saving}
                  />
                  <button
                    type="button"
                    onClick={() => handleListRemove(goals, setGoals, i)}
                    className="shrink-0 w-9 h-10 flex items-center justify-center rounded-lg border border-stone-200 text-stone-400 hover:text-red-500 hover:border-red-200 hover:bg-red-50 transition-colors"
                    disabled={saving}
                    aria-label="Remove goal"
                  >
                    <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
              ))}
              <button
                type="button"
                onClick={() => handleListAdd(goals, setGoals)}
                className="text-sm text-amber-700 hover:text-amber-800 font-medium flex items-center gap-1 mt-1"
                disabled={saving}
              >
                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                </svg>
                Add goal
              </button>
            </div>
          </div>

          {/* Pain Points */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">Pain Points</label>
            <div className="space-y-2">
              {painPoints.map((point, i) => (
                <div key={i} className="flex gap-2">
                  <input
                    type="text"
                    value={point}
                    onChange={(e) => handleListChange(painPoints, setPainPoints, i, e.target.value)}
                    className="input"
                    placeholder={`Pain point ${i + 1}`}
                    disabled={saving}
                  />
                  <button
                    type="button"
                    onClick={() => handleListRemove(painPoints, setPainPoints, i)}
                    className="shrink-0 w-9 h-10 flex items-center justify-center rounded-lg border border-stone-200 text-stone-400 hover:text-red-500 hover:border-red-200 hover:bg-red-50 transition-colors"
                    disabled={saving}
                    aria-label="Remove pain point"
                  >
                    <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
              ))}
              <button
                type="button"
                onClick={() => handleListAdd(painPoints, setPainPoints)}
                className="text-sm text-amber-700 hover:text-amber-800 font-medium flex items-center gap-1 mt-1"
                disabled={saving}
              >
                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                </svg>
                Add pain point
              </button>
            </div>
          </div>

          {/* Quote */}
          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">Quote</label>
            <textarea
              value={quote}
              onChange={(e) => setQuote(e.target.value)}
              rows={2}
              className="input resize-none"
              placeholder="A representative quote from this persona..."
              disabled={saving}
            />
          </div>

          {/* Actions */}
          <div className="flex gap-3 pt-2 border-t border-stone-100">
            <button
              type="submit"
              disabled={saving || !name.trim() || !role.trim()}
              className="btn-primary flex-1 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {saving ? (
                <span className="flex items-center justify-center gap-2">
                  <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                  </svg>
                  Saving...
                </span>
              ) : (
                persona ? 'Save Changes' : 'Create Persona'
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
