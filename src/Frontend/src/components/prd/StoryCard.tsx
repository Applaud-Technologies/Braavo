import type { UserStory, StoryPriority } from '../../api/stories';
import type { Persona } from '../../api/personas';

interface StoryCardProps {
  story: UserStory;
  persona?: Persona;
  onEdit: (story: UserStory) => void;
}

const priorityConfig: Record<StoryPriority, { label: string; badgeClass: string }> = {
  Must: { label: 'Must Have', badgeClass: 'bg-emerald-100 text-emerald-800 border-emerald-200' },
  Should: { label: 'Should Have', badgeClass: 'bg-amber-100 text-amber-800 border-amber-200' },
  Could: { label: 'Could Have', badgeClass: 'bg-sky-100 text-sky-800 border-sky-200' },
  Wont: { label: "Won't Have", badgeClass: 'bg-stone-100 text-stone-600 border-stone-200' },
};

export function StoryCard({ story, persona, onEdit }: StoryCardProps) {
  const priority = priorityConfig[story.priority];

  return (
    <div className="card p-6 flex flex-col gap-4">
      {/* Header row: priority badge + edit button */}
      <div className="flex items-start justify-between gap-2">
        <span
          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${priority.badgeClass}`}
        >
          {priority.label}
        </span>
        <button
          onClick={() => onEdit(story)}
          className="text-xs px-3 py-1.5 rounded-lg bg-stone-100 text-stone-600 hover:bg-amber-50 hover:text-amber-700 border border-stone-200 hover:border-amber-300 transition-colors font-medium shrink-0"
        >
          Edit
        </button>
      </div>

      {/* Story statement */}
      <div className="space-y-1">
        <p className="text-sm text-stone-700">
          <span className="font-medium text-stone-500">As a</span>{' '}
          <span className="font-semibold text-stone-800">{story.asA || '…'}</span>
        </p>
        <p className="text-sm text-stone-700">
          <span className="font-medium text-stone-500">I want</span>{' '}
          <span className="text-stone-800">{story.iWant || '…'}</span>
        </p>
        <p className="text-sm text-stone-700">
          <span className="font-medium text-stone-500">So that</span>{' '}
          <span className="text-stone-800">{story.soThat || '…'}</span>
        </p>
      </div>

      {/* Linked persona */}
      {persona && (
        <div className="flex items-center gap-2 text-xs text-stone-500">
          {/* Avatar icon */}
          <span className="inline-flex items-center justify-center w-5 h-5 rounded-full bg-amber-100 text-amber-700 font-bold shrink-0">
            {persona.name.charAt(0).toUpperCase()}
          </span>
          <span>{persona.name}</span>
          <span className="text-stone-400">·</span>
          <span className="text-stone-400">{persona.role}</span>
        </div>
      )}

      {/* Acceptance criteria */}
      {story.acceptanceCriteria.length > 0 && (
        <div>
          <p className="text-xs text-stone-400 font-medium uppercase tracking-wide mb-2">
            Acceptance Criteria
          </p>
          <ul className="space-y-1">
            {story.acceptanceCriteria.map((criterion, i) => (
              <li key={i} className="flex items-start gap-2 text-sm text-stone-600">
                <svg
                  className="w-4 h-4 mt-0.5 text-stone-400 shrink-0"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <rect x="3" y="3" width="18" height="18" rx="3" strokeWidth={2} />
                </svg>
                <span>{criterion}</span>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
