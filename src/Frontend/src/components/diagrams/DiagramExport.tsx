import { useState } from 'react';
import { toPng, toSvg } from 'html-to-image';

export interface DiagramExportProps {
  targetId: string;
  filename: string;
}

type ExportFormat = 'png' | 'svg';

export default function DiagramExport({ targetId, filename }: DiagramExportProps) {
  const [loading, setLoading] = useState<ExportFormat | null>(null);

  const getElement = (): HTMLElement | null => {
    return document.getElementById(targetId);
  };

  const handleExport = async (format: ExportFormat) => {
    const element = getElement();
    if (!element) {
      console.error(`Element with id "${targetId}" not found`);
      return;
    }

    setLoading(format);
    try {
      let dataUrl: string;
      let ext: string;

      if (format === 'png') {
        dataUrl = await toPng(element, { backgroundColor: '#fafaf9' });
        ext = 'png';
      } else {
        dataUrl = await toSvg(element, { backgroundColor: '#fafaf9' });
        ext = 'svg';
      }

      const link = document.createElement('a');
      link.download = `${filename}.${ext}`;
      link.href = dataUrl;
      link.click();
    } catch (err) {
      console.error(`Export as ${format} failed:`, err);
    } finally {
      setLoading(null);
    }
  };

  const baseClasses =
    'inline-flex items-center gap-1.5 px-3 py-1.5 rounded-xl text-sm font-medium transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-1 focus:ring-amber-400 disabled:cursor-not-allowed disabled:opacity-50';

  return (
    <div className="inline-flex items-center gap-2">
      <button
        type="button"
        onClick={() => handleExport('png')}
        disabled={loading !== null}
        className={`${baseClasses} bg-gradient-to-r from-amber-500 to-amber-600 text-white shadow-warm hover:from-amber-600 hover:to-amber-700 active:scale-[0.98]`}
      >
        {loading === 'png' ? (
          <svg
            className="w-3.5 h-3.5 animate-spin"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
          </svg>
        ) : (
          <svg
            className="w-3.5 h-3.5"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            strokeWidth={2}
            aria-hidden="true"
          >
            <path strokeLinecap="round" strokeLinejoin="round" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
          </svg>
        )}
        Export PNG
      </button>

      <button
        type="button"
        onClick={() => handleExport('svg')}
        disabled={loading !== null}
        className={`${baseClasses} border border-amber-500 text-amber-700 bg-transparent hover:bg-amber-50 active:scale-[0.98]`}
      >
        {loading === 'svg' ? (
          <svg
            className="w-3.5 h-3.5 animate-spin"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
          </svg>
        ) : (
          <svg
            className="w-3.5 h-3.5"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            strokeWidth={2}
            aria-hidden="true"
          >
            <path strokeLinecap="round" strokeLinejoin="round" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
          </svg>
        )}
        Export SVG
      </button>
    </div>
  );
}
