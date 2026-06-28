import { useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import WireframePreview from '../components/WireframePreview';
import api from '../services/api';

export default function WireframePage() {
  const [searchParams] = useSearchParams();
  const documentId = searchParams.get('documentId');

  const [screenName, setScreenName] = useState('');
  const [fidelity, setFidelity] = useState<'low' | 'high'>('low');
  const [wireframe, setWireframe] = useState<{ html: string; screenName: string } | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleGenerate = async () => {
    if (!documentId) {
      setError('No document selected');
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const response = await api.post<{
        html: string;
        screenName: string;
        success: boolean;
        error?: string;
      }>('/wireframes/generate', {
        documentId,
        screenName: screenName || null,
        fidelity,
      });

      if (response.data.success) {
        setWireframe({
          html: response.data.html,
          screenName: response.data.screenName,
        });
      } else {
        setError(response.data.error || 'Failed to generate wireframe');
      }
    } catch (err) {
      setError('Failed to generate wireframe');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-primary-600 text-white p-4 shadow">
        <h1 className="text-xl font-bold">Wireframe Generator</h1>
      </header>

      <main className="container mx-auto p-6">
        <div className="bg-white rounded-lg shadow p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">Generate Wireframe</h2>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Screen Name (optional)
              </label>
              <input
                type="text"
                value={screenName}
                onChange={(e) => setScreenName(e.target.value)}
                placeholder="e.g., Login Screen, Dashboard"
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Fidelity
              </label>
              <select
                value={fidelity}
                onChange={(e) => setFidelity(e.target.value as 'low' | 'high')}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
                <option value="low">Low Fidelity (Sketch)</option>
                <option value="high">High Fidelity (Polished)</option>
              </select>
            </div>

            <div className="flex items-end">
              <button
                onClick={handleGenerate}
                disabled={isLoading || !documentId}
                className="w-full px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700 disabled:opacity-50"
              >
                {isLoading ? 'Generating...' : 'Generate Wireframe'}
              </button>
            </div>
          </div>

          {error && (
            <div className="text-red-600 text-sm">{error}</div>
          )}

          {!documentId && (
            <div className="text-amber-600 text-sm">
              No document selected. Go to Chat to create a PRD first.
            </div>
          )}
        </div>

        {wireframe && (
          <WireframePreview html={wireframe.html} screenName={wireframe.screenName} />
        )}
      </main>
    </div>
  );
}
