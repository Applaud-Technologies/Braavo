import { Provider } from 'react-redux';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { store } from './store/store';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import ProtectedRoute from './components/ProtectedRoute';

function Dashboard() {
  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-primary-600 text-white p-4">
        <h1 className="text-2xl font-bold">Braavo</h1>
      </header>
      <main className="container mx-auto p-4">
        <p className="text-gray-700">Welcome to Braavo!</p>
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
        </Routes>
      </BrowserRouter>
    </Provider>
  );
}

export default App;
