import { describe, it, expect, beforeEach } from 'vitest';
import authReducer, { logout, clearError } from './authSlice';

describe('authSlice', () => {
  const initialState = {
    user: null,
    token: null,
    isLoading: false,
    error: null,
  };

  it('should return initial state', () => {
    expect(authReducer(undefined, { type: 'unknown' })).toEqual(initialState);
  });

  it('should handle logout', () => {
    const stateWithUser = {
      ...initialState,
      user: { id: '1', email: 'test@test.com', name: 'Test', role: 'user' },
      token: 'token123',
    };
    const state = authReducer(stateWithUser, logout());
    expect(state.user).toBeNull();
    expect(state.token).toBeNull();
  });

  it('should handle clearError', () => {
    const stateWithError = { ...initialState, error: 'Some error' };
    const state = authReducer(stateWithError, clearError());
    expect(state.error).toBeNull();
  });
});
