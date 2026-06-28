import { useEffect, useRef, useState } from 'react';
import mermaid from 'mermaid';

mermaid.initialize({
  startOnLoad: false,
  theme: 'neutral',
  securityLevel: 'strict',
});

export interface MermaidDiagramProps {
  code: string;
  id: string;
  className?: string;
}

export default function MermaidDiagram({ code, id, className = '' }: MermaidDiagramProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!containerRef.current || !code) return;

    setError(null);

    const renderDiagram = async () => {
      try {
        const { svg } = await mermaid.render(`mermaid-${id}`, code);
        if (containerRef.current) {
          containerRef.current.innerHTML = svg;
        }
      } catch (err) {
        console.error('Mermaid render error:', err);
        setError('Error rendering diagram');
      }
    };

    renderDiagram();
  }, [code, id]);

  if (error) {
    return (
      <div className={`rounded-xl border border-red-200 bg-red-50 p-4 ${className}`}>
        <p className="text-sm text-red-600 font-mono">{error}</p>
      </div>
    );
  }

  return (
    <div
      id={id}
      ref={containerRef}
      className={`mermaid-container overflow-auto rounded-xl bg-stone-50 border border-stone-200 p-4 ${className}`}
    />
  );
}
