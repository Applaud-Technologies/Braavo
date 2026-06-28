import { useState } from 'react';
import api from '../services/api';
import MermaidDiagram from './MermaidDiagram';

interface DiagramPanelProps {
  documentId: string;
}

const DIAGRAM_TYPES = [
  { value: 'Flowchart', label: 'Flowchart' },
  { value: 'Sequence', label: 'Sequence Diagram' },
  { value: 'EntityRelationship', label: 'ER Diagram' },
  { value: 'UserJourney', label: 'User Journey' },
];

export default function DiagramPanel({ documentId }: DiagramPanelProps) {
  const [selectedType, setSelectedType] = useState('Flowchart');
  const [diagramCode, setDiagramCode] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleGenerate = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await api.post<{ mermaidCode: string; success: boolean; error?: string }>(
        '/diagrams/generate',
        { documentId, type: selectedType }
      );
      if (response.data.success) {
        setDiagramCode(response.data.mermaidCode);
      } else {
        setError(response.data.error || 'Failed to generate diagram');
      }
    } catch (err) {
      setError('Failed to generate diagram');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="p-4 border rounded-lg bg-white">
      <h3 className="text-lg font-semibold mb-4">Generate Diagram</h3>
      <div className="flex gap-4 mb-4">
        <select
          value={selectedType}
          onChange={(e) => setSelectedType(e.target.value)}
          className="border rounded px-3 py-2"
          disabled={isLoading}
        >
          {DIAGRAM_TYPES.map((t) => (
            <option key={t.value} value={t.value}>{t.label}</option>
          ))}
        </select>
        <button
          onClick={handleGenerate}
          disabled={isLoading}
          className="bg-primary-600 text-white px-4 py-2 rounded hover:bg-primary-700 disabled:opacity-50"
        >
          {isLoading ? 'Generating...' : 'Generate'}
        </button>
      </div>
      {error && <p className="text-red-600 mb-4">{error}</p>}
      {diagramCode && <MermaidDiagram code={diagramCode} />}
    </div>
  );
}
