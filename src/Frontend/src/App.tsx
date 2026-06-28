import { Provider } from 'react-redux';
import { BrowserRouter, Routes, Route, useNavigate } from 'react-router-dom';
import { store } from './store/store';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import ChatPage from './pages/ChatPage';
import WireframePage from './pages/WireframePage';
import { WelcomePage } from './pages/WelcomePage';
import { ProductListPage } from './pages/ProductListPage';
import { CreateProductPage } from './pages/CreateProductPage';
import { ProductDetailPage } from './pages/ProductDetailPage';
import { PersonasPage } from './pages/PersonasPage';
import { StoriesPage } from './pages/StoriesPage';
import ProtectedRoute from './components/ProtectedRoute';

function Dashboard() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-primary-600 text-white p-4">
        <h1 className="text-2xl font-bold">Braavo</h1>
      </header>
      <main className="container mx-auto p-8">
        <h2 className="text-xl font-semibold mb-4">Welcome to Braavo!</h2>
        <button
          onClick={() => navigate('/chat')}
          className="px-6 py-3 bg-primary-600 text-white rounded-lg hover:bg-primary-700"
        >
          Start New PRD
        </button>
      </main>
    </div>
  );
}

function App() {
  return (
    <Provider store={store}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/chat"
            element={
              <ProtectedRoute>
                <ChatPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/wireframes"
            element={
              <ProtectedRoute>
                <WireframePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/welcome"
            element={
              <ProtectedRoute>
                <WelcomePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/products"
            element={
              <ProtectedRoute>
                <ProductListPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/products/new"
            element={
              <ProtectedRoute>
                <CreateProductPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/products/:id"
            element={
              <ProtectedRoute>
                <ProductDetailPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/products/:id/personas"
            element={
              <ProtectedRoute>
                <PersonasPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/products/:id/stories"
            element={
              <ProtectedRoute>
                <StoriesPage />
              </ProtectedRoute>
            }
          />
        </Routes>
      </BrowserRouter>
    </Provider>
  );
}

export default App;
