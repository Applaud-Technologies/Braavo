import { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchProduct } from '../store/slices/productsSlice';
import { fetchPersonas } from '../store/slices/personasSlice';
import { fetchStories } from '../store/slices/storiesSlice';
import { fetchFeatures } from '../store/slices/featuresSlice';
import { fetchTimeline } from '../store/slices/timelineSlice';
import { fetchValidations, downloadMarkdown, downloadPdf } from '../store/slices/exportSlice';
import type { RootState } from '../store/store';

export function PreviewPage() {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const { currentProduct, loading: productLoading } = useAppSelector(
    (state: RootState) => state.products
  );
  const { items: personas } = useAppSelector((state: RootState) => state.personas);
  const { items: stories } = useAppSelector((state: RootState) => state.stories);
  const { items: features } = useAppSelector((state: RootState) => state.features);
  const { phases } = useAppSelector((state: RootState) => state.timeline);
  const {
    validations,
    loading: exportLoading,
    error: exportError,
  } = useAppSelector((state: RootState) => state.export);

  useEffect(() => {
    if (id) {
      dispatch(fetchProduct(id));
      dispatch(fetchPersonas(id));
      dispatch(fetchStories(id));
      dispatch(fetchFeatures(id));
      dispatch(fetchTimeline(id));
      dispatch(fetchValidations(id));
    }
  }, [dispatch, id]);

  const warnings = validations.filter((v) => !v.isValid);
  const completionPct =
    validations.length > 0
      ? Math.round(
          (validations.filter((v) => v.isValid).length / validations.length) * 100
        )
      : currentProduct?.completionPercentage ?? 0;

  const handleDownloadMarkdown = () => {
    if (!id || !currentProduct) return;
    dispatch(downloadMarkdown({ productId: id, productName: currentProduct.name }));
  };

  const handleDownloadPdf = () => {
    if (!id || !currentProduct) return;
    dispatch(downloadPdf({ productId: id, productName: currentProduct.name }));
  };

  const mvpFeatures = features.filter((f) => f.phase === 'Mvp');
  const enhancedFeatures = features.filter((f) => f.phase === 'Enhanced');
  const futureFeatures = features.filter((f) => f.phase === 'Future');

  if (productLoading && !currentProduct) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-stone-50">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-amber-600" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-stone-50 print:bg-white">
      {/* Top bar — hidden on print */}
      <div className="print:hidden sticky top-0 z-10 bg-white border-b border-stone-200 shadow-sm">
        <div className="max-w-4xl mx-auto px-6 py-3 flex items-center justify-between">
          <button
            onClick={() => navigate(`/products/${id}`)}
            className="text-stone-500 hover:text-amber-700 transition-colors flex items-center gap-1 text-sm font-medium"
          >
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
            Back
          </button>

          <h2 className="text-sm font-semibold text-stone-700 truncate mx-4">
            {currentProduct ? `${currentProduct.name} PRD` : 'PRD Preview'}
          </h2>

          <div className="flex items-center gap-2">
            <button
              onClick={handleDownloadPdf}
              disabled={exportLoading}
              className="flex items-center gap-1.5 px-3 py-1.5 bg-stone-800 text-white text-sm rounded-lg font-medium hover:bg-stone-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 10v6m0 0l-3-3m3 3l3-3M3 17V7a2 2 0 012-2h6l2 2h6a2 2 0 012 2v8a2 2 0 01-2 2H5a2 2 0 01-2-2z" />
              </svg>
              PDF
            </button>
            <button
              onClick={handleDownloadMarkdown}
              disabled={exportLoading}
              className="flex items-center gap-1.5 px-3 py-1.5 border border-stone-300 text-stone-700 text-sm rounded-lg font-medium hover:bg-stone-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              Markdown
            </button>
          </div>
        </div>
      </div>

      <div className="max-w-4xl mx-auto px-6 py-8 print:px-0 print:py-0">
        {/* Export error */}
        {exportError && (
          <div className="print:hidden mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
            {exportError}
          </div>
        )}

        {/* Validation warnings banner */}
        {warnings.length > 0 && (
          <div className="print:hidden mb-4 p-4 bg-amber-50 border border-amber-200 rounded-xl">
            <div className="flex items-start gap-3">
              <svg className="w-5 h-5 text-amber-600 mt-0.5 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
              <div>
                <p className="text-sm font-semibold text-amber-800">
                  {warnings.length} {warnings.length === 1 ? 'warning' : 'warnings'}
                </p>
                <ul className="mt-1 space-y-0.5">
                  {warnings.map((w) =>
                    w.warnings.map((msg, i) => (
                      <li key={`${w.section}-${i}`} className="text-xs text-amber-700">
                        {w.section}: {msg}
                      </li>
                    ))
                  )}
                </ul>
              </div>
            </div>
          </div>
        )}

        {/* Progress bar */}
        {validations.length > 0 && (
          <div className="print:hidden mb-6">
            <div className="flex items-center justify-between text-xs text-stone-500 mb-1">
              <span>PRD Completion</span>
              <span className="font-semibold text-stone-700">{completionPct}%</span>
            </div>
            <div className="h-2 bg-stone-200 rounded-full overflow-hidden">
              <div
                className="h-full bg-amber-500 rounded-full transition-all duration-500"
                style={{ width: `${completionPct}%` }}
              />
            </div>
          </div>
        )}

        {/* PRD Content */}
        <article className="bg-white rounded-2xl shadow-sm border border-stone-200 p-10 print:shadow-none print:border-0 print:rounded-none print:p-0">
          {/* Document title */}
          <div className="mb-10 pb-8 border-b border-stone-200">
            <h1 className="text-4xl font-display font-bold text-stone-900 mb-2">
              {currentProduct?.name ?? 'Product'} PRD
            </h1>
            <p className="text-stone-500 text-sm">Product Requirements Document</p>
          </div>

          {/* Section 1: Overview */}
          <section className="mb-10">
            <h2 className="text-2xl font-display font-semibold text-stone-800 mb-6 pb-2 border-b border-stone-100">
              1. Overview
            </h2>

            {currentProduct?.vision && (
              <div className="mb-6">
                <h3 className="text-base font-semibold text-stone-700 mb-2">Vision</h3>
                <p className="text-stone-600 leading-relaxed">{currentProduct.vision}</p>
              </div>
            )}

            {currentProduct?.problemStatement && (
              <div className="mb-6">
                <h3 className="text-base font-semibold text-stone-700 mb-2">Problem Statement</h3>
                <p className="text-stone-600 leading-relaxed">{currentProduct.problemStatement}</p>
              </div>
            )}

            {currentProduct?.valueProposition && (
              <div className="mb-6">
                <h3 className="text-base font-semibold text-stone-700 mb-2">Value Proposition</h3>
                <p className="text-stone-600 leading-relaxed">{currentProduct.valueProposition}</p>
              </div>
            )}

            {currentProduct?.description && (
              <div className="mb-6">
                <h3 className="text-base font-semibold text-stone-700 mb-2">Description</h3>
                <p className="text-stone-600 leading-relaxed">{currentProduct.description}</p>
              </div>
            )}

            {currentProduct?.targetMarket && currentProduct.targetMarket.length > 0 && (
              <div className="mb-6">
                <h3 className="text-base font-semibold text-stone-700 mb-2">Target Market</h3>
                <ul className="list-disc list-inside space-y-1">
                  {currentProduct.targetMarket.map((item, i) => (
                    <li key={i} className="text-stone-600 text-sm">{item}</li>
                  ))}
                </ul>
              </div>
            )}

            {currentProduct?.businessGoals && currentProduct.businessGoals.length > 0 && (
              <div>
                <h3 className="text-base font-semibold text-stone-700 mb-2">Business Goals</h3>
                <ul className="list-disc list-inside space-y-1">
                  {currentProduct.businessGoals.map((goal, i) => (
                    <li key={i} className="text-stone-600 text-sm">{goal}</li>
                  ))}
                </ul>
              </div>
            )}
          </section>

          {/* Section 2: User Personas */}
          <section className="mb-10">
            <h2 className="text-2xl font-display font-semibold text-stone-800 mb-6 pb-2 border-b border-stone-100">
              2. User Personas
            </h2>

            {personas.length === 0 ? (
              <p className="text-stone-400 italic text-sm">No personas defined yet.</p>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {personas.map((persona) => (
                  <div
                    key={persona.id}
                    className="border border-stone-200 rounded-xl p-5 bg-stone-50"
                  >
                    <div className="flex items-start justify-between mb-3">
                      <div>
                        <h4 className="font-semibold text-stone-800">{persona.name}</h4>
                        <p className="text-sm text-stone-500">{persona.role}</p>
                      </div>
                      <span className="text-xs px-2 py-0.5 bg-amber-100 text-amber-700 rounded-full font-medium">
                        {persona.technicalLevel}
                      </span>
                    </div>

                    {persona.goals.length > 0 && (
                      <div className="mb-2">
                        <p className="text-xs font-semibold text-stone-600 mb-1">Goals</p>
                        <ul className="list-disc list-inside space-y-0.5">
                          {persona.goals.map((g, i) => (
                            <li key={i} className="text-xs text-stone-500">{g}</li>
                          ))}
                        </ul>
                      </div>
                    )}

                    {persona.painPoints.length > 0 && (
                      <div className="mb-2">
                        <p className="text-xs font-semibold text-stone-600 mb-1">Pain Points</p>
                        <ul className="list-disc list-inside space-y-0.5">
                          {persona.painPoints.map((p, i) => (
                            <li key={i} className="text-xs text-stone-500">{p}</li>
                          ))}
                        </ul>
                      </div>
                    )}

                    {persona.quote && (
                      <blockquote className="mt-3 italic text-xs text-stone-400 border-l-2 border-amber-300 pl-3">
                        "{persona.quote}"
                      </blockquote>
                    )}
                  </div>
                ))}
              </div>
            )}
          </section>

          {/* Section 3: User Stories */}
          <section className="mb-10">
            <h2 className="text-2xl font-display font-semibold text-stone-800 mb-6 pb-2 border-b border-stone-100">
              3. User Stories
            </h2>

            {stories.length === 0 ? (
              <p className="text-stone-400 italic text-sm">No user stories defined yet.</p>
            ) : (
              <div className="space-y-3">
                {stories.map((story) => (
                  <div key={story.id} className="border border-stone-200 rounded-xl p-4 bg-stone-50">
                    <div className="flex items-start gap-3">
                      <span
                        className={`mt-0.5 flex-shrink-0 text-xs font-semibold px-2 py-0.5 rounded-full ${
                          story.priority === 'Must'
                            ? 'bg-red-100 text-red-700'
                            : story.priority === 'Should'
                            ? 'bg-amber-100 text-amber-700'
                            : story.priority === 'Could'
                            ? 'bg-blue-100 text-blue-700'
                            : 'bg-stone-100 text-stone-500'
                        }`}
                      >
                        {story.priority}
                      </span>
                      <div className="flex-1 min-w-0">
                        <p className="text-sm text-stone-700">
                          As a <strong>{story.asA}</strong>, I want{' '}
                          <strong>{story.iWant}</strong>
                          {story.soThat ? (
                            <>
                              {' '}so that <strong>{story.soThat}</strong>
                            </>
                          ) : null}
                          .
                        </p>
                        {story.acceptanceCriteria.length > 0 && (
                          <div className="mt-2">
                            <p className="text-xs font-semibold text-stone-500 mb-1">
                              Acceptance Criteria
                            </p>
                            <ul className="list-disc list-inside space-y-0.5">
                              {story.acceptanceCriteria.map((ac, i) => (
                                <li key={i} className="text-xs text-stone-500">{ac}</li>
                              ))}
                            </ul>
                          </div>
                        )}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </section>

          {/* Section 4: Features */}
          <section className="mb-10">
            <h2 className="text-2xl font-display font-semibold text-stone-800 mb-6 pb-2 border-b border-stone-100">
              4. Features
            </h2>

            {features.length === 0 ? (
              <p className="text-stone-400 italic text-sm">No features defined yet.</p>
            ) : (
              <div className="space-y-6">
                {[
                  { label: 'MVP', items: mvpFeatures, color: 'bg-green-100 text-green-700' },
                  { label: 'Enhanced', items: enhancedFeatures, color: 'bg-blue-100 text-blue-700' },
                  { label: 'Future', items: futureFeatures, color: 'bg-purple-100 text-purple-700' },
                ]
                  .filter(({ items }) => items.length > 0)
                  .map(({ label, items, color }) => (
                    <div key={label}>
                      <h3 className="text-sm font-semibold text-stone-600 mb-3 flex items-center gap-2">
                        <span className={`px-2 py-0.5 rounded-full text-xs font-semibold ${color}`}>
                          {label}
                        </span>
                      </h3>
                      <div className="space-y-2">
                        {items.map((feature) => (
                          <div
                            key={feature.id}
                            className="border border-stone-200 rounded-lg p-3 bg-stone-50"
                          >
                            <div className="flex items-start justify-between gap-2">
                              <div>
                                <p className="text-sm font-medium text-stone-800">{feature.name}</p>
                                {feature.description && (
                                  <p className="text-xs text-stone-500 mt-0.5">{feature.description}</p>
                                )}
                              </div>
                              {feature.effort && (
                                <span className="text-xs px-2 py-0.5 bg-stone-200 text-stone-600 rounded-full flex-shrink-0">
                                  {feature.effort}
                                </span>
                              )}
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  ))}
              </div>
            )}
          </section>

          {/* Section 5: Timeline */}
          <section className="mb-10">
            <h2 className="text-2xl font-display font-semibold text-stone-800 mb-6 pb-2 border-b border-stone-100">
              5. Timeline
            </h2>

            {phases.length === 0 ? (
              <p className="text-stone-400 italic text-sm">No timeline defined yet.</p>
            ) : (
              <div className="space-y-4">
                {phases.map((phase, index) => (
                  <div key={phase.id} className="border border-stone-200 rounded-xl p-5 bg-stone-50">
                    <div className="flex items-center gap-3 mb-3">
                      <span className="w-7 h-7 flex items-center justify-center rounded-full bg-amber-100 text-amber-700 text-xs font-bold flex-shrink-0">
                        {index + 1}
                      </span>
                      <div>
                        <h4 className="font-semibold text-stone-800">{phase.name}</h4>
                        <p className="text-xs text-stone-500">
                          {phase.durationWeeks} {phase.durationWeeks === 1 ? 'week' : 'weeks'}
                          {phase.startDate
                            ? ` · Starting ${new Date(phase.startDate).toLocaleDateString()}`
                            : ''}
                        </p>
                      </div>
                    </div>

                    {phase.milestones.length > 0 && (
                      <div className="ml-10 space-y-2">
                        {phase.milestones.map((milestone) => (
                          <div key={milestone.id} className="text-xs">
                            <span className="font-medium text-stone-700">
                              Week {milestone.weekNumber}: {milestone.name}
                            </span>
                            {milestone.deliverables.length > 0 && (
                              <ul className="mt-1 list-disc list-inside space-y-0.5 text-stone-500">
                                {milestone.deliverables.map((d, i) => (
                                  <li key={i}>{d}</li>
                                ))}
                              </ul>
                            )}
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </section>

          {/* Footer */}
          <div className="pt-8 border-t border-stone-100 text-xs text-stone-400 flex justify-between">
            <span>{currentProduct?.name ?? 'Product'} — Product Requirements Document</span>
            <span>v{currentProduct?.version ?? 1}</span>
          </div>
        </article>
      </div>

      {/* Print styles injected via <style> */}
      <style>{`
        @media print {
          .print\\:hidden { display: none !important; }
          body { background: white; }
        }
      `}</style>
    </div>
  );
}

export default PreviewPage;
