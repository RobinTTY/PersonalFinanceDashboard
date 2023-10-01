import React from 'react';
import ReactDOM from 'react-dom/client';
import { setupWorker } from 'msw';
import { handlers } from './graphql/mock-data/handlers';
import { App } from './App';

// TODO: this isn't necessarily needed
import './index.css';

// TODO: Maybe create seperate clean entrypoint for production
const worker = setupWorker(...handlers);
async function prepare() {
  if (import.meta.env.DEV) {
    return worker.start();
  }
  return Promise.resolve();
}

prepare().then(() => {
  ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
      <App />
    </React.StrictMode>
  );
});
