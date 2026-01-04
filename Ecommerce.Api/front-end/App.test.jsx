import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import App from './App';

// Mock the global fetch API to prevent actual network requests during tests
global.fetch = jest.fn(() => Promise.resolve({ ok: false }));

describe('App Frontend Integration', () => {
  beforeEach(() => {
    fetch.mockClear();
  });

  test('renders the main layout and static text', () => {
    render(<App />);
    
    // Check for static elements to ensure the page loads
    expect(screen.getByText(/Langster/i)).toBeInTheDocument();
    expect(screen.getByText(/Discover/i)).toBeInTheDocument();
    expect(screen.getByText(/Premium Collection 2024/i)).toBeInTheDocument();
  });

  test('fetches data from backend and updates the UI', async () => {
    // Mock successful API responses for specific endpoints
    fetch.mockImplementation((url) => {
      if (url.includes('/user/profile')) {
        return Promise.resolve({
          ok: true,
          json: async () => ({ name: 'Test User' }),
        });
      }
      if (url.includes('/stats/overview')) {
        return Promise.resolve({
          ok: true,
          json: async () => ({
            customers: '99K+',
            products: '100+',
            satisfaction: '100%'
          }),
        });
      }
      return Promise.reject(new Error('Unknown URL'));
    });

    render(<App />);

    // Wait for the DOM to update with fetched data
    await waitFor(() => {
      expect(screen.getByText('Test User')).toBeInTheDocument();
      expect(screen.getByText('99K+')).toBeInTheDocument();
      expect(screen.getByText('100+')).toBeInTheDocument();
    });
  });

  test('handles API errors gracefully without crashing', async () => {
    // Mock API failure
    fetch.mockRejectedValue(new Error('Network Error'));

    render(<App />);

    // Ensure the default state remains and app doesn't crash
    await waitFor(() => {
      expect(screen.getByText('Shelton Bumhe')).toBeInTheDocument();
    });
  });
});