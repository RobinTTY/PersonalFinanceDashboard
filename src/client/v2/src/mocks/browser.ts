import { setupWorker } from 'msw/browser';
import { handlers } from './handlers';

/**
 * The MSW worker for the browser. Starting it (see `enableApiMocking`) registers the
 * Service Worker copied to `public/mockServiceWorker.js` by `npx msw init`, which then
 * intercepts network requests and answers them from `handlers`.
 */
export const worker = setupWorker(...handlers);

/**
 * Starts request mocking. Awaited during app bootstrap so the worker is active before
 * the first query fires. `onUnhandledRequest: 'bypass'` lets any request without a
 * matching handler reach the network untouched, so partially mocked APIs keep working.
 */
export async function enableApiMocking() {
  await worker.start({
    onUnhandledRequest: 'bypass',
    serviceWorker: {
      // Resolve the worker relative to Vite's base URL so mocking also works when the
      // app is served from a sub-path.
      url: `${import.meta.env.BASE_URL}mockServiceWorker.js`,
    },
  });
}
