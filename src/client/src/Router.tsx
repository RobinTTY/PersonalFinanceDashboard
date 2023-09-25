import {
  createBrowserRouter,
  createRoutesFromElements,
  Route,
  RouterProvider,
} from 'react-router-dom';

import { Shell } from './components/shell/Shell';
import { ErrorPage } from './routes/ErrorPage';
import { Dashboard } from './routes/Dashboard';
import { Transactions } from './routes/Transactions';
import { Accounts } from './routes/Accounts';

const router = createBrowserRouter(
  createRoutesFromElements(
    <Route
      path="/"
      element={<Shell />}
      // loader={rootLoader}
      // action={rootAction}
      errorElement={<ErrorPage />}
    >
      {/* <Route errorElement={<ErrorPage />}> */}
      <Route index element={<Accounts />} />
      <Route path="dashboard" element={<Dashboard />} />
      <Route path="accounts" element={<Accounts />} />
      <Route path="transactions" element={<Transactions />} />
      {/* </Route> */}
    </Route>
  )
);

export function Router() {
  return <RouterProvider router={router} />;
}
