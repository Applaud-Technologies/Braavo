import type { Persona } from '../../api/personas';

interface PersonaCardProps {
  persona: Persona;
  onEdit: (persona: Persona) => void;
  onDelete: (persona: Persona) => void;
}

const technicalLevelConfig = {
  Low: { label: 'Low', width: 'w-1/3', color: 'bg-emerald-400' },
  Medium: { label: 'Medium', width: 'w-2/3', color: 'bg-amber-400' },
  High: { label: 'High', width: 'w-full', color: 'bg-rose-400' },
};

export function PersonaCard({ persona, onEdit, onDelete }: PersonaCardProps) {
  const techLevel = technicalLevelConfig[persona.technicalLevel];
  const displayedGoals = persona.goals.slice(0, 3);
  const displayedPainPoints = persona.painPoints.slice(0, 3);

  return (
    <div className="card p-6 flex flex-col gap-4">
      {/* Header */}
      <div className="flex items-start justify-between gap-2">
        <div>
          <h3 className="text-lg font-display font-semibold text-stone-800">{persona.name}</h3>
          <p className="text-sm text-stone-500 mt-0.5">{persona.role}</p>
        </div>
        <div className="flex gap-2 shrink-0">
          <button
            onClick={() => onEdit(persona)}
            className="text-xs px-3 py-1.5 rounded-lg bg-stone-100 text-stone-600 hover:bg-amber-50 hover:text-amber-700 border border-stone-200 hover:border-amber-300 transition-colors font-medium"
          >
            Edit
          </button>
          <button
            onClick={() => onDelete(persona)}
            className="text-xs px-3 py-1.5 rounded-lg bg-stone-100 text-stone-600 hover:bg-red-50 hover:text-red-700 border border-stone-200 hover:border-red-300 transition-colors font-medium"
          >
            Delete
          </button>
        </div>
      </div>

      {/* Technical level */}
      <div>
        <div className="flex justify-between items-center text-xs mb-1.5">
          <span className="text-stone-400 font-medium uppercase tracking-wide">Technical Level</span>
          <span className="text-stone-600 font-semibold">{techLevel.label}</span>
        </div>
        <div className="w-full bg-stone-100 rounded-full h-1.5 overflow-hidden">
          <div className={`${techLevel.width} ${techLevel.color} h-full rounded-full transition-all duration-300`} />
        </div>
      </div>

      {/* Goals */}
      {displayedGoals.length > 0 && (
        <div>
          <p className="text-xs text-stone-400 font-medium uppercase tracking-wide mb-2">Goals</p>
          <ul className="space-y-1">
            {displayedGoals.map((goal, i) => (
              <li key={i} className="flex items-start gap-2 text-sm text-stone-600">
                <span className="mt-1.5 w-1.5 h-1.5 rounded-full bg-amber-400 shrink-0" />
                <span>{goal}</span>
              </li>
            ))}
            {persona.goals.length > 3 && (
              <li className="text-xs text-stone-400 pl-3.5">+{persona.goals.length - 3} more</li>
            )}
          </ul>
        </div>
      )}

      {/* Pain points */}
      {displayedPainPoints.length > 0 && (
        <div>
          <p className="text-xs text-stone-400 font-medium uppercase tracking-wide mb-2">Pain Points</p>
          <ul className="space-y-1">
            {displayedPainPoints.map((point, i) => (
              <li key={i} className="flex items-start gap-2 text-sm text-stone-600">
                <span className="mt-1.5 w-1.5 h-1.5 rounded-full bg-rose-400 shrink-0" />
                <span>{point}</span>
              </li>
            ))}
            {persona.painPoints.length > 3 && (
              <li className="text-xs text-stone-400 pl-3.5">+{persona.painPoints.length - 3} more</li>
            )}
          </ul>
        </div>
      )}

      {/* Quote */}
      {persona.quote && (
        <blockquote className="mt-auto border-l-2 border-amber-300 pl-3 text-sm italic text-stone-500">
          "{persona.quote}"
        </blockquote>
      )}
    </div>
  );
}
