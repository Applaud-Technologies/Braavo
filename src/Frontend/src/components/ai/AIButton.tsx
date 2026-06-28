import React from 'react';

interface AIButtonProps {
  onClick: () => void;
  loading?: boolean;
  disabled?: boolean;
  children: React.ReactNode;
  variant?: 'primary' | 'secondary';
}

export default function AIButton({
  onClick,
  loading = false,
  disabled = false,
  children,
  variant = 'primary',
}: AIButtonProps) {
  const isDisabled = disabled || loading;

  const baseClasses =
    'inline-flex items-center gap-2 px-4 py-2 rounded-xl font-medium text-sm transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-amber-400 disabled:cursor-not-allowed disabled:opacity-50';

  const variantClasses =
    variant === 'primary'
      ? 'bg-gradient-to-r from-amber-500 to-amber-600 text-white shadow-warm hover:from-amber-600 hover:to-amber-700 hover:shadow-warm-lg active:scale-[0.98]'
      : 'border border-amber-500 text-amber-700 bg-transparent hover:bg-amber-50 active:scale-[0.98]';

  return (
    <button
      type="button"
      onClick={onClick}
      disabled={isDisabled}
      className={`${baseClasses} ${variantClasses}`}
    >
      {loading ? (
        <svg
          className="w-4 h-4 animate-spin shrink-0"
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          aria-hidden="true"
        >
          <circle
            className="opacity-25"
            cx="12"
            cy="12"
            r="10"
            stroke="currentColor"
            strokeWidth="4"
          />
          <path
            className="opacity-75"
            fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
          />
        </svg>
      ) : (
        <span className="text-base leading-none shrink-0" aria-hidden="true">
          ✨
        </span>
      )}
      {children}
    </button>
  );
}
