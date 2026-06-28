import { useSelector, useDispatch } from 'react-redux';
import type { RootState, AppDispatch } from '../store/store';
import { login, register, logout, clearError } from '../store/slices/authSlice';

export function useAuth() {
  const dispatch = useDispatch<AppDispatch>();
  const { user, token, isLoading, error } = useSelector((state: RootState) => state.auth);

  return {
    user,
    token,
    isLoading,
    error,
    isAuthenticated: !!token,
    login: (email: string, password: string) => dispatch(login({ email, password })),
    register: (email: string, name: string, password: string) => dispatch(register({ email, name, password })),
    logout: () => dispatch(logout()),
    clearError: () => dispatch(clearError()),
  };
}
