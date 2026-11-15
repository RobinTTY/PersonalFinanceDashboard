import { render, screen } from '@test-utils';
import { createMemoryRouter, RouterProvider } from 'react-router-dom';
import { RouteErrorBoundary } from './RouteErrorBoundary';

function Broken(): never {
  throw new Error('Render failed');
}

function renderRoutes(initialPath: string) {
  const router = createMemoryRouter(
    [{ path: '/', errorElement: <RouteErrorBoundary />, element: <Broken /> }],
    { initialEntries: [initialPath] }
  );
  render(<RouterProvider router={router} />);
}

describe('RouteErrorBoundary', () => {
  beforeEach(() => {
    // React and the router log caught render errors
    vi.spyOn(console, 'error').mockImplementation(() => {});
    vi.spyOn(console, 'warn').mockImplementation(() => {});
  });

  afterEach(() => vi.restoreAllMocks());

  it('renders a fallback when a route throws while rendering', () => {
    renderRoutes('/');

    expect(screen.getByRole('heading', { name: 'Something went wrong' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Reload page' })).toBeInTheDocument();
  });

  it('renders a not found message for unknown paths', () => {
    renderRoutes('/unknown');

    expect(screen.getByRole('heading', { name: 'Page not found' })).toBeInTheDocument();
  });
});
