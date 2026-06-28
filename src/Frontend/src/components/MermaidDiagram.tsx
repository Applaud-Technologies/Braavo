import { useEffect, useRef } from 'react';
import mermaid from 'mermaid';

let idCounter = 0;

mermaid.initialize({
  startOnLoad: false,
  theme: 'neutral',
  securityLevel: 'strict',
});

interface MermaidDiagramProps {
  code: string;
  className?: string;
}

export default function MermaidDiagram({ code, className = '' }: MermaidDiagramProps) {
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (containerRef.current && code) {
      const renderDiagram = async () => {
        try {
          const id = `mermaid-${++idCounter}`;
          const { svg } = await mermaid.render(id, code);
          if (containerRef.current) {
            containerRef.current.innerHTML = svg;
          }
        } catch (error) {
          console.error('Mermaid render error:', error);
          if (containerRef.current) {
            containerRef.current.innerHTML = `<pre class="text-red-500">Error rendering diagram</pre>`;
          }
        }
      };
      renderDiagram();
    }
  }, [code]);

  return (
    <div
      ref={containerRef}
      className={`mermaid-container overflow-auto ${className}`}
    />
  );
}
