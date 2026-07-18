import ReactDOM from 'react-dom/client';
import App from './App';

// Start the MSW worker before rendering so mocked responses are ready for the first
// query. Guarded by an env flag and dynamically imported, so the mocking code and its
// data are only pulled in when explicitly enabled — never in a production build.
async function enableMocking() {
  if (import.meta.env.VITE_ENABLE_API_MOCKING !== 'true') {
    return;
  }

  const { enableApiMocking } = await import('./mocks/browser');
  await enableApiMocking();
}

enableMocking().then(() => {
  ReactDOM.createRoot(document.getElementById('root')!).render(<App />);
});
