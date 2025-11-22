import { render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { vi } from 'vitest';
import { mockProducts } from '../data/mockProducts';

vi.mock('react-hot-toast', () => ({
  __esModule: true,
  default: { success: vi.fn(), error: vi.fn() }
}));
vi.mock('../api/axios', () => ({
  __esModule: true,
  default: {
    get: vi.fn().mockResolvedValue({ data: { items: mockProducts } })
  }
}));

describe('Store page', () => {
  it('renders featured products from API', async () => {
    const { Store } = await import('./Store');
    render(
      <BrowserRouter>
        <Store />
      </BrowserRouter>
    );

    expect(screen.getByText(/Featured products/i)).toBeInTheDocument();

    await waitFor(() => {
      expect(screen.getAllByText(/SKU/i).length).toBeGreaterThan(0);
    });
  });
});
