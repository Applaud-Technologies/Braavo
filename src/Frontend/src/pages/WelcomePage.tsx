import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '../store/hooks';
import type { RootState } from '../store/store';

export function WelcomePage() {
  const navigate = useNavigate();
  const user = useAppSelector((state: RootState) => state.auth.user);

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
      <div className="max-w-4xl mx-auto px-8 py-16">
        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            Welcome to Braavo{user?.email ? `, ${user.email.split('@')[0]}` : ''}
          </h1>
          <p className="text-xl text-gray-600">
            Build comprehensive Product Requirements Documents with AI assistance
          </p>
        </div>

        <div className="grid md:grid-cols-2 gap-8 mb-12">
          <div className="bg-white p-6 rounded-xl shadow-sm">
            <h3 className="text-lg font-semibold mb-2">Guided PRD Builder</h3>
            <p className="text-gray-600 mb-4">
              Step through each section of your PRD with visual controls and AI suggestions.
            </p>
          </div>
          <div className="bg-white p-6 rounded-xl shadow-sm">
            <h3 className="text-lg font-semibold mb-2">Version History</h3>
            <p className="text-gray-600 mb-4">
              Track changes and restore previous versions of your documentation.
            </p>
          </div>
        </div>

        <div className="text-center">
          <button
            onClick={() => navigate('/products')}
            className="px-8 py-4 bg-blue-600 text-white text-lg rounded-lg hover:bg-blue-700 transition-colors"
          >
            View Your Products
          </button>
        </div>
      </div>
    </div>
  );
}
