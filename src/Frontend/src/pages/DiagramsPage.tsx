import { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import {
  generateUserJourney,
  generateFeatureHierarchy,
  generateComponentDiagram,
  clearDiagrams,
} from '../store/slices/diagramsSlice';
import { fetchPersonas } from '../store/slices/personasSlice';
import { fetchProduct } from '../store/slices/productsSlice';
import type { RootState } from '../store/store';
import MermaidDiagram from '../components/diagrams/MermaidDiagram';
import DiagramExport from '../components/diagrams/DiagramExport';

// ── sub-components ────────────────────────────────────────────────────────────

interface DiagramSectionProps {
  code: string | null;
  loading: boolean;
  error: string | null;
  diagramId: string;
  filename: string;
  placeholder: string;
}

function DiagramSection({ code, loading, error, diagramId, filename, placeholder }: DiagramSectionProps) {
  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <svg className="w-6 h-6 animate-spin text-amber-600" fill="none" viewBox="0 0 24 24">
          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
        </svg>
        <span className="ml-3 text-stone-500 text-sm">Generating diagram...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm">
        <p className="font-medium">Generation failed</p>
        <p className="mt-1">{error}</p>
      </div>
    );
  }

  if (code) {
    return (
      <>
        <MermaidDiagram code={code} id={diagramId} className="mb-4" />
        <div className="flex items-center justify-between">
          <DiagramExport targetId={diagramId} filename={filename} />
          <details className="text-sm">
            <summary className="cursor-pointer text-stone-500 hover:text-stone-700 select-none">
              View Mermaid source
            </summary>
            <pre className="mt-2 p-3 bg-stone-100 rounded-lg text-xs font-mono text-stone-700 overflow-auto max-h-48 whitespace-pre-wrap">
              {code}
            </pre>
          </details>
        </div>
      </>
    );
  }

  return (
    <div className="text-center py-8 text-stone-400 text-sm">{placeholder}</div>
  );
}

// ── page ─────────────────────────────────────────────────────────────────────

export function DiagramsPage() {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const {
    userJourneyCode,
    userJourneyLoading,
    userJourneyError,
    featureHierarchyCode,
    featureHierarchyLoading,
    featureHierarchyError,
    componentCode,
    componentLoading,
    componentError,
  } = useAppSelector((state: RootState) => state.diagrams);

  const { items: personas } = useAppSelector((state: RootState) => state.personas);
  const { currentProduct } = useAppSelector((state: RootState) => state.products);

  const [selectedPersonaId, setSelectedPersonaId] = useState<string>('');

  useEffect(() => {
    if (id) {
      dispatch(fetchProduct(id));
      dispatch(fetchPersonas(id));
    }
    return () => {
      dispatch(clearDiagrams());
    };
  }, [dispatch, id]);

  // Pre-select first persona when personas load
  useEffect(() => {
    if (personas.length > 0 && !selectedPersonaId) {
      setSelectedPersonaId(personas[0].id);
    }
  }, [personas, selectedPersonaId]);

  // ── handlers ────────────────────────────────────────────────────────────────

  const handleGenerateUserJourney = () => {
    if (!id) return;
    dispatch(generateUserJourney({ productId: id, personaId: selectedPersonaId || undefined }));
  };

  const handleGenerateFeatureHierarchy = () => {
    if (!id) return;
    dispatch(generateFeatureHierarchy(id));
  };

  const handleGenerateComponent = () => {
    if (!id) return;
    dispatch(generateComponentDiagram(id));
  };

  // ── render ──────────────────────────────────────────────────────────────────

  return (
    <div className="min-h-screen bg-stone-50">
      <div className="max-w-5xl mx-auto px-6 py-10">
        {/* Back link */}
        <button
          onClick={() => navigate(`/products/${id}`)}
          className="mb-8 text-stone-500 hover:text-amber-700 transition-colors flex items-center gap-1 text-sm font-medium"
        >
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
          </svg>
          {currentProduct ? `Back to ${currentProduct.name}` : 'Back to Product'}
        </button>

        {/* Header */}
        <div className="mb-10">
          <h1 className="text-3xl font-display font-semibold text-stone-800">Diagrams</h1>
          {currentProduct && (
            <p className="text-stone-500 mt-1 text-sm">{currentProduct.name}</p>
          )}
        </div>

        <div className="space-y-6">
          {/* User Journey card */}
          <div className="card p-6">
            <div className="flex items-start justify-between mb-4">
              <div>
                <h3 className="font-display font-semibold text-lg text-stone-800">User Journey</h3>
                <p className="text-stone-500 text-sm mt-0.5">
                  Generate a Mermaid journey diagram showing how a persona moves through the product.
                </p>
              </div>
            </div>

            {/* Persona selector */}
            <div className="flex items-end gap-3 mb-4">
              <div className="flex-1 max-w-xs">
                <label className="block text-xs font-medium text-stone-500 mb-1">
                  Persona (optional)
                </label>
                {personas.length > 0 ? (
                  <select
                    value={selectedPersonaId}
                    onChange={(e) => setSelectedPersonaId(e.target.value)}
                    className="input w-full"
                  >
                    <option value="">All personas</option>
                    {personas.map((p) => (
                      <option key={p.id} value={p.id}>
                        {p.name} — {p.role}
                      </option>
                    ))}
                  </select>
                ) : (
                  <p className="text-sm text-stone-400 italic py-2">
                    No personas yet.{' '}
                    <Link
                      to={`/products/${id}/personas`}
                      className="text-amber-700 hover:underline"
                    >
                      Add personas
                    </Link>{' '}
                    to personalise the journey.
                  </p>
                )}
              </div>
              <button
                onClick={handleGenerateUserJourney}
                disabled={userJourneyLoading}
                className="btn-primary flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {userJourneyLoading ? (
                  <>
                    <svg className="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                    </svg>
                    Generating...
                  </>
                ) : (
                  <>
                    <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 17V7m0 10a2 2 0 01-2 2H5a2 2 0 01-2-2V7a2 2 0 012-2h2a2 2 0 012 2m0 10a2 2 0 002 2h2a2 2 0 002-2M9 7a2 2 0 012-2h2a2 2 0 012 2m0 10V7m0 10a2 2 0 002 2h2a2 2 0 002-2V7a2 2 0 00-2-2h-2a2 2 0 00-2 2" />
                    </svg>
                    Generate Journey
                  </>
                )}
              </button>
            </div>

            <DiagramSection
              code={userJourneyCode}
              loading={userJourneyLoading}
              error={userJourneyError}
              diagramId="user-journey-diagram"
              filename="user-journey"
              placeholder='Click "Generate Journey" to create a user journey diagram.'
            />
          </div>

          {/* Feature Hierarchy card */}
          <div className="card p-6">
            <div className="flex items-center justify-between mb-4">
              <div>
                <h3 className="font-display font-semibold text-lg text-stone-800">Feature Hierarchy</h3>
                <p className="text-stone-500 text-sm mt-0.5">
                  Visualise how features are organised as a Mermaid mind-map or tree.
                </p>
              </div>
              <button
                onClick={handleGenerateFeatureHierarchy}
                disabled={featureHierarchyLoading}
                className="btn-primary flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {featureHierarchyLoading ? (
                  <>
                    <svg className="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                    </svg>
                    Generating...
                  </>
                ) : (
                  <>
                    <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h8m-8 6h16" />
                    </svg>
                    Generate Hierarchy
                  </>
                )}
              </button>
            </div>

            <DiagramSection
              code={featureHierarchyCode}
              loading={featureHierarchyLoading}
              error={featureHierarchyError}
              diagramId="feature-hierarchy-diagram"
              filename="feature-hierarchy"
              placeholder='Click "Generate Hierarchy" to create a feature hierarchy diagram.'
            />
          </div>

          {/* Component Architecture card */}
          <div className="card p-6">
            <div className="flex items-center justify-between mb-4">
              <div>
                <h3 className="font-display font-semibold text-lg text-stone-800">Component Architecture</h3>
                <p className="text-stone-500 text-sm mt-0.5">
                  Visualise the system's components, services, and their dependencies.
                </p>
              </div>
              <button
                onClick={handleGenerateComponent}
                disabled={componentLoading || !id}
                className="btn-primary flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {componentLoading ? (
                  <>
                    <svg className="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                    </svg>
                    Generating...
                  </>
                ) : (
                  <>
                    <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                    </svg>
                    Generate Architecture
                  </>
                )}
              </button>
            </div>

            <DiagramSection
              code={componentCode}
              loading={componentLoading}
              error={componentError}
              diagramId="component-diagram"
              filename="component-architecture"
              placeholder='Click "Generate Architecture" to create a component architecture diagram.'
            />
          </div>

          {/* Timeline Gantt card — link to timeline page */}
          <div className="card p-6">
            <div className="flex items-center justify-between">
              <div>
                <h3 className="font-display font-semibold text-lg text-stone-800">Timeline Gantt</h3>
                <p className="text-stone-500 text-sm mt-0.5">
                  Gantt charts are generated from your project timeline phases and milestones.
                </p>
              </div>
              <Link
                to={`/products/${id}/timeline`}
                className="btn-secondary flex items-center gap-2"
              >
                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
                Go to Timeline
              </Link>
            </div>
            <p className="mt-4 text-xs text-stone-400">
              Open the Timeline page to define phases and milestones, then generate a Gantt diagram from there.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
