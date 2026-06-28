import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import {
  fetchTimeline,
  updateTimeline,
  generateGanttFromTimeline,
  clearTimeline,
} from '../store/slices/timelineSlice';
import { fetchProduct } from '../store/slices/productsSlice';
import type { RootState } from '../store/store';
import type { TimelinePhase, TimelineMilestone } from '../api/timeline';
import MermaidDiagram from '../components/diagrams/MermaidDiagram';
import DiagramExport from '../components/diagrams/DiagramExport';

// ── helpers ──────────────────────────────────────────────────────────────────

function newPhase(): TimelinePhase {
  const today = new Date().toISOString().slice(0, 10);
  return {
    id: crypto.randomUUID(),
    name: '',
    startDate: today,
    endDate: today,
    milestones: [],
  };
}

function newMilestone(): TimelineMilestone {
  return { name: '', date: new Date().toISOString().slice(0, 10) };
}

// ── sub-components ────────────────────────────────────────────────────────────

interface MilestoneRowProps {
  milestone: TimelineMilestone;
  index: number;
  onChange: (index: number, updated: TimelineMilestone) => void;
  onDelete: (index: number) => void;
}

function MilestoneRow({ milestone, index, onChange, onDelete }: MilestoneRowProps) {
  return (
    <div className="flex gap-2 items-center mt-2">
      <input
        type="text"
        value={milestone.name}
        onChange={(e) => onChange(index, { ...milestone, name: e.target.value })}
        placeholder="Milestone name"
        className="input flex-1 text-sm py-1.5"
      />
      <input
        type="date"
        value={milestone.date}
        onChange={(e) => onChange(index, { ...milestone, date: e.target.value })}
        className="input w-40 text-sm py-1.5"
      />
      <button
        type="button"
        onClick={() => onDelete(index)}
        className="text-stone-400 hover:text-red-500 transition-colors p-1"
        aria-label="Delete milestone"
      >
        <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>
  );
}

interface PhaseCardProps {
  phase: TimelinePhase;
  index: number;
  onUpdate: (index: number, updated: TimelinePhase) => void;
  onDelete: (index: number) => void;
  onMoveUp: (index: number) => void;
  onMoveDown: (index: number) => void;
  isFirst: boolean;
  isLast: boolean;
}

function PhaseCard({
  phase,
  index,
  onUpdate,
  onDelete,
  onMoveUp,
  onMoveDown,
  isFirst,
  isLast,
}: PhaseCardProps) {
  const handleMilestoneChange = (mIdx: number, updated: TimelineMilestone) => {
    const milestones = [...phase.milestones];
    milestones[mIdx] = updated;
    onUpdate(index, { ...phase, milestones });
  };

  const handleMilestoneDelete = (mIdx: number) => {
    const milestones = phase.milestones.filter((_, i) => i !== mIdx);
    onUpdate(index, { ...phase, milestones });
  };

  const handleAddMilestone = () => {
    onUpdate(index, { ...phase, milestones: [...phase.milestones, newMilestone()] });
  };

  return (
    <div className="card p-6">
      {/* Phase header */}
      <div className="flex items-start gap-3 mb-4">
        <div className="flex flex-col gap-1">
          <button
            type="button"
            onClick={() => onMoveUp(index)}
            disabled={isFirst}
            className="text-stone-400 hover:text-amber-600 disabled:opacity-30 transition-colors"
            aria-label="Move phase up"
          >
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 15l7-7 7 7" />
            </svg>
          </button>
          <button
            type="button"
            onClick={() => onMoveDown(index)}
            disabled={isLast}
            className="text-stone-400 hover:text-amber-600 disabled:opacity-30 transition-colors"
            aria-label="Move phase down"
          >
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
            </svg>
          </button>
        </div>

        <div className="flex-1 grid grid-cols-1 md:grid-cols-3 gap-3">
          <div className="md:col-span-1">
            <label className="block text-xs font-medium text-stone-500 mb-1">Phase Name</label>
            <input
              type="text"
              value={phase.name}
              onChange={(e) => onUpdate(index, { ...phase, name: e.target.value })}
              placeholder="e.g. Discovery"
              className="input w-full"
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-stone-500 mb-1">Start Date</label>
            <input
              type="date"
              value={phase.startDate}
              onChange={(e) => onUpdate(index, { ...phase, startDate: e.target.value })}
              className="input w-full"
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-stone-500 mb-1">End Date</label>
            <input
              type="date"
              value={phase.endDate}
              onChange={(e) => onUpdate(index, { ...phase, endDate: e.target.value })}
              className="input w-full"
            />
          </div>
        </div>

        <button
          type="button"
          onClick={() => onDelete(index)}
          className="text-stone-400 hover:text-red-500 transition-colors mt-5 p-1"
          aria-label="Delete phase"
        >
          <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
            />
          </svg>
        </button>
      </div>

      {/* Milestones */}
      <div className="border-t border-stone-100 pt-4">
        <h4 className="text-xs font-semibold text-stone-500 uppercase tracking-wider mb-2">
          Milestones
        </h4>
        {phase.milestones.map((m, mIdx) => (
          <MilestoneRow
            key={mIdx}
            milestone={m}
            index={mIdx}
            onChange={handleMilestoneChange}
            onDelete={handleMilestoneDelete}
          />
        ))}
        <button
          type="button"
          onClick={handleAddMilestone}
          className="mt-3 text-sm text-amber-700 hover:text-amber-800 font-medium flex items-center gap-1 transition-colors"
        >
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Add Milestone
        </button>
      </div>
    </div>
  );
}

// ── page ─────────────────────────────────────────────────────────────────────

export function TimelinePage() {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const { phases: savedPhases, loading, saving, error, ganttCode, ganttLoading, ganttError } =
    useAppSelector((state: RootState) => state.timeline);
  const { currentProduct } = useAppSelector((state: RootState) => state.products);

  // Local draft — keeps UI snappy without a round-trip on every keystroke
  const [phases, setPhases] = useState<TimelinePhase[]>([]);
  const [isDirty, setIsDirty] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<number | null>(null);

  useEffect(() => {
    if (id) {
      dispatch(fetchProduct(id));
      dispatch(fetchTimeline(id));
    }
    return () => {
      dispatch(clearTimeline());
    };
  }, [dispatch, id]);

  // Sync server state into local draft when it arrives
  useEffect(() => {
    setPhases(savedPhases);
    setIsDirty(false);
  }, [savedPhases]);

  // ── handlers ────────────────────────────────────────────────────────────────

  const handlePhaseUpdate = (index: number, updated: TimelinePhase) => {
    const next = [...phases];
    next[index] = updated;
    setPhases(next);
    setIsDirty(true);
  };

  const handlePhaseDelete = (index: number) => {
    setDeleteTarget(index);
  };

  const confirmDelete = () => {
    if (deleteTarget === null) return;
    const next = phases.filter((_, i) => i !== deleteTarget);
    setPhases(next);
    setIsDirty(true);
    setDeleteTarget(null);
  };

  const handleMoveUp = (index: number) => {
    if (index === 0) return;
    const next = [...phases];
    [next[index - 1], next[index]] = [next[index], next[index - 1]];
    setPhases(next);
    setIsDirty(true);
  };

  const handleMoveDown = (index: number) => {
    if (index === phases.length - 1) return;
    const next = [...phases];
    [next[index], next[index + 1]] = [next[index + 1], next[index]];
    setPhases(next);
    setIsDirty(true);
  };

  const handleAddPhase = () => {
    setPhases([...phases, newPhase()]);
    setIsDirty(true);
  };

  const handleSave = async () => {
    if (!id) return;
    await dispatch(updateTimeline({ productId: id, phases })).unwrap();
    setIsDirty(false);
  };

  const handleGenerateGantt = async () => {
    if (!id) return;
    // Save first if dirty so the backend sees the latest data
    if (isDirty) {
      await dispatch(updateTimeline({ productId: id, phases })).unwrap();
      setIsDirty(false);
    }
    dispatch(generateGanttFromTimeline(id));
  };

  // ── render ──────────────────────────────────────────────────────────────────

  if (loading && phases.length === 0) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-stone-50">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-amber-600" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-stone-50">
      <div className="max-w-5xl mx-auto px-6 py-10">
        {/* Back link */}
        <button
          onClick={() => navigate(`/products/${id}`)}
          className="mb-8 text-stone-500 hover:text-amber-700 transition-colors flex items-center gap-1 text-sm font-medium"
        >
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
          </svg>
          {currentProduct ? `Back to ${currentProduct.name}` : 'Back to Product'}
        </button>

        {/* Header */}
        <div className="flex items-center justify-between mb-8">
          <div>
            <h1 className="text-3xl font-display font-semibold text-stone-800">
              Timeline &amp; Milestones
            </h1>
            {currentProduct && (
              <p className="text-stone-500 mt-1 text-sm">{currentProduct.name}</p>
            )}
          </div>
          <div className="flex gap-3">
            {isDirty && (
              <button
                onClick={handleSave}
                disabled={saving}
                className="btn-primary flex items-center gap-2"
              >
                {saving ? (
                  <>
                    <svg className="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                    </svg>
                    Saving...
                  </>
                ) : (
                  'Save Changes'
                )}
              </button>
            )}
            <button onClick={handleAddPhase} className="btn-secondary flex items-center gap-2">
              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
              </svg>
              Add Phase
            </button>
          </div>
        </div>

        {/* Error state */}
        {error && (
          <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm">
            <p className="font-medium">Error</p>
            <p className="mt-1">{error}</p>
          </div>
        )}

        {/* Empty state */}
        {!loading && phases.length === 0 && !error && (
          <div className="card p-12 text-center mb-8">
            <div className="text-5xl mb-4">📅</div>
            <h2 className="text-xl font-display font-semibold text-stone-700 mb-2">
              No phases yet
            </h2>
            <p className="text-stone-500 mb-6">
              Add your first project phase to start building your timeline.
            </p>
            <button onClick={handleAddPhase} className="btn-primary">
              + Add Phase
            </button>
          </div>
        )}

        {/* Phase cards */}
        {phases.length > 0 && (
          <div className="space-y-4 mb-8">
            {phases.map((phase, index) => (
              <PhaseCard
                key={phase.id}
                phase={phase}
                index={index}
                onUpdate={handlePhaseUpdate}
                onDelete={handlePhaseDelete}
                onMoveUp={handleMoveUp}
                onMoveDown={handleMoveDown}
                isFirst={index === 0}
                isLast={index === phases.length - 1}
              />
            ))}
          </div>
        )}

        {/* Generate Gantt */}
        <div className="card p-6 mb-8">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h3 className="font-display font-semibold text-lg text-stone-800">Gantt Diagram</h3>
              <p className="text-stone-500 text-sm mt-0.5">
                Generate a Mermaid Gantt chart from your current timeline phases.
              </p>
            </div>
            <button
              onClick={handleGenerateGantt}
              disabled={ganttLoading || phases.length === 0}
              className="btn-primary flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {ganttLoading ? (
                <>
                  <svg className="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                  </svg>
                  Generating...
                </>
              ) : (
                <>
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                  Generate Gantt
                </>
              )}
            </button>
          </div>

          {ganttError && (
            <div className="p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm mb-4">
              <p className="font-medium">Generation failed</p>
              <p className="mt-1">{ganttError}</p>
            </div>
          )}

          {ganttCode && (
            <>
              <MermaidDiagram
                code={ganttCode}
                id="gantt-diagram"
                className="mb-4"
              />
              <div className="flex items-center justify-between">
                <DiagramExport targetId="gantt-diagram" filename="gantt-chart" />
                <details className="text-sm">
                  <summary className="cursor-pointer text-stone-500 hover:text-stone-700 select-none">
                    View Mermaid source
                  </summary>
                  <pre className="mt-2 p-3 bg-stone-100 rounded-lg text-xs font-mono text-stone-700 overflow-auto max-h-48 whitespace-pre-wrap">
                    {ganttCode}
                  </pre>
                </details>
              </div>
            </>
          )}

          {!ganttCode && !ganttLoading && !ganttError && (
            <div className="text-center py-8 text-stone-400 text-sm">
              Click &ldquo;Generate Gantt&rdquo; to create a visual timeline diagram.
            </div>
          )}
        </div>
      </div>

      {/* Delete phase confirmation dialog */}
      {deleteTarget !== null && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-stone-900/50 backdrop-blur-sm">
          <div className="bg-white rounded-2xl shadow-xl w-full max-w-sm p-8">
            <h2 className="text-lg font-display font-semibold text-stone-800 mb-2">Delete Phase</h2>
            <p className="text-stone-600 text-sm mb-6">
              Are you sure you want to delete{' '}
              <strong>{phases[deleteTarget]?.name || 'this phase'}</strong>? All milestones in this phase
              will also be removed.
            </p>
            <div className="flex gap-3">
              <button
                onClick={confirmDelete}
                className="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg font-medium hover:bg-red-700 transition-colors"
              >
                Delete
              </button>
              <button
                onClick={() => setDeleteTarget(null)}
                className="btn-secondary"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
