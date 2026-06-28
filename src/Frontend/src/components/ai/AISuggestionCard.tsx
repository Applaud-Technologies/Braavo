import React from 'react';

interface AISuggestionCardProps {
  title: string;
  children: React.ReactNode;
  onAccept: () => void;
  onReject: () => void;
  acceptLabel?: string;
  rejectLabel?: string;
}

export default function AISuggestionCard({
  title,
  children,
  onAccept,
  onReject,
  acceptLabel = 'Accept',
  rejectLabel = 'Dismiss',
}: AISuggestionCardProps) {
  return (
    <div className="relative rounded-xl p-px bg-gradient-to-br from-amber-400 via-amber-200 to-amber-400 shadow-warm-lg">
      {/* Inner card */}
      <div className="rounded-[11px] bg-white p-4">
        {/* Header row */}
        <div className="flex items-start justify-between gap-3 mb-3">
          <h3 className="font-display font-semibold text-stone-800 text-base leading-snug">
            {title}
          </h3>
          {/* AI Generated badge */}
          <span className="shrink-0 inline-flex items-center gap-1 px-2 py-0.5 rounded-full bg-amber-100 text-amber-700 text-xs font-medium border border-amber-200">
            <span aria-hidden="true">✨</span>
            AI Generated
          </span>
        </div>

        {/* Content */}
        <div className="text-stone-700 text-sm leading-relaxed mb-4">
          {children}
        </div>

        {/* Action row */}
        <div className="flex items-center gap-2 justify-end">
          <button
            type="button"
            onClick={onReject}
            className="px-3 py-1.5 rounded-lg text-sm font-medium text-stone-500 bg-stone-100 hover:bg-stone-200 transition-colors duration-150 focus:outline-none focus:ring-2 focus:ring-stone-300"
          >
            {rejectLabel}
          </button>
          <button
            type="button"
            onClick={onAccept}
            className="px-3 py-1.5 rounded-lg text-sm font-medium text-white bg-emerald-600 hover:bg-emerald-700 transition-colors duration-150 focus:outline-none focus:ring-2 focus:ring-emerald-400 focus:ring-offset-1"
          >
            {acceptLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
