import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import App from './App';

describe('App', () => {
  it('redirects to login when not authenticated', () => {
    render(<App />);
    expect(screen.getByText('Sign in to Braavo')).toBeInTheDocument();
  });
});
