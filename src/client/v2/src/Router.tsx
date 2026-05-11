import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import { AppLayout } from './layouts/AppLayout';
import { AccountsPage } from './pages/Accounts.page';
import { AnalyticsPage } from './pages/Analytics.page';
import { DashboardPage } from './pages/Dashboard.page';
import { SettingsPage } from './pages/Settings.page';

const router = createBrowserRouter([
  {
    path: '/',
    element: <AppLayout />,
    children: [
      { index: true, element: <Navigate to="/dashboard" replace /> },
      { path: 'dashboard', element: <DashboardPage /> },
      { path: 'accounts', element: <AccountsPage /> },
      { path: 'analytics', element: <AnalyticsPage /> },
      { path: 'settings', element: <SettingsPage /> },
    ],
  },
]);

export function Router() {
  return <RouterProvider router={router} />;
}
