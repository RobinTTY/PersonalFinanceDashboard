import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import { RouteErrorBoundary } from './components/RouteErrorBoundary/RouteErrorBoundary';
import { AppLayout } from './layouts/AppLayout';

// Pages are loaded on first visit, so e.g. the dashboard chart isn't part of the initial bundle
const router = createBrowserRouter([
  {
    path: '/',
    element: <AppLayout />,
    errorElement: <RouteErrorBoundary />,
    children: [
      {
        // Errors in a page render inside the layout, so the navigation stays usable
        errorElement: <RouteErrorBoundary />,
        children: [
          { index: true, element: <Navigate to="/dashboard" replace /> },
          {
            path: 'dashboard',
            lazy: { Component: async () => (await import('./pages/Dashboard.page')).DashboardPage },
          },
          {
            path: 'accounts',
            lazy: {
              Component: async () => (await import('./pages/Accounts/Accounts.page')).AccountsPage,
            },
          },
          {
            path: 'analytics',
            lazy: { Component: async () => (await import('./pages/Analytics.page')).AnalyticsPage },
          },
          {
            path: 'settings',
            lazy: { Component: async () => (await import('./pages/Settings.page')).SettingsPage },
          },
        ],
      },
    ],
  },
]);

export function Router() {
  return <RouterProvider router={router} />;
}
