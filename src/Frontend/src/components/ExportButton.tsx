import { useState } from 'react';
import { useSelector } from 'react-redux';
import type { RootState } from '../store/store';

interface ExportButtonProps {
  documentId: string;
  includeDiagrams?: boolean;
}

export default function ExportButton({ documentId, includeDiagrams = false }: ExportButtonProps) {
  const [isLoading, setIsLoading] = useState(false);
  const token = useSelector((state: RootState) => state.auth.token);

  const handleExport = async () => {
    setIsLoading(true);
    try {
      const params = new URLSearchParams({
        documentId,
        includeDiagrams: String(includeDiagrams),
      });
      const response = await fetch(`/api/export?${params}`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!response.ok) throw new Error('Export failed');

      const blob = await response.blob();
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'prd-export.zip';
      a.click();
      URL.revokeObjectURL(url);
    } catch (err) {
      console.error('Export failed:', err);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <button
      onClick={handleExport}
      disabled={isLoading}
      className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700 disabled:opacity-50"
    >
      {isLoading ? 'Exporting...' : 'Export ZIP'}
    </button>
  );
}
